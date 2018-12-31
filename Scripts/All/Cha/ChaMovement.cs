using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaMovement : MonoBehaviour {
    
    #region public
    public float speed;
    public float turnspeed;
    public float jumpspeed;

    public const string startingPositionKey = "starting position";
    #endregion

    #region private
    private bool spacePermit;
    private Vector3 movement;
    private Vector3 worldMovement;
    #endregion

    #region definition
    Rigidbody Cha;
    #endregion

    #region baseFunction
    private void Start()
    {
        Cha = GetComponent<Rigidbody>();
        spacePermit = true;
    }

    private void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        move(v, h);
    }
    #endregion

    #region function
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ground")
        {
            spacePermit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "ground")
        {
            spacePermit = false;
        }
    }

    private void move(float v, float h)
    {
        if (spacePermit)
        {
            if (v > 0 && h == 0)
            {
                movement.Set(0, Cha.velocity.y, 1.5f * speed * v);
            }
            else if (v <= 0 && h == 0)
            {
                movement.Set(0, Cha.velocity.y, 1f * speed * v);
            }
            else if (h != 0 && v > 0)
            {
                movement.Set(0.8f * speed * h, Cha.velocity.y, 0.8f * speed * v);
            }
            else if (h != 0 && v < 0)
            {
                movement.Set(0.7f * speed, Cha.velocity.y, 0.7f * speed * v);
            }
            else if (v == 0)
            {
                movement.Set(1.1f * speed * h, Cha.velocity.y, 0);
            }

            worldMovement = transform.TransformDirection(movement);
            Cha.velocity = worldMovement;
        }
        if(Input.GetKeyDown(KeyCode.Space) && spacePermit)
        {
            Cha.AddForce(new Vector3(0, jumpspeed, 0));
        }
        
    }
    #endregion


}
