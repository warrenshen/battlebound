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
 *   cardByCardId: {
 *     [id]: {
 *       templateId: string,
 *       level: int,
 *     },
 *     ...
 *   },
 *   decks: {
 *    [name]: [int (card id), ...]
 *   },
 *   activeDeck: string,
 * }
 * 
 * Card schema: {
 *   id: string,
 *   type: string,
 *   name: string,
 *   level: int,
 *   manaCost: int,
 *   health: int,
 *   attack: int,
 *   canAttack: bool, // Field probably not set until card is played on field.
 * }
 * 
 * Move schema: {
 *   type: string,
 *   attributes: { ... },
 * }
 * 
 * ChallengeState schema: {
 *   nonce: int, // A counter incremented every time the ChallengeState is updated.
 *   current: {
 *     opponentIdByPlayerId: { playerId: opponentId },
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