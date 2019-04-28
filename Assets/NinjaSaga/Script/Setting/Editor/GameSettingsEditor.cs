using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameSettingsEditor : EditorWindow {
    private GameSettings settings;
    private string databasePath;

    [MenuItem("Tools/Game Settings")]
    public static void Init()
    {
        GameSettingsEditor window = EditorWindow.GetWindow<GameSettingsEditor>();
        window.minSize = new Vector2(500,200);
        window.Show();
    }
    private void OnEnable()
    {
        if (settings == null) LoadSettings();
    }
    private void OnDisable()
    {
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        DisplayMainArea();
        EditorGUILayout.EndVertical();
    }
    void LoadSettings()
    {
        databasePath = "Assets/NinjaSaga/Setting/Resources/GameSettings.asset";
        settings = (GameSettings)AssetDatabase.LoadAssetAtPath(databasePath, typeof(GameSettings));
        if (settings == null) CreatDatabase();
    }
    void CreatDatabase()
    {
        settings = ScriptableObject.CreateInstance<GameSettings>();
        AssetDatabase.CreateAsset(settings, databasePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }
    private void BeginHeader(string label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    }
    private void DisplayWarningText(string label)
    {
        GUIStyle style2 = new GUIStyle(EditorStyles.wordWrappedLabel);
        style2.wordWrap = true;
        style2.stretchHeight = true;
        style2.normal.textColor = Color.red;
        EditorGUILayout.LabelField(label, style2);
    }
    void DisplayMainArea()
    {
        EditorGUIUtility.labelWidth = 270;

        EditorGUILayout.Space();
        BeginHeader("Global Game Settings");
        settings.timeScale = EditorGUILayout.FloatField(new GUIContent("TimeScale:"), settings.timeScale);
        settings.framerate = EditorGUILayout.IntField(new GUIContent("Framerate:"), settings.framerate);
        settings.showFPSCounter = EditorGUILayout.Toggle(new GUIContent("Show FPS counter:"), settings.showFPSCounter);
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        BeginHeader("Global Audio Settings");
        settings.SFXVolume = EditorGUILayout.FloatField(new GUIContent("SFX Volume: "), Mathf.Clamp(settings.SFXVolume, 0f, 1f));
        settings.MusicVolume = EditorGUILayout.FloatField(new GUIContent("Music Volume: "), Mathf.Clamp(settings.MusicVolume, 0f, 1f));
        EditorGUILayout.Space();
    }
}
