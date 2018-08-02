/* global define, module, require */
const CryptoJS = require('crypto-js');
const WebSocket = require('ws');
const assert = require('assert');

var GameSparks = function() {};

GameSparks.prototype = {

  init: function(options) {
    this.options = options;
    this.socketUrl = options.url;

    this.pendingRequests = {};
    this.requestCounter = 0;

    this.connect();
  },

  buildServiceUrl: function(live, options) {
    var stage;
      var urlAddition = options.key;
      var credential;
      var index;

      if (live) {
        stage = "live";
      } else {
        stage = "preview";
      }

      if (!options.credential || options.credential.length === 0) {
        credential = "device";
      } else {
        credential = options.credential;
      }

      if (options.secret) {
        index = options.secret.indexOf(":");
        if (index > 0) {
          credential = "secure";

          urlAddition = options.secret.substr(0, index) + "/" + urlAddition;
        }
    }

      return "wss://" + stage + "-" + urlAddition + ".ws.gamesparks.net/ws/" + credential + "/" + urlAddition;
  },

  initPreview: function(options) {
    options.url = this.buildServiceUrl(false, options);
    this.init(options);
  },

  initLive: function(options) {
    options.url = this.buildServiceUrl(true, options);
    this.init(options);
  },

  reset: function() {
    this.initialised = false;
    this.connected = false;
    this.error = false;
    this.disconnected = false;

    if (this.webSocket != null){
      this.webSocket.onclose = null;
      this.webSocket.close();
    }
  },

  connect: function() {
    this.reset();

    try {
      this.webSocket = new WebSocket(this.socketUrl);
      this.webSocket.onopen = this.onWebSocketOpen.bind(this);
      this.webSocket.onclose = this.onWebSocketClose.bind(this);
      this.webSocket.onerror = this.onWebSocketError.bind(this);
      this.webSocket.onmessage = this.onWebSocketMessage.bind(this);
    } catch(e) {
      this.log(e.message);
    }
  },

  disconnect: function() {
    if (this.webSocket && this.connected) {
      this.disconnected = true;
      this.webSocket.close();
    }
  },

  onWebSocketOpen: function(ev) {
    this.log('WebSocket onOpen');

    if (this.options.onOpen) {
      this.options.onOpen(ev);
    }

    this.connected = true;
  },

  onWebSocketClose: function(ev) {
    this.log('WebSocket onClose');

    if (this.options.onClose) {
      this.options.onClose(ev);
    }

    this.connected = false;

    // Attemp a re-connection if not in error state or deliberately disconnected.
    if (!this.error && !this.disconnected) {
      this.connect();
    }
  },

  onWebSocketError: function(ev) {

    this.log('WebSocket onError: Sorry, but there is some problem with your socket or the server is down');

    if (this.options.onError) {
      this.options.onError(ev);
    }

    // Reset the socketUrl to the original.
    this.socketUrl = this.options.url;

    this.error = true;
  },

  onWebSocketMessage: function(message) {
    this.log('WebSocket onMessage: ' + message.data);

    var result;
    try {
      result = JSON.parse(message.data);
    } catch (e) {
      this.log('An error ocurred while parsing the JSON Data: ' + message + '; Error: ' + e);
      return;
    }

    if (this.options.onMessage) {
      this.options.onMessage(result);
    }

    // Extract any auth token.
    if (result['authToken']) {
      this.authToken = result['authToken'];
      delete result['authToken'];
    }

    if (result['connectUrl']) {
      // Any time a connectUrl is in the response we should update and reconnect.
      this.socketUrl = result['connectUrl'];
      this.connect();
    }

    var resultType = result['@class'];

    if (resultType === '.AuthenticatedConnectResponse') {
      this.handshake(result);
    } else if (resultType.match(/Response$/)){
      if (result['requestId']) {
        var requestId = result['requestId'];
        delete result['requestId'];

        if (this.pendingRequests[requestId]) {
          this.pendingRequests[requestId](result);
          this.pendingRequests[requestId] = null;
        }
      }
    }

  },

  handshake: function(result) {

    if (result['nonce']) {

      var hmac;

      if (this.options.onNonce) {
        hmac = this.options.onNonce(result['nonce']);
      } else if (this.options.secret) {
        hmac = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(result['nonce'], this.options.secret));
      }

      var toSend = {
        '@class' : '.AuthenticatedConnectRequest',
        hmac : hmac
      };

      if (this.authToken) {
        toSend.authToken = this.authToken;
      }

      if (this.sessionId) {
        toSend.sessionId = this.sessionId;
      }

      const browserData = this.getBrowserData();
      toSend.platform = browserData.browser;
      toSend.os = browserData.operatingSystem;

      this.webSocketSend(toSend);

    } else if (result['sessionId']) {
      this.sessionId = result['sessionId'];
      this.initialised = true;

      if (this.options.onInit) {
        this.options.onInit();
      }

      this.keepAliveInterval = setInterval(this.keepAlive.bind(this), 30000);
    }
  },

  keepAlive: function() {
    if (this.initialised && this.connected) {
      this.webSocket.send(' ');
    }
  },

  send: function(requestType, onResponse){
    this.sendWithData(requestType, {}, onResponse);
  },

  sendWithData: function(requestType, json, onResponse) {
    if (!this.initialised) {
      onResponse({ error: 'NOT_INITIALISED' });
      return;
    }

    // Ensure requestType starts with a dot.
    if (requestType.indexOf('.') !== 0) {
      requestType = '.' + requestType;
    }

    json['@class'] = requestType;

    json.requestId = (new Date()).getTime() + "_" + (++this.requestCounter);

    if (onResponse != null) {
      this.pendingRequests[json.requestId] = onResponse;
      // Time out handler.
      setTimeout((function() {
        if (this.pendingRequests[json.requestId]) {
          this.pendingRequests[json.requestId]({ error: 'NO_RESPONSE' });
        }
      }).bind(this), 32000);
    }

    this.webSocketSend(json);
  },

  webSocketSend: function(data) {

    if (this.options.onSend) {
      this.options.onSend(data);
    }

    var requestString = JSON.stringify(data);
    this.log('WebSocket send: ' + requestString);
    this.webSocket.send(requestString);
  },

  getSocketUrl: function() {
    return this.socketUrl;
  },

  getSessionId: function() {
    return this.sessionId;
  },

  getAuthToken: function() {
    return this.authToken;
  },

  setAuthToken: function(authToken) {
    this.authToken = authToken;
  },

  isConnected: function() {
    return this.connected;
  },

  log: function(message) {
    if (this.options.logger) {
      this.options.logger(message);
    }
  },

  getBrowserData: function() {
    return {
      browser: "Firefox",
      operatingSystem: "Mac"
    };
  }
};

