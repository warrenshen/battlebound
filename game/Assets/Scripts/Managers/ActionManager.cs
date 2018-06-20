using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionManager : MonoBehaviour {
    public bool allowPan = false;
    public float cardOffsetFromCamera = 8.0f;

    private float SCROLL_DAMPING = 0.1f;
    private float M_THRESHOLD = 2000f;
    private float L_SPEED = 9f;

    [SerializeField]
    private CardObject target;
    private Quaternion[] dragTilts = new Quaternion[5];

    //for storing prev state values
    private SpriteRenderer sp;
    private int selectedSortingOrder;

    private int boardLayerMask;

    public static ActionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        if (Application.loadedLevelName == "Battle")
            boardLayerMask = LayerMask.GetMask("Board");
        InitializeDragTilts();
    }

    // Update is called once per frame
    private void Update () {
        if(allowPan) ScrollToPan(new Vector3(0f, 1f, 0f));
        if(target) {
            RepositionCard(target);
            AdjustCardTilt(target);
            if (Input.GetMouseButtonUp(0))
                ClearDragTarget();
        }
	}

    public void SetDragTarget(CardObject target, SpriteRenderer target_sp) {
        this.target = target;
        this.sp = target_sp;
        selectedSortingOrder = this.sp.sortingOrder;
        this.sp.sortingOrder = 100;
    }

    public bool HasDragTarget() {
        return this.target != null;
    }

    public CardObject GetDragTarget() {
        return this.target;
    }

    public void ClearDragTarget()
    {
        if (!this.target)
            return;
        this.sp.sortingOrder = selectedSortingOrder;
        this.target = null;
    }

    public void ResetTarget() {
        //target.transform.localPosition = target.reset.resetPosition;
        //target.transform.localRotation = target.reset.resetRotation;
        //target.transform.localScale = target.reset.resetScale;
        target.card.Owner.Hand.RepositionCards();
    }

    private void DestroyTarget() {
        GameObject.Destroy(target.gameObject);
    }

    private void ScrollToPan(Vector3 axes) {
        Camera.main.transform.Translate(axes * Input.mouseScrollDelta.y * SCROLL_DAMPING);
    }

    private void RepositionCard(CardObject cardObj)
    {
        //set target position by mouse position
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cardOffsetFromCamera));
        cardObj.transform.position = Vector3.Lerp(cardObj.transform.position, endPos, Time.deltaTime * L_SPEED);
    }

    private void AdjustCardTilt(CardObject cardObj)
    {
        Vector3 mDeltaPosition = Camera.main.WorldToScreenPoint(cardObj.transform.position);
        mDeltaPosition = Input.mousePosition - mDeltaPosition;
        var magnitude = mDeltaPosition.sqrMagnitude;
        Vector3 resetRotation = cardObj.reset.resetRotation.eulerAngles;

        mDeltaPosition.Normalize();
        var heading = Vector3.Dot(mDeltaPosition, Vector3.up); //direction

        //determine if significant movement
        if (magnitude < M_THRESHOLD)
        {
            cardObj.transform.localRotation = dragTilts[4]; //no movement, movement has slowed
        }
        else
        {
            //passed threshold, set tilt values
            SetDragTilts(magnitude, resetRotation);

            if (heading >= 0.5f)
            {
                cardObj.transform.localRotation = dragTilts[0];  // Up
            }
            else if (heading <= -0.5f)
            {
                cardObj.transform.localRotation = dragTilts[1];  // Down
            }
            else
            {
                heading = Vector3.Dot(mDeltaPosition, Vector3.right);
                if (heading >= 0.5f)
                {
                    cardObj.transform.localRotation = dragTilts[2];  // Right
                }
                else { cardObj.transform.localRotation = dragTilts[3]; }  // Left 
            }
        }
        mDeltaPosition = Input.mousePosition;
    }

    private void InitializeDragTilts() {
        dragTilts[0].eulerAngles = new Vector3(70f, 0);
        dragTilts[1].eulerAngles = new Vector3(70f, 0);
        dragTilts[2].eulerAngles = new Vector3(70f, 0);
        dragTilts[3].eulerAngles = new Vector3(70f, 0);
        dragTilts[4].eulerAngles = new Vector3(70f, 0);
    }

    private void SetDragTilts(float magnitude, Vector3 resetRotation)
    { // ex: magnitude of 40000 is decently large
        float maxTilt = 15f;
        float normalized = maxTilt * Mathf.Clamp((magnitude - M_THRESHOLD) / 30000, 0f, 1.0f);

        // calculate tilts based off default
        dragTilts[0].eulerAngles = new Vector3(resetRotation.x - normalized, 0);
        dragTilts[1].eulerAngles = new Vector3(resetRotation.x + normalized, 0);
        dragTilts[2].eulerAngles = new Vector3(resetRotation.x, -normalized * 0.5f);
        dragTilts[3].eulerAngles = new Vector3(resetRotation.x, normalized * 0.5f);
        dragTilts[4].eulerAngles = new Vector3(resetRotation.x, 0, 0);
    }

    public void AddCardToDeck(CardObject card) {
        if(CollectionManager.Instance != null)
            CollectionManager.Instance.AddToDeck(card);
    }
}
