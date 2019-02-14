using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitState))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Linked Components")]
    public Transform weaponBone;
    private UnitAnimator animator;
    private UnitState playerState;
    private Rigidbody rb;

    [Header("Attack Date & Combos")]
    public float hitZRange = 2;//Z轴攻击范围
    private int attackNum = -1;//当前攻击组合编号

    [Space(5)]
    public DamageObject[] generalAttackCombo;//普攻连击数据
    public DamageObject heavyBlowData;//重击数据
    private DamageObject lastAttack;

    [Header("Stats")]
    public DIRECTION currentDirection;
    private float lastAttackTime = 0;//time of the last attack
    private bool continueGeneralAttackCombo;//true if a generalattack combo need to continue;

    [SerializeField]
    private bool isGrounded;
    private InputManager inputManager;
    private INPUTACTION lastAttackInput;
    private DIRECTION lastAttackDirection;

    private void Start()
    {
        animator = GetComponentInChildren<UnitAnimator>();
        playerState = GetComponent<UnitState>();
        rb = GetComponent<Rigidbody>();
        inputManager = GameObject.FindObjectOfType<InputManager>();
    }
    int anjiancount = 0;
    int zhixingcount = 0;
    private void Update()
    {
        if (animator) isGrounded = animator.animator.GetBool("isGrounded");
        if (Input.GetKeyDown(KeyCode.X))
        {
            anjiancount++;
            //print("按键次数" + anjiancount + "\\执行次数" + zhixingcount);
        }
    }
    private void MovementInputEvent(Vector2 inputVector)
    {

    }
    private void CombatInputEvent(INPUTACTION action)
    {
        //普攻
        if (action == INPUTACTION.GENERALATTACK && playerState.currentState != UNITSTATE.GENERALATTACK && isGrounded)
        {
            //continue to the next attack if the time is inside the combo window   最后一次攻击时间，最后一次攻击的持续时间，最后一次攻击连击的重置时间
            bool insideComboWindow = (lastAttack != null && Time.time < (lastAttackTime + lastAttack.duration + lastAttack.comboResetTime));
            if (insideComboWindow && !continueGeneralAttackCombo && attackNum < generalAttackCombo.Length - 1)
                attackNum += 1;
            else
                attackNum = 0;
            if (generalAttackCombo[attackNum] != null && generalAttackCombo[attackNum].animTrigger.Length > 0)
                DoAttack(generalAttackCombo[attackNum], UNITSTATE.GENERALATTACK, INPUTACTION.GENERALATTACK);
            return;
        }
        //如果在一次普攻动作进行中又按下了普攻键，则进行普攻连击
        if (action == INPUTACTION.GENERALATTACK && (playerState.currentState == UNITSTATE.GENERALATTACK) && !continueGeneralAttackCombo && isGrounded)
        {
            if (attackNum < generalAttackCombo.Length - 1)
            {
                continueGeneralAttackCombo = true;
                return;
            }
        }
    }
    private void DoAttack(DamageObject d, UNITSTATE state, INPUTACTION inputAction)
    {
        animator.SetAnimatorTrigger(d.animTrigger);
        playerState.SetState(state);
        lastAttack = d;
        lastAttack.inflictor = gameObject;
        lastAttackTime = Time.time;
        lastAttackDirection = currentDirection;
        Invoke("Ready", d.duration);
    }
    /// <summary>
    /// 攻击结束，玩家可以进行下一次动作
    /// </summary>
    public void Ready()
    {
        if (continueGeneralAttackCombo)
        {
            continueGeneralAttackCombo = false;
            if (attackNum < generalAttackCombo.Length - 1)
                attackNum += 1;
            else
                attackNum = 0;
            if (generalAttackCombo[attackNum] != null && generalAttackCombo[attackNum].animTrigger.Length > 0)
                DoAttack(generalAttackCombo[attackNum], UNITSTATE.GENERALATTACK, INPUTACTION.GENERALATTACK);
            return;
        }

        playerState.SetState(UNITSTATE.IDLE);
    }
    private void OnEnable()
    {
        InputManager.onInputEvent += MovementInputEvent;
        InputManager.onCombatInputEvent += CombatInputEvent;
    }
    private void OnDisable()
    {
        InputManager.onInputEvent -= MovementInputEvent;
        InputManager.onCombatInputEvent -= CombatInputEvent;
    }
}
