﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericModalPanel : MonoBehaviour
{
    [SerializeField]
    private Text questionText;

    [SerializeField]
    private Image questionImage;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Button cancelButton;

    public static GenericModalPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Close();
    }

    public void Show(
        string question,
        UnityAction confirmEvent,
        UnityAction cancelEvent,
        string confirmText = "Confirm",
        string cancelText = "Cancel"
    )
    {
        this.gameObject.SetActive(true);

        this.questionText.text = question;
        this.confirmButton.GetComponentInChildren<Text>().text = confirmText;
        this.cancelButton.GetComponentInChildren<Text>().text = cancelText;

        this.confirmButton.onClick.RemoveAllListeners();
        this.confirmButton.onClick.AddListener(confirmEvent);
        this.confirmButton.onClick.AddListener(Close);

        this.cancelButton.onClick.RemoveAllListeners();
        this.cancelButton.onClick.AddListener(cancelEvent);
        this.cancelButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
