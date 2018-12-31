using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour {

    public Slider slider;
    AudioSource BGM;

    private void Start()
    {
        BGM = GetComponent<AudioSource>();
    }

    private void Update()
    {
        BGM.volume = slider.value;
    }
}
