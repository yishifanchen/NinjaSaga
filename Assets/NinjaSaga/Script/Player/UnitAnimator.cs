using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    [HideInInspector]
    public DIRECTION currentDirection;//当前方向

    [HideInInspector]
    public Animator animator;
    private Rigidbody rb;
    private Dictionary<string, AnimationEventEffects> playerEffectDict = new Dictionary<string, AnimationEventEffects>();
    //private Dictionary<string, PlayerEffect> playerEffectDict = new Dictionary<string, PlayerEffect>();


    private string[] resetTriggerName = { "GeneralAttack1" , "GeneralAttack2", "GeneralAttack3", "GeneralAttack4", "GeneralAttack5", "Skill1", "Skill2", "Skill3", "Skill4", "Charge_Start", "Charge_Release" };
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = transform.parent.GetComponent<Rigidbody>();

        if (!animator) Debug.LogError("No animator found inside" + gameObject.name);
        if (!rb) Debug.LogError("No rigidbody component found on " + gameObject.name);

        currentDirection = DIRECTION.Right;

        //PlayerEffect[] peArray = gameObject.transform.parent.GetComponentsInChildren<PlayerEffect>();
        //foreach (PlayerEffect pe in peArray)
        //{
        //    playerEffectDict.Add(pe.gameObject.name, pe);
        //}
        AnimationEventEffects[] effectArray= gameObject.transform.parent.GetComponentsInChildren<AnimationEventEffects>();
        foreach (AnimationEventEffects pe in effectArray)
        {
            playerEffectDict.Add(pe.gameObject.name, pe);
        }
    }

    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }
    public void SetAnimatorBool(string name, bool state)
    {
        animator.SetBool(name, state);
    }
    public void SetAnimatorFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }
    public void SetDirection(DIRECTION dir)
    {
        currentDirection = dir;
    }
    public void ResetTrigger()
    {
        for (int i = 0; i < resetTriggerName.Length; i++)
        {
            animator.ResetTrigger(resetTriggerName[i]);
        }
    }
    /// <summary>
    /// adds a small forward force
    /// </summary>
    /// <param name="force"></param>
    public void AddForce(float force)
    {
        StartCoroutine(AddForceCoroutine(force));
    }
    /// <summary>
    /// 添加三维方向的力
    /// </summary>
    /// <param name="v3Force"></param>
    public void AddVector3Force(string v3Force)
    {
        Vector3 force = new Vector3(float.Parse(v3Force.Split(',')[0]),
            float.Parse(v3Force.Split(',')[1]),
            float.Parse(v3Force.Split(',')[2]));
        StartCoroutine(AddForceV3Coroutine(force, float.Parse(v3Force.Split(',')[3])));
    }
    /// <summary>
    /// 随着时间推移增加了较小的力
    /// </summary>
    /// <param name="force"></param>
    /// <returns></returns>
    IEnumerator AddForceCoroutine(float force)
    {
        DIRECTION startDir = currentDirection;
        float speed = 8;
        float t = 0;
        while (t < 1)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = Vector2.right * (int)startDir * Mathf.Lerp(force, rb.velocity.y, MathUtilities.Sinerp(0, 1, t));
            t += Time.fixedDeltaTime * speed;
            yield return null;
        }
    }
    /// <summary>
    /// 随着时间推移增加了较小的力
    /// </summary>
    /// <param name="force"></param>
    /// <returns></returns>
    IEnumerator AddForceV3Coroutine(Vector3 v3Force,float timer)
    {
        DIRECTION startDir = currentDirection;
        float speed = 8;
        float t = 0;
        while (t < timer)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = (int)startDir * v3Force;
            t += Time.fixedDeltaTime * speed;
            yield return null;
        }
    }
    public void ShowEffect(string str)
    {
        playerEffectDict[str].InstantiateEffect(0,0);
    }
    public void Check4Hit()
    {
        PlayerCombat playerCombat = transform.parent.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.CheckForHit();
        }
        else
        {
            Debug.LogError("No playerCombat component find on object"+transform.parent);
        }
    }
}
