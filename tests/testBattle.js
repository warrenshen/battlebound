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

describe("challenge events", function() {
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

  describe("dying breath", function() {
    it("should run Bombshell Bombadier's deathwish", function() {
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
                "id": "C10-5b0b012e7486050526c9c1a8-5",
                "level": 0,
                "category": 0,
                "attack": 50,
                "health": 10,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  0,
                  12
                ],
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
            "id": "5b0b012e7486050526c9c1a8",
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
                "id": "C2-5b0b017502bd4e052f08a28d-2",
                "level": 0,
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
            "deckSize": 1,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "5b0b017502bd4e052f08a28d",
            "expiredStreak": 0
          },
        },
        "opponentIdByPlayerId": {
          "ID_PLAYER": "ID_OPPONENT",
          "ID_OPPONENT": "ID_PLAYER",
        },
        "lastMoves": [],
        "moves": [],
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
        "nonce": 14
      };

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C2-5b0b017502bd4e052f08a28d-2",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C10-5b0b012e7486050526c9c1a8-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");
            assert.equal(lastMoves[2].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");
            assert.equal(lastMoves[3].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");

            assert.equal(playerState.healthMax, 100);
            assert.equal(playerState.health, 40);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should run both Bombshell Bombadier's deathwish", function() {
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
                "id": "C10-5b0b012e7486050526c9c1a8-5",
                "level": 0,
                "category": 0,
                "attack": 50,
                "health": 10,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  0,
                  12
                ],
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
            "id": "5b0b012e7486050526c9c1a8",
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
                "id": "EMPTY"
              },
              {
                "id": "C10-5b0b017502bd4e052f08a28d-10",
                "level": 0,
                "category": 0,
                "attack": 50,
                "health": 40,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  12
                ],
                "baseId": "C10",
                "attackStart": 50,
                "costStart": 70,
                "healthStart": 40,
                "healthMax": 40,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 1
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
            "deckSize": 1,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "5b0b017502bd4e052f08a28d",
            "expiredStreak": 0
          },
        },
        "opponentIdByPlayerId": {
          "ID_PLAYER": "ID_OPPONENT",
          "ID_OPPONENT": "ID_PLAYER",
        },
        "lastMoves": [],
        "moves": [],
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
        "nonce": 14
      };

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C10-5b0b017502bd4e052f08a28d-10",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C10-5b0b012e7486050526c9c1a8-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 7);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "C10-5b0b017502bd4e052f08a28d-10");
            assert.equal(lastMoves[2].attributes.card.id, "C10-5b0b017502bd4e052f08a28d-10");
            assert.equal(lastMoves[3].attributes.card.id, "C10-5b0b017502bd4e052f08a28d-10");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[6].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[5].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[6].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[4].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");
            assert.equal(lastMoves[5].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");
            assert.equal(lastMoves[6].attributes.card.id, "C10-5b0b012e7486050526c9c1a8-5");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");

            assert.equal(playerState.healthMax, 100);
            assert.equal(playerState.health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            assert.equal(opponentState.healthMax, 100);
            assert.equal(opponentState.health, 40);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should run Adderspine Weevil's deathwish", function() {
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
                "id": "C50-ID_OPPONENT-12",
                "level": 0,
                "category": 0,
                "attack": 40,
                "health": 20,
                "cost": 60,
                "name": "Adderspine Weevil",
                "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
                "abilities": [
                  1,
                  30
                ],
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
            "id": "5b0b012e7486050526c9c1a8",
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
                "id": "C53-ID_PLAYER-9",
                "level": 0,
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
                "id": "C40-ID_PLAYER-22",
                "level": 0,
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
                "id": "C40-ID_PLAYER-23",
                "level": 0,
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
            "id": "5b0b017502bd4e052f08a28d",
            "expiredStreak": 0
          },
        },
        "opponentIdByPlayerId": {
          "ID_PLAYER": "ID_OPPONENT",
          "ID_OPPONENT": "ID_PLAYER",
        },
        "lastMoves": [],
        "moves": [],
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
        "nonce": 14
      };

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C53-ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C50-ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C53-ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C50-ID_OPPONENT-12");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "EMPTY");

            assert.equal(playerField[1].id, "C40-ID_PLAYER-22");
            assert.equal(playerField[1].health, 10);

            assert.equal(playerField[3].id, "C40-ID_PLAYER-23");
            assert.equal(playerField[3].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("warcry: friendly", function() {
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C44-5b0b017502bd4e052f08a28d-18",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "C44-5b0b017502bd4e052f08a28d-19",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "C44-5b0b017502bd4e052f08a28d-20",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
          ],
          "hand": [
            {
              "id": "C53-5b0b017502bd4e052f08a28d-9",
              "level": 0,
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
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "C40-5b0b017502bd4e052f08a28d-22",
              "level": 0,
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
              "healthMax": 30
            },
            {
              "id": "C36-5b0b017502bd4e052f08a28d-26",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 20,
              "name": "Ritual Hatchling",
              "description": "Warcry: sacrifice 20 dmg from your life total",
              "abilities": [
                24
              ],
              "baseId": "C36",
              "attackStart": 40,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support grant taunt to adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C53-5b0b017502bd4e052f08a28d-9",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C53-5b0b017502bd4e052f08a28d-9");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "C53-5b0b017502bd4e052f08a28d-9");

            assert.equal(playerField[1].id, "C44-5b0b017502bd4e052f08a28d-18");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[3].id, "C44-5b0b017502bd4e052f08a28d-19");
            assert.equal(playerField[3].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[5].id, "C44-5b0b017502bd4e052f08a28d-20");
            assert.equal(playerField[5].abilities.indexOf(1) >= 0, false);

            resolve();
          }
        );
      });
    });

    it("should support grant taunt to adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C53-5b0b017502bd4e052f08a28d-9",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C53-5b0b017502bd4e052f08a28d-9");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C53-5b0b017502bd4e052f08a28d-9");

            assert.equal(playerField[1].id, "C44-5b0b017502bd4e052f08a28d-18");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[3].id, "C44-5b0b017502bd4e052f08a28d-19");
            assert.equal(playerField[3].abilities.indexOf(1) >= 0, false);

            assert.equal(playerField[5].id, "C44-5b0b017502bd4e052f08a28d-20");
            assert.equal(playerField[5].abilities.indexOf(1) >= 0, false);

            resolve();
          }
        );
      });
    });

    it("should support heal adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C40-5b0b017502bd4e052f08a28d-22",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C40-5b0b017502bd4e052f08a28d-22");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "C40-5b0b017502bd4e052f08a28d-22");

            assert.equal(playerField[1].id, "C44-5b0b017502bd4e052f08a28d-18");
            assert.equal(playerField[1].health, 30);

            assert.equal(playerField[3].id, "C44-5b0b017502bd4e052f08a28d-19");
            assert.equal(playerField[3].health, 30);

            assert.equal(playerField[5].id, "C44-5b0b017502bd4e052f08a28d-20");
            assert.equal(playerField[5].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support heal adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C40-5b0b017502bd4e052f08a28d-22",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C40-5b0b017502bd4e052f08a28d-22");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C40-5b0b017502bd4e052f08a28d-22");

            assert.equal(playerField[1].id, "C44-5b0b017502bd4e052f08a28d-18");
            assert.equal(playerField[1].health, 30);

            assert.equal(playerField[3].id, "C44-5b0b017502bd4e052f08a28d-19");
            assert.equal(playerField[3].health, 10);

            assert.equal(playerField[5].id, "C44-5b0b017502bd4e052f08a28d-20");
            assert.equal(playerField[5].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support damage player face", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C36-5b0b017502bd4e052f08a28d-26",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C36-5b0b017502bd4e052f08a28d-26");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 70);
            assert.equal(playerState.health, 80);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C36-5b0b017502bd4e052f08a28d-26");

            resolve();
          }
        );
      });
    });

    //     {
    //       "id": "C45-5b0b017502bd4e052f08a28d-17",
    //       "level": 0,
    //       "category": 0,
    //       "attack": 40,
    //       "health": 60,
    //       "cost": 80,
    //       "name": "Thunderous Desperado",
    //       "description": "Warcry: Revive your highest cost fallen creature",
    //       "abilities": [
    //         29
    //       ],
    //       "baseId": "C45",
    //       "attackStart": 40,
    //       "costStart": 80,
    //       "healthStart": 60,
    //       "healthMax": 60
    //     },
    //   ],
  });

  describe("warcry: in front", function() {
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
              "id": "C44-ID_OPPONENT-18",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
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
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C13-ID_PLAYER-13",
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
              "healthMax": 30,
            },
            {
              "id": "C51-ID_PLAYER-12",
              "level": 0,
              "category": 0,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Thief of Night",
              "description": "Warcy: Silence creature in front",
              "abilities": [
                18
              ],
              "baseId": "C51",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support attack in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C13-ID_PLAYER-13",
            attributesJson: {
              fieldIndex: 5,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C13-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldIndex, 5);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[5].id, "C13-ID_PLAYER-13");
            assert.equal(playerField[5].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C13-ID_PLAYER-13",
            attributesJson: {
              fieldIndex: 4,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C13-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldIndex, 4);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[4].id, "C13-ID_PLAYER-13");
            assert.equal(playerField[4].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "C44-ID_OPPONENT-18");
            assert.equal(opponentField[0].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support silence in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C51-ID_PLAYER-12",
            attributesJson: {
              fieldIndex: 5,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C51-ID_PLAYER-12");
            assert.equal(lastMoves[0].attributes.fieldIndex, 5);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[5].id, "C51-ID_PLAYER-12");
            assert.equal(playerField[5].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "C44-ID_OPPONENT-18");
            assert.equal(opponentField[0].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("warcry: all enemy", function() {
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
              "id": "C12-ID_OPPONENT-8",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "C49-ID_OPPONENT-13",
              "level": 0,
              "category": 0,
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
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 6,
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
          "id": "5b0b012e7486050526c9c1a8",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C46-ID_PLAYER-16",
              "level": 0,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Cereboarus",
              "description": "Lifesteal; Deathwish: Draw a card",
              "abilities": [
                5,
                6
              ],
              "baseId": "C46",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 5,
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
              "id": "C52-ID_PLAYER-4",
              "level": 0,
              "category": 0,
              "attack": 60,
              "health": 50,
              "cost": 70,
              "name": "POWER SIPH#NER",
              "description": "Warcy: Silence all opponent creatures",
              "abilities": [
                31
              ],
              "baseId": "C52",
              "attackStart": 60,
              "costStart": 70,
              "healthStart": 50,
              "healthMax": 50,
            },
          ],
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support silence all enemy creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C52-ID_PLAYER-4",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C52-ID_PLAYER-4");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 0);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "C52-ID_PLAYER-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C12-ID_OPPONENT-8");
            assert.equal(opponentField[0].isSilenced, 1);

            assert.equal(opponentField[1].id, "C49-ID_OPPONENT-13");
            assert.equal(opponentField[1].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("lethal and lifesteal", function() {
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
              "id": "C12-ID_OPPONENT-8",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
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
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b012e7486050526c9c1a8",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C46-ID_PLAYER-16",
              "level": 0,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Cereboarus",
              "description": "Lifesteal; Deathwish: Draw a card",
              "abilities": [
                5,
                6
              ],
              "baseId": "C46",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 5,
            },
            {
              "id": "C49-ID_PLAYER-13",
              "level": 0,
              "category": 0,
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
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 6,
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
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support lethal", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C49-ID_PLAYER-13",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C12-ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C49-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C12-ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C49-ID_PLAYER-13");
            assert.equal(playerField[1].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support lifesteal", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C46-ID_PLAYER-16",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C12-ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C46-ID_PLAYER-16");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C12-ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 90);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C46-ID_PLAYER-16");
            assert.equal(playerField[0].health, 20);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("attack", function() {
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
              "id": "C40-ID_OPPONENT-22",
              "level": 0,
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
              "spawnRank": 0
            },
            {
              "id": "C40-ID_OPPONENT-23",
              "level": 0,
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
              "spawnRank": 1
            },
            {
              "id": "C40-ID_OPPONENT-24",
              "level": 0,
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
              "spawnRank": 2
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C34-ID_PLAYER-28",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 30,
              "name": "Redhaired Paladin",
              "description": "Deal 10 dmg to adjacent creatures of targeted creature on attack",
              "abilities": [
                22
              ],
              "baseId": "C34",
              "attackStart": 40,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 3
            },
            {
              "id": "C35-ID_PLAYER-27",
              "level": 0,
              "category": 0,
              "attack": 50,
              "health": 70,
              "cost": 70,
              "name": "Firesworn Godblade",
              "description": "Deal dmg to adjacent creatures of targeted creature on attack",
              "abilities": [
                23
              ],
              "baseId": "C35",
              "attackStart": 50,
              "costStart": 70,
              "healthStart": 70,
              "healthMax": 70,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 4,
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
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support attack damage adjacent by 10 damage", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C34-ID_PLAYER-28",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-23",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C34-ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-23");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C34-ID_PLAYER-28");
            assert.equal(playerField[0].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 20);

            assert.equal(opponentField[1].id, "EMPTY");

            assert.equal(opponentField[2].id, "C40-ID_OPPONENT-24");
            assert.equal(opponentField[2].health, 20);

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent by 10 damage", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C34-ID_PLAYER-28",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C34-ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-24");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C34-ID_PLAYER-28");
            assert.equal(playerField[0].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 30);

            assert.equal(opponentField[1].id, "C40-ID_OPPONENT-23");
            assert.equal(opponentField[1].health, 20);

            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C35-ID_PLAYER-27",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-23",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C35-ID_PLAYER-27");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-23");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C35-ID_PLAYER-27");
            assert.equal(playerField[1].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "EMPTY");
            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C35-ID_PLAYER-27",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C35-ID_PLAYER-27");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-24");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C35-ID_PLAYER-27");
            assert.equal(playerField[1].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 30);

            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("take damage", function() {
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
              "id": "C12-ID_OPPONENT-8",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
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
            }
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C37-ID_PLAYER-25",
              "level": 0,
              "category": 0,
              "attack": 80,
              "health": 70,
              "cost": 50,
              "name": "Hellbringer",
              "description": "Whenever this creature takes damage, damage your face 30 damage",
              "abilities": [
                17
              ],
              "baseId": "C37",
              "attackStart": 80,
              "costStart": 50,
              "healthStart": 70,
              "healthMax": 70,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
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
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support damage player face on take damage", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C37-ID_PLAYER-25",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C12-ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C37-ID_PLAYER-25");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C12-ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 70);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C37-ID_PLAYER-25");
            assert.equal(playerField[0].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "id": "C10-5b0b012e7486050526c9c1a8-5",
              "level": 0,
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
              "id": "C2-5b0b012e7486050526c9c1a8-2",
              "level": 0,
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C2-5b0b017502bd4e052f08a28d-2",
              "level": 0,
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
              "id": "C3-5b0b017502bd4e052f08a28d-5",
              "level": 0,
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
              "id": "C23-5b0b012e7486050526c9c1a8-4",
              "level": 0,
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
              "id": "C19-5b0b012e7486050526c9c1a8-0",
              "level": 0,
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
              "id": "C20-5b0b012e7486050526c9c1a8-1",
              "level": 0,
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
              "id": "C22-5b0b012e7486050526c9c1a8-3",
              "level": 0,
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
              "id": "C21-5b0b012e7486050526c9c1a8-2",
              "level": 0,
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
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support brr brr blizzard", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-5b0b012e7486050526c9c1a8-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C23-5b0b012e7486050526c9c1a8-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].isFrozen, 1);
            assert.equal(opponentField[1].isFrozen, 1);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support riot up", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C20-5b0b012e7486050526c9c1a8-1",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C20-5b0b012e7486050526c9c1a8-1");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].abilities.indexOf(2) >= 0, true);
            assert.equal(playerField[2].abilities.indexOf(2) >= 0, true);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support touch of zeus", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C19-5b0b012e7486050526c9c1a8-0",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C2-5b0b012e7486050526c9c1a8-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C19-5b0b012e7486050526c9c1a8-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C2-5b0b012e7486050526c9c1a8-2");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[1].id, "EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support deep freeze", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C21-5b0b012e7486050526c9c1a8-2",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C2-5b0b012e7486050526c9c1a8-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C21-5b0b012e7486050526c9c1a8-2");
            assert.equal(lastMoves[0].attributes.card.name, "Deep Freeze");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C2-5b0b012e7486050526c9c1a8-2");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "C2-5b0b012e7486050526c9c1a8-2");
            assert.equal(opponentField[1].health, 20);
            assert.equal(opponentField[1].isFrozen, 1);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "id": "C10-5b0b012e7486050526c9c1a8-5",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "Deathrattle: Damage opponent face by 10 dmg",
              "abilities": [
                10,
              ],
              "baseId": "C10",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 2
            },
            {
              "id": "C2-5b0b012e7486050526c9c1a8-2",
              "level": 0,
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C2-5b0b017502bd4e052f08a28d-2",
              "level": 0,
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
              "id": "C3-5b0b017502bd4e052f08a28d-5",
              "level": 0,
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
              "id": "C23-5b0b012e7486050526c9c1a8-4",
              "level": 0,
              "category": 1,
              "cost": 50,
              "name": "Death Note",
              "description": "Kill an opponent creature",
              "baseId": "C23",
              "costStart": 50,
            },
            {
              "id": "C24-5b0b012e7486050526c9c1a8-5",
              "level": 0,
              "category": 1,
              "cost": 30,
              "name": "Mudslinging",
              "description": "Give all friendly creatures taunt",
              "baseId": "C24",
              "costStart": 30,
            },
            {
              "id": "C25-5b0b012e7486050526c9c1a8-6",
              "level": 0,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support death note", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-5b0b012e7486050526c9c1a8-4",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C10-5b0b012e7486050526c9c1a8-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C23-5b0b012e7486050526c9c1a8-4");
            assert.equal(lastMoves[0].attributes.card.name, "Death Note");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C10-5b0b012e7486050526c9c1a8-5");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 90);
            assert.equal(playerState.manaCurrent, 20);

            resolve();
          }
        );
      });
    });

    it("should support mudslinging", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C24-5b0b012e7486050526c9c1a8-5",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C24-5b0b012e7486050526c9c1a8-5");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C2-5b0b017502bd4e052f08a28d-2");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[2].id, "C3-5b0b017502bd4e052f08a28d-5");
            assert.equal(playerField[2].abilities.indexOf(1) >= 0, true);

            resolve();
          }
        );
      });
    });

    it("should support silence of the lambs", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C25-5b0b012e7486050526c9c1a8-6",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C25-5b0b012e7486050526c9c1a8-6");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C2-5b0b017502bd4e052f08a28d-2");
            assert.equal(playerField[1].isSilenced, 1);

            assert.equal(playerField[2].id, "C3-5b0b017502bd4e052f08a28d-5");
            assert.equal(playerField[2].isSilenced, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "C10-5b0b012e7486050526c9c1a8-5");
            assert.equal(opponentField[0].isSilenced, 1);

            assert.equal(opponentField[1].id, "C2-5b0b012e7486050526c9c1a8-2");
            assert.equal(opponentField[1].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "id": "C10-5b0b012e7486050526c9c1a8-5",
              "level": 0,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "Deathrattle: Damage opponent face by 10 dmg",
              "abilities": [
                10,
              ],
              "baseId": "C10",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 2
            },
            {
              "id": "C2-5b0b012e7486050526c9c1a8-2",
              "level": 0,
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C2-5b0b017502bd4e052f08a28d-2",
              "level": 0,
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
              "id": "C3-5b0b017502bd4e052f08a28d-5",
              "level": 0,
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
              "id": "C23-5b0b012e7486050526c9c1a8-4",
              "level": 0,
              "category": 1,
              "cost": 40,
              "name": "Greedy Fingers",
              "description": "Draw three cards",
              "baseId": "C23",
              "costStart": 40,
            },
          ],
          "deckSize": 1,
          "deck": [
            {
              "id": "C24-5b0b012e7486050526c9c1a8-5",
              "level": 0,
              "category": 1,
              "cost": 30,
              "name": "Mudslinging",
              "description": "Give all friendly creatures taunt",
              "baseId": "C24",
              "costStart": 30,
            },
            {
              "id": "C25-5b0b012e7486050526c9c1a8-6",
              "level": 0,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
            {
              "id": "C26-5b0b012e7486050526c9c1a8-7",
              "level": 0,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
          ],
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support greedy fingers", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-5b0b012e7486050526c9c1a8-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C23-5b0b012e7486050526c9c1a8-4");
            assert.equal(lastMoves[0].attributes.card.name, "Greedy Fingers");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "C2-5b0b017502bd4e052f08a28d-2",
              "level": 0,
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
              "id": "C3-5b0b017502bd4e052f08a28d-5",
              "level": 0,
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
              "id": "C23-5b0b012e7486050526c9c1a8-4",
              "level": 0,
              "category": 1,
              "cost": 20,
              "name": "Spray n' Pray",
              "description": "Deal 10 dmg to three random opponent targetables",
              "baseId": "C23",
              "costStart": 40,
            },
          ],
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    it("should support spray n' pray", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-5b0b012e7486050526c9c1a8-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "C23-5b0b012e7486050526c9c1a8-4");
            assert.equal(lastMoves[2].attributes.card.id, "C23-5b0b012e7486050526c9c1a8-4");
            assert.equal(lastMoves[3].attributes.card.id, "C23-5b0b012e7486050526c9c1a8-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.healthMax, 100);
            assert.equal(opponentState.health, 70);

            resolve();
          }
        );
      });
    });
  });
});

