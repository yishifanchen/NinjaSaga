using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour,IDamagable<DamageObject> {
    public bool destroyOnHit;
    void Start () {
		
	}
	public void Hit(DamageObject DO)
    {
        print(11);
    }
}
