using UnityEngine;
/// <summary>
/// 存活时间
/// </summary>
public class TimeToLive : MonoBehaviour {
    public float lifeTime = 0.5f;
	void Start () {
        Invoke("DestroyGO",lifeTime);
	}
	void DestroyGO()
    {
        Destroy(gameObject);
    }
}
