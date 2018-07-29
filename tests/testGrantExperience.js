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

describe("challenge grant experience", function() {
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

  describe("add to experience cards by player ID - creatures", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_OPPONENT-12",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 20,
              "cost": 60,
              "name": "Adderspine Weevil",
              "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
              "abilities": [],
              "baseId": "C50",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 9
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 90,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_PLAYER-9",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 50,
              "cost": 40,
              "name": "Lil' Rusty",
              "description": "Warcy: Grant Taunt to adjacent creatures",
              "abilities": [
                20
              ],
              "baseId": "C53",
              "attackStart": 20,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_PLAYER-22",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Seahorse Squire",
              "description": "Warcry: Heal adjacent creatures by 20 hp",
              "abilities": [
                26
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 3
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-23",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Seahorse Squire",
              "description": "Warcry: Heal adjacent creatures by 20 hp",
              "abilities": [
                26
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 4
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "hand": [],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": ["B4"],
        "ID_OPPONENT": [],
      },
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
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support card attack card", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-12");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 2);
            assert.equal(expCards[0], "B4");
            assert.equal(expCards[1], "C53");

            resolve();
          }
        );
      });
    });

    it("should support card attack face", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "TARGET_ID_FACE",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "TARGET_ID_FACE");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 2);
            assert.equal(expCards[0], "B4");
            assert.equal(expCards[1], "C53");

            resolve();
          }
        );
      });
    });
  });

  describe("add to experience cards by player ID - duplicate experience card", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_OPPONENT-12",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 20,
              "cost": 60,
              "name": "Adderspine Weevil",
              "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
              "abilities": [],
              "baseId": "C50",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 9
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 90,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_PLAYER-9",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 50,
              "cost": 40,
              "name": "Lil' Rusty",
              "description": "Warcy: Grant Taunt to adjacent creatures",
              "abilities": [
                20
              ],
              "baseId": "C53",
              "attackStart": 20,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_PLAYER-22",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Seahorse Squire",
              "description": "Warcry: Heal adjacent creatures by 20 hp",
              "abilities": [
                26
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 3
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-23",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Seahorse Squire",
              "description": "Warcry: Heal adjacent creatures by 20 hp",
              "abilities": [
                26
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 4
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "hand": [],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": ["C53"],
        "ID_OPPONENT": [],
      },
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
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support card attack card", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-12");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C53");

            resolve();
          }
        );
      });
    });
  });

  describe("add to experience cards by player ID - spells", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_OPPONENT-5",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 50,
              "health": 10,
              "cost": 70,
              "name": "Bombshell Bombadier",
              "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
              "abilities": [],
              "baseId": "C10",
              "attackStart": 50,
              "costStart": 70,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_OPPONENT-2",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 3
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
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-2",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [
                0,
                8
              ],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "ID_PLAYER-5",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [
                0,
                8
              ],
              "baseId": "C3",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 0
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "hand": [
            {
              "id": "ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": null,
              "baseId": "C23",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-0",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 20,
              "name": "Touch of Zeus",
              "description": "Deal 20 damage to a creature",
              "abilities": null,
              "baseId": "C19",
              "attackStart": null,
              "costStart": 20,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-1",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": null,
              "baseId": "C20",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-3",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 40,
              "name": "Widespread Frostbite",
              "description": "Freeze creature and one opposite it for two turns, and two adjacent creatures for one turn",
              "abilities": null,
              "baseId": "C22",
              "attackStart": null,
              "costStart": 40,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-2",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 30,
              "name": "Deep Freeze",
              "description": "Deal 10 damage to creature and freeze it",
              "abilities": null,
              "baseId": "C21",
              "attackStart": null,
              "costStart": 30,
              "healthStart": null,
              "healthMax": null
            }
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": [],
        "ID_OPPONENT": [],
      },
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
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support untargeted spell", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-1",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-1");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C20");

            resolve();
          }
        );
      });
    });

    it("should support targeted spell", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-2");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C19");

            resolve();
          }
        );
      });
    });
  });

  describe("grant experience", function() {
    const challengeStateData = {
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": ["C0", "C7", "B6", "B15"],
        "ID_OPPONENT": ["C2", "C4", "B6", "B8"],
      },
    };

    const LEVEL_TO_EXP_MAX = {
      1: 5,
      2: 10,
      3: 50,
      4: 250,
      5: 1000
    };

    it("should grant experience to correct cards of player", function() {
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
                  level: 1,
                  id: "C0",
                  templateId: "C0",
                },
                C7: {
                  level: 2,
                  id: "C7",
                  templateId: "C7",
                  exp: 9,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
                C8: {
                  level: 2,
                  id: "C8",
                  templateId: "C8",
                  exp: 9,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
              },
            },
            bCardsJson: [
              {
                level: 2,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 2,
                id: "B15",
                templateId: "B15",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 2,
                id: "B16",
                templateId: "B16",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
            ],
          },
          function(response) {
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal("C0", newDecksData.cardByCardId["C0"].id);
            assert.equal(1, newDecksData.cardByCardId["C0"].level);
            assert.equal(1, newDecksData.cardByCardId["C0"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newDecksData.cardByCardId["C0"].expMax);

            assert.equal("C7", newDecksData.cardByCardId["C7"].id);
            assert.equal(3, newDecksData.cardByCardId["C7"].level);
            assert.equal(0, newDecksData.cardByCardId["C7"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newDecksData.cardByCardId["C7"].expMax);

            // Should not have any change.
            assert.equal("C8", newDecksData.cardByCardId["C8"].id);
            assert.equal(2, newDecksData.cardByCardId["C8"].level);
            assert.equal(9, newDecksData.cardByCardId["C8"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C8"].expMax);

            assert.equal(newBCards.length, 2);

            assert.equal("B6", newBCards[0].id);
            assert.equal(3, newBCards[0].level);
            assert.equal(0, newBCards[0].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newBCards[0].expMax);

            assert.equal("B15", newBCards[1].id);
            assert.equal(2, newBCards[1].level);
            assert.equal(4, newBCards[1].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newBCards[1].expMax);

            assert.equal(4, expCards.length);

            assert.equal(expCards[0].id, "B6");
            assert.equal(expCards[0].expPrevious, 9);
            assert.equal(expCards[0].exp, 0);
            assert.equal(expCards[0].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[0].levelPrevious, 2);
            assert.equal(expCards[0].level, 3);

            assert.equal(expCards[1].id, "B15");
            assert.equal(expCards[1].expPrevious, 3);
            assert.equal(expCards[1].exp, 4);
            assert.equal(expCards[1].expMax, LEVEL_TO_EXP_MAX[2]);
            assert.equal(expCards[1].levelPrevious, 2);
            assert.equal(expCards[1].level, 2);

            assert.equal(expCards[2].id, "C0");
            assert.equal(expCards[2].expPrevious, 0);
            assert.equal(expCards[2].exp, 1);
            assert.equal(expCards[2].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[2].levelPrevious, 1);
            assert.equal(expCards[2].level, 1);

            assert.equal(expCards[3].id, "C7");
            assert.equal(expCards[3].expPrevious, 9);
            assert.equal(expCards[3].exp, 0);
            assert.equal(expCards[3].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[3].levelPrevious, 2);
            assert.equal(expCards[3].level, 3);

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
                  level: 1,
                  id: "C2",
                  templateId: "C2",
                },
                C4: {
                  level: 2,
                  id: "C4",
                  templateId: "C4",
                  exp: 0,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
                C7: {
                  level: 2,
                  id: "C7",
                  templateId: "C7",
                  exp: 0,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
              },
            },
            bCardsJson: [
              {
                level: 2,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 1,
                id: "B8",
                templateId: "B8",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[1],
              },
            ],
          },
          function(response) {
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal("C2", newDecksData.cardByCardId["C2"].id);
            assert.equal(1, newDecksData.cardByCardId["C2"].level);
            assert.equal(1, newDecksData.cardByCardId["C2"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newDecksData.cardByCardId["C2"].expMax);

            assert.equal("C4", newDecksData.cardByCardId["C4"].id);
            assert.equal(2, newDecksData.cardByCardId["C4"].level);
            assert.equal(1, newDecksData.cardByCardId["C4"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C4"].expMax);

            // Should not have any change.
            assert.equal("C7", newDecksData.cardByCardId["C7"].id);
            assert.equal(2, newDecksData.cardByCardId["C7"].level);
            assert.equal(0, newDecksData.cardByCardId["C7"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C7"].expMax);

            assert.equal(newBCards.length, 2);

            assert.equal("B6", newBCards[0].id);
            assert.equal(3, newBCards[0].level);
            assert.equal(0, newBCards[0].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newBCards[0].expMax);

            assert.equal("B8", newBCards[1].id);
            assert.equal(1, newBCards[1].level);
            assert.equal(4, newBCards[1].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newBCards[1].expMax);

            assert.equal(4, expCards.length);

            assert.equal(expCards[0].id, "B6");
            assert.equal(expCards[0].expPrevious, 9);
            assert.equal(expCards[0].exp, 0);
            assert.equal(expCards[0].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[0].levelPrevious, 2);
            assert.equal(expCards[0].level, 3);

            assert.equal(expCards[1].id, "B8");
            assert.equal(expCards[1].expPrevious, 3);
            assert.equal(expCards[1].exp, 4);
            assert.equal(expCards[1].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[1].levelPrevious, 1);
            assert.equal(expCards[1].level, 1);

            assert.equal(expCards[2].id, "C2");
            assert.equal(expCards[2].expPrevious, 0);
            assert.equal(expCards[2].exp, 1);
            assert.equal(expCards[2].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[2].levelPrevious, 1);
            assert.equal(expCards[2].level, 1);

            assert.equal(expCards[3].id, "C4");
            assert.equal(expCards[3].expPrevious, 0);
            assert.equal(expCards[3].exp, 1);
            assert.equal(expCards[3].expMax, LEVEL_TO_EXP_MAX[2]);
            assert.equal(expCards[3].levelPrevious, 2);
            assert.equal(expCards[3].level, 2);

            resolve();
          }
        );
      });
    });
  });
});
