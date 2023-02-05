using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class CollectableDroplet : CollectableBase
{
    [Header("Droplet Variables")]
    [SerializeField] private int n_dropletIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;
    public string collectionName = "We Forgor";

    protected override void DestroyCollectable()
    {
        //StartCoroutine(PlayerController.instance.AddTetherSegments(n_dropletIncrement));
        PlayerController.instance.AddTetherSegments(n_dropletIncrement);
        Debug.Log("Collected " + collectionName); //will output to hud

        if (p_particle != null)
            p_particle.Play();

        AudioManager.instance.PlayOneShotWithParameters("RootGrow", transform);
        
        if (Hud.instance)
            Hud.instance.ShowCollectibleTextFor(collectionName, 2.5f);

        b_magnetismEnable
            = false;

        GetComponent<Collider>().enabled
            = false;

        transform.GetChild(0).GetComponent<VisualEffect>().Stop();
        transform.GetChild(1).gameObject.SetActive(false);

        StartCoroutine(LetParticlesDie());
    }

    private IEnumerator LetParticlesDie()
    {
        yield return new WaitForSecondsRealtime(6.0f);

        Destroy(gameObject);
    }
}
