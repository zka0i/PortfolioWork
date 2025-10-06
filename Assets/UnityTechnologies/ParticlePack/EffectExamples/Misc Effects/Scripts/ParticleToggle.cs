using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleToggle : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(ToggleEmission());
    }

    IEnumerator ToggleEmission()
    {
        while (true)
        {
            ps.Play();          // Turn ON for 1 second
            yield return new WaitForSeconds(1f);

            ps.Stop();          // Turn OFF for 2 seconds
            yield return new WaitForSeconds(2f);
        }
    }
}
