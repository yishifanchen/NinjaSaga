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
    private Dictionary<string, PlayerEffect> playerEffectDict = new Dictionary<string, PlayerEffect>();

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = transform.parent.GetComponent<Rigidbody>();

        if (!animator) Debug.LogError("No animator found inside" + gameObject.name);
        if (!rb) Debug.LogError("No rigidbody component found on " + gameObject.name);

        currentDirection = DIRECTION.Right;

        PlayerEffect[] peArray = gameObject.transform.parent.GetComponentsInChildren<PlayerEffect>();
        foreach (PlayerEffect pe in peArray)
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
    /// <summary>
    /// adds a small forward force
    /// </summary>
    /// <param name="force"></param>
    public void AddForce(float force)
    {
        StartCoroutine(AddForceCoroutine(force));
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
    public void ShowEffect(string str)
    {
        playerEffectDict[str].ShowEffect();
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
