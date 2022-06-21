using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        /*
            Oyuncunun kafas�n�n �zerinde bir tu� ismi ��kart�yorum e�er bu kodu yazmazsam text g�r�nmeyebiliyor
            main cameran�n rotasyonuna g�re texti de�i�tiriyorum.
         */
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
