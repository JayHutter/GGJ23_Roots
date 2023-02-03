using UnityEngine;

public class CollectablePuddle : CollectableBase
{
    [SerializeField] private int n_dropletIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;

    [SerializeField] private float f_soakDelay = 0.0f;

    protected override void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player" && b_soakEnabled)
        {
            f_timer += Time.deltaTime;

            if (f_timer > f_soakDelay)
            {
                f_timer = 0;
                f_timerLimit -= f_soakDelay;
                // g_player.n_vineLength += n_dropletIncrement;
                // Shrink puddle?

                if (p_particle != null)
                    p_particle.Play();
            }

            if (f_timerLimit <= 0)
                DestroyCollectable();
        }
    }
}
