// ====================================================================================================
//
// Cloud Code for AttackModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function damageCard(card, damage) {
    if (card.hasShield) {
        card.hasShield = 0;
    } else {
        card.health -= damage;
    }
    
    return card;
}
