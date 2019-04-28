using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSlowMotionDelay : MonoBehaviour {

    public float slowMotionTimeScale = .2f;
    public void StartSlowMotionDelay(float duration)
    {

    }
    IEnumerator SlowMotionRoutine(float duration)
    {
        Time.timeScale = slowMotionTimeScale;

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < (startTime + duration))
        {
            yield return null;
        }


    }
}
