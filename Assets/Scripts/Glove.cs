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


namespace ManusMachina {
    /// <summary>
    /// Glove class
    /// </summary>
    [System.Serializable]
    public class Glove {
        private GLOVE_HAND hand;
        private HandSimulator hs;

        /// <summary>
        /// Converts a Quaternion from Manus to Unity format
        /// </summary>
        /// <param name="q">Quaternion in Manus format</param>
        /// <returns>Quaternion in Unity format</returns>
        private static Quaternion ManusToUnity(GLOVE_QUATERNION q) {
            return new Quaternion(-q.y, -q.z, q.x, q.w);
        }

        /// <summary>
        /// Converts a Vector from Manus to Unity format
        /// </summary>
        /// <param name="v">Vector in Manus format</param>
        /// <returns>Vector in Unity format</returns>
        private static Vector3 ManusToUnity(GLOVE_VECTOR v) {
            return new Vector3(-v.x / 100.0f, v.y / 100.0f, v.z / 100.0f);
        }

        /// <summary>
        /// Convert Manus Pose to Unity Transform
        /// </summary>
        /// <param name="unity">Unity Transform Object, which will store the result</param>
        /// <param name="manus">GLOVE_POSE Manus Thumb Struct to be converted</param>
        private void ManusToUnity(ref Transform unity, GLOVE_POSE manus) {
            // Do not update position until positional tracking is implemented.
            //unity.position = ManusToUnity(manus.position);
            unity.rotation = ManusToUnity(manus.orientation);
        }

        /// <summary>
        /// Whether is is a right or left hand as a GLOVE_HAND.
        /// </summary>
        public GLOVE_HAND GloveHand { get { return hand; } }

        /// <summary>
        /// Determine whether a Glove is connected.
        /// Note: the library needs some time to connect. Queuering whether a glove
        /// is connected immediately after calling ManusInit() will return false.
        /// </summary>
        public bool Connected {
            get {
                GLOVE_DATA data = new GLOVE_DATA();
                return Manus.ManusGetData(hand, ref data) == Manus.SUCCESS;
            }
        }

        /// <summary>
        /// Acceleration in Unity format
        /// </summary>
        public Vector3 Acceleration {
            get {
                GLOVE_DATA data = new GLOVE_DATA();
                if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data)) {
                    return ManusToUnity(data.Acceleration);
                } else {
                    return new Vector3();
                }
            }
        }

        /// <summary>
        /// Euler vector in Unity format
        /// </summary>
        public Vector3 Euler {
            get {
                GLOVE_DATA data = new GLOVE_DATA();
                if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data)) {
                    return new Vector3(data.Euler.x, data.Euler.y, data.Euler.z);
                } else {
                    return new Vector3();
                }
            }
        }

        /// <summary>
        /// Fingers array
        /// </summary>
        public float[] Fingers {
            get {
                GLOVE_DATA data = new GLOVE_DATA();
                if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data)) {
                    return data.Fingers;
                } else {
                    return new float[5];
                }
            }
        }

        /// <summary>
        /// Returns the current Quaternion in Unity format. 
        /// </summary>
        public Quaternion Quaternion {
            get {
                GLOVE_DATA data = new GLOVE_DATA();
                if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data)) {
                    return ManusToUnity(data.Quaternion);
                } else {
                    return new Quaternion();
                }
            }
        }

        /// <summary>
        /// Retrieves the current Skeletal from the glove and
        /// converts is from Manus to Unity format.
        /// </summary>
        /// <param name="unitySkel">Skeletal from Unity, which is to be updated.</param>
        public void UpdateTransform(ref Transform rootTransform) {
            GLOVE_DATA data = new GLOVE_DATA();
            if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data))
                hs.Simulate(data, ref rootTransform, hand);
        }

        /// <summary>
        /// Set the ouput power of the vibration motor.
        /// </summary>
        /// <param name="power">The power of the vibration motor ranging from 0 to 1 (ex. 0.5 = 50% power).</param>
        public int SetVibration(float power) {
            return Manus.ManusSetVibration(hand, power);
        }

        /// <summary>
        /// Constructor for Glove class
        /// </summary>
        /// <param name="gh">Left or right glove</param>
        public Glove(GLOVE_HAND gh) {
            hand = gh;
            hs = new HandSimulator(gh);
        }

        public Transform GetFingerObject(ref Transform rootTransform, int finger)
        {
            GLOVE_DATA data = new GLOVE_DATA();
            if (Manus.SUCCESS == Manus.ManusGetData(hand, ref data))
                return hs.GetFinger(data, finger, ref rootTransform, hand);
            return null;
        }
    }

}



