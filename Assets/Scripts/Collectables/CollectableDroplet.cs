using UnityEngine;

public class CollectableDroplet : CollectableBase
{
    [Header("Droplet Variables")]
    [SerializeField] private int n_dropletIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;
    public string collectionName = "We Forgor";

    protected override void DestroyCollectable()
    {
        // g_player.n_vineLength += n_dropletIncrement;

        //StartCoroutine(PlayerController.instance.AddTetherSegments(n_dropletIncrement));
        PlayerController.instance.AddTetherSegments(n_dropletIncrement);
        Debug.Log("Collected " + collectionName); //will output to hud

        if (p_particle != null)
            p_particle.Play();

        AudioManager.instance.PlayOneShotWithParameters("RootGrow", transform);
        
        if (Hud.instance)
            Hud.instance.ShowCollectibleTextFor(collectionName, 2.5f);

        base.DestroyCollectable();
    }
}
