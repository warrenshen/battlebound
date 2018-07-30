using UnityEngine;
using UnityEngine.UI;

public class InspectorControlPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject mockMovesPanel;

    [SerializeField]
    private Button toggleInspectorButton;

    [SerializeField]
    private Text developmentModeText;

    public static InspectorControlPanel Instance { get; private set; }

    // Use this for initialization
    private void Awake()
    {
        Instance = this;

        this.developmentModeText.text = FlagHelper.IsServerEnabled() ? "C" : "D";
        this.toggleInspectorButton.onClick.AddListener(OnToggleInspectorButtonClick);

#if UNITY_EDITOR
        this.gameObject.SetActive(true);
#else
        this.gameObject.SetActive(false);
#endif
    }

    private void OnToggleInspectorButtonClick()
    {
        this.mockMovesPanel.SetActive(!this.mockMovesPanel.activeSelf);
    }
}
