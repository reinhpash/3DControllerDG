using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform[] wheelsToRotate;
    public float rotationSpeed;
    public CarController controller;

    private void Awake()
    {
        controller = this.GetComponent<CarController>();
    }

    private void Update()
    {
        //Eðer araba scriptti aktif ise
        if (controller.isActive)
        {
            //Kullanýcýdan input alýyoruz 

            float vertical = Input.GetAxisRaw("Vertical");
            foreach (var wheel in wheelsToRotate)
            {
                //aldýðýmýz input doðrultusunda tekerlerin dönmesini saðlýyoruz
                wheel.transform.Rotate(Time.deltaTime * vertical * rotationSpeed, 0, 0, Space.Self);
            }
        }
    }

}
