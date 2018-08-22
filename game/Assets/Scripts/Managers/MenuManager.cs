using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static float TWEEN_DURATION = 0.05f;

    public UMP_Manager UMP;

    public Transform gallery;
    private int galleryIndex;

    public Transform marketplacePreview;

    [SerializeField]
    private string currentView;
    public HyperCard.Card galleryCard;

    [SerializeField]
    private Text rankLabel;

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

        RenderRank(-1, -1);
    }

    private void Start()
    {
        this.currentView = "home";
        SparkSingleton.Instance.AddAuthenticatedCallback(RenderUserData);

        this.galleryIndex = Random.Range(0, gallery.childCount);
        SetGalleryCreature();

        InvokeRepeating("RotateGalleryCreature", 2.0f, 4f);
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

    private void RenderRank(int rankGlobal, int rankElo)
    {
        string rankString;

        if (rankGlobal < 0)
        {
            rankString = "Loading...";
        }
        else if (rankGlobal == 0)
        {
            rankString = "Unranked";
        }
        else
        {
            rankString = string.Format("#{0}", rankGlobal.ToString("N0"));
        }

        string value;
        if (rankElo > 0)
        {
            value = string.Format("{0} [{1}]", rankString, rankElo.ToString());
        }
        else
        {
            value = string.Format("{0}", rankString);
        }
        this.rankLabel.text = value;
    }

    private void RotateGalleryCreature()
    {
        this.galleryIndex = (this.galleryIndex + 1) % gallery.childCount;
        SetGalleryCreature();
    }

    private void RenderUserData()
    {
        string displayName = SparkSingleton.Instance.DisplayName;
        string address = SparkSingleton.Instance.Address;
        int rankGlobal = SparkSingleton.Instance.RankGlobal;
        int rankElo = SparkSingleton.Instance.RankElo;

        if (displayName == null && address == null)
        {
            this.usernameText.text = "Not logged in";
            return;
        }

        if (address != null)
        {
            this.usernameText.text = string.Format("{0} {1}", displayName, address);
        }
        else
        {
            this.usernameText.text = displayName;
        }

        RenderRank(rankGlobal, rankElo);

        DeckStore.Instance().GetDecksWithCallback(null);
    }

    public void LoadWallet()
    {
        LO_LoadingScreen.LoadScene("Wallet");
    }

    public void ViewMarketplace()
    {
        UMP.ChangeWindow(5);
        if (gallery.gameObject.activeSelf == true)
        {
            gallery.gameObject.SetActive(false);
        }
        if (galleryCard.gameObject.activeSelf == true)
        {
            galleryCard.gameObject.SetActive(false);
        }
        marketplacePreview.gameObject.SetActive(true);
    }

    public void ViewMainMenu()
    {
        UMP.ChangeWindow(0);
        if (marketplacePreview.gameObject.activeSelf == true)
        {
            marketplacePreview.gameObject.SetActive(false);
        }
        galleryCard.gameObject.SetActive(true);
        gallery.gameObject.SetActive(true);
    }
}
