using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public Material rayColor;
    public LineRenderer lr;
    public GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameManager.GetGameManager();
        lr.SetWidth(0.05f, 0.05f);
    }
	
	// Update is called once per frame
	void Update () {
        if(gm.right.hg == HandGesture.Point)
        {
            Debug.Log("Point");
            DrawRay();
        }
        else
        {
            if (lr.enabled)
            {
                lr.enabled = false;
            }
        }
	}

    public void DrawRay()
    {
        Ray r = gm.right.GetFingerRay(Finger.IndexFinger, FingerPart.DistalPhalanges);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            lr.enabled = true;
            lr.SetPositions(new Vector3[] { r.origin, hit.point });
        }
        else
        {
            lr.enabled = false;
        }
    }

    /*
    void OnPostRender()
    {
        if (!rayColor)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        Ray r = gm.right.GetFingerRay(Finger.IndexFinger, FingerPart.DistalPhalanges);
        GL.PushMatrix();
        rayColor.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(r.origin);
        GL.Vertex(r.direction);
        GL.End();
        GL.PopMatrix();
    }*/
}
