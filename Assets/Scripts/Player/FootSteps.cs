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
     Animasyonalr �zerinden step isimli bir event f�rlat�yorum ve bu eventi burada yakalay�p sesi oynat�yorum.
     
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
