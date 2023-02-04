using UnityEngine;

public class CollectableBase : MonoBehaviour
{
    [SerializeField] protected bool             b_soakEnabled               = false;

    [Header("Particles Variables")]
    [SerializeField] protected ParticleSystem   p_particle;

    [Header("Magnetism Variables")]
    [SerializeField] protected bool             b_magnetismEnable           = false;
    [SerializeField] protected GameObject       g_player;
    [SerializeField] protected float            f_playerRange               = 0.0f;

    [SerializeField] protected float            f_collectableSpeedMax       = 0.0f;
    [SerializeField] protected float            f_collectableAcceleration   = 0.0f;
    protected float                             f_collectableSpeedCurrent   = 0.0f;
    protected Vector3                           v_lastPlayerPos             = Vector3.zero;

    [Header("OnTriggerStay(Soak) Variables")]
    [SerializeField] protected float            f_timerLimit                = 0.0f;
    protected float                             f_timer                     = 0.0f;

    private void Start()
    {
        g_player = PlayerController.instance.gameObject;
    }

    private void Update()
    {
        if (b_magnetismEnable)
            CollectableMagnetism();
    }

    /// <summary>
    /// Move towards the player if they are within a certain range
    /// These objects will accelerate towards the player and decelerate when the
    /// player leaves its range
    /// Only works when b_magnetismEnable is true
    /// </summary>
    protected void CollectableMagnetism()
    {
        float dist
            = Vector3.Distance
            (
                transform.position,
                g_player.transform.position
            );

        if (dist <= f_playerRange)
        {
            if (f_collectableSpeedCurrent < f_collectableSpeedMax)
                f_collectableSpeedCurrent += f_collectableAcceleration * Time.deltaTime;
            else if (f_collectableSpeedCurrent > f_collectableSpeedMax)
                f_collectableSpeedCurrent = f_collectableSpeedMax;

            v_lastPlayerPos
                = g_player.transform.position;
        }
        else
        {
            if (f_collectableSpeedCurrent > 0.0f)
            {
                f_collectableSpeedCurrent -=  f_collectableAcceleration * Time.deltaTime;
                if (f_collectableSpeedCurrent < 0.0f)
                    f_collectableSpeedCurrent = 0.0f;
            }
        }

        if (f_collectableSpeedCurrent > 0.0f)
            transform.position
                = Vector3.MoveTowards
                (
                    transform.position,
                    v_lastPlayerPos,
                    f_collectableSpeedCurrent
                );
    }

    /// <summary>
    /// Upon destruction, destroy the game object and play sound/particle effects
    /// and increment any values via override
    /// </summary>
    virtual protected void DestroyCollectable()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Upon touching the player, start sequence to destroy the collectable
    /// </summary>
    /// <param name="c">Player Gameobject</param>
    protected void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player" && !b_soakEnabled)
            DestroyCollectable();
    }

    /// <summary>
    /// Upon touching and being held onto the player, wait until value has
    /// exhausted before destroying
    /// </summary>
    /// <param name="c">Player Gameobject</param>
    virtual protected void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player" && b_soakEnabled)
        {
            f_timer += Time.deltaTime;

            if (f_timer > f_timerLimit)
                DestroyCollectable();
        }
    }
}