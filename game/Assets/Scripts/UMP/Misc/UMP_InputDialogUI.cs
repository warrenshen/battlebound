using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class UMP_InputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputField;

    public void SetConfirmAction(UnityAction<UMP_InputDialogUI, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputValue()));
    }

    public void SetInputPlaceholder(string placeholder)
    {
        Text placeholderText = (Text)inputField.placeholder;
        placeholderText.text = placeholder;
    }

    public void SetInputContentType(InputField.ContentType contentType)
    {
        inputField.contentType = contentType;
    }

    public string GetInputValue()
    {
        return inputField.text;
    }
}
