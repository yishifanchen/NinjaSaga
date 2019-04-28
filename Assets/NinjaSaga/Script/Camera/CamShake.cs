using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamShake : MonoBehaviour {

    public AnimationCurve camShakeY;
    public AnimationCurve camShakeX;
    public AnimationCurve camShakeZ;
    public float multiplier = 1;
    public bool randomize;
    public float time = .5f;

    public void Shake(float intensity)
    {
        StartCoroutine(DoShake(intensity));
    }
    IEnumerator DoShake(float scale)
    {
        Vector3 rand = new Vector3(GetRandomValue(), GetRandomValue(), GetRandomValue());
        scale *= multiplier;
        float t = 0;
        while (t < time)
        {
            if (randomize)
                transform.localPosition = new Vector3(camShakeX.Evaluate(t) * scale * rand.x, camShakeY.Evaluate(t) * scale * rand.y, camShakeZ.Evaluate(t) * scale * rand.z);
            else
                transform.localPosition = new Vector3(camShakeX.Evaluate(t) * scale, camShakeY.Evaluate(t) * scale, camShakeZ.Evaluate(t) * scale);

            t += Time.deltaTime / time;
            yield return null;
        }
        transform.localPosition = Vector3.zero;
    }
    int GetRandomValue()
    {
        int[] i = { -1,1};
        return i[Random.Range(0, 2)];
    }
}
