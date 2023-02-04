using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class FaceAnimator : MonoBehaviour
{
    public Texture normal;
    public Texture sad;
    public Texture open;

    public MeshRenderer mouthMesh;
    Material mouthMat;

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
        mouthMat = mouthMesh.material;
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
                mouthMat.SetTexture("_BaseMap", normal);
                break;
            case State.SAD:
                mouthMat.SetTexture("_BaseMap", sad);
                break;
            case State.OPEN:
                mouthMat.SetTexture("_BaseMap", open);
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
