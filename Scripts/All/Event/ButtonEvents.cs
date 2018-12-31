using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour {

    public Text expText;
    // Use this for initialization
    public void OnEnter()
    {
        expText.enabled = true;
    }

    public void OnExit()
    {
        expText.enabled = false;
    }

}
