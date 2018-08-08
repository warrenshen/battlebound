using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static float TWEEN_DURATION = 0.05f;

    public UMP_Manager UMP;

    public Transform gallery;
    private int galleryIndex;

    public Transform marketplacePreview;
    public GameObject marketplacePreviewSummon;

    [SerializeField]
    private string currentView;
    public HyperCard.Card galleryCard;

    [SerializeField]
    private Text usernameText;

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (!SparkSingleton.Instance.IsAuthenticated)
        {
            this.usernameText.text = "Not logged in";
        }
        SparkSingleton.Instance.AddAuthenticatedCallback(RenderUserData);
    }

    private void Start()
    {
        this.currentView = "home";
        this.galleryIndex = Random.Range(0, gallery.childCount);
        this.SetGalleryCreature();

        InvokeRepeating("RotateGalleryCreature", 2.0f, 5f);
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

    private void RenderUserData()
    {
        string displayName = SparkSingleton.Instance.DisplayName;
        if (displayName == null)
        {
            this.usernameText.text = "Not logged in";
        }
        else
        {
            this.usernameText.text = displayName;
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
        target.transform.localScale = Vector3.one * 1.10f;
    }

    public void HoverExitEffect(GameObject target)
    {
        //LeanTween.scale(target, Vector3.one, TWEEN_DURATION).setEaseInQuad();
        target.transform.localScale = Vector3.one;
    }

    public void ViewMarketplace()
    {
        UMP.ChangeWindow(5);
        if (gallery.gameObject.activeSelf == true)
            gallery.gameObject.SetActive(false);
        if (galleryCard.gameObject.activeSelf == true)
            galleryCard.gameObject.SetActive(false);
        marketplacePreview.gameObject.SetActive(true);
    }

    public void ViewMainMenu()
    {
        UMP.ChangeWindow(0);
        if (marketplacePreview.gameObject.activeSelf == true)
            marketplacePreview.gameObject.SetActive(false);
        galleryCard.gameObject.SetActive(true);
        gallery.gameObject.SetActive(true);
    }

    public void SetMarketplacePreview(Card card)
    {
        this.marketplacePreviewSummon.SetActive(false);  //assumes that initial value is set via inspector
        Vector3 position = this.marketplacePreviewSummon.transform.position;

        this.marketplacePreviewSummon = CardSingleton.Instance.GetSummonFromPool(card.GetName());
        this.marketplacePreviewSummon.transform.position = position;
        this.marketplacePreviewSummon.transform.localScale = new Vector3(4, 5, 1);
        this.marketplacePreviewSummon.transform.LookAt(Camera.main.transform);
        this.marketplacePreviewSummon.SetActive(true);
    }
}
