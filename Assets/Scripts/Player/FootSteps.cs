using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footSteps;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    /*
     Animasyonalr üzerinden step isimli bir event fýrlatýyorum ve bu eventi burada yakalayýp sesi oynatýyorum.
     
     */
    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    AudioClip GetRandomClip()
    {
        return footSteps[Random.Range(0, footSteps.Length)];
    }
}
