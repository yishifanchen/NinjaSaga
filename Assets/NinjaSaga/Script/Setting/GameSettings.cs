using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject {

    [Header("Application Settings")]
    public float timeScale = 1;
    public int framerate = 60;
    public bool showFPSCounter = false;

    [Header("Audio Settings")]
    public float MusicVolume = .7f;
    public float SFXVolume = .9f;
}
