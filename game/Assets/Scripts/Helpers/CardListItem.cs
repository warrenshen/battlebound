using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class CardListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected Image cardImage;

    [SerializeField]
    protected Card card;

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (this.card == null)
        {
            Debug.LogError("Invalid card instance variable.");
            return;
        }

        Card.SetHyperCardArtwork(ref MarketplaceManager.Instance.showcaseCard, this.card);
        Card.SetHyperCardFromData(ref MarketplaceManager.Instance.showcaseCard, this.card);
        MarketplaceManager.Instance.HoverEnterEffect(this.gameObject);
        MarketplaceManager.Instance.SetMarketplacePreview(card);
        ActionManager.Instance.SetCursor(1);
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        MarketplaceManager.Instance.HoverExitEffect(this.gameObject);
        ActionManager.Instance.SetCursor(0);
    }
}
