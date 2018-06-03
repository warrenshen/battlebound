using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {
    public bool allowPan = false;

    private float SCROLL_DAMPING = 0.1f;
    private float M_THRESHOLD = 2000f;
    private float L_SPEED = 9f;

    private Camera cam;
    private Transform target;
    private SpriteRenderer sp;
    private Quaternion[] dragTilts = new Quaternion[5];

    private Collection collection;

    private void Awake()
    {
        cam = Camera.main;
        collection = cam.GetComponent<Collection>();
    }

    // Update is called once per frame
    void Update () {
        if(allowPan) ScrollToPan(new Vector3(0f, 1f, 0f));
        if(target) {
            RepositionCard(target);
            AdjustCardTilt(target);
        }
        //RaycastMouse();
	}

    private void RaycastMouse() {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Name = " + hit.collider.name);
            }
        }
    }

    public void SetDragTarget(Transform target, SpriteRenderer target_sp) {
        this.target = target;
        this.sp = target_sp;
        this.sp.sortingOrder = 100;
    }

    public bool HasDragTarget() {
        return this.target != null;
    }

    public Transform GetDragTarget() {
        return this.target;
    }

    public void ClearDragTarget()
    {
        this.sp.sortingOrder = 0;
        this.target = null;
    }

    private void ScrollToPan(Vector3 axes) {
        transform.Translate(axes * Input.mouseScrollDelta.y * SCROLL_DAMPING);
    }

    private void RepositionCard(Transform card)
    {
        float SELECTED_CARD_LIFT = 0.72f;

        //set target position by mouse position
        Vector3 endPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z - SELECTED_CARD_LIFT));
        card.position = Vector3.Lerp(card.position, endPos, Time.deltaTime * L_SPEED);
    }

    private void AdjustCardTilt(Transform card)
    {
        Vector3 mDeltaPosition = cam.WorldToScreenPoint(card.position);
        mDeltaPosition = Input.mousePosition - mDeltaPosition;
        var magnitude = mDeltaPosition.sqrMagnitude;

        mDeltaPosition.Normalize();
        var heading = Vector3.Dot(mDeltaPosition, Vector3.up); //direction

        //determine if significant movement
        if (magnitude < M_THRESHOLD)
        {
            card.localRotation = dragTilts[4]; //no movement, movement has slowed
        }
        else
        {
            //passed threshold, set tilt values
            SetDragTilts(magnitude);

            if (heading >= 0.5f)
            {
                card.localRotation = dragTilts[0];  // Up
            }
            else if (heading <= -0.5f)
            {
                card.localRotation = dragTilts[1];  // Down
            }
            else
            {
                heading = Vector3.Dot(mDeltaPosition, Vector3.right);
                if (heading >= 0.5f)
                {
                    card.localRotation = dragTilts[2];  // Right
                }
                else { card.localRotation = dragTilts[3]; }  // Left 
            }
        }
        mDeltaPosition = Input.mousePosition;
    }

    private void SetDragTilts(float magnitude)
    { // ex: magnitude of 40000 is decently large
        float maxTilt = 15f;
        float normalized = maxTilt * Mathf.Clamp((magnitude - M_THRESHOLD) / 30000, 0f, 1.0f);

        // calculate tilts based off default
        dragTilts[0].eulerAngles = new Vector3(normalized, 0);
        dragTilts[1].eulerAngles = new Vector3(-normalized, 0);
        dragTilts[2].eulerAngles = new Vector3(0, -normalized);
        dragTilts[3].eulerAngles = new Vector3(0, normalized);
        //no-tilt
        dragTilts[4].eulerAngles = new Vector3(0, 0, 0);
    }

    public void AddCardToDeck(CardObject card) {
        if(collection != null)
           collection.AddToDeck(card);
    }
}
