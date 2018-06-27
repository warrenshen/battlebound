using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MockMovesPanel : MonoBehaviour
{
    [SerializeField]
    private Button endTurnButton;

    [SerializeField]
    private Button playCardButton;

    [SerializeField]
    private Button playSpellGeneralButton;

    [SerializeField]
    private Button playSpellTargetedButton;

    [SerializeField]
    private Button cardAttackButton;

    [SerializeField]
    private InputField cardIdInputField;

    [SerializeField]
    private InputField fieldIdInputField;

    [SerializeField]
    private InputField targetIdInputField;

    [SerializeField]
    private InputField fieldIndexInputField;

    [SerializeField]
    private Button submitMoveButton;

    private string moveCategory;

    private void Awake()
    {
        this.endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
        this.playCardButton.onClick.AddListener(OnPlayCardButtonClick);
        this.playSpellGeneralButton.onClick.AddListener(OnPlaySpellGeneralButtonClick);
        this.playSpellTargetedButton.onClick.AddListener(OnPlaySpellTargetedButtonClick);
        this.cardAttackButton.onClick.AddListener(OnCardAttackButtonClick);
        this.submitMoveButton.onClick.AddListener(OnSubmitMoveButtonClick);
    }

    private void DeactivateAllFields()
    {
        cardIdInputField.transform.parent.gameObject.SetActive(false);
        fieldIdInputField.transform.parent.gameObject.SetActive(false);
        targetIdInputField.transform.parent.gameObject.SetActive(false);
        fieldIndexInputField.transform.parent.gameObject.SetActive(false);
    }

    private void ActivateField(InputField inputField)
    {
        inputField.transform.parent.gameObject.SetActive(true);
    }

    private void OnEndTurnButtonClick()
    {
        this.moveCategory = ChallengeMove.MOVE_CATEGORY_END_TURN;
        DeactivateAllFields();
    }

    private void OnPlayCardButtonClick()
    {
        this.moveCategory = ChallengeMove.MOVE_CATEGORY_PLAY_MINION;
        DeactivateAllFields();
        ActivateField(this.cardIdInputField);
        ActivateField(this.fieldIndexInputField);
    }

    private void OnPlaySpellGeneralButtonClick()
    {
        this.moveCategory = ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_GENERAL;
        DeactivateAllFields();
        ActivateField(this.cardIdInputField);
        ActivateField(this.fieldIdInputField);
    }

    private void OnPlaySpellTargetedButtonClick()
    {
        this.moveCategory = ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED;
        DeactivateAllFields();
        ActivateField(this.cardIdInputField);
        ActivateField(this.fieldIdInputField);
        ActivateField(this.targetIdInputField);
    }

    private void OnCardAttackButtonClick()
    {
        this.moveCategory = ChallengeMove.MOVE_CATEGORY_CARD_ATTACK;
        DeactivateAllFields();
        ActivateField(this.cardIdInputField);
        ActivateField(this.fieldIdInputField);
        ActivateField(this.targetIdInputField);
    }

    private void OnSubmitMoveButtonClick()
    {
        List<ChallengeMove> challengeMoves = new List<ChallengeMove>();
        ChallengeMove challengeMove = new ChallengeMove();
        ChallengeMove.ChallengeMoveAttributes attributes = new ChallengeMove.ChallengeMoveAttributes();

        challengeMove.SetCategory(this.moveCategory);
        challengeMove.SetPlayerId(BattleManager.Instance.ActivePlayer.Id);

        if (this.moveCategory == ChallengeMove.MOVE_CATEGORY_END_TURN)
        {

        }
        else if (this.moveCategory == ChallengeMove.MOVE_CATEGORY_PLAY_MINION)
        {
            attributes.SetCardId(this.cardIdInputField.text);
            attributes.SetFieldIndex(Int32.Parse(fieldIndexInputField.text));
        }
        else if (this.moveCategory == ChallengeMove.MOVE_CATEGORY_CARD_ATTACK)
        {
            attributes.SetCardId(this.cardIdInputField.text);
            attributes.SetFieldId(this.fieldIdInputField.text);
            attributes.SetTargetId(this.targetIdInputField.text);
        }
        else if (this.moveCategory == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_GENERAL)
        {
            attributes.SetCardId(this.cardIdInputField.text);
            attributes.SetFieldId(this.fieldIdInputField.text);
        }
        else if (this.moveCategory == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED)
        {
            attributes.SetCardId(this.cardIdInputField.text);
            attributes.SetFieldId(this.fieldIdInputField.text);
            attributes.SetTargetId(this.targetIdInputField.text);
        }

        challengeMove.SetMoveAttributes(attributes);
        challengeMoves.Add(challengeMove);

        BattleSingleton.Instance.EmitChallengeMoves(challengeMoves);
    }
}
