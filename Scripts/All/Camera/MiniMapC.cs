using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapC : MonoBehaviour {

    public Transform Cha;

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - Cha.position;
    }

    private void Update()
    {
        transform.position = Cha.position + offset;
    }

}
