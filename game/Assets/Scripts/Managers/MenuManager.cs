using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static float TWEEN_DURATION = 0.05f;

    public Transform gallery;
    private Transform activeGalleryCreature;
    private int galleryIndex;

    public HyperCard.Card galleryCard;

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private Button logoutButton;

    private void Awake()
    {
        if (!SparkSingleton.Instance.IsAuthenticated)
        {
            this.usernameText.text = "Not logged in";
        }
        this.logoutButton.onClick.AddListener(OnLogoutButtonClick);

        SparkSingleton.Instance.AddAuthenticatedCallback(RenderUserData);

        this.galleryIndex = Random.Range(0, gallery.childCount);
        this.SetGalleryCreature();

        InvokeRepeating("RotateGalleryCreature", 3.0f, 5f);
    }

    private void SetGalleryCreature()
    {
        foreach (Transform child in gallery)
        {
            child.gameObject.SetActive(false);
        }
        GalleryIdle galleryCreature = gallery.GetChild(this.galleryIndex).GetComponent<GalleryIdle>();
        galleryCreature.gameObject.SetActive(true);

        Card.SetHyperCardArtwork(ref this.galleryCard, galleryCreature.card);
        Card.SetHyperCardFromData(ref this.galleryCard, galleryCreature.card);
    }

    private void RotateGalleryCreature()
    {
        this.galleryIndex = (this.galleryIndex + 1) % gallery.childCount;
        SetGalleryCreature();
    }

    private void OnLogoutButtonClick()
    {
        SparkSingleton.Instance.Logout();
        string playerId = SparkSingleton.Instance.GetPlayerId();
        Application.LoadLevel("Login");
    }

    private void RenderUserData()
    {
        string playerId = SparkSingleton.Instance.GetPlayerId();
        if (playerId == null)
        {
            this.usernameText.text = "Not logged in";
        }
        else
        {
            this.usernameText.text = playerId;
            // Preload decks.
            DeckStore.Instance().GetDecksWithCallback(null);
        }
    }

    public void LoadMatchmaking()
    {
        Application.LoadLevel("Matchmaking");
    }

    public void LoadCollection()
    {
        Application.LoadLevel("Collection");
    }

    public void LoadMarketplace()
    {
        Application.LoadLevel("Marketplace");
    }

    public void HoverEnterEffect(GameObject target)
    {
        //LeanTween.scale(target, Vector3.one * 1.1f, TWEEN_DURATION).setEaseInQuad();
        target.transform.localScale = Vector3.one * 1.1f;
    }

    public void HoverExitEffect(GameObject target)
    {
        //LeanTween.scale(target, Vector3.one, TWEEN_DURATION).setEaseInQuad();
        target.transform.localScale = Vector3.one;
    }
}