GameSparks.prototype.acceptChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("AcceptChallengeRequest", request, onResponse);
}
GameSparks.prototype.accountDetailsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("AccountDetailsRequest", request, onResponse);
}
GameSparks.prototype.analyticsRequest = function(data, end, key, start, onResponse )
{
    var request = {};
		request["data"] = data;
		request["end"] = end;
		request["key"] = key;
		request["start"] = start;
    gamesparks.sendWithData("AnalyticsRequest", request, onResponse);
}
GameSparks.prototype.aroundMeLeaderboardRequest = function(count, friendIds, leaderboardShortCode, social, onResponse )
{
    var request = {};
		request["count"] = count;
		request["friendIds"] = friendIds;
		request["leaderboardShortCode"] = leaderboardShortCode;
		request["social"] = social;
    gamesparks.sendWithData("AroundMeLeaderboardRequest", request, onResponse);
}
GameSparks.prototype.authenticationRequest = function(password, userName, onResponse )
{
    var request = {};
		request["password"] = password;
		request["userName"] = userName;
    gamesparks.sendWithData("AuthenticationRequest", request, onResponse);
}
GameSparks.prototype.buyVirtualGoodsRequest = function(currencyType, quantity, shortCode, onResponse )
{
    var request = {};
		request["currencyType"] = currencyType;
		request["quantity"] = quantity;
		request["shortCode"] = shortCode;
    gamesparks.sendWithData("BuyVirtualGoodsRequest", request, onResponse);
}
GameSparks.prototype.changeUserDetailsRequest = function(displayName, onResponse )
{
    var request = {};
		request["displayName"] = displayName;
    gamesparks.sendWithData("ChangeUserDetailsRequest", request, onResponse);
}
GameSparks.prototype.chatOnChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("ChatOnChallengeRequest", request, onResponse);
}
GameSparks.prototype.consumeVirtualGoodRequest = function(quantity, shortCode, onResponse )
{
    var request = {};
		request["quantity"] = quantity;
		request["shortCode"] = shortCode;
    gamesparks.sendWithData("ConsumeVirtualGoodRequest", request, onResponse);
}
GameSparks.prototype.createChallengeRequest = function(accessType, challengeMessage, challengeShortCode, currency1Wager, currency2Wager, currency3Wager, currency4Wager, currency5Wager, currency6Wager, endTime, expiryTime, maxAttempts, maxPlayers, minPlayers, silent, startTime, usersToChallenge, onResponse )
{
    var request = {};
		request["accessType"] = accessType;
		request["challengeMessage"] = challengeMessage;
		request["challengeShortCode"] = challengeShortCode;
		request["currency1Wager"] = currency1Wager;
		request["currency2Wager"] = currency2Wager;
		request["currency3Wager"] = currency3Wager;
		request["currency4Wager"] = currency4Wager;
		request["currency5Wager"] = currency5Wager;
		request["currency6Wager"] = currency6Wager;
		request["endTime"] = endTime;
		request["expiryTime"] = expiryTime;
		request["maxAttempts"] = maxAttempts;
		request["maxPlayers"] = maxPlayers;
		request["minPlayers"] = minPlayers;
		request["silent"] = silent;
		request["startTime"] = startTime;
		request["usersToChallenge"] = usersToChallenge;
    gamesparks.sendWithData("CreateChallengeRequest", request, onResponse);
}
GameSparks.prototype.declineChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("DeclineChallengeRequest", request, onResponse);
}
GameSparks.prototype.deviceAuthenticationRequest = function(deviceId, deviceModel, deviceName, deviceOS, deviceType, operatingSystem, onResponse )
{
    var request = {};
		request["deviceId"] = deviceId;
		request["deviceModel"] = deviceModel;
		request["deviceName"] = deviceName;
		request["deviceOS"] = deviceOS;
		request["deviceType"] = deviceType;
		request["operatingSystem"] = operatingSystem;
    gamesparks.sendWithData("DeviceAuthenticationRequest", request, onResponse);
}
GameSparks.prototype.dismissMessageRequest = function(messageId, onResponse )
{
    var request = {};
		request["messageId"] = messageId;
    gamesparks.sendWithData("DismissMessageRequest", request, onResponse);
}
GameSparks.prototype.endSessionRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("EndSessionRequest", request, onResponse);
}
GameSparks.prototype.facebookConnectRequest = function(accessToken, code, onResponse )
{
    var request = {};
		request["accessToken"] = accessToken;
		request["code"] = code;
    gamesparks.sendWithData("FacebookConnectRequest", request, onResponse);
}
GameSparks.prototype.findChallengeRequest = function(accessType, count, offset, onResponse )
{
    var request = {};
		request["accessType"] = accessType;
		request["count"] = count;
		request["offset"] = offset;
    gamesparks.sendWithData("FindChallengeRequest", request, onResponse);
}
GameSparks.prototype.getChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("GetChallengeRequest", request, onResponse);
}
GameSparks.prototype.getDownloadableRequest = function(shortCode, onResponse )
{
    var request = {};
		request["shortCode"] = shortCode;
    gamesparks.sendWithData("GetDownloadableRequest", request, onResponse);
}
GameSparks.prototype.getMessageRequest = function(messageId, onResponse )
{
    var request = {};
		request["messageId"] = messageId;
    gamesparks.sendWithData("GetMessageRequest", request, onResponse);
}
GameSparks.prototype.getRunningTotalsRequest = function(friendIds, shortCode, onResponse )
{
    var request = {};
		request["friendIds"] = friendIds;
		request["shortCode"] = shortCode;
    gamesparks.sendWithData("GetRunningTotalsRequest", request, onResponse);
}
GameSparks.prototype.getUploadUrlRequest = function(uploadData, onResponse )
{
    var request = {};
		request["uploadData"] = uploadData;
    gamesparks.sendWithData("GetUploadUrlRequest", request, onResponse);
}
GameSparks.prototype.getUploadedRequest = function(uploadId, onResponse )
{
    var request = {};
		request["uploadId"] = uploadId;
    gamesparks.sendWithData("GetUploadedRequest", request, onResponse);
}
GameSparks.prototype.googlePlayBuyGoodsRequest = function(currencyCode, signature, signedData, subUnitPrice, onResponse )
{
    var request = {};
		request["currencyCode"] = currencyCode;
		request["signature"] = signature;
		request["signedData"] = signedData;
		request["subUnitPrice"] = subUnitPrice;
    gamesparks.sendWithData("GooglePlayBuyGoodsRequest", request, onResponse);
}
GameSparks.prototype.iOSBuyGoodsRequest = function(currencyCode, receipt, sandbox, subUnitPrice, onResponse )
{
    var request = {};
		request["currencyCode"] = currencyCode;
		request["receipt"] = receipt;
		request["sandbox"] = sandbox;
		request["subUnitPrice"] = subUnitPrice;
    gamesparks.sendWithData("IOSBuyGoodsRequest", request, onResponse);
}
GameSparks.prototype.joinChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("JoinChallengeRequest", request, onResponse);
}
GameSparks.prototype.leaderboardDataRequest = function(challengeInstanceId, entryCount, friendIds, leaderboardShortCode, offset, social, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["entryCount"] = entryCount;
		request["friendIds"] = friendIds;
		request["leaderboardShortCode"] = leaderboardShortCode;
		request["offset"] = offset;
		request["social"] = social;
    gamesparks.sendWithData("LeaderboardDataRequest", request, onResponse);
}
GameSparks.prototype.listAchievementsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListAchievementsRequest", request, onResponse);
}
GameSparks.prototype.listChallengeRequest = function(entryCount, offset, shortCode, state, onResponse )
{
    var request = {};
		request["entryCount"] = entryCount;
		request["offset"] = offset;
		request["shortCode"] = shortCode;
		request["state"] = state;
    gamesparks.sendWithData("ListChallengeRequest", request, onResponse);
}
GameSparks.prototype.listChallengeTypeRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListChallengeTypeRequest", request, onResponse);
}
GameSparks.prototype.listGameFriendsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListGameFriendsRequest", request, onResponse);
}
GameSparks.prototype.listInviteFriendsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListInviteFriendsRequest", request, onResponse);
}
GameSparks.prototype.listLeaderboardsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListLeaderboardsRequest", request, onResponse);
}
GameSparks.prototype.listMessageRequest = function(entryCount, offset, onResponse )
{
    var request = {};
		request["entryCount"] = entryCount;
		request["offset"] = offset;
    gamesparks.sendWithData("ListMessageRequest", request, onResponse);
}
GameSparks.prototype.listMessageSummaryRequest = function(entryCount, offset, onResponse )
{
    var request = {};
		request["entryCount"] = entryCount;
		request["offset"] = offset;
    gamesparks.sendWithData("ListMessageSummaryRequest", request, onResponse);
}
GameSparks.prototype.listVirtualGoodsRequest = function(onResponse )
{
    var request = {};
    gamesparks.sendWithData("ListVirtualGoodsRequest", request, onResponse);
}
GameSparks.prototype.logChallengeEventRequest = function(challengeInstanceId, eventKey, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["eventKey"] = eventKey;
    gamesparks.sendWithData("LogChallengeEventRequest", request, onResponse);
}
GameSparks.prototype.logEventRequest = function(eventKey, onResponse )
{
    var request = {};
		request["eventKey"] = eventKey;
    gamesparks.sendWithData("LogEventRequest", request, onResponse);
}
GameSparks.prototype.pushRegistrationRequest = function(deviceOS, pushId, onResponse )
{
    var request = {};
		request["deviceOS"] = deviceOS;
		request["pushId"] = pushId;
    gamesparks.sendWithData("PushRegistrationRequest", request, onResponse);
}
GameSparks.prototype.registrationRequest = function(displayName, password, userName, onResponse )
{
    var request = {};
		request["displayName"] = displayName;
		request["password"] = password;
		request["userName"] = userName;
    gamesparks.sendWithData("RegistrationRequest", request, onResponse);
}
GameSparks.prototype.sendFriendMessageRequest = function(friendIds, message, onResponse )
{
    var request = {};
		request["friendIds"] = friendIds;
		request["message"] = message;
    gamesparks.sendWithData("SendFriendMessageRequest", request, onResponse);
}
GameSparks.prototype.socialLeaderboardDataRequest = function(challengeInstanceId, entryCount, friendIds, leaderboardShortCode, offset, social, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["entryCount"] = entryCount;
		request["friendIds"] = friendIds;
		request["leaderboardShortCode"] = leaderboardShortCode;
		request["offset"] = offset;
		request["social"] = social;
    gamesparks.sendWithData("SocialLeaderboardDataRequest", request, onResponse);
}
GameSparks.prototype.twitterConnectRequest = function(accessSecret, accessToken, onResponse )
{
    var request = {};
		request["accessSecret"] = accessSecret;
		request["accessToken"] = accessToken;
    gamesparks.sendWithData("TwitterConnectRequest", request, onResponse);
}
GameSparks.prototype.windowsBuyGoodsRequest = function(currencyCode, receipt, subUnitPrice, onResponse )
{
    var request = {};
		request["currencyCode"] = currencyCode;
		request["receipt"] = receipt;
		request["subUnitPrice"] = subUnitPrice;
    gamesparks.sendWithData("WindowsBuyGoodsRequest", request, onResponse);
}
GameSparks.prototype.withdrawChallengeRequest = function(challengeInstanceId, message, onResponse )
{
    var request = {};
		request["challengeInstanceId"] = challengeInstanceId;
		request["message"] = message;
    gamesparks.sendWithData("WithdrawChallengeRequest", request, onResponse);
}


