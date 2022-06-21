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
            Oyuncunun kafasýnýn üzerinde bir tuþ ismi çýkartýyorum eðer bu kodu yazmazsam text görünmeyebiliyor
            main cameranýn rotasyonuna göre texti deðiþtiriyorum.
         */
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
