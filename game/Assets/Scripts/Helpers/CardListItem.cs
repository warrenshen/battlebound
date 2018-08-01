using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class CardListItem : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    protected Card card;

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        Card.SetHyperCardArtwork(ref MarketplaceManager.Instance.showcaseCard, this.card);
        Card.SetHyperCardFromData(ref MarketplaceManager.Instance.showcaseCard, this.card);
    }
}
