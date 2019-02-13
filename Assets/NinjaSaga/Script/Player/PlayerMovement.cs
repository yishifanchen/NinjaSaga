using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnitState))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour {

    [Header("Link Components")]
    private UnitAnimator animator;
    private Rigidbody rb;
    private UnitState playerState;
    private CapsuleCollider capsule;

    [Header("Settings")]
    public float walkSpeed = 3f;//行走速度
    public float zSpeed = 1.5f;//Z轴速度
    public float rotationSpeed = 15;//转向速度
    public float jumpRotationSpeed = 30;//跳跃转向速度
    public float jumpForce = 8f;//跳跃力大小，决定跳的高度
    public float landRecoveryTime = .1f;//落地起身恢复时间
    public float AirMaxSpeed = 3f;
    public float AirAcceleration = 3f;
    public bool AllowDepthJumping;//是否允许在跳跃过程中移动z轴
    public LayerMask collisionLayer;

    [Header("Stats")]
    public DIRECTION currentDirection;
    public Vector2 inputDirection;
    public bool isGrounded=true;
    public bool jumpInProgress;

    private Vector3 fixedVelocity;
    private bool updateVelocity;

    private List<UNITSTATE> MovementStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK,
        UNITSTATE.JUMPING,
        UNITSTATE.LAND
    };

    private void Start()
    {
        if (!animator) animator = GetComponentInChildren<UnitAnimator>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!playerState) playerState = GetComponent<UnitState>();
        if (!capsule) capsule = GetComponent<CapsuleCollider>();

        if (!animator) Debug.LogError("No animator found inside"+gameObject.name);
        if (!rb) Debug.LogError("No rigidbody component found on " + gameObject.name);
        if (!playerState) Debug.LogError("No UnitState component found on " + gameObject.name);
        if (!capsule) Debug.LogError("No Capsule Collider found on " + gameObject.name);
    }
    void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (animator)
        {
            animator.SetAnimatorBool("isGrounded", isGrounded);
            animator.SetAnimatorBool("Falling", !isGrounded && rb.velocity.y < 0.1f);
        }

        if (updateVelocity&&MovementStates.Contains(playerState.currentState))
        {
            rb.velocity = fixedVelocity;
            updateVelocity = false;
        }
    }
    private void InputEvent(Vector2 dir)
    {
        inputDirection = dir;
        if (MovementStates.Contains(playerState.currentState))
        {
            if (jumpInProgress)
                MoveAirborne();
            else
                MoveGrounded();
        }
    }
    private void InputEventAction(INPUTACTION action)
    {
        if (MovementStates.Contains(playerState.currentState))
        {
            if (action == INPUTACTION.JUMP)
            {
                if (playerState.currentState != UNITSTATE.JUMPING && IsGrounded())
                {
                    StopAllCoroutines();
                    StartCoroutine(DoJump());
                }
            }
        }
    }
    /// <summary>
    /// 判断是否在地上
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        Vector3 bottomCapsulePos = transform.position + (Vector3.up) * (capsule.radius - 0.1f);
        if (Physics.CheckCapsule(transform.position + capsule.center, bottomCapsulePos, capsule.radius, collisionLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        return isGrounded;
    }
    private void MoveGrounded()
    {
        if (rb != null && inputDirection.sqrMagnitude > 0)
        {
            SetVelocity(new Vector3(inputDirection.x * -walkSpeed, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, inputDirection.y * -zSpeed));
            SetPlayerState(UNITSTATE.WALK);
        }
        else
        {
            SetVelocity(new Vector3(0, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, 0));
            SetPlayerState(UNITSTATE.IDLE);
        }

        //set current direction based on the input vector.(ignore up and down by using 'mathf.sign' because we want the player to stay int current direction when moving up/down)
        int dir = Mathf.RoundToInt(Mathf.Sign((float)-inputDirection.x));
        if (Mathf.Abs(inputDirection.x) > 0)
        {
            currentDirection = (DIRECTION)dir;
        }
        LookToDir(currentDirection);
        animator.SetAnimatorFloat("MovementSpeed",rb.velocity.magnitude);
    }
    private void MoveAirborne()
    {
        if (true)//后续更改
        {
            int lastKnownDirection = (int)currentDirection;
            if (Mathf.Abs(inputDirection.x) > 0)
            {
                lastKnownDirection = Mathf.RoundToInt(-inputDirection.x);
            }
            LookToDir((DIRECTION)lastKnownDirection);

            int dir = Mathf.Clamp(Mathf.RoundToInt(-inputDirection.x), -1, 1);
            float xpeed = Mathf.Clamp(rb.velocity.x + AirMaxSpeed * dir * Time.fixedDeltaTime * AirAcceleration, -AirMaxSpeed, AirMaxSpeed);
            if (!updateVelocity)
            {
                SetVelocity(new Vector3(xpeed, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, AllowDepthJumping ? -inputDirection.y * zSpeed : 0));
            }
        }
    }
    /// <summary>
    /// 设置移动速度
    /// </summary>
    /// <param name="velocity"></param>
    void SetVelocity(Vector3 velocity)
    {
        fixedVelocity = velocity;
        updateVelocity = true;
    }
    /// <summary>
    /// 设置玩家动作状态
    /// </summary>
    /// <param name="state"></param>
    public void SetPlayerState(UNITSTATE state)
    {
        if (playerState != null)
            playerState.SetState(state);
        else
            Debug.LogError("No playerstate found on this gamaobject");
    }
    public void LookToDir(DIRECTION dir)
    {
        Vector3 newDir = Vector3.zero;
        if (dir == DIRECTION.Right || dir == DIRECTION.Left)
        {
            if (isGrounded)
            {
                newDir = Vector3.RotateTowards(transform.forward, Vector3.forward * -(int)dir, rotationSpeed*Time.deltaTime,0);
            }
            else
            {
                newDir = Vector3.RotateTowards(transform.forward, Vector3.forward * -(int)dir, jumpRotationSpeed * Time.deltaTime, 0);
            }
            transform.rotation = Quaternion.LookRotation(newDir);
            currentDirection = dir;
        }
    }
    IEnumerator DoJump()
    {
        //设置跳跃状态
        jumpInProgress = true;
        playerState.SetState(UNITSTATE.JUMPING);

        //播放动画
        animator.SetAnimatorBool("JumpInProgress", true);
        animator.SetAnimatorTrigger("JumpUp");
        
        //set state
        yield return new WaitForFixedUpdate();

        //start jump
        while (isGrounded)
        {
            SetVelocity(Vector3.up * jumpForce);
            yield return new WaitForFixedUpdate();
        }

        //continue until we hit the ground
        while (!isGrounded)
        {
            yield return new WaitForFixedUpdate();
        }

        //落地
        SetVelocity(Vector3.zero);
        playerState.SetState(UNITSTATE.LAND);

        animator.SetAnimatorFloat("MovementSpeed",0f);
        animator.SetAnimatorBool("JumpInProgress",false);

        jumpInProgress = false;
        if (playerState.currentState == UNITSTATE.LAND)
        {
            yield return new WaitForSeconds(landRecoveryTime);
            SetPlayerState(UNITSTATE.IDLE);
        }
    }
    private void OnEnable()
    {
        InputManager.onInputEvent += InputEvent;
        InputManager.onCombatInputEvent += InputEventAction;
    }
    private void OnDisable()
    {
        InputManager.onInputEvent -= InputEvent;
        InputManager.onCombatInputEvent -= InputEventAction;
    }
    
}
public enum DIRECTION
{
    Right=-1,
    Left=1,
    Up=2,
    Down=-2
}
