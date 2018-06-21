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
    
	private static string MOVE_CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
	private static string MOVE_CATEGORY_PLAY_CARD = "MOVE_CATEGORY_PLAY_CARD";
	private static string MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";

	public void Awake()
	{
		this.endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
		this.playCardButton.onClick.AddListener(OnPlayCardButtonClick);
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
		this.moveCategory = MOVE_CATEGORY_END_TURN;
		DeactivateAllFields();
	}

    private void OnPlayCardButtonClick()
	{
		this.moveCategory = MOVE_CATEGORY_PLAY_CARD;
		DeactivateAllFields();
		ActivateField(this.cardIdInputField);
		ActivateField(this.fieldIndexInputField);
	}

	private void OnCardAttackButtonClick()
	{
		this.moveCategory = MOVE_CATEGORY_CARD_ATTACK;
		DeactivateAllFields();
		ActivateField(this.cardIdInputField);
		ActivateField(this.fieldIdInputField);
		ActivateField(this.targetIdInputField);
	}

    private void OnSubmitMoveButtonClick()
	{
		List<ChallengeMove> challengeMoves = new List<ChallengeMove>();
		ChallengeMove challengeMove = new ChallengeMove();
		ChallengeMove.Attributes attributes = new ChallengeMove.Attributes();

		if (this.moveCategory == MOVE_CATEGORY_END_TURN)
		{
			challengeMove.SetCategory(MOVE_CATEGORY_END_TURN);
		}
		else if (this.moveCategory == MOVE_CATEGORY_PLAY_CARD)
		{
			challengeMove.SetCategory(MOVE_CATEGORY_PLAY_CARD);
			attributes.SetCardId(this.cardIdInputField.text);
			attributes.SetFieldIndex(Int32.Parse(fieldIndexInputField.text));
		}

		challengeMove.SetMoveAttributes(attributes);
		challengeMoves.Add(challengeMove);

		BattleSingleton.Instance.EmitChallengeMoves(challengeMoves);
	}
}
