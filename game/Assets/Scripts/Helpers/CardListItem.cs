﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
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
        MenuManager.Instance.HoverEnterEffect(this.gameObject);
        MenuManager.Instance.SetMarketplacePreview(card);
        ActionManager.Instance.SetCursor(1);
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        MenuManager.Instance.HoverExitEffect(this.gameObject);
        ActionManager.Instance.SetCursor(0);
    }
}
