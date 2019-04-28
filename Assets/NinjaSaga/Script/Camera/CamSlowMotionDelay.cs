using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSlowMotionDelay : MonoBehaviour {

    public float slowMotionTimeScale = .2f;
    public void StartSlowMotionDelay(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SlowMotionRoutine(duration));
    }
    IEnumerator SlowMotionRoutine(float duration)
    {
        Time.timeScale = slowMotionTimeScale;

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < (startTime + duration))
        {
            yield return null;
        }

        GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
        if (settings != null)
        {
            Time.timeScale = settings.timeScale;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
