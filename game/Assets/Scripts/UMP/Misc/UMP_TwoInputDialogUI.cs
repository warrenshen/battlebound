using System;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class UMP_TwoInputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputFieldOne;
    [SerializeField]
    private Text inputLabelOne;

    [SerializeField]
    private InputField inputFieldTwo;
    [SerializeField]
    private Text inputLabelTwo;

    public void SetConfirmAction(UnityAction<UMP_TwoInputDialogUI, string, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputOneValue(), GetInputTwoValue()));
    }

    private string GeneratePlaceholder(string context)
    {
        return String.Format("Enter {0}...", context);
    }

    //Begin setting labels and placeholders
    public void SetInputContextOne(string context)
    {
        inputLabelOne.text = context;

        Text placeholderText = (Text)inputFieldOne.placeholder;
        placeholderText.text = GeneratePlaceholder(context);
    }

    public void SetInputContextTwo(string context)
    {
        inputLabelTwo.text = context;

        Text placeholderText = (Text)inputFieldTwo.placeholder;
        placeholderText.text = GeneratePlaceholder(context);
    }

    //Begin contentType
    public void SetInputContentTypeOne(InputField.ContentType contentType)
    {
        inputFieldOne.contentType = contentType;
    }

    public void SetInputContentTypeTwo(InputField.ContentType contentType)
    {
        inputFieldTwo.contentType = contentType;
    }

    //Get inputField values
    public string GetInputOneValue()
    {
        return inputFieldOne.text;
    }

    public string GetInputTwoValue()
    {
        return inputFieldTwo.text;
    }

    public new void Close()
    {
        base.Close();
        this.inputFieldOne.text = "";
        this.inputFieldTwo.text = "";
    }
}
