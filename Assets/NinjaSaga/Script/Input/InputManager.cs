using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Keyboard keys")]
    public KeyCode Left = KeyCode.LeftArrow;
    public KeyCode Right = KeyCode.RightArrow;
    public KeyCode Up = KeyCode.UpArrow;
    public KeyCode Down = KeyCode.DownArrow;
    public KeyCode GeneralAttackKey = KeyCode.X;//普攻
    public KeyCode HeavyBlowKey = KeyCode.Z;//重击
    public KeyCode DefendKey = KeyCode.D;//防御、格挡
    public KeyCode JumpKey = KeyCode.C;//跳跃
    public KeyCode Skill1 = KeyCode.S;

    //输入委托
    public delegate void InputEventHandler(Vector2 dir);
    public static event InputEventHandler onInputEvent;
    public delegate void CombatInputEventHandler(INPUTACTION action);
    public static event CombatInputEventHandler onCombatInputEvent;

    [HideInInspector]
    public Vector2 dir;
    public static bool defendKeyDown;

    private void Update()
    {
        KeyboardControls();
    }

    private void KeyboardControls()
    {
        float x = 0;
        float y = 0;
        if (Input.GetKey(Left)) x = -1;
        if (Input.GetKey(Right)) x = 1;
        if (Input.GetKey(Up)) y = 1;
        if (Input.GetKey(Down)) y = -1;
        dir = new Vector2(x, y);
        InputEvent(dir);

        if (Input.GetKeyDown(JumpKey)) CombatInputEvent(INPUTACTION.JUMP);
        if (Input.GetKeyDown(GeneralAttackKey)) CombatInputEvent(INPUTACTION.GENERALATTACK);
        if (Input.GetKeyDown(HeavyBlowKey)) CombatInputEvent(INPUTACTION.HEAVYBLOWKEY);
        if (Input.GetKeyDown(Skill1)) CombatInputEvent(INPUTACTION.SKILL1);
        defendKeyDown = Input.GetKey(DefendKey);
    }
    public static void InputEvent(Vector2 dir)
    {
        if (onInputEvent != null) onInputEvent(dir);
    }
    public static void CombatInputEvent(INPUTACTION action)
    {
        if (onCombatInputEvent != null) onCombatInputEvent(action);
    }
    /// <summary>
    /// 是否按下防御键
    /// </summary>
    /// <returns></returns>
    public bool IsDefendKeyDown()
    {
        return defendKeyDown;
    }
}
public enum INPUTACTION
{
    NONE,
    GENERALATTACK,
    HEAVYBLOWKEY,
    JUMP,
    DEFEND,
    SKILL1,
    SKILL2,
    SKILL3,
    SKILL4,
}
