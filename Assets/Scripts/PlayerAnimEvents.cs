using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    void PlayFootsteps()
    {
        AudioManager.instance.PlayOneShotWithParameters("Footsteps", transform);
    }

    void PlayCrouchsteps()
    {
        AudioManager.instance.PlayOneShotWithParameters("Crouchsteps", transform);
    }
}
