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

describe("challenge end", function() {
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

  describe("challenge grant experience", function() {
    const challengeStateData = {
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "moves": [
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MULLIGAN",
          "attributes": {
            "deckCardIndices": []
          },
          "rank": 0
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MULLIGAN",
          "attributes": {
            "deckCardIndices": []
          },
          "rank": 1
        },
        {
          "category": "MOVE_CATEGORY_FINISH_MULLIGAN",
          "rank": 2
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C4-ID_OPPONENT-4",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 20,
              "cost": 20,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C4",
              "attackStart": 30,
              "costStart": 20,
              "healthStart": 20,
              "healthMax": 20,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 0
            },
            "cardId": "C4-ID_OPPONENT-4",
            "fieldIndex": 3,
            "handIndex": 1
          },
          "rank": 3
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 4
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C9-ID_PLAYER-9",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Taunt; Shield",
              "abilities": [
                1,
                2
              ],
              "baseId": "C9",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60
            }
          },
          "rank": 5
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C5-ID_PLAYER-5",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Wave Charmer",
              "description": "Charge",
              "abilities": [
                4
              ],
              "baseId": "C5",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 1
            },
            "cardId": "C5-ID_PLAYER-5",
            "fieldIndex": 2,
            "handIndex": 0
          },
          "rank": 6
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "B6-ID_PLAYER-6",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 60,
              "cost": 50,
              "name": "Poseidon's Handmaiden",
              "description": "Charge; Deathrattle: Deal 20 damage to your opponent",
              "abilities": [
                0,
                2
              ],
              "baseId": "C6",
              "attackStart": 30,
              "costStart": 50,
              "healthStart": 60,
              "healthMax": 60
            }
          },
          "rank": 7
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 8
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C2-ID_OPPONENT-2",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10
            }
          },
          "rank": 9
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C2-ID_OPPONENT-2",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 2
            },
            "cardId": "C2-ID_OPPONENT-2",
            "fieldIndex": 2,
            "handIndex": 2
          },
          "rank": 10
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C4-ID_OPPONENT-4",
            "fieldId": "ID_PLAYER",
            "targetId": "C5-ID_PLAYER-5"
          },
          "rank": 11
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 12
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "B15-ID_PLAYER-15",
              "level": 0,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": null,
              "baseId": "C15",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            }
          },
          "rank": 13
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "B6-ID_PLAYER-6",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 60,
              "cost": 50,
              "name": "Poseidon's Handmaiden",
              "description": "Charge; Deathrattle: Deal 20 damage to your opponent",
              "abilities": [
                0,
                2
              ],
              "baseId": "C6",
              "attackStart": 30,
              "costStart": 50,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 3
            },
            "cardId": "B6-ID_PLAYER-6",
            "fieldIndex": 3,
            "handIndex": 3
          },
          "rank": 14
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B6-ID_PLAYER-6",
            "fieldId": "ID_OPPONENT",
            "targetId": "TARGET_ID_FACE"
          },
          "rank": 15
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 16
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C13-ID_OPPONENT-13",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Flamebelcher",
              "description": "Battlecry: Deal 10 damage to any minion in front",
              "abilities": [
                15
              ],
              "baseId": "C13",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 30,
              "healthMax": 30
            }
          },
          "rank": 17
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "B6-ID_OPPONENT-6",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 60,
              "cost": 50,
              "name": "Poseidon's Handmaiden",
              "description": "Charge; Deathrattle: Deal 20 damage to your opponent",
              "abilities": [
                0,
                2
              ],
              "baseId": "C6",
              "attackStart": 30,
              "costStart": 50,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 4
            },
            "cardId": "B6-ID_OPPONENT-6",
            "fieldIndex": 3,
            "handIndex": 0
          },
          "rank": 18
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B6-ID_OPPONENT-6",
            "fieldId": "ID_PLAYER",
            "targetId": "B6-ID_PLAYER-6"
          },
          "rank": 19
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C2-ID_OPPONENT-2",
            "fieldId": "ID_PLAYER",
            "targetId": "TARGET_ID_FACE"
          },
          "rank": 20
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 21
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C0-ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Marshwater Squealer",
              "description": "At the end of each turn, recover 10 health",
              "abilities": [
                7
              ],
              "baseId": "C0",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30
            }
          },
          "rank": 22
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C7-ID_PLAYER-7",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 40,
              "cost": 30,
              "name": "Emberkitty",
              "description": "Deal 10 damage to any opponent in front on end turn",
              "abilities": [
                13
              ],
              "baseId": "C7",
              "attackStart": 30,
              "costStart": 30,
              "healthStart": 40,
              "healthMax": 40,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 5
            },
            "cardId": "C7-ID_PLAYER-7",
            "fieldIndex": 2,
            "handIndex": 0
          },
          "rank": 23
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C0-ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Marshwater Squealer",
              "description": "At the end of each turn, recover 10 health",
              "abilities": [
                7
              ],
              "baseId": "C0",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 6
            },
            "cardId": "C0-ID_PLAYER-0",
            "fieldIndex": 4,
            "handIndex": 3
          },
          "rank": 24
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 25
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C1-ID_OPPONENT-1",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 60,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [
                0,
                8
              ],
              "baseId": "C1",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 60,
              "healthMax": 60
            }
          },
          "rank": 26
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "B8-ID_OPPONENT-8",
              "level": 0,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Firestrided Tigress",
              "description": "Deal 20 damage to any opponent in front on end turn",
              "abilities": [
                14
              ],
              "baseId": "C8",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 7
            },
            "cardId": "B8-ID_OPPONENT-8",
            "fieldIndex": 1,
            "handIndex": 0
          },
          "rank": 27
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C2-ID_OPPONENT-2",
            "fieldId": "ID_PLAYER",
            "targetId": "B6-ID_PLAYER-6"
          },
          "rank": 28
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B6-ID_OPPONENT-6",
            "fieldId": "ID_PLAYER",
            "targetId": "TARGET_ID_FACE"
          },
          "rank": 29
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 30
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C11-ID_PLAYER-11",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 50,
              "cost": 30,
              "name": "Taji the Fearless",
              "description": "Taunt",
              "abilities": [
                1
              ],
              "baseId": "C11",
              "attackStart": 30,
              "costStart": 30,
              "healthStart": 50,
              "healthMax": 50
            }
          },
          "rank": 31
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C0-ID_PLAYER-0",
            "fieldId": "ID_OPPONENT",
            "targetId": "B8-ID_OPPONENT-8"
          },
          "rank": 32
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B6-ID_PLAYER-6",
            "fieldId": "ID_OPPONENT",
            "targetId": "B6-ID_OPPONENT-6"
          },
          "rank": 33
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C7-ID_PLAYER-7",
            "fieldId": "ID_OPPONENT",
            "targetId": "TARGET_ID_FACE"
          },
          "rank": 34
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 35
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C9-ID_OPPONENT-9",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Taunt; Shield",
              "abilities": [
                1,
                2
              ],
              "baseId": "C9",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60
            }
          },
          "rank": 36
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C9-ID_OPPONENT-9",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Taunt; Shield",
              "abilities": [
                1,
                2
              ],
              "baseId": "C9",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 8
            },
            "cardId": "C9-ID_OPPONENT-9",
            "fieldIndex": 2,
            "handIndex": 2
          },
          "rank": 37
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B6-ID_OPPONENT-6",
            "fieldId": "ID_PLAYER",
            "targetId": "TARGET_ID_FACE"
          },
          "rank": 38
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "B8-ID_OPPONENT-8",
            "fieldId": "ID_PLAYER",
            "targetId": "B6-ID_PLAYER-6"
          },
          "rank": 39
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 40
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_DRAW_CARD",
          "attributes": {
            "card": {
              "id": "C8-ID_PLAYER-8",
              "level": 0,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Firestrided Tigress",
              "description": "Deal 20 damage to any opponent in front on end turn",
              "abilities": [
                14
              ],
              "baseId": "C8",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40
            }
          },
          "rank": 41
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED",
          "attributes": {
            "card": {
              "id": "B15-ID_PLAYER-15",
              "level": 0,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": null,
              "baseId": "C15",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            },
            "cardId": "B15-ID_PLAYER-15",
            "handIndex": 2
          },
          "rank": 42
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_CARD_ATTACK",
          "attributes": {
            "cardId": "C7-ID_PLAYER-7",
            "fieldId": "ID_OPPONENT",
            "targetId": "C9-ID_OPPONENT-9"
          },
          "rank": 43
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_PLAY_MINION",
          "attributes": {
            "card": {
              "id": "C11-ID_PLAYER-11",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 50,
              "cost": 30,
              "name": "Taji the Fearless",
              "description": "Taunt",
              "abilities": [
                1
              ],
              "baseId": "C11",
              "attackStart": 30,
              "costStart": 30,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 9
            },
            "cardId": "C11-ID_PLAYER-11",
            "fieldIndex": 3,
            "handIndex": 2
          },
          "rank": 44
        },
        {
          "playerId": "ID_PLAYER",
          "category": "MOVE_CATEGORY_END_TURN",
          "rank": 45
        },
        {
          "playerId": "ID_OPPONENT",
          "category": "MOVE_CATEGORY_SURRENDER_BY_CHOICE",
          "rank": 46
        }
      ],
    };

    it("should grant experience to correct cards of player", function() {
      // console.log(
      //   challengeStateData.moves.map((move) => move.playerId)
      // );
      // console.log(
      //   challengeStateData.moves
      //     .filter((move) => move.playerId == "ID_OPPONENT")
      //     .filter((move) => ["MOVE_CATEGORY_PLAY_SPELL_TARGETED", "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED", "MOVE_CATEGORY_CARD_ATTACK"].indexOf(move.category) >= 0)
      //     .map((move) => move.attributes.cardId)
      // );

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeGrantExperience",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            decksDataJson: {
              cardByCardId: {
                C0: {
                  level: 0,
                  id: "C0",
                  templateId: "C0",
                },
                C7: {
                  level: 2,
                  id: "C7",
                  templateId: "C7",
                  exp: 9,
                  expMax: 10,
                },
              },
            },
            bCardsJson: [
              {
                level: 1,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: 10,
              },
              {
                level: 2,
                id: "B15",
                templateId: "B15",
                exp: 3,
                expMax: 10,
              },
            ],
          },
          function(response) {
            // Expected new decks data.
            // { cardByCardId:
            //   { C0: { level: 0, id: 'C0', templateId: 'C0', exp: 1, expMax: 10 },
            //     C7: { level: 3, id: 'C7', templateId: 'C7', exp: 0, expMax: 10 } } }

            // Expected new b-cards.
            // [ { level: 2, id: 'B6', templateId: 'B6', exp: 0, expMax: 10 },
            //   { level: 2, id: 'B15', templateId: 'B15', exp: 4, expMax: 10 } ]

            // Expected experience cards.
            // [ { id: 'B6',
            //   levelPrevious: 1,
            //   expPrevious: 9,
            //   level: 2,
            //   exp: 0,
            //   expMax: 10 },
            // { id: 'B15',
            //   levelPrevious: 2,
            //   expPrevious: 3,
            //   level: 2,
            //   exp: 4,
            //   expMax: 10 },
            // { id: 'C0',
            //   templateId: 'C0',
            //   levelPrevious: 0,
            //   expPrevious: null,
            //   level: 0,
            //   exp: 1,
            //   expMax: 10 },
            // { id: 'C7',
            //   templateId: 'C7',
            //   levelPrevious: 2,
            //   expPrevious: 9,
            //   level: 3,
            //   exp: 0,
            //   expMax: 10 } ]
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal("C0", newDecksData.cardByCardId["C0"].id);
            assert.equal(0, newDecksData.cardByCardId["C0"].level);
            assert.equal(1, newDecksData.cardByCardId["C0"].exp);
            assert.equal(10, newDecksData.cardByCardId["C0"].expMax);

            assert.equal("C7", newDecksData.cardByCardId["C7"].id);
            assert.equal(3, newDecksData.cardByCardId["C7"].level);
            assert.equal(0, newDecksData.cardByCardId["C7"].exp);
            assert.equal(10, newDecksData.cardByCardId["C7"].expMax);

            assert.equal("B6", newBCards[0].id);
            assert.equal(2, newBCards[0].level);
            assert.equal(0, newBCards[0].exp);
            assert.equal(10, newBCards[0].expMax);

            assert.equal("B15", newBCards[1].id);
            assert.equal(2, newBCards[1].level);
            assert.equal(4, newBCards[1].exp);
            assert.equal(10, newBCards[1].expMax);

            assert.equal(4, expCards.length);

            assert.equal(expCards[0].id, "B6");
            assert.equal(expCards[0].expPrevious, 9);
            assert.equal(expCards[0].exp, 0);
            assert.equal(expCards[0].expMax, 10);
            assert.equal(expCards[0].levelPrevious, 1);
            assert.equal(expCards[0].level, 2);

            assert.equal(expCards[1].id, "B15");
            assert.equal(expCards[1].expPrevious, 3);
            assert.equal(expCards[1].exp, 4);
            assert.equal(expCards[1].expMax, 10);
            assert.equal(expCards[1].levelPrevious, 2);
            assert.equal(expCards[1].level, 2);

            assert.equal(expCards[2].id, "C0");
            assert.equal(expCards[2].expPrevious, 0);
            assert.equal(expCards[2].exp, 1);
            assert.equal(expCards[2].expMax, 10);
            assert.equal(expCards[2].levelPrevious, 0);
            assert.equal(expCards[2].level, 0);

            resolve();
          }
        );
      });
    });

    it("should grant experience to correct cards of opponent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeGrantExperience",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            decksDataJson: {
              cardByCardId: {
                C2: {
                  level: 0,
                  id: "C2",
                  templateId: "C2",
                },
                C4: {
                  level: 1,
                  id: "C4",
                  templateId: "C4",
                  exp: 0,
                  expMax: 10,
                },
              },
            },
            bCardsJson: [
              {
                level: 1,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: 10,
              },
              {
                level: 1,
                id: "B8",
                templateId: "B8",
                exp: 3,
                expMax: 10,
              },
            ],
          },
          function(response) {
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal(4, expCards.length);

            resolve();
          }
        );
      });
    });
  });
});
