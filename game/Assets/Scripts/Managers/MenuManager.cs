using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
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

    public void LoadBattle()
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
}
