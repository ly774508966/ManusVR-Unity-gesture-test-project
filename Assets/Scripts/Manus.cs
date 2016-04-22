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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ManusMachina {
    /// <summary>
    /// ManusController class to load/unload Manus library.
    /// To be assigned to an empty GameObject
    /// </summary>
    public class Manus {

        internal const int ERROR = -1;
        internal const int SUCCESS = 0;
        internal const int INVALID_ARGUMENT = 1;
        internal const int OUT_OF_RANGE = 2;
        internal const int DISCONNECTED = 3;

#if UNITY_STANDALONE //Linux, OSX, Windows
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusGetData(GLOVE_HAND hand, ref GLOVE_DATA data, uint timeout = 0);
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusGetSkeletal(GLOVE_HAND hand, ref GLOVE_SKELETAL model, uint timeout = 0);
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusSetHandedness(GLOVE_HAND hand, bool right_hand);
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusCalibrate(GLOVE_HAND hand, bool gyro = true, bool accel = true, bool fingers = false);
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusSetVibration(GLOVE_HAND hand, float power);
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusInit();
        [DllImport("Manus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ManusExit();
#endif // UNITY_STANDALONE

#if UNITY_ANDROID
        private static AndroidJavaObject Android;

        internal static int ManusGetData(GLOVE_HAND hand, ref GLOVE_DATA data, uint timeout = 0) {
            float[] AndroidData = Android.Call<float[]>("getData", (int)hand);
            if (AndroidData.Length != 15) return ERROR;

            data.Acceleration = new GLOVE_VECTOR(new List<float>(AndroidData).GetRange(0, 3).ToArray());
            data.Euler = new GLOVE_VECTOR(new List<float>(AndroidData).GetRange(3, 3).ToArray());
            data.Quaternion = new GLOVE_QUATERNION(new List<float>(AndroidData).GetRange(6, 4).ToArray());
            data.Fingers = new List<float>(AndroidData).GetRange(10, 5).ToArray();
            return SUCCESS;
        }

        internal static int ManusSetHandedness(GLOVE_HAND hand, bool right_hand) {
            return Android.Call<int>("setHandedness", (int)hand, right_hand);
        }

        internal static int ManusSetVibration(GLOVE_HAND hand, float power) {
            return Android.Call<int>("setVibration", (int)hand, power);
        }

        internal static int ManusCalibrate(GLOVE_HAND hand, bool gyro = true, bool accel = true, bool fingers = false) {
            return Android.Call<int>("calibrate", (int)hand, gyro, accel, fingers);
        }

        internal static int ManusInit() {
            if (Android == null) {
                using (AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject joContext = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                        Android = new AndroidJavaObject("com.manusmachina.labs.manussdk.Unity", joContext);
                    }
                }
                if (null != Android) return SUCCESS;
            }
            return ERROR;
        }

        internal static int ManusExit() {
            if (Android != null) {
                Android.Dispose();
                Android = null;
                return SUCCESS;
            }
            return ERROR;
        }
#endif // UNITY_ANDROID
    }


#pragma warning disable 0649 // Disable 'field never assigned' warning

    /// <summary>
    ///  Quaternion representing an orientation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GLOVE_QUATERNION {
        public float w, x, y, z;

        internal GLOVE_QUATERNION(float w, float x, float y, float z) {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal GLOVE_QUATERNION(float[] a) {
            this.w = a[0];
            this.x = a[1];
            this.y = a[2];
            this.z = a[3];
        }
    }

    /// <summary>
    /// Three element vector, can either represent a rotation, translation or acceleration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GLOVE_VECTOR {
        public float x, y, z;

        internal GLOVE_VECTOR(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal GLOVE_VECTOR(float[] a) {
            this.x = a[0];
            this.y = a[1];
            this.z = a[2];
        }
    }

    /// <summary>
    /// Pose structure representing an orientation and position. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct GLOVE_POSE {
        public GLOVE_QUATERNION orientation;
        public GLOVE_VECTOR position;
    }

    /// <summary>
    /// Raw data packet from the glove.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GLOVE_DATA {

        /// <summary>
        /// Linear acceleration vector in Gs.
        /// </summary>
        public GLOVE_VECTOR Acceleration;
        /// <summary>
        /// Orientation in euler angles.
        /// </summary>
        public GLOVE_VECTOR Euler;
        /// <summary>
        /// Orientation in quaternions.
        /// </summary>
        public GLOVE_QUATERNION Quaternion;
        /// <summary>
        /// Normalized bend value for each finger ranging from 0 to 1.
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] Fingers;
        /// <summary>
        /// Sequence number of the data packet.
        /// </summary>
        uint PacketNumber;
    }

    /// <summary>
    /// Structure containing the pose of each bone in the thumb.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct GLOVE_THUMB {
        public GLOVE_POSE metacarpal, proximal,
           distal;
    }

    /// <summary>
    /// Structure containing the pose of each bone in a finger.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct GLOVE_FINGER {
        public GLOVE_POSE metacarpal, proximal,
           intermediate, distal;
    }

    /// <summary>
    /// Skeletal model of the hand which contains a pose for the palm and all the bones in the fingers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct GLOVE_SKELETAL {
        public GLOVE_POSE palm;
        public GLOVE_FINGER thumb, index, middle,
           ring, pinky;
    }

#pragma warning restore 0649
    /// <summary>
    /// Indicates which hand is being queried for.
    /// </summary>
    public enum GLOVE_HAND {
        GLOVE_LEFT = 0,
        GLOVE_RIGHT,
    };



}