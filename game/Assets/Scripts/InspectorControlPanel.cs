using UnityEngine;
using UnityEngine.UI;

public class InspectorControlPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject mockMovesPanel;
	[SerializeField]
	private Button toggleInspectorButton;
	[SerializeField]
	private bool developmentMode; // true = connected with GS ("C"), false = not connected ("D").
	public bool DevelopmentMode => developmentMode;
	[SerializeField]
	private Text developmentModeText;

	public static InspectorControlPanel Instance { get; private set; }

    // Use this for initialization
    private void Awake()
    {
		Instance = this;

		this.developmentModeText.text = this.developmentMode ? "C" : "D";
		this.toggleInspectorButton.onClick.AddListener(OnToggleInspectorButtonClick);
    }

	private void OnToggleInspectorButtonClick()
	{
		this.mockMovesPanel.SetActive(!this.mockMovesPanel.activeSelf);
	}
}
