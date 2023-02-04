using UnityEngine;

public class CollectableDroplet : CollectableBase
{
    [Header("Droplet Variables")]
    [SerializeField] private int n_dropletIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;

    protected override void DestroyCollectable()
    {
        // g_player.n_vineLength += n_dropletIncrement;

        //StartCoroutine(PlayerController.instance.AddTetherSegments(n_dropletIncrement));
        PlayerController.instance.AddTetherSegments(n_dropletIncrement);

        if (p_particle != null)
            p_particle.Play();

        base.DestroyCollectable();
    }
}
