using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(LineRenderer) )]
public class CurvedLineRenderer : MonoBehaviour 
{
	//PUBLIC
	public float lineSegmentSize = 0.15f;
	[Header("Gizmos")]
	public bool showGizmos = true;
	public float gizmoSize = 0.1f;
	public Color gizmoColor = new Color(1,0,0,0.5f);
	//PRIVATE
	private CurvedLinePoint[] linePoints = new CurvedLinePoint[0];
	private Vector3[] linePositions = new Vector3[0];
	private Vector3[] linePositionsOld = new Vector3[0];

    private float damping = 0.7f;
    public float lineWidth = 1;

    public LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    public void Update () 
	{
		GetPoints();
		SetPointsToLine();
        UseWidth();
	}

	void GetPoints()
	{
		//find curved points in children
		linePoints = this.GetComponentsInChildren<CurvedLinePoint>();
		//add positions
		linePositions = new Vector3[linePoints.Length];
		for( int i = 0; i < linePoints.Length; i++ )
		{
			linePositions[i] = linePoints[i].transform.position;
		}
	}

    public void SetPointPositions(Vector3 start, Vector3 end) {
        linePoints[0].transform.position = start;
        linePoints[2].transform.position = end;

        Vector3 diff = end - start;
        Vector3 perp = Vector3.Cross(diff, Vector3.forward).normalized;
        linePoints[1].transform.position = start + diff/2 + perp * Vector3.Dot(diff.normalized, Vector3.right) * damping;
    }

    public void SetWidth(float width) {
        this.lineWidth = width;

    }

    private void UseWidth() {
        lineRenderer.startWidth = this.lineWidth;
        lineRenderer.endWidth = this.lineWidth;
        lineRenderer.widthMultiplier = this.lineWidth;
    }

	void SetPointsToLine()
	{
		//create old positions if they dont match
		if( linePositionsOld.Length != linePositions.Length )
		{
			linePositionsOld = new Vector3[linePositions.Length];
		}

		//check if line points have moved
		bool moved = false;
		for( int i = 0; i < linePositions.Length; i++ )
		{
			//compare
			if( linePositions[i] != linePositionsOld[i] )
			{
				moved = true;
			}
		}

		//update if moved
		if( moved == true )
		{
			//get smoothed values
			Vector3[] smoothedPoints = LineSmoother.SmoothLine( linePositions, lineSegmentSize );

			//set line settings
            lineRenderer.positionCount = smoothedPoints.Length;
            lineRenderer.SetPositions( smoothedPoints );
		}
	}

	void OnDrawGizmosSelected()
	{
		Update();
	}

	void OnDrawGizmos()
	{
		if( linePoints.Length == 0 )
		{
			GetPoints();
		}

		//settings for gizmos
		foreach( CurvedLinePoint linePoint in linePoints )
		{
			linePoint.showGizmo = showGizmos;
			linePoint.gizmoSize = gizmoSize;
			linePoint.gizmoColor = gizmoColor;
		}
	}
}