describe("divine shield", function() {
  it("should pop shield on take damage", function() {
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
              "id": "C9-5b0b017502bd4e052f08a28d-9",
              "level": 0,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Charge; Shield",
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
              "hasShield": 1,
              "isFrozen": 0,
              "spawnRank": 7
            },
            {
              "id": "C1-5b0b017502bd4e052f08a28d-1",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 50,
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
              "healthMax": 60,
              "buffs": [],
              "canAttack": 0,
              "hasShield": 0,
              "isFrozen": 0,
              "spawnRank": 5
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
          "id": "5b0b012e7486050526c9c1a8",
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
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "C7-5b0b012e7486050526c9c1a8-7",
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
              "canAttack": 1,
              "hasShield": 0,
              "isFrozen": 0,
              "spawnRank": 8
            },
            {
              "id": "C4-5b0b012e7486050526c9c1a8-4",
              "level": 0,
              "category": 0,
              "attack": 30,
              "health": 10,
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
              "canAttack": 1,
              "hasShield": 0,
              "isFrozen": 0,
              "spawnRank": 2
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
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
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
      "nonce": 14
    };

    return new Promise((resolve) => {
      gamesparks.sendWithData(
        "LogEventRequest",
        {
          eventKey: "TestChallengeCardAttackCard",
          challengeStateString: JSON.stringify(challengeStateData),
          challengePlayerId: "ID_PLAYER",
          cardId: "C4-5b0b012e7486050526c9c1a8-4",
          attributesJson: {
            fieldId: "ID_OPPONENT",
            targetId: "C9-5b0b017502bd4e052f08a28d-9",
          },
        },
        function(response) {
          const challengeStateData = response.scriptData.challengeStateData;

          const lastMoves = challengeStateData.lastMoves;
          assert.equal(lastMoves.length, 1);
          assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");

          const playerState = challengeStateData.current["ID_PLAYER"];
          const playerField = playerState.field;
          assert.equal(playerField[4].id, "EMPTY");

          const opponentState = challengeStateData.current["ID_OPPONENT"];
          const opponentField = opponentState.field;
          assert.equal(opponentField[2].id, "C9-5b0b017502bd4e052f08a28d-9");
          assert.equal(opponentField[2].abilities.indexOf(2) >= 0, false);
          assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);

          assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
          assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

          // Follow up attack after shield is popped.
          gamesparks.sendWithData(
            "LogEventRequest",
            {
              eventKey: "TestChallengeCardAttackCard",
              challengeStateString: JSON.stringify(challengeStateData),
              challengePlayerId: "ID_PLAYER",
              cardId: "C7-5b0b012e7486050526c9c1a8-7",
              attributesJson: {
                fieldId: "ID_OPPONENT",
                targetId: "C9-5b0b017502bd4e052f08a28d-9",
              },
            },
            function(response) {
              const challengeStateData = response.scriptData.challengeStateData;

              const lastMoves = challengeStateData.lastMoves;
              assert.equal(lastMoves.length, 1);
              assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");

              const playerState = challengeStateData.current["ID_PLAYER"];
              const playerField = playerState.field;
              assert.equal(playerField[3].id, "EMPTY");

              const opponentState = challengeStateData.current["ID_OPPONENT"];
              const opponentField = opponentState.field;

              assert.equal(opponentField[2].id, "C9-5b0b017502bd4e052f08a28d-9");
              assert.equal(opponentField[2].health, 30);
              assert.equal(opponentField[2].abilities.indexOf(2) >= 0, false);
              assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);

              assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
              assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

              resolve();
            }
          );
        }
      );
    });
  });

  it("should not pop shield on attack opponent face", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 30,
          "manaMax": 50,
          "health": 60,
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
              "id": "EMPTY"
            },
            {
              "id": "C4-5b0b017502bd4e052f08a28d-4",
              "level": 0,
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
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
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
              "id": "HIDDEN"
            },
            {
              "id": "HIDDEN"
            },
            {
              "id": "HIDDEN"
            }
          ],
          "deckSize": 11,
          "cardCount": 17,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 0,
          "manaMax": 50,
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
              "id": "C6-5b0b012e7486050526c9c1a8-6",
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
              "spawnRank": 5
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
              "id": "HIDDEN"
            },
            {
              "id": "HIDDEN"
            },
            {
              "id": "HIDDEN"
            }
          ],
          "deckSize": 12,
          "cardCount": 17,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b012e7486050526c9c1a8",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "moveTakenThisTurn": 0,
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 6,
      "nonce": 14
    };

    return new Promise((resolve) => {
      gamesparks.sendWithData(
        "LogEventRequest",
        {
          eventKey: "TestChallengeCardAttackCard",
          challengeStateString: JSON.stringify(challengeStateData),
          challengePlayerId: "ID_PLAYER",
          cardId: "C6-5b0b012e7486050526c9c1a8-6",
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

          const playerState = challengeStateData.current["ID_PLAYER"];
          const playerField = playerState.field;
          assert.equal(playerField[2].id, "C6-5b0b012e7486050526c9c1a8-6");
          assert.equal(playerField[2].health, 60);
          assert.equal(playerField[2].abilities.indexOf(2) >= 0, true);
          assert.equal(playerField[2].abilities.indexOf(0) >= 0, true);

          const opponentState = challengeStateData.current["ID_OPPONENT"];
          assert.equal(opponentState.health, 30);

          assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
          assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

          resolve();
        }
      );
    });
  });
});
