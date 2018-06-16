// ====================================================================================================
//
// Schemas.
//
// ====================================================================================================

/**
 * Player scriptData schema: {
 *   address: string, // Ethereum address.
 *   addressChallenge: string, // Challenge string used for address update.
 *   winStreak: int, // Player's current win streak.
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
 * ChallengeCard schema: {
 *   id: string,
 *   category: int,
 *   name: string,
 *   description: string,
 *   level: int,
 *   manaCost: int,
 *   health: int,
 *   healthStart: int, // The initial value of health.
 *   attack: int,
 *   attackStart: int, // The initial value of attack.
 *   canAttack: bool, // Field probably not set until card is played on field.
 *   hasShield: bool, // Field probably not set until card is played on field.
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
 *   type: string,
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
 *       hasTurn: bool,
 *       turnCount: int,
 *       manaCurrent: int,
 *       manaMax: int,
 *       health: int,
 *       armor: int,
 *       field: [Card, ...],
 *       hand: [Card, ...],
 *       handSize: int,
 *       deck: [Card, ...],
 *       deckSize: int,
 *     },
 *     [playerIdTwo]: ...,
 *   },
 *   moves: [Move, ...], // An array of moves by both players in chronological order.
 *   lastMoves: [Move, ...], // An array of the move(s) in last request of player whose turn it is.
 * }
 **/