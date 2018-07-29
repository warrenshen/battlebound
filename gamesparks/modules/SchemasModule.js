// ====================================================================================================
//
// Schemas.
//
// ====================================================================================================

/**
 * Player scriptData schema: {
 *   address: string, // Ethereum address.
 *   addressChallenge: string, // Challenge string used for address update.
 *   activeChallengeId: string, // Challenge ID of active challenge player is in (helps prevent simultaneous challenges).
 *   level: int, // Player's current level.
 *   exp: int, // Player's current exp.
 *   infoByMatchType: {
 *     [matchType]: {
 *       winStreak: int,
 *       wins: int,
 *       losses: int,
 *     },     
 *     ...
 *   }
 * }
 * 
 * PlayerDecks schema: {
 *   bCardIds: [int (card id), ...], // Card IDs of cards on the blockchain.
 *   cardByCardId: {
 *     [id]: {
 *       templateId: string,
 *       level: int,
 *     },
 *     ...
 *   },
 *   deckByName: {
 *     [name]: [int (card id), ...]
 *   },
 *   activeDeck: string,
 * }
 * 
 * Template schema: {
 *   id: string,
 *   category: int,
 *   name: string,
 *   image: string,
 *   description: string,
 *   cost: int,
 *   health: int,
 *   attack: int,
 *   abilities: [int, ...],
 * }
 * 
 * Card schema: {
 *   id: string,
 *   level: int,
 *   exp: int, // Experience points card has now.
 *   expMax: int, // Experience points necessary to level - max level reached if 0.
 *   seller: string,
 *   txHash: string,
 *   auction: {
 *     startingPrice: int,
 *     endingPrice: int,
 *     duration: int,
 *     startedAt: int,
 *   },
 * }
 * 
 * ChallengeCard schema: {
 *   id: string, // Challenge-unique card ID: "{card ID}-{player ID}-{auto-incrementing ID}".
 *   baseId: string, // Card ID in owner player.
 *   playerId: string,
 *   category: int,
 *   name: string,
 *   color: int, // Enum color class.
 *   description: string,
 *   level: int,
 *   cost: int,
 *   costStart: int,
 *   health: int,
 *   healthStart: int, // The initial value of health.
 *   healthMax: int, // The max value of health - starts at initial health.
 *   attack: int,
 *   attackStart: int, // The initial value of attack.
 *   canAttack: int, // Number of attacks left this turn - not set until card is played.
 *   isFrozen: int, // Number of turns stuck frozen - not set until card is played.
 *   isSilenced: bool (int), // If card is silenced - not set until card is played.
 *   spawnRank: int, // Number of cards played before this card - not set until card is played on field.
 *   deathRank: int, // Number of cards dead before this card - not set until card is dead.
 *   abilities: [int, ...], // Array of enums of abilities card possesses.
 *   abilitiesStart: [int, ...], // Array of enums of abilities card starts with.
 *   handBuffs: [int, ...], // Array of hand-related buffs.
 *   buffs: [
 *     {
 *       granterId: int, // Card ID granting this buff.
 *       attack: int,
 *       abilities: [int, ...],
 *     },
 *     ...
 *   ],
 * }
 * 
 * ChallengeState schema: {
 *   nonce: int, // A counter incremented every time the ChallengeState is updated.
 *   opponentIdByPlayerId: { [playerId]: opponentId },
 *   turnCountByPlayerId: { [playerId]: int },
 *   moveTakenThisTurn: bool, // A bool for whether the active player has made a move this turn.
 *   expiredStreakByPlayerId: { [playerId]: int }, // A counter for how many turns a player has expired in a row.
 *   expCardIdsByPlayerId: { [playerId]: [string, ...] }, // List of card IDs to gain experience for each player.
 *   current: {
 *     [playerIdOne]: {
 *       id: string, // Player ID for convenience.
 *       hasTurn: bool,
 *       turnCount: int,
 *       manaCurrent: int,
 *       manaMax: int,
 *       health: int,
 *       healthMax: int,
 *       armor: int,
 *       field: [Card, ...], // 6-element array: object of { id: "EMPTY" } = empty space.
 *       hand: [Card, ...] // Object of { id: "HIDDEN" } = upside-down card.
 *       deck: [Card, ...],
 *       deckSize: int,
 *       expiredStreak: int,
 *       cardCount: int, // "Auto-increment" int for any draw or spawn-without-draw card actions.
 *       mulliganCards: [Card, ...],
 *       mode: int, // Enum: mulligan, battle, etc.
 *     },
 *     [playerIdTwo]: ...,
 *   },
 *   moves: [Move, ...], // An array of moves by both players in chronological order.
 *   lastMoves: [Move, ...], // An array of the move(s) in last request of player whose turn it is.
 *   spawnCount: int, // "Auto-increment" int for any creature spawn by either player.
 *   deathCount: int, // "Auto-increment" int for any creature death by either player.
 *   deadCards: [ChallengeCard, ...], // Append-only list of dead cards.
 *   isFinal: bool (int), // Whether challenge has been post-processed (grant experience, etc).
 * }
 * 
 * Move schema: {
 *   playerId: string,
 *   category: string,
 *   rank: int, // Number of moves before this move.
 *   attributes: {
 *     cardId: string, // Card ID of card (attacker, played card, etc).
 *     card: ChallengeCard object, // Card associated with `cardId`.
 *     fieldId: string, // Player ID of target's field.
 *     targetId: string, // Card ID of target.
 *     handIndex: int, // Hand index card played from.
 *     fieldIndex: int, // Field index card played at.
 *     newStates: [ChallengeCard, ...],
 *   },
 * }
 **/