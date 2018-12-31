using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {

    public delegate void AnimationEvent();
    public static event AnimationEvent OnSlashAnimationHit;
    public static event AnimationEvent Jump;
    public static event AnimationEvent Foot;
    public static event AnimationEvent Landed;

    void SlashAnimationHitEvent()
    {
        print("event called");
        OnSlashAnimationHit();
    }

    void JumpEvent()
    {
        Jump();
    }

    void FootR()
    {
        Foot();
    }

    void FootL()
    {
        Foot();
    }

    void Land()
    {
        Landed();
    }
}
