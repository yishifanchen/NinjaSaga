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
    public float ZSpeed = 1.5f;//Z轴速度
    public float rotationSpeed = 15;//转向速度
    public float jumpRotationSpeed = 30;//跳跃转向速度

    [Header("Stats")]
    public DIRECTION currentDirection;
    public Vector2 inputDirection;
    public bool isGrounded=true;

    private Vector3 fixedVelocity;
    private bool updateVelocity;

    private List<UNITSTATE> MovementStates = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK,
        UNITSTATE.JUMPING
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
    private void FixedUpdate()
    {
        if (updateVelocity)
        {
            rb.velocity = fixedVelocity;
            updateVelocity = false;
        }
    }
    private void InputEvent(Vector2 dir)
    {
        inputDirection = dir;
        MoveGrounded();
    }
    private void MoveGrounded()
    {
        if (rb != null && inputDirection.sqrMagnitude > 0)
        {
            SetVelocity(new Vector3(inputDirection.x * -walkSpeed, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, inputDirection.y * -ZSpeed));
            SetPLayerState(UNITSTATE.WALK);
        }
        else
        {
            SetVelocity(new Vector3(0, rb.velocity.y + Physics.gravity.y * Time.fixedDeltaTime, 0));
            SetPLayerState(UNITSTATE.IDLE);
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
    public void SetPLayerState(UNITSTATE state)
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
            transform.rotation = Quaternion.LookRotation(newDir);
            currentDirection = dir;
        }
    }
    private void OnEnable()
    {
        InputManager.onInputEvent += InputEvent;
    }
    private void OnDisable()
    {
        InputManager.onInputEvent -= InputEvent;
    }
    
}
public enum DIRECTION
{
    Right=-1,
    Left=1,
    Up=2,
    Down=-2
}
