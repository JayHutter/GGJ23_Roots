using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
public class CollectablePuddle : CollectableBase
{
    [Header("Puddle Variables")]
    [SerializeField] private int            n_dropletIncrement  = 0;
    //[SerializeField] private PLAYERSTATES g_player;

    [SerializeField] private float          f_soakDelay         = 0.0f;

    [Header("VFX Variables")]
    [SerializeField] private VisualEffect   vfx;
    [SerializeField] float                  radius              = 0.5f;
    private SphereCollider                  sphereCollider;

    private float                           amount              = 3.0f;
    private float                           maxAmount           = 3.0f;

    private void Start()
    {
        amount
            = maxAmount;

        sphereCollider
            = GetComponent<SphereCollider>();

        sphereCollider.radius
            = radius;

        vfx.SetFloat("Radius", radius);
    }

    protected override void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player" && b_soakEnabled)
        {
            f_timer += Time.deltaTime;
            amount  -= Time.deltaTime;

            vfx.SetFloat("Scale", Mathf.InverseLerp(0, maxAmount, amount));

            if (f_timer > f_soakDelay)
            {
                f_timer = 0;
                f_timerLimit -= f_soakDelay;
                // g_player.n_vineLength += n_dropletIncrement;

                if (p_particle != null)
                    p_particle.Play();
            }

            if (f_timerLimit <= 0)
                DestroyCollectable();
        }
    }
}
