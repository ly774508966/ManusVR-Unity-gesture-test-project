/*
   Copyright 2015 Manus VR

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using UnityEngine;
using System;
using ManusMachina;

public class HandController : MonoBehaviour
{
    public Transform rootTranform;
    public GLOVE_HAND hand;
    public HandGesture hg;
    private Glove glove;
    private int testloop;

    void Start()
    {
        Manus.ManusInit();
        glove = new Glove(hand);
        testloop = 0;
        hg = HandGesture.Default;
    }

    void Update()
    {
        if (glove != null)
        {
            glove.UpdateTransform(ref rootTranform);
        }
        hg = GetGesture();
    }


    void onApplicationQuit()
    {
        Manus.ManusExit();
    }

    /// <summary>
    /// Draws a debug ray from the top of the fingers of each hand.
    /// </summary>
    public void DrawDebugRayFingers()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Finger)).Length; i++)
        {
            Transform t = glove.GetFingerObject(ref rootTranform, i);
            Ray r = new Ray(t.position, t.forward);
            Debug.DrawRay(r.origin, r.direction, Color.red);
        }
    }

    /// <summary>
    /// Gets the ray from a finger.
    /// </summary>
    /// <param name="finger">The finger you want to shoot the ray from</param>
    /// <param name="fingerPart">From which fingerpart you want to shoot the ray</param>
    /// <returns></returns>
    public Ray GetFingerRay(Finger finger, FingerPart fingerPart)
    {
        int part = (int)fingerPart;
        int f = (int)finger;
        Transform t = glove.GetFingerObject(ref rootTranform, f);
        Ray r = new Ray(t.position, t.forward);
        return r;
    }

    /// <summary>
    /// Detects a few gestures.
    /// </summary>
    /// <returns>A handgesture</returns>
    public HandGesture GetGesture()
    {
        if(glove.Fingers[(int)Finger.IndexFinger] <= 0.40f && OtherGesturesClosed(Finger.IndexFinger))
        {
            return HandGesture.Point;
        }
        else if(glove.Fingers[(int)Finger.MiddleFinger] <= 0.40f && OtherGesturesClosed(Finger.MiddleFinger))
        {
            return HandGesture.FuckYou;
        }
        else if (glove.Fingers[(int)Finger.Thumb] <= 0.50f && OtherGesturesClosed(Finger.Thumb))
        {
            return HandGesture.ThumbsUp;
        }
        else if (FingersClosed())
        {
            return HandGesture.Fist;
        }
        else
        {
            return HandGesture.Default;
        }
    }

    /// <summary>
    /// Checks if other fingers then the given finger are closed.
    /// </summary>
    /// <param name="finger">The finger that is open.</param>
    /// <returns>If the other fingers are closed.</returns>
    public bool OtherGesturesClosed(Finger finger)
    {
        for (int i = 0; i < Enum.GetValues(typeof(Finger)).Length; i++)
        {
            if((int) finger != i && glove.Fingers[i] <= 0.40f)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if all the fingers are closed.
    /// </summary>
    /// <returns></returns>
    public bool FingersClosed()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Finger)).Length; i++)
        {
            if (glove.Fingers[i] <= 0.40f)
            {
                return false;
            }
        }
        return true;
    }
}

    /*
    void FixedUpdate()
    {
        testloop++;
        if (testloop >= 120)
        {
            testloop = 0;
            //Waarde 0 is open, 1 is closed.
            //Debug.Log(hand.ToString() + glove.Fingers[(int) Finger.Thumb]); //duim 
            //Debug.Log(hand.ToString() + glove.Fingers[(int) Finger.IndexFinger]); //wijsvinger
            //Debug.Log(hand.ToString() + glove.Fingers[(int) Finger.MiddleFinger]); //middelvinger
            //Debug.Log(hand.ToString() + glove.Fingers[(int) Finger.RingFinger]); //ringvinger
            //Debug.Log(hand.ToString() + glove.Fingers[(int) Finger.Pink]); //pink

            //Debug.Log(rootTranform.position); 
            
            if (glove.Fingers[1] >= 0.60f)
            {
                Transform t = glove.GetFingerObject(ref rootTranform, (int)Finger.IndexFinger);
                Debug.Log(t.position);
                Ray r = new Ray(t.position, Vector3.up);
                Debug.DrawRay(r.origin, r.direction, Color.red);
            } 
        }
        DrawRayFingers();
    }*/