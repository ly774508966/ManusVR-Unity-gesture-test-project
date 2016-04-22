using UnityEngine;
using System.Collections;

public enum Finger {
    Thumb = 0,
    IndexFinger = 1,
    MiddleFinger = 2,
    RingFinger = 3,
    Pink = 4
}

/// <summary>
/// FingerParts on a finger, from a knuckle to the fingertop
/// </summary>
public enum FingerPart
{
    Knuckle = 0,
    ProximalPhalanges = 1,
    IntermediatePhalanges = 2,
    DistalPhalanges = 3
}

public enum HandGesture
{
    Default = 0,
    Point = 1,
    FuckYou = 2,
    ThumbsUp = 3,
    Fist = 4
}

