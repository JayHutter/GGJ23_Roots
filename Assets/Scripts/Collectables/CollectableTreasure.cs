using UnityEngine;

public class CollectableTreasure : CollectableBase
{
    [Header("Scoring Variables")]
    [SerializeField] private int            n_scoreIncrement = 0;
    //[SerializeField] private PLAYERSTATES g_player;

    protected override void DestroyCollectable()
    {
        // g_player.n_playerScore += n_scoreIncrement;

        if (p_particle != null)
            p_particle.Play();

        DestroyCollectable();
    }
}
