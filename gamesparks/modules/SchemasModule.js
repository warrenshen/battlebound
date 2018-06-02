// ====================================================================================================
//
// Schemas.
//
// ====================================================================================================

/**
 * Player scriptData schema: {
 *   address: string, // Ethereum address.
 *   addressChallenge: string, // Challenge string used for address update.
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
 *   abilities: [int, ...],
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