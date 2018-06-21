using UnityEngine;
using UnityEngine.UI;

public class InspectorControlPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject mockMovesPanel;
	[SerializeField]
	private Button toggleInspectorButton;

    // Use this for initialization
    private void Awake()
    {
		this.toggleInspectorButton.onClick.AddListener(OnToggleInspectorButtonClick);
    }

	private void OnToggleInspectorButtonClick()
	{
		this.mockMovesPanel.SetActive(!this.mockMovesPanel.active);
	}
}
