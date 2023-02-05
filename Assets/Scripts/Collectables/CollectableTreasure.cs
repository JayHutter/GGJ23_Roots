using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class CollectableTreasure : CollectableBase
{
    [Header("Scoring Variables")]
    [SerializeField] private int            n_scoreIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;

    [Header("Player VFX Variables")]
    [SerializeField] private VisualEffect vfxSparks;
    [SerializeField] private GameObject player;


    private void Start()
    {
        if (vfxSparks)
            vfxSparks.Stop();
    }

    protected override void DestroyCollectable()
    {
        PlayerController.instance.carrots += n_scoreIncrement;

        if (p_particle != null)
            p_particle.Play();

        b_magnetismEnable
            = false;

        GetComponent<Collider>().enabled
            = false;

        transform.GetChild(0).GetComponent<VisualEffect>().Stop();
        transform.GetChild(1).gameObject.SetActive(false);

        PlayEffects();
        AudioManager.instance.PlayOneShotWithParameters("VineBoom", transform);
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

        mat.SetColor("_FresnelColour", new Color(255.0f, 46.0f, 0.0f, 0.0f));

        float f_duration = 0.3f;
        float f_thistimer = 0.0f;

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

        mat.SetColor("_FresnelColour", new Color(255.0f, 46.0f, 0.0f, 0.0f));

        float f_thistimer = 0.3f;

        while (f_thistimer >= 0.0f)
        {
            mat.SetFloat("_Transparency", f_thistimer);
            f_thistimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        mat.SetFloat("_Transparency", 0.0f);

        StartCoroutine(LetParticlesDie());
    }

    private IEnumerator LetParticlesDie()
    {
        yield return new WaitForSecondsRealtime(6.0f);

        Destroy(gameObject);
    }
}
