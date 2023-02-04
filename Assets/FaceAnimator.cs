using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;

public class FaceAnimator : MonoBehaviour
{
    public Texture normal;
    public Texture sad;
    public Texture open;
    Material mouthMat;

    public DecalProjector decal;

    public enum State
    {
        NORMAL,
        SAD,
        OPEN
    }

    public State currentState;
    public State previousState;

    private bool canChange = true;

    private void Start()
    {
        mouthMat = decal.material;
    }

    public void SetAnimationState(State state)
    {
        if (state == currentState || !canChange) 
            return;

        previousState = currentState;
        currentState = state;

        switch(state)
        {
            case State.NORMAL:
                mouthMat.SetTexture("Base_Map", normal);
                break;
            case State.SAD:
                mouthMat.SetTexture("Base_Map", sad);
                break;
            case State.OPEN:
                mouthMat.SetTexture("Base_Map", open);
                break;
        }
    }

    public IEnumerator SetAnimationFor(State animState, float time)
    {
        SetAnimationState(animState);
        canChange = false;
        yield return new WaitForSeconds(time);
        canChange = true;
        SetAnimationState(previousState);
    }
}
