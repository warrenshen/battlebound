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
 *   winStreak: int, // Player's current win streak.
 *   level: int, // Player's current level.
 *   exp: int, // Player's current exp.
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
 *   id: string,
 *   category: int,
 *   name: string,
 *   description: string,
 *   level: int,
 *   cost: int,
 *   costStart: int,
 *   health: int,
 *   healthStart: int, // The initial value of health.
 *   attack: int,
 *   attackStart: int, // The initial value of attack.
 *   canAttack: bool (int), // Field probably not set until card is played on field.
 *   hasShield: bool (int), // Field probably not set until card is played on field.
 *   abilities: [int, ...], // Array of enums of abilities card possesses.
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
 * Move schema: {
 *   playerId: string,
 *   category: string,
 *   attributes: { ... },
 * }
 * 
 * ChallengeState schema: {
 *   nonce: int, // A counter incremented every time the ChallengeState is updated.
 *   opponentIdByPlayerId: { [playerId]: opponentId },
 *   turnCountByPlayerId: { [playerId]: int },
 *   moveTakenThisTurn: bool, // A bool for whether the active player has made a move this turn.
 *   expiredStreakByPlayerId: { [playerId]: int }, // A counter for how many turns a player has expired in a row.
 *   current: {
 *     [playerIdOne]: {
 *       id: string, // Player ID for convenience.
 *       hasTurn: bool,
 *       turnCount: int,
 *       manaCurrent: int,
 *       manaMax: int,
 *       health: int,
 *       armor: int,
 *       field: [Card, ...], // 6-element array: object of { id: "EMPTY" } = empty space.
 *       hand: [Card, ...] // Object of { id: "HIDDEN" } = upside-down card.
 *       deck: [Card, ...],
 *       deckSize: int,
 *       expiredStreak: int,
 *     },
 *     [playerIdTwo]: ...,
 *   },
 *   moves: [Move, ...], // An array of moves by both players in chronological order.
 *   lastMoves: [Move, ...], // An array of the move(s) in last request of player whose turn it is.
 * }
 **/