using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public HandController left;
    public HandController right;
    
    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        GameObject leftHand = GameObject.FindGameObjectWithTag("LeftHand");
        GameObject rightHand = GameObject.FindGameObjectWithTag("RightHand");
        left = leftHand.GetComponent<HandController>();
        right = rightHand.GetComponent<HandController>();
    }

    public static GameManager GetGameManager()
    {
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        //left.DrawDebugRayFingers();
        //right.DrawDebugRayFingers();
        if (right.hg == HandGesture.Point)
        {
            Ray r = right.GetFingerRay(Finger.IndexFinger, FingerPart.DistalPhalanges);
            Debug.DrawRay(r.origin, r.direction, Color.red);
        }
    }
}
