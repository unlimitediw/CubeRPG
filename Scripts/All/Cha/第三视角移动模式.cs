using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 第三视角移动模式 : MonoBehaviour {

    #region 速度参数
    public float speed;
    public float turnSpeed;
    public float jumpSpeed;
    #endregion

    #region 检测参数
    private bool movePermit;
    #endregion

    #region 预设
    private Vector3 movement;
    private Vector3 worldMovement;
    private Rigidbody Cha;          //角色

    private void Start()
    {
        Cha = GetComponent<Rigidbody>();
        movePermit = true;
    }
    #endregion

    #region 更新区
    //根据固定时间更新
    private void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
    }
    #endregion

    #region 检测方法
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ground")
        {
            movePermit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "ground")
        {
            movePermit = false;
        }
    }
    #endregion

    #region 移动方法
    private void Move(float v,float h)
    {
        if (movePermit)
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
            else if (h!= 0&&v < 0)
            {
                movement.Set(0.7f * speed * h, Cha.velocity.y, 0.7f * speed * v);
            }
            else if (v == 0)
            {
                movement.Set(1.1f * speed * h, Cha.velocity.y, 0);
            }

            worldMovement = transform.TransformDirection(movement);
            Cha.velocity = worldMovement;
        }
        if (Input.GetKeyDown(KeyCode.Space) && movePermit)
        {
            Cha.AddForce(new Vector3(0, jumpSpeed, 0));
        }
    }
    #endregion

}
