using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class UMP_InputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputField;

    public void SetConfirmAction(UnityAction<string, UMP_InputDialogUI> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(GetInputValue(), this));
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
