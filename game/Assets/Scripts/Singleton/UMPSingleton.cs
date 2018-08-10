using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class UMPSingleton : Singleton<UMPSingleton>
{
    private bool isConfirmationOne;
    [SerializeField]
    private UMP_ConfirmationDialogUI ConfirmationDialog;
    [SerializeField]
    private UMP_ConfirmationDialogUI ConfirmationDialogTwo;

    [SerializeField]
    private UMP_InputDialogUI InputFieldDialog;
    [SerializeField]
    private UMP_TwoInputDialogUI TwoInputFieldDialog;
    [SerializeField]
    private UMP_ThreeInputDialogUI ThreeInputFieldDialog;

    private bool isInputFieldTextAreaOne;
    [SerializeField]
    private UMP_TwoInputDialogUI InputFieldAndTextArea;
    [SerializeField]
    private UMP_TwoInputDialogUI InputFieldAndTextAreaTwo;

    private new void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        this.isConfirmationOne = false;
        this.isInputFieldTextAreaOne = false;
    }

    public void CloseAllDialogs()
    {
        ConfirmationDialog.Close();
        ConfirmationDialogTwo.Close();
        InputFieldDialog.Close();
        TwoInputFieldDialog.Close();
        ThreeInputFieldDialog.Close();
        InputFieldAndTextArea.Close();
        InputFieldAndTextAreaTwo.Close();
    }

    public void ShowConfirmationDialog(
        string title,
        string message,
        UnityAction confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel"
    )
    {
        UMP_ConfirmationDialogUI dialogUI;
        if (this.isConfirmationOne)
        {
            this.isConfirmationOne = false;
            dialogUI = ConfirmationDialogTwo;
        }
        else
        {
            this.isConfirmationOne = true;
            dialogUI = ConfirmationDialog;
        }

        dialogUI.SetTitle(title);
        dialogUI.SetMessage(message);

        dialogUI.SetConfirmAction(confirmAction);
        if (cancelAction == null)
        {
            dialogUI.SetCancelButtonActive(false);
        }
        else
        {
            dialogUI.SetCancelAction(cancelAction);
        }

        dialogUI.SetConfirmLabel(confirmLabel);
        dialogUI.SetCancelLabel(cancelLabel);

        dialogUI.gameObject.SetActive(true);
    }

    public void ShowInputFieldDialog(
        string title,
        string message,
        UnityAction<UMP_InputDialogUI, string> confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        string placeholderMessage = "Enter value...",
        InputField.ContentType contentType = InputField.ContentType.Standard
    )
    {
        InputFieldDialog.SetTitle(title);
        InputFieldDialog.SetMessage(message);

        InputFieldDialog.SetConfirmAction(confirmAction);
        if (cancelAction == null)
        {
            InputFieldDialog.SetCancelButtonActive(false);
        }
        else
        {
            InputFieldDialog.SetCancelAction(cancelAction);
        }

        InputFieldDialog.SetConfirmLabel(confirmLabel);
        InputFieldDialog.SetCancelLabel(cancelLabel);

        InputFieldDialog.SetInputPlaceholder(placeholderMessage);
        InputFieldDialog.SetInputContentType(contentType);

        InputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowTwoInputFieldDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        UnityAction<UMP_TwoInputDialogUI, string, string> confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard
    )
    {
        TwoInputFieldDialog.SetTitle(title);
        TwoInputFieldDialog.SetMessage(message);

        TwoInputFieldDialog.SetConfirmAction(confirmAction);
        if (cancelAction == null)
        {
            TwoInputFieldDialog.SetCancelButtonActive(false);
        }
        else
        {
            TwoInputFieldDialog.SetCancelAction(cancelAction);
        }

        TwoInputFieldDialog.SetConfirmLabel(confirmLabel);
        TwoInputFieldDialog.SetCancelLabel(cancelLabel);

        TwoInputFieldDialog.SetInputContextOne(contextOne);
        TwoInputFieldDialog.SetInputContextTwo(contextTwo);

        TwoInputFieldDialog.SetInputContentTypeOne(contentTypeOne);
        TwoInputFieldDialog.SetInputContentTypeTwo(contentTypeTwo);

        TwoInputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowThreeInputFieldDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        string contextThree,
        UnityAction<UMP_ThreeInputDialogUI, string, string, string> confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard,
        InputField.ContentType contentTypeThree = InputField.ContentType.Standard
    )
    {
        ThreeInputFieldDialog.SetTitle(title);
        ThreeInputFieldDialog.SetMessage(message);

        ThreeInputFieldDialog.SetConfirmAction(confirmAction);
        if (cancelAction == null)
        {
            ThreeInputFieldDialog.SetCancelButtonActive(false);
        }
        else
        {
            ThreeInputFieldDialog.SetCancelAction(cancelAction);
        }

        ThreeInputFieldDialog.SetConfirmLabel(confirmLabel);
        ThreeInputFieldDialog.SetCancelLabel(cancelLabel);

        ThreeInputFieldDialog.SetInputContextOne(contextOne);
        ThreeInputFieldDialog.SetInputContextTwo(contextTwo);
        ThreeInputFieldDialog.SetInputContextThree(contextThree);

        ThreeInputFieldDialog.SetInputContentTypeOne(contentTypeOne);
        ThreeInputFieldDialog.SetInputContentTypeTwo(contentTypeTwo);
        ThreeInputFieldDialog.SetInputContentTypeThree(contentTypeThree);

        ThreeInputFieldDialog.gameObject.SetActive(true);
    }

    public void ShowInputFieldAndAreaDialog(
        string title,
        string message,
        string contextOne,  //example: "value" => "Enter value..." in placeholder
        string contextTwo,
        UnityAction<UMP_TwoInputDialogUI, string, string> confirmAction,
        UnityAction cancelAction,
        string confirmLabel = "Confirm",
        string cancelLabel = "Cancel",
        InputField.ContentType contentTypeOne = InputField.ContentType.Standard,
        InputField.ContentType contentTypeTwo = InputField.ContentType.Standard
    )
    {
        UMP_TwoInputDialogUI dialogUI;
        if (this.isInputFieldTextAreaOne)
        {
            this.isInputFieldTextAreaOne = false;
            dialogUI = InputFieldAndTextAreaTwo;
        }
        else
        {
            this.isInputFieldTextAreaOne = true;
            dialogUI = InputFieldAndTextArea;
        }

        dialogUI.SetTitle(title);
        dialogUI.SetMessage(message);

        dialogUI.SetConfirmAction(confirmAction);
        if (cancelAction == null)
        {
            dialogUI.SetCancelButtonActive(false);
        }
        else
        {
            dialogUI.SetCancelAction(cancelAction);
        }

        dialogUI.SetConfirmLabel(confirmLabel);
        dialogUI.SetCancelLabel(cancelLabel);

        dialogUI.SetInputContextOne(contextOne);
        dialogUI.SetInputContextTwo(contextTwo);

        dialogUI.SetInputContentTypeOne(contentTypeOne);
        dialogUI.SetInputContentTypeTwo(contentTypeTwo);

        dialogUI.gameObject.SetActive(true);
    }
}
