using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static Action<InputAction.CallbackContext> onMove;
    public static Action<InputAction.CallbackContext> onAim;
    public static Action<InputAction.CallbackContext> onJump;
    public static Action<InputAction.CallbackContext> onSprint;
    public static Action<InputAction.CallbackContext> onMelee;
    public static Action<InputAction.CallbackContext> onAimSight;
    public static Action<InputAction.CallbackContext> onShoot;
    public static Action<InputAction.CallbackContext> onCrouch;
    public static Action<InputAction.CallbackContext> onLight;
    public static Action<InputAction.CallbackContext> onPause;


    public static Action<InputAction.CallbackContext> onDebug1;

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (onMove != null)
            onMove(context);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (onAim != null)
            onAim(context);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (onJump != null)
            onJump(context);
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (onSprint != null)
            onSprint(context);
    }
    public void OnMelee(InputAction.CallbackContext context)
    {
        if (onMelee != null)
            onMelee(context);
    }
    public void OnAimSight(InputAction.CallbackContext context)
    {
        if (onAimSight != null)
            onAimSight(context);
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (onShoot != null)
            onShoot(context);
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (onCrouch != null)
            onCrouch(context);
    }
    public void OnLight(InputAction.CallbackContext context)
    {
        if (onLight != null)
            onLight(context);
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (onPause != null)
            onPause(context);
    }

    public void OnDebug1(InputAction.CallbackContext context)
    {
        if (onDebug1 != null)
            onDebug1(context);
    }
}

