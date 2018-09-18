using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;

public class MenuManager : MonoBehaviour
{
    public static float TWEEN_DURATION = 0.05f;

    public const string MATCH_TYPE_CASUAL = "CasualMatch";
    public const string MATCH_TYPE_RANKED = "RankedMatch";

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

        MatchFoundMessage.Listener = MatchFoundMessageHandler;
        MatchNotFoundMessage.Listener = MatchNotFoundMessageHandler;

        RenderRank(-1, -1);
    }

    private void Start()
    {
        this.currentView = "home";
        SparkSingleton.Instance.AddAuthenticationCallback(RenderUserData);

        this.galleryIndex = Random.Range(0, gallery.childCount);
        InvokeRepeating("RotateGalleryCreature", 0.0f, 4.0f);
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
            LO_LoadingScreen.LoadScene("Login");
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

    public void StartSingleplayer()
    {
        BattleSingleton.Instance.SetModeSingleplayer();
        LO_LoadingScreen.LoadScene("Battle");
    }

    public void StartMultiplayer()
    {
        if (!FlagHelper.IsServerEnabled())
        {
            Debug.LogError("Cannot play multiplayer when in local development mode.");
            return;
        }

        BattleSingleton.Instance.SetModeMultiplayer();
        FindMatch();

        // TODO: @nick
    }

    private void FindMatch()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("FindMatch");
        request.SetEventAttribute("playerDeck", SparkSingleton.Instance.ActiveDeck);
        request.SetEventAttribute("matchShortCode", MATCH_TYPE_CASUAL);
        request.Send(
            OnFindMatchSuccess,
            OnFindMatchError
        );
    }

    private void OnFindMatchSuccess(LogEventResponse response)
    {
        Debug.Log("FindMatch request success.");
    }

    private void OnFindMatchError(LogEventResponse response)
    {
        Debug.LogError("FindMatch request error.");
        SendForfeitActiveChallengeRequest();
    }

    public void SendForfeitActiveChallengeRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("ForfeitActiveChallenge");

        request.Send(
            OnForfeitActiveChallengeSuccess,
            OnForfeitActiveChallengeError
        );
    }

    private void OnForfeitActiveChallengeSuccess(LogEventResponse response)
    {
        Debug.Log("ForfeitActiveChallenge request success.");
        FindMatch();
    }

    private void OnForfeitActiveChallengeError(LogEventResponse response)
    {
        Debug.LogError("ForfeitActiveChallenge request error.");
    }

    private void MatchFoundMessageHandler(MatchFoundMessage message)
    {
        Debug.Log("MatchFoundMessage received.");
    }

    private void MatchNotFoundMessageHandler(MatchNotFoundMessage message)
    {
        Debug.Log("MatchNotFoundMessage received.");
    }
}
