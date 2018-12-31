using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MyRpgController : MonoBehaviour
{
    #region Variables

    //经验
    private int level = 1;
    public Text levelText;
    public Text hpText;
    public Text mpText;
    public Text apText;
    public Text expText;
    public float experience { get; private set; }
    public Slider experienceBar;
    public Slider hpSlider;
    public Slider mpSlider;
    public Slider apSlider;

    //元素
    Rigidbody Cha;
    protected Animator animator;

    //世界本地转换
    private Vector3 movement;
    private Vector3 worldMovement;

    //跳跃
    public float gravity = -9.8f;
    public float jumpSpeed = 12;
    public float doubleJumpSpeed = 12;
    float fallingVelocity = -1f;
    bool canJump;
    bool isJumping = false;
    bool isGrounded;
    bool isFalling;
    bool startFall;
    bool doubleJumping = true;
    bool canDoubleJump = false;
    bool isDoubleJumping = false;
    bool doubleJumped = false;
    public AudioSource JumpAudio;
    public AudioSource LandAudio;
    public AudioSource WalkAndRunAudio;

    //行走
    bool canMove = true;
    public float walkSpeed = 0f;
    public float runSpeed = 6f;
    float moveSpeed;
    Vector3 inputVec;
    Vector3 newVelocity;

    //空中活动
    public float inAirSpeed = 8f;
    float maxVelocity = 2f;
    float minVelocity = -2f;

    //isStarfing
    bool canAction = true;
    bool isBlocking = false;
    bool isDead = false;
    bool isStrafing = true;
    bool ASD = false;
    bool isKnockback;
    private float incapacitatedTime = 0;
    public float knockbackMultiplier = 1f;


    //攻防
    private Weapon weapon;
    public float weaponDamage;
    public float bonusDamage;
    private float attackDamage;

    public float attackSpeed;
    public float attackSpeedBonus;

    public float attackRange;
    private List<Transform> enemiesInRange = new List<Transform>();

    //数值
    private float MaxHp = 200;
    private float MaxMp = 200;
    private float MaxAp = 100;
    private float currentHp = 200;
    private float currentMp = 100;
    private float currentAp = 50;

    public int strength;//{ get; private set; }
    public int vitality;// { get; private set; }
    public int agility;// { get; private set; }
    public int intelligence { get; private set; }


    //鼠标操作与旋转
    public float currentRX = 0.0f;
    public float currentRY = 0.0f;
    private bool Right;
    public float Rsmoothing = 5f;

    //镜头
    public static bool close = false;
    #endregion

    //面板显示状态机

    //读存
    public SaveData playerSaveData;
    public const string startingPositionKey = "starting position";

    #region Initalization

    private void Start()
    {
        /*string startingPositionName = "";
        playerSaveData.Load(startingPositionKey, ref startingPositionName);
        Transform startingPosition = StartingPosition.FindStartingPosition(startingPositionName);
        transform.position = startingPosition.position;
        transform.rotation = startingPosition.rotation;*/

        //动画预设
        animator = GetComponentInChildren<Animator>();
        Cha = GetComponent<Rigidbody>();
        AnimationEvents.OnSlashAnimationHit += _ProductHit;
        AnimationEvents.Jump += JumpVoice;
        AnimationEvents.Landed += LandVoice;
        AnimationEvents.Foot += WalkAndRunVoice;

        stateUpdate();
        setAttackDamage();
        getAttackSpeed();
    }

    #endregion

    #region UpdateAndInput

    private void Update()
    {
        //面板显示
        //动作许可检索
        if (incapacitatedTime > 0) return;
        if (animator)
        {
            if (!isDead)
            {
                if (Input.GetKeyDown(KeyCode.Z) && canAction && isGrounded && !isBlocking)
                {
                    Attack(1);
                }

                if (Input.GetKeyDown(KeyCode.V) && canAction && isGrounded && !isBlocking)
                {
                    Attack(1);
                }
            }
            Jumping();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            GetHit(10);
        }
    }

    private void FixedUpdate()
    {
        if (incapacitatedTime > 0) return;
        //使用重力
        CheckForGrounded();
        Cha.AddForce(0, gravity, 0, ForceMode.Acceleration);
        AirControl();
        //行动检索
        if (canMove && !isBlocking && !isDead)
        {
            moveSpeed = UpdateMovement();
            if (Input.GetKey(KeyCode.W) && !ASD)
            {

                isStrafing = false;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                isStrafing = true;
                ASD = true;
            }
            else
            {
                ASD = false;
            }
        }
        //下落检索
        if (Cha.velocity.y < fallingVelocity)
        {
            isFalling = true;
            animator.SetInteger("Jumping", 2);
            canJump = false;
        }

    }

    private void LateUpdate()
    {
        float velocityXel = transform.InverseTransformDirection(Cha.velocity).x;
        float velocityZel = transform.InverseTransformDirection(Cha.velocity).z;

        animator.SetFloat("Velocity X", velocityXel / runSpeed);
        animator.SetFloat("Velocity Z", velocityZel / runSpeed);

        if (!isDead && canMove)
        {
            if (moveSpeed > 0)
            {
                animator.SetBool("Moving", true);
                if (isStrafing)
                {
                    animator.SetBool("Strafing", true);
                }
                else
                {
                    animator.SetBool("Strafing", false);
                }
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }
    }

    #endregion

    #region UpdateMovement

    void moveBaseValue()
    {
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        movement.Set(x, 0, z);
        worldMovement = transform.TransformDirection(movement);
        inputVec = worldMovement;
    }

    float UpdateMovement()
    {
        moveBaseValue();
        Vector3 motion = inputVec;
        if (isGrounded)
        {
            if (motion.magnitude > 1)
            {
                motion.Normalize();
            }
            //地面速度
            if (canMove && !isBlocking)
            {
                if (isStrafing)
                {
                    newVelocity = new Vector3(motion.x * walkSpeed, newVelocity.y, motion.z * walkSpeed);
                }
                else
                {
                    newVelocity = new Vector3(motion.x * runSpeed, newVelocity.y, motion.z * runSpeed);
                }
            }
        }
        else
        {
            //惯性速度
            newVelocity = Cha.velocity;
        }
        newVelocity.y = Cha.velocity.y;
        Cha.velocity = newVelocity;
        //返回移动值
        return inputVec.magnitude;
    }

    #endregion

    #region Jumping

    void CheckForGrounded()
    {
        //地面检测
        float distanceToGround;
        float threshold = 0.45f;//临界值
        RaycastHit hit;
        Vector3 offset = new Vector3(0, 0.4f, 0);
        if (Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f))
        {
            distanceToGround = hit.distance;
            if (distanceToGround < threshold)
            {
                isGrounded = true;
                canJump = true;
                startFall = false;
                doubleJumped = false;
                canDoubleJump = false;
                isFalling = false;
                if (!isJumping)
                {
                    animator.SetInteger("Jumping", 0);
                }
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = true;
        }
    }

    void Jumping()
    {
        if (isGrounded)
        {
            if (canJump && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(_Jump());
            }
        }
        else
        {
            canDoubleJump = true;
            canJump = false;
            if (isFalling)
            {
                //转换为下落动画
                animator.SetInteger("Jumping", 2);
                //地面动画判定
                if (!startFall)
                {
                    animator.SetTrigger("JumpTrigger");
                    startFall = true;
                }
            }
            if (canDoubleJump && doubleJumping && Input.GetKeyDown(KeyCode.Space) && !doubleJumped && isFalling)
            {
                //二次速迭代
                Cha.velocity += doubleJumpSpeed * Vector3.up;
                animator.SetInteger("Jumping", 3);
                doubleJumped = true;
            }
        }
    }

    void JumpVoice()
    {
        JumpAudio.Play();
    }

    void LandVoice()
    {
        LandAudio.Play();
    }

    void WalkAndRunVoice()
    {
        WalkAndRunAudio.Play();
    }

    IEnumerator _Jump()
    {
        isJumping = true;
        animator.SetInteger("Jumping", 1);
        animator.SetTrigger("JumpTrigger");
        //一次速迭代
        Cha.velocity += jumpSpeed * Vector3.up;
        canJump = false;
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }

    void AirControl()
    {
        if (!isGrounded)
        {
            Vector3 motion = inputVec;
            motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;
            Cha.AddForce(motion * inAirSpeed, ForceMode.Acceleration);
            //速度限制
            float velocityX = 0;
            float velocityZ = 0;
            if (Cha.velocity.x > maxVelocity)
            {
                velocityX = GetComponent<Rigidbody>().velocity.x - maxVelocity;
                if (velocityX < 0)
                {
                    velocityX = 0;
                }
                Cha.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
            }
            if (Cha.velocity.x < minVelocity)
            {
                velocityX = Cha.velocity.x - minVelocity;
                if (velocityX > 0)
                {
                    velocityX = 0;
                }
                Cha.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
            }
            if (Cha.velocity.z > maxVelocity)
            {
                velocityZ = Cha.velocity.z - maxVelocity;
                if (velocityZ < 0)
                {
                    velocityZ = 0;
                }
                Cha.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
            }
            if (Cha.velocity.z < minVelocity)
            {
                velocityZ = Cha.velocity.z - minVelocity;
                if (velocityZ > 0)
                {
                    velocityZ = 0;
                }
                Cha.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
            }
        }
    }

    #endregion

    #region MiscMethods
    //攻击元素混合

    //0 = no side
    //1 = left
    //2 = right
    //3 = dual

    //武装准备
    void setAttackDamage()
    {
        attackDamage = GameLogic.CalculatePlayerBaseAttackDamage(this) + weaponDamage + bonusDamage;
    }
    void getAttackSpeed()
    {
        attackSpeed = GameLogic.CalculatePlayerBaseAttackSpeed(this) + attackSpeedBonus + 0.9f;
    }

    //攻击事件
    void Attack(int attackSide)
    {
        if (canAction)
        {
            if (weapon == Weapon.UNARMED)
            {
                float maxAttacks = 3f;
                if (attackSide == 1)
                {
                    float attackNumber = Mathf.Floor(Random.Range(1.3f, maxAttacks));
                    {
                        if (isGrounded)
                        {

                            animator.SetTrigger("Attack" + (3 * attackNumber).ToString() + "Trigger");
                            animator.speed = attackSpeed;
                            StartCoroutine(_LockAttack(attackSpeed));
                            StartCoroutine(_LockMovementForAttack(0.1f, 0.7f));

                            /*
                            StartCoroutine(_LockMovementAndAttack(0, 2f));
                            */
                        }
                    }
                }
            }
        }
    }

    void _ProductHit()
    {
        getEnemiesInRange();
        foreach (Transform enemy in enemiesInRange)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec == null) continue;
            ec.GetHit(attackDamage);
        }
    }

    void GetHit(float damage)
    {
        if (!isDead)
        {
            currentHp -= damage;
            stateUpdate();
            if (currentHp <= 0)
            {
                StartCoroutine(_Death());
            }
            int hits = 5;
            int hitNumber = Random.Range(0, hits);
            animator.SetTrigger("GetHit" + (hitNumber + 1).ToString() + "Trigger");
            GetIncapacitated(0.3f);


            //震退
            if (hitNumber <= 1)
            {
                StartCoroutine(_Knockback(-transform.forward, 8, 4));
            }
            if (hitNumber == 2)
            {
                StartCoroutine(_Knockback(transform.forward, 8, 4));
            }
            if (hitNumber == 3)
            {
                StartCoroutine(_Knockback(transform.right, 8, 4));
            }
            if (hitNumber == 4)
            {
                StartCoroutine(_Knockback(-transform.right, 8, 4));
            }
        }
    }

    private void GetIncapacitated(float time)
    {
        if(incapacitatedTime < time)
        {
            StopCoroutine(GetIncapacitataedRoutine());
            incapacitatedTime = time;
            StartCoroutine(GetIncapacitataedRoutine());
        }
    }

    IEnumerator GetIncapacitataedRoutine()
    {
        while(incapacitatedTime > 0)
        {
            yield return new WaitForSeconds(0.1f);
            incapacitatedTime -= 0.1f;
        }
    }

    IEnumerator _Knockback(Vector3 knockDirection,int knockbackAmount,int variableAmount)
    {
        isKnockback = true;
        StartCoroutine(_KnockbackForce(knockDirection, knockbackAmount, variableAmount));
        yield return new WaitForSeconds(0.1f);
        isKnockback = false;
    }
    
    IEnumerator _KnockbackForce(Vector3 knockDirection, int knockbackAmount, int variableAmount)
    {
        while(isKnockback)
        {
            Cha.AddForce(knockDirection * ((knockbackAmount + Random.Range(-variableAmount, variableAmount)) * knockbackMultiplier * 10), ForceMode.Impulse);
            yield return null;
        }
    }

    IEnumerator _Death()
    {
        animator.SetTrigger("Death1Trigger");
        /*
        StartCoroutine(_LockMovementAndAttack(0.1f, 1.5f));
        */
        StartCoroutine(_LockAttack(attackSpeed));
        StartCoroutine(_LockMovementForAttack(0.1f, 1.5f));
        isDead = true;
        animator.SetBool("Moving", false);
        inputVec = new Vector3(0, 0, 0);
        yield return null;
    }

    IEnumerator _Revive()
    {
        animator.SetTrigger("ReviveTrigger");
        isDead = false;
        yield return null;
    }

    IEnumerator _LockMovementForAttack(float delayTime, float lockTime)
    {
        canMove = false;
        Cha.velocity = Vector3.zero;
        Cha.angularVelocity = Vector3.zero;
        inputVec = Vector3.zero;
        yield return new WaitForSeconds(delayTime);
        animator.SetBool("Moving", false);
        animator.applyRootMotion = true;
        yield return new WaitForSeconds(lockTime);
        canMove = true;
        animator.applyRootMotion = false;
    }

    IEnumerator _LockAttack(float attackSpeed)
    {
        canAction = false;
        yield return new WaitForSeconds(1 / attackSpeed);
        animator.speed = 1;
        canAction = true;
    }

    
    #endregion

    #region EnemyInteraction
    void getEnemiesInRange()
    {
        enemiesInRange.Clear();
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * 1f), 1f))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemiesInRange.Add(c.transform);
            }
        }
        
    }
    #endregion

    #region NpcInteraction
    public void OnInteractionF(Interactable interactable)
    {

    }
    #endregion

    #region _Coroutines

    //锁定行动
    /*
    public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
    {
        yield return new WaitForSeconds(delayTime);
        canAction = false;
        canMove = false;
        animator.SetBool("Moving", false);
        Cha.velocity = Vector3.zero;
        Cha.angularVelocity = Vector3.zero;
        inputVec = Vector3.zero;
        animator.applyRootMotion = true;
        yield return new WaitForSeconds(lockTime);
        canAction = true;
        canMove = true;
        animator.applyRootMotion = false;
    }
    */
    #endregion

    #region incomes



    public void SetExperience(float exp)
    {
        experience += exp;
        float experienceNeeded = GameLogic.ExperienceForNextLevel(level);
        float previousExperience = GameLogic.ExperienceForNextLevel(level - 1);
        //LevelUp
        while(experience >= (experienceNeeded - previousExperience))
        {
            experience = experience - (experienceNeeded - previousExperience);
            LevelUp();
            experienceNeeded = GameLogic.ExperienceForNextLevel(level);
            previousExperience = GameLogic.ExperienceForNextLevel(level - 1);
        }
        experienceBar.value = experience/(experienceNeeded - previousExperience);

        expText.text = experience + "/" + (experienceNeeded - previousExperience);
    }

    void LevelUp()
    {
        level++;
        levelText.text = level.ToString();
    }

    #endregion
    
    #region state
    void stateUpdate()
    {
        hpSlider.value = currentHp / MaxHp;
        hpText.text = currentHp + "/" + MaxHp;
        mpSlider.value = currentMp / MaxMp;
        mpText.text = currentMp + "/" + MaxMp;
        apSlider.value = currentAp / MaxAp;
        apText.text = currentAp + "/" + MaxAp;
    }
    #endregion
    
    #region collider
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "wall")
        {
            close = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall")
        {
            close = false;
        }
    }
    #endregion
}