const gamesparks = new GameSparks();

describe("challenge play mulligan", function() {
  before(function() {
    return new Promise((resolve) => {
      gamesparks.initPreview({
        key: "o353744GfN7z",
        secret: "TF0GGPC2YKf7CglgrE1M7RrmEDX86tDc",
        onNonce: null,
        onInit: function() {
          gamesparks.authenticationRequest("password", "testuser", function(response) {
            if (response.error) {
              console.log("AuthenticationResponse: " + JSON.stringify(response));
              resolve();
              return;
            }

            assert.equal(gamesparks.isConnected(), true);
            resolve();
          });
        },
        onMessage: null,
        logger: null,
      });
    })
  });

  describe("neither player waiting", function() {
    const challengeStateData = {
      "id": "ID_CHALLENGE",
      "mode": 1,
      "current": {
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 5,
          "cardCount": 30,
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_PLAYER-1",
              "level": 0,
              "category": 1,
              "color": 3,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": [],
              "baseId": "C15",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 10,
              "cost": 20,
              "name": "Blessed Newborn",
              "description": "Battlecry: Draw a card",
              "abilities": [
                4
              ],
              "baseId": "C3",
              "playerId": "ID_PLAYER",
              "attackStart": 10,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10,
              "abilitiesStart": [
                4
              ],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-19",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-2",
              "level": 0,
              "category": 0,
              "color": 3,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Wave Charmer",
              "description": "Charge",
              "abilities": [
                4
              ],
              "baseId": "C5",
              "playerId": "ID_PLAYER",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                4
              ],
              "handBuffs": []
            }
          ],
          "id": "ID_PLAYER",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_PLAYER-15",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-16",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-17",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-18",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
          ],
        },
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 4,
          "cardCount": 8,
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_OPPONENT-6",
              "level": 0,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Pricklepillar",
              "description": "Taunt; Lethal",
              "abilities": [
                1,
                21
              ],
              "baseId": "C49",
              "playerId": "ID_OPPONENT",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                1,
                21
              ],
              "handBuffs": []
            },
            {
              "id": "ID_OPPONENT-10",
              "level": 0,
              "category": 0,
              "color": 5,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "playerId": "ID_OPPONENT",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
              "abilitiesStart": [
                29
              ],
              "handBuffs": []
            },
            {
              "id": "ID_OPPONENT-7",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_OPPONENT",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            }
          ],
          "id": "ID_OPPONENT",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_OPPONENT-15",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_OPPONENT-16",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_OPPONENT-17",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
          ],
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 0,
      "deathCount": 0,
      "nonce": 4,
    };

    it("should support discard all cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.hand.length, 4);
            assert.equal(playerState.mulliganCards.length, 0);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 5);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 4);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[4].playerId, "ID_PLAYER");

            resolve();
          }
        );
      });
    });

    it("should support discard all cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.hand.length, 3);
            assert.equal(opponentState.mulliganCards.length, 0);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 3);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-19"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-19");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: ["ID_OPPONENT-7"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.mulliganCards.length, 0);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 3);
            assert.equal(opponentHand[0].id, "ID_OPPONENT-7");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-0", "ID_PLAYER-19", "ID_PLAYER-2"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-0");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-2");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: ["ID_OPPONENT-6", "ID_OPPONENT-10", "ID_OPPONENT-7"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.mulliganCards.length, 0);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 3);
            assert.equal(opponentHand[0].id, "ID_OPPONENT-6");
            assert.equal(opponentHand[1].id, "ID_OPPONENT-10");
            assert.equal(opponentHand[2].id, "ID_OPPONENT-7");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            resolve();
          }
        );
      });
    });
  });

  describe("one player waiting", function() {
    const challengeStateData = {
      "id": "ID_CHALLENGE",
      "mode": 1,
      "current": {
        "ID_PLAYER": {
          "hasTurn": 0,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 5,
          "cardCount": 30,
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_PLAYER-1",
              "level": 0,
              "category": 1,
              "color": 3,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": [],
              "baseId": "C15",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 10,
              "cost": 20,
              "name": "Blessed Newborn",
              "description": "Battlecry: Draw a card",
              "abilities": [
                4
              ],
              "baseId": "C3",
              "playerId": "ID_PLAYER",
              "attackStart": 10,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10,
              "abilitiesStart": [
                4
              ],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-19",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            },
            {
              "id": "ID_PLAYER-2",
              "level": 0,
              "category": 0,
              "color": 3,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Wave Charmer",
              "description": "Charge",
              "abilities": [
                4
              ],
              "baseId": "C5",
              "playerId": "ID_PLAYER",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                4
              ],
              "handBuffs": []
            }
          ],
          "id": "ID_PLAYER",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_PLAYER-15",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-16",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-17",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_PLAYER-18",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
          ],
        },
        "ID_OPPONENT": {
          "hasTurn": 1,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [
            {
              "id": "ID_OPPONENT-6",
              "level": 0,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Pricklepillar",
              "description": "Taunt; Lethal",
              "abilities": [
                1,
                21
              ],
              "baseId": "C49",
              "playerId": "ID_OPPONENT",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                1,
                21
              ],
              "handBuffs": []
            },
            {
              "id": "ID_OPPONENT-10",
              "level": 0,
              "category": 0,
              "color": 5,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "playerId": "ID_OPPONENT",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
              "abilitiesStart": [
                29
              ],
              "handBuffs": []
            },
            {
              "id": "ID_OPPONENT-7",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_OPPONENT",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "handBuffs": []
            },
          ],
          "deckSize": 4,
          "cardCount": 8,
          "mode": 2,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_OPPONENT-15",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_OPPONENT-16",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
            {
              "id": "ID_OPPONENT-17",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 10,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
            },
          ],
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 0,
      "deathCount": 0,
      "nonce": 4,
    };


    it("should support discard all cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.hand.length, 4);
            assert.equal(playerState.mulliganCards.length, 0);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 7);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 4);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[4].playerId, "ID_PLAYER");

            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[6].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[6].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.hand.length, 4);
            assert.equal(opponentState.mulliganCards.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-19"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-19");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 5);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.hand.length, 4);
            assert.equal(opponentState.mulliganCards.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-0", "ID_PLAYER-19", "ID_PLAYER-2"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-0");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-2");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.hand.length, 4);
            assert.equal(opponentState.mulliganCards.length, 0);

            resolve();
          }
        );
      });
    });
  });
});
