using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float duration = 2f;

    public ParticleSystem smokeVFX;
    public GameObject explosionVFX;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;

            ParticleSystem.EmissionModule emission = smokeVFX.emission;
            emission.rateOverTime = 0f;

            explosionVFX.SetActive(true);

            Destroy(this.gameObject, duration);
        }
    }

}
