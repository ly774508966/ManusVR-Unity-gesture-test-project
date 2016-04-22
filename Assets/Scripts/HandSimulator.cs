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

namespace ManusMachina {
    public class HandSimulator {
        private GameObject gameObject;
        private AnimationClip animationClip;
        private static float timeFactor = 10.0f;

        /// <summary>
        /// Constructor which loads the HandModel
        /// </summary>
        public HandSimulator(GLOVE_HAND gh) {
            if (gh == GLOVE_HAND.GLOVE_LEFT) {
                gameObject = Resources.Load<GameObject>("Manus_Handv2_Left");
                animationClip = Resources.Load<AnimationClip>("Manus_Handv2_Left");
            } else {
                gameObject = Resources.Load<GameObject>("Manus_Handv2_Right");
                animationClip = Resources.Load<AnimationClip>("Manus_Handv2_Right");
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Finds a deep child in a transform
        /// </summary>
        /// <param name="aParent">Transform to be searched</param>
        /// <param name="aName">Name of the (grand)child to be found</param>
        /// <returns></returns>
        private static Transform FindDeepChild(Transform aParent, string aName) {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent) {
                result = FindDeepChild(child, aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Updates a skeletal from glove data
        /// </summary>
        /// <param name="data">The glove data to be used for the update</param>
        /// <param name="rootTransform">Root of the transform to be updated</param>
        /// <param name="hand">Hand to be updated</param>
        internal void Simulate(GLOVE_DATA data, ref Transform rootTransform, GLOVE_HAND hand) {
            for (int i = 0; i < 5; i++) {
                animationClip.SampleAnimation(gameObject, data.Fingers[i] * timeFactor);
                GameObjectToSkeletal(ref rootTransform, i, data.Quaternion, hand);
            }
        }

        /// <summary>
        /// Gets the transform of a fingertop.
        /// </summary>
        /// <param name="data">Glove data</param>
        /// <param name="finger">The finger you want</param>
        /// <param name="rootTransform">Transform of hand</param>
        /// <param name="hand">Which hand it is</param>
        /// <returns>The fingertop from the finger you want</returns>
        internal Transform GetFinger(GLOVE_DATA data, int finger, ref Transform rootTransform, GLOVE_HAND hand)
        {
            int j = 3;
            return FindDeepChild(rootTransform, "Finger_" + finger.ToString() + j.ToString());
        }

        /// <summary>
        /// Gets the transoform of a fingerPart.
        /// </summary>
        /// <param name="data">Glove data</param>
        /// <param name="finger">The finger you want</param>
        /// <param name="fingerPart">The fingerpart you want, for example the fingertop (Distal phalanges)</param> 
        /// <param name="rootTransform">Transform of hand</param>
        /// <param name="hand">Which hand it is</param>
        /// <returns>The fingertop from the finger you want</returns>
        internal Transform GetFinger(GLOVE_DATA data, int finger, FingerPart fingerPart, ref Transform rootTransform, GLOVE_HAND hand)
        {
            int part = (int)fingerPart;
            return FindDeepChild(rootTransform, "Finger_" + finger.ToString() + part.ToString());
        }

        /// <summary>
        /// Updates a Finger in a hand model
        /// </summary>
        /// <param name="t">Root of the transform to be updated</param>
        /// <param name="finger">Finger number to be updated</param>
        /// <param name="quat">Manus Quaternion to be used</param>
        /// <param name="hand">Hand to be updates</param>
        private void GameObjectToSkeletal(ref Transform t, int finger, GLOVE_QUATERNION q, GLOVE_HAND hand) {
            if (null == t) return;
            t.localRotation = new Quaternion(-q.y, -q.z, q.x, q.w);
            for (int j = 0; j < 4; j++) {
                FindDeepChild(t, "Finger_" + finger.ToString() + j.ToString()).localRotation =
                    FindDeepChild(gameObject.transform, "Finger_" + finger.ToString() + j.ToString()).localRotation;
                //Debug.Log(FindDeepChild(t, "Finger_" + finger.ToString() + j.ToString()).localRotation);
                //Debug.Log(FindDeepChild(gameObject.transform, "Finger_" + finger.ToString() + j.ToString()).localRotation);
            }
        }
    }
}