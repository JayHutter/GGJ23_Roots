using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private float power = 0.1f;
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            Vector3 pos = collisionEvents[i].intersection;
            GameObject sound = new GameObject("Sound");
            sound.transform.position = pos;
            AudioManager.instance.PlayOneShotWithParameters("Splash", sound.transform);
            Destroy(sound);
            i++;
        }

        if (other.layer == LayerMask.NameToLayer("Growable"))
        {
            if(other.transform.GetComponent<Growable>() != null)
            {
                if(other.transform.GetComponent<Growable>().plant != null)
                    other.transform.GetComponent<Growable>().plant.watered += power;
            }
        }
    }
}
