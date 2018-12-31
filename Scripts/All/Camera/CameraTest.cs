using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour {

    public Camera cam1;
    public Camera cam2;

    public Rigidbody Cha;
    public Transform cha;
    public Transform chaa;
    public float currentRX = 0.0f;
    public float currentRY = 0.0f;
    public float Rsmoothing = 5f;
    public float scrollSpeed;
    public float currentScrollSpeed;
    public float scrollSmoothing = 5f;
    public LayerMask layerMask;
    

    private float distance = 30.0f;
    public float Kdistance = 30.0f;
    private float stackValue = 0.0f;
    private float Sdistance = 0.0f;
    private float k = 3;

    private bool Right;
    private bool Ypermit;
    private bool wallPermit = false;
    private bool freeze = false;
    private bool Spleft;

    

    private void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;
        currentRY = 10.0f;
        Ypermit = true;
        wallPermit = true;
    }

    private void Update()
    {
        float afterScrollSpeed = scrollSpeed * (distance + 10);
        if (Input.GetKey(KeyCode.Mouse1) || (Input.GetKey(KeyCode.Mouse0) && Spleft))
        {
                Right = true;
                currentRX += Input.GetAxis("Mouse X");
                if (Ypermit)
                {
                    currentRY -= Input.GetAxis("Mouse Y");
                    if (Input.GetAxis("Mouse Y") <= 0)
                    {
                        stackValue -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                    else
                    {
                        stackValue = 0;
                    }
                    if (stackValue < 0)
                    {
                        Kdistance -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                }
                else
                {
                    if (Input.GetAxis("Mouse Y") >= 0)
                    {
                        Kdistance -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                        stackValue -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                    else
                    {
                        currentRY -= Input.GetAxis("Mouse Y");
                    }
                }
        }
        if (Input.GetKey(KeyCode.Mouse0) && !Spleft)
        {
                Right = false;
                if (Ypermit)
                {
                    currentRX += Input.GetAxis("Mouse X");
                    currentRY -= Input.GetAxis("Mouse Y");
                    if (Input.GetAxis("Mouse Y") <= 0)
                    {
                        stackValue -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                    else
                    {
                        stackValue = 0;
                    }
                    if (stackValue < 0)
                    {
                        Kdistance -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                }
                else
                {
                    if (Input.GetAxis("Mouse Y") >= 0)
                    {
                        Kdistance -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                        stackValue -= Input.GetAxis("Mouse Y") * currentScrollSpeed;
                    }
                    else
                    {
                        currentRY -= Input.GetAxis("Mouse Y");
                    }
                }
        }
        Kdistance -= Input.GetAxis("Mouse ScrollWheel") * afterScrollSpeed;
        currentRY = Mathf.Clamp(currentRY, 0, 90);
        Kdistance = Mathf.Clamp(Kdistance, 3, 60);
        distance = Mathf.Lerp(distance, Kdistance, scrollSmoothing * Time.deltaTime);

        getInteraction();

    }

    void getInteraction()
    {
        RaycastHit hit;
        Vector3 direction = transform.position - Cha.position;
        if (Physics.Raycast(Cha.position, direction, out hit, 100f, layerMask))
        {

            /*if(MyRpgController.close && wallPermit)
            {
                cam1.enabled = false;
                cam2.enabled = true;
            }
            else if(wallPermit)
            {
                cam2.enabled = false;
                cam1.enabled = true;
                if (hit.distance - 3 <= distance)
                    distance = hit.distance - 3 - Time.deltaTime * 1000;
                cam2.enabled = false;
                cam1.enabled = true;
                if (hit.distance - 3 <= distance)
                    distance = hit.distance - 3;
            }
            else
            {
                cam2.enabled = false;
                cam1.enabled = true;
                if (hit.distance - 3 <= distance)
                    distance = hit.distance - 3;
            }*/
            //最终版本，但是超长距离的夹角问题还是没有解决
            if (hit.distance > 7 && !(MyRpgController.close && wallPermit))
            {
                cam1.enabled = true;
                cam2.enabled = false;
                if (hit.distance - 3 <= distance)
                    distance = hit.distance - 3;
                Spleft = false;
            }
            else
            {
                cam2.enabled = true;
                cam1.enabled = false;
                if (hit.distance - 3 <= distance)
                    distance = hit.distance - 3;
                Spleft = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "ground")
        {
            Ypermit = false;
        }
        if(other.tag == "wall")
        {
            wallPermit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ground")
        {
            Ypermit = true;
        }
        if(other.tag == "wall")
        {
            wallPermit = false;
        }
    }



    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        if (Right)
        {
            Quaternion rotation = Quaternion.Euler(currentRY, currentRX, 0);
            transform.position = cha.position + rotation * dir;
            Quaternion targetRotation = Quaternion.Euler(0, currentRX, 0);
            Cha.rotation = Quaternion.Slerp(Cha.rotation, targetRotation, Rsmoothing * Time.deltaTime);
            transform.LookAt(chaa);
        }
        if(!Right)
        {
            Quaternion rotation = Quaternion.Euler(currentRY, currentRX, 0);
            transform.position = cha.position + rotation * dir;
            transform.LookAt(chaa);
        }

    }
}
