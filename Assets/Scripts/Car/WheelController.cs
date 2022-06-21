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
        //E�er araba scriptti aktif ise
        if (controller.isActive)
        {
            //Kullan�c�dan input al�yoruz 

            float vertical = Input.GetAxisRaw("Vertical");
            foreach (var wheel in wheelsToRotate)
            {
                //ald���m�z input do�rultusunda tekerlerin d�nmesini sa�l�yoruz
                wheel.transform.Rotate(Time.deltaTime * vertical * rotationSpeed, 0, 0, Space.Self);
            }
        }
    }

}
