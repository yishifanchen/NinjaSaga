using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour {

    [HideInInspector]
    public Animator animator;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }
    public void SetAnimatorBool(string name,bool state)
    {
        animator.SetBool(name,state);
    }
    public void SetAnimatorFloat(string name,float value)
    {
        animator.SetFloat(name,value);
    }
}
