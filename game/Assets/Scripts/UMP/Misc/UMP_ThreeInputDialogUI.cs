using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class UMP_ThreeInputDialogUI : UMP_ConfirmationDialogUI
{
    [SerializeField]
    private InputField inputFieldOne;
    [SerializeField]
    private Text inputLabelOne;

    [SerializeField]
    private InputField inputFieldTwo;
    [SerializeField]
    private Text inputLabelTwo;

    [SerializeField]
    private InputField inputFieldThree;
    [SerializeField]
    private Text inputLabelThree;

    public void SetConfirmAction(UnityAction<UMP_ThreeInputDialogUI, string, string, string> action)
    {
        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(() => action.Invoke(this, GetInputOneValue(), GetInputTwoValue(), GetInputThreeValue()));
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

    public void SetInputContextThree(string context)
    {
        inputLabelThree.text = context;

        Text placeholderText = (Text)inputFieldThree.placeholder;
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

    public void SetInputContentTypeThree(InputField.ContentType contentType)
    {
        inputFieldThree.contentType = contentType;
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

    public string GetInputThreeValue()
    {
        return inputFieldThree.text;
    }
}