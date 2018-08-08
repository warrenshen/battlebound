using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : ObjectUI
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private string functionName;
    [SerializeField]
    private GameObject labelObject;

    [SerializeField]
    private bool activeState;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Texture2D activeImage;
    private Sprite activeSprite;

    [SerializeField]
    private Texture2D inactiveImage;
    private Sprite inactiveSprite;

    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
        this.scalingFactor = 1.10f;
        this.activeState = true;
        this.spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        this.audioSource = gameObject.GetComponent<AudioSource>();

        if (this.activeImage)
        {
            this.activeSprite = Sprite.Create(this.activeImage, new Rect(0.0f, 0.0f, this.activeImage.width, this.activeImage.height), new Vector2(0.5f, 0.5f), 100.0f);
            this.spriteRenderer.sprite = this.activeSprite;
        }
        if (this.inactiveImage)
        {
            this.inactiveSprite = Sprite.Create(this.inactiveImage, new Rect(0.0f, 0.0f, this.inactiveImage.width, this.inactiveImage.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public override void EnterHover()
    {
        if (!this.activeState)
            return;

        base.EnterHover();
        ActionManager.Instance.SetCursor(1);
    }

    public override void ExitHover()
    {
        if (!this.activeState)
            return;

        base.ExitHover();
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetPassiveCursor();
        }
    }

    public override void MouseUp()
    {
        //to-do: make uninteractable when not your turn
        base.MouseUp();
        if (target != null)
            target.SendMessage(functionName);

        if (this.audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void ToggleState()
    {
        bool oldActiveState = this.activeState;

        if (BattleState.Instance().You.Id == BattleState.Instance().ActivePlayer.Id)
        {
            this.activeState = true;
        }
        else
        {
            this.activeState = false;
        }

        if (this.activeState != oldActiveState)
        {
            return;
        }

        if (this.activeState)
        {
            if (activeImage != null)
            {
                this.spriteRenderer.sprite = this.activeSprite;
            }
        }
        else
        {
            if (inactiveImage != null)
            {
                this.spriteRenderer.sprite = this.inactiveSprite;
            }
        }
        if (this.labelObject != null)
        {
            this.labelObject.SetActive(this.activeState);
        }
    }
}
