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
    public float walkSpeed = 3f;
    public float ZSpeed = 1.5f;

    private Vector2 inputDirection;
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
        animator.SetAnimatorFloat("MovementSpeed",rb.velocity.magnitude);
    }
    void SetVelocity(Vector3 velocity)
    {
        fixedVelocity = velocity;
        updateVelocity = true;
    }
    public void SetPLayerState(UNITSTATE state)
    {
        if (playerState != null)
            playerState.SetState(state);
        else
            Debug.LogError("No playerstate found on this gamaobject");

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
