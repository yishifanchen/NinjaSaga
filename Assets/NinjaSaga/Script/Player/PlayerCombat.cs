using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitState))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCombat : MonoBehaviour {
    [Header("Linked Components")]
    private UnitAnimator animator;
    private UnitState playerState;
    private Rigidbody rb;

    private InputManager inputManager;

    private void Start()
    {
        animator = GetComponentInChildren<UnitAnimator>();
        playerState = GetComponent<UnitState>();
        rb = GetComponent<Rigidbody>();
        inputManager = GameObject.FindObjectOfType<InputManager>();
    }
}
