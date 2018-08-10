using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to lock mouse interactions while moving CardObject using LeanTween
public static class CardTween
{
    public static float TWEEN_DURATION = 0.5f;

    public static LTDescr move(CardObject target, Vector3 finalPosition, float duration)
    {
        target.noInteraction = true;
        return LeanTween.move(target.gameObject, finalPosition, duration)
            .setOnStart(() => target.visual.Redraw())
            .setOnComplete(() =>
            {
                target.noInteraction = false;
            });
    }

    public static LTDescr moveLocal(CardObject target, Vector3 finalPosition, float duration)
    {
        target.noInteraction = true;
        return LeanTween.moveLocal(target.gameObject, finalPosition, duration)
            .setOnStart(() => target.visual.Redraw())
            .setOnComplete(() =>
            {
                target.noInteraction = false;
            });
    }
}

[System.Serializable]
public class ActionManager : MonoBehaviour
{
    public float cardOffsetFromCamera = 8;

    public static float TWEEN_DURATION = 0.5f;
    private float SCROLL_DAMPING = 0.1f;
    private float M_THRESHOLD = 2000;
    private float L_SPEED = 10;

    private Quaternion[] dragTilts = new Quaternion[5];

    //for storing prev state values
    private SpriteRenderer sp;
    private int selectedSortingOrder;

    private int cardAndUILayerMask;
    public Texture2D[] cursors;

    public int stencilCount;

    public bool active = true;
    public static ActionManager Instance { get; private set; }

    [SerializeField]
    private CardObject target;

    [SerializeField]
    private MouseWatchable lastHitWatchable;

    private void Awake()
    {
        Instance = this;
        this.stencilCount = 3;
    }

    private void Start()
    {
        cardAndUILayerMask = LayerMask.GetMask("Card", "UI");
        InitializeDragTilts();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!active || EffectManager.IsWaiting())
        {
            return;
        }
        MouseWatch();
    }

    private void LateUpdate()
    {
        if (!active)
            return;

        if (target)
        {
            this.AdjustCardTilt(target);
            this.RepositionCard(target);
            if (Input.GetMouseButtonUp(0))
            {
                ClearDragTarget();
            }
        }
    }

    public void SetDragTarget(CardObject target)
    {
        this.target = target;
        this.SetCursor(4);
    }

    private void MouseWatch()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100f))
        {
            return;
        }

        MouseWatchable hitWatchable = hit.collider.GetComponent<MouseWatchable>();
        if (hitWatchable == null || hitWatchable.noInteraction)
        {
            ResetLastHitWatchable();
            return;
        }

        if (hitWatchable != lastHitWatchable)
        {
            ResetLastHitWatchable();
            hitWatchable.EnterHover();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            hitWatchable.MouseDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            hitWatchable.MouseUp();
        }
        lastHitWatchable = hitWatchable;
    }

    private void ResetLastHitWatchable()
    {
        if (lastHitWatchable)
        {
            lastHitWatchable.ExitHover();
            lastHitWatchable = null;
        }
    }

    public bool HasDragTarget()
    {
        return this.target != null;
    }

    public CardObject GetDragTarget()
    {
        return this.target;
    }

    public void ClearDragTarget()
    {
        if (!this.HasDragTarget())
            return;
        this.target = null;
        this.SetCursor(0);
    }

    public LTDescr ResetTarget()
    {
        if (this.target == null)
            return null;
        CardTween.moveLocal(this.target, this.target.reset.position, CardTween.TWEEN_DURATION);
        return LeanTween.rotateLocal(this.target.gameObject, this.target.reset.rotation.eulerAngles, CardTween.TWEEN_DURATION);
    }
    public LTDescr ResetTarget(CardObject cardObject)
    {
        if (cardObject == null)
            return null;
        CardTween.moveLocal(cardObject, cardObject.reset.position, CardTween.TWEEN_DURATION);
        return LeanTween.rotateLocal(cardObject.gameObject, cardObject.reset.rotation.eulerAngles, CardTween.TWEEN_DURATION);
    }

    private void RepositionCard(CardObject cardObject)
    {
        //set target position by mouse position
        Vector3 endPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cardOffsetFromCamera));
        cardObject.transform.position = Vector3.Lerp(cardObject.transform.position, endPos, Time.deltaTime * L_SPEED);
    }

    private void AdjustCardTilt(CardObject cardObject)
    {
        Vector3 mDeltaPosition = Camera.main.WorldToScreenPoint(cardObject.transform.position);
        mDeltaPosition = Input.mousePosition - mDeltaPosition;
        var magnitude = mDeltaPosition.sqrMagnitude;
        Vector3 resetRotation = cardObject.reset.rotation.eulerAngles;

        mDeltaPosition.Normalize();
        var heading = Vector3.Dot(mDeltaPosition, Vector3.up); //direction

        //determine if significant movement
        if (magnitude < M_THRESHOLD)
        {
            cardObject.transform.localRotation = dragTilts[4]; //no movement, movement has slowed
        }
        else
        {
            //passed threshold, set tilt values
            SetDragTilts(magnitude, resetRotation);

            if (heading >= 0.5f)
            {
                cardObject.transform.localRotation = dragTilts[0];  // Up
            }
            else if (heading <= -0.5f)
            {
                cardObject.transform.localRotation = dragTilts[1];  // Down
            }
            else
            {
                heading = Vector3.Dot(mDeltaPosition, Vector3.right);
                if (heading >= 0.5f)
                {
                    cardObject.transform.localRotation = dragTilts[2];  // Right
                }
                else { cardObject.transform.localRotation = dragTilts[3]; }  // Left 
            }
        }
        mDeltaPosition = Input.mousePosition;
    }

    private void InitializeDragTilts()
    {
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

    public void SetCursor(int cursor)
    {
        Cursor.SetCursor(ActionManager.Instance.cursors[cursor], Vector2.zero, CursorMode.Auto);
    }

    public void SetActive(bool val)
    {
        this.active = val;
    }

    private void LoadMainMenu()
    {
        Application.LoadLevel("Menu");
    }
}
