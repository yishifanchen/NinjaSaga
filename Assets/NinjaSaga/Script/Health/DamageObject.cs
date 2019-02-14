using UnityEngine;
[System.Serializable]
public class DamageObject
{
    public string animTrigger = "";//动画trigger
    public int damage;
    public float duration = 1f;
    public float comboResetTime = .5f;
    public string hitSFX = "";
    public bool knockDown;
    public bool slowMotionEffect;
    public bool defenceOverride;
    public bool isGroundAttack;

    [Header("Hit Collider Settings")]
    public float collSize;
    public float collDistance;
    public float collHeight;

    [HideInInspector]
    public GameObject inflictor;
    public DamageObject(int _damage, GameObject _inflictor)
    {
        damage = _damage;
        inflictor = _inflictor;
    }
}