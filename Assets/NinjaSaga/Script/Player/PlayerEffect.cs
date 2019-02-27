using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家特效
/// </summary>
public class PlayerEffect : MonoBehaviour
{
    ParticleSystem[] ps;
    private void Start()
    {
        ps = transform.GetComponentsInChildren<ParticleSystem>();
    }
    public void ShowEffect()
    {
        foreach(ParticleSystem p in ps)
        {
            p.Play();
        }
    }
}
