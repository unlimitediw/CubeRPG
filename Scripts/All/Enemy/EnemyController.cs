using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public Animator anim;
    public float totalHealth;
    public float currentHealth;
    public float expGranted;
    public float atkDamage;
    public float atkSpeed;
    public float moveSpeed;
    public float weight = 1;


    /*MyRpgController[] rpgPlayers; */
    MyRpgController myRpg;
    Rigidbody Mummy;
    Collider MummyCollider;

    private bool death = false;

    private void Start()
    {
        currentHealth = totalHealth;
        Mummy = GetComponent<Rigidbody>();
        MummyCollider = GetComponent<Collider>();
        GameObject myRpgGameObject = GameObject.FindGameObjectWithTag("Player");
        myRpg = myRpgGameObject.GetComponent<MyRpgController>();
        //多人游戏版本
        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0;i< players.Length;i++)
        {
            rpgPlayers[i] = players[i].GetComponent<MyRpgController>();
        }*/
        //单人与多队友

    }

    private void Update()
    {

    }

    public void GetHit(float damage)
    {
        if (death) return;
        anim.SetInteger("Condition", 3);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            StartCoroutine(Death());
            return;
        }
        StartCoroutine(recoverFormHit());

    }



    IEnumerator recoverFormHit()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetInteger("Condition", 0);
    }

    IEnumerator Death()
    {
        death = true;
        anim.SetInteger("Condition", 4);
        /*foreach(MyRpgController pc in rpgPlayers)
        {
            pc.getExperience(expGranted * (1.05f - 0.05f * rpgPlayers.Length));
        }*/
        myRpg.SetExperience(expGranted * weight);
        yield return new WaitForSeconds(3f);
        MummyCollider.enabled = false;
        Mummy.velocity = new Vector3(0,-0.5f,0);
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
