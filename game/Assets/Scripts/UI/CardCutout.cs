using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCutout : ObjectUI
{
    CollectionCardObject source;
    HyperCard.Card visual;

    // Use this for initialization
    public void Initialize(CollectionCardObject source) //, List<Card> deck)
    {
        this.source = source;
        this.visual = this.GetComponent<HyperCard.Card>();
        this.SetCardArtwork();

        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>() as BoxCollider;
        base.Initialize();
    }

    private void SetCardArtwork()
    {
        //this.visual.SetFrontTiling(this.templateData.frontScale, this.templateData.frontOffset);
        //this.visual.SetBackTiling(this.templateData.backScale, this.templateData.backOffset);
        this.visual.SetCardArtwork(this.source.frontImage, this.source.backImage);

        this.visual.Stencil = ActionManager.Instance.stencilCount;
        ActionManager.Instance.stencilCount += 3 % 255;
    }

    public void PositionForDecklist(int index)
    {
        gameObject.transform.parent = GameObject.Find("Build Panel").transform;
        //this.PositionCutout(deck.Count);

        gameObject.transform.localPosition = new Vector3((index + 0.5F) * 0.44F, Random.Range(-0.05F, 0.05F), 0F);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.Rotate(new Vector3(0f, 180f, -90f), Space.Self);
    }

    public override void MouseDown()
    {
        CollectionManager.Instance.RemoveFromDecklist(this.source, this);
        Destroy(gameObject);
    }
}
