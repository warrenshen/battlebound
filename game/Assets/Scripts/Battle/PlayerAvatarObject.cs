﻿using System.Collections;
using System;

using UnityEngine;
using TMPro;

public class PlayerAvatarObject : TargetableObject, IPlayerAvatarObject
{
    private PlayerAvatar playerAvatar;

    [SerializeField]
    private BoardWeapon weapon;
    public BoardWeapon Weapon => weapon;

    [SerializeField]
    private TextMeshPro healthLabel;
    [SerializeField]
    public OrbFill orbFill;

    [SerializeField]
    private TextMeshPro nameLabel;

    [SerializeField]
    private SpriteRenderer emoteRenderer;

    private AudioSource audioSource;

    public void Initialize(PlayerAvatar playerAvatar)
    {
        this.playerAvatar = playerAvatar;

        this.audioSource = GetComponent<AudioSource>();
        this.nameLabel.text = this.playerAvatar.GetDisplayName();
        //already has cylinder collider in scene
        UpdateStatText();
    }

    public override bool IsAvatar()
    {
        return true;
    }

    public override string GetCardId()
    {
        return this.playerAvatar.GetCardId();
    }

    public override string GetPlayerId()
    {
        return this.playerAvatar.GetPlayerId();
    }

    public override Targetable GetTargetable()
    {
        return this.playerAvatar;
    }

    //public void Fight(TargetableObject other)
    //{
    //    int damageDone = 0;

    //    //move/animate
    //    Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
    //    LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
    //    //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

    //    FXPoolManager.Instance.PlayEffect("SlashVFX", other.transform.position);
    //    SoundManager.Instance.PlaySound("SlashSFX", other.transform.position);
    //    damageDone = this.weapon.AttackMade(other); //diff from boardcreature fight!!

    //    if (!other.IsAvatar())
    //    {
    //        FXPoolManager.Instance.PlayEffect("SlashVFX", this.transform.position);
    //        this.TakeDamage(((BoardCreature)other).GetAttack());

    //        if (((BoardCreature)other).HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later                
    //            SoundManager.Instance.PlaySound("HitTauntSFX", other.transform.position);
    //    }

    //    this.UpdateStatText();
    //}

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Take damage called with amount <= 0.");
            return;
        }

        LeanTween.scale(gameObject, transform.localScale * 1.1f, 1).setEasePunch();
        this.audioSource.Play();

        UpdateStatText();
        TextManager.Instance.ShowTextAtTarget(this.transform, amount.ToString(), Color.red);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Heal called with amount <= 0.");
            return;
        }

        TextManager.Instance.ShowTextAtTarget(transform, amount.ToString(), Color.green);
        FXPoolManager.Instance.PlayEffect("HealPillarVFX", transform.position);

        UpdateStatText();
    }

    public void Die()
    {
        //StartCoroutine("Dissolve", 2);
    }

    //private IEnumerator Dissolve(float duration)
    //{
    //    SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);
    //    float elapsedTime = 0;
    //    while (elapsedTime < duration)
    //    {
    //        spriteRenderer.material.SetFloat("_Progress", Mathf.Lerp(1, 0, (elapsedTime / duration)));
    //        elapsedTime += Time.deltaTime;
    //        yield return new WaitForEndOfFrame();
    //    }
    //    Destroy(gameObject);
    //}

    public void Redraw()
    {
        UpdateStatText();
    }

    private void UpdateStatText()
    {
        float scaleFactor = 1.6f;
        LeanTween
            .scale(this.healthLabel.gameObject, new Vector3(scaleFactor, scaleFactor, scaleFactor), 1)
            .setEasePunch();
        this.healthLabel.text = String.Format("{0}", this.playerAvatar.Health);
        this.orbFill.Fill = (float)this.playerAvatar.Health / (float)this.playerAvatar.MaxHealth;
        //this.healthLabel.text = String.Format("{0}/{1}", this.playerAvatar.Health, this.playerAvatar.MaxHealth);
    }

    public override void MouseDown()
    {
        base.MouseDown();
        BattleManager.Instance.ShowEmoteMenu();
    }

    public void ShowEmote(int id)
    {
        if (LeanTween.isTweening(emoteRenderer.gameObject))
        {
            LeanTween.cancel(emoteRenderer.gameObject);
        }

        emoteRenderer.sprite = BattleManager.Instance.EmoteSprites[id];
        emoteRenderer.gameObject.SetActive(true);
        LeanTween.scale(emoteRenderer.gameObject, Vector3.one * 1.2F, 1)
                 .setEasePunch()
                 .setOnComplete(() =>
                 {
                     emoteRenderer.gameObject.SetActive(false);
                 });
    }
}
