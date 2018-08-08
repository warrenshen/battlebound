using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class UMP_ConfirmationDialogUI : UMP_DialogUI
{
    [SerializeField]
    private Text mTitle;

    [SerializeField]
    protected Button cancelButton;

    [SerializeField]
    protected Button confirmButton;

    public void SetConfirmAction(UnityAction action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(action);
        this.confirmButton.onClick.AddListener(Close);
    }

    public void SetCancelAction(UnityAction action)
    {
        this.cancelButton.onClick.RemoveAllListeners();
        this.cancelButton.onClick.AddListener(action);
        this.cancelButton.onClick.AddListener(Close);
    }

    public void SetConfirmLabel(string label)
    {
        this.confirmButton.GetComponentInChildren<Text>().text = label;
    }

    public void SetCancelLabel(string label)
    {
        this.cancelButton.GetComponentInChildren<Text>().text = label;
    }

    public void SetTitle(string title)
    {
        mTitle.text = title;
    }

    public void SetCancelButtonActive(bool isActive)
    {
        this.cancelButton.gameObject.SetActive(isActive);
    }
}

