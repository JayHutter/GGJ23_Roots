using UnityEngine;
using System.Collections;
using UnityEngine.VFX;
using FMOD.Studio;

[RequireComponent(typeof(SphereCollider))]
public class CollectablePuddle : CollectableBase
{
    [Header("Puddle Variables")]
    [SerializeField] private float          f_dropletIncrement  = 0;
    [SerializeField] private WaterTank      g_tank;

    [SerializeField] private float          f_soakDelay         = 0.0f;

    [Header("VFX Variables")]
    [SerializeField] private VisualEffect   vfx;
    [SerializeField] float                  radius              = 0.5f;
    private SphereCollider                  sphereCollider;

    private float                           amount              = 3.0f;
    [SerializeField] private float                           maxAmount           = 3.0f;

    [Header("Player VFX Variables")]
    [SerializeField] private VisualEffect   vfxSparks;
    [SerializeField] private GameObject     player;

    public bool isInfinite = false;

    private void Start()
    {
        if (vfxSparks)
            vfxSparks.Stop();

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
            g_tank = c.GetComponentInChildren<WaterTank>();

            if (amount <= 0 || g_tank.IsFull())
                return;

            f_timer += Time.deltaTime;
            amount  -= Time.deltaTime;

            if (f_timer > f_soakDelay)
            {
                f_timer = 0;
                f_timerLimit -= f_soakDelay;

                g_tank.AddWater(f_dropletIncrement);

                PlayEffects();

                AudioManager.instance.PlayOneShotWithParameters("Slurp", transform);

                if (p_particle != null)
                    p_particle.Play();
            }

            //if (f_timerLimit <= 0)
            if (amount <= 0)
            {
                DestroyCollectable();
            }
        }
    }

    private void PlayEffects()
    {
        if (vfxSparks)
            vfxSparks.Play();

        if (player)
            StartCoroutine(GlowUp());
    }

    private IEnumerator GlowUp()
    {
        Material mat
            = player.GetComponent<SkinnedMeshRenderer>().materials[1];

        mat.SetColor("_FresnelColour", new Color(0.0f, 191.0f, 191.0f, 0.0f) * 0.25f);

        float f_duration    = 0.3f;
        float f_thistimer   = 0.0f;

        while (f_thistimer <= f_duration)
        {
            mat.SetFloat("_Transparency", f_thistimer);
            f_thistimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(GlowDown());
    }

    private IEnumerator GlowDown()
    {
        if (vfxSparks)
            vfxSparks.Stop();

        Material mat
            = player.GetComponent<SkinnedMeshRenderer>().materials[1];

        mat.SetColor("_FresnelColour", new Color(0.0f, 191.0f, 191.0f, 0.0f));

        float f_thistimer = 0.3f;

        while (f_thistimer >= 0.0f)
        {
            mat.SetFloat("_Transparency", f_thistimer);
            f_thistimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        mat.SetFloat("_Transparency", 0.0f);
    }

    private void Update()
    {
        vfx.SetFloat("Scale", Mathf.InverseLerp(0, maxAmount, amount));
    }

    public float GetAmount()
    {
        return amount;
    }
}
