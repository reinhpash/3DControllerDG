using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Motor")]
    public Rigidbody sphereRB;

    [Header("Car Settings")]
    [SerializeField] private float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrenght = 180, gravity = 10f;
    public Rigidbody carRB;

    [Header("Car air Settings")]
    [SerializeField] private float airDrag = .1f;
    private float defaultDrag;

    [Header("Inputs")]
    private float speedInput;
    private float turnInput;

    [Header("Car Ground Check")]
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private float groundRayLenght = .5f;
    [SerializeField] private Transform rayPoint;
    private bool isGrounded;

    [Header("Wheels")]
    [SerializeField] private Transform leftFrontWheel, rightFrontWheel;
    [SerializeField] private float wheelMaxTurn;
    [SerializeField] private float alignToGroundTime;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip gasSound;
    [Tooltip("Motor sesi minumum pitch")]
    [SerializeField] private float engineMinPitch = 0.05f;
    [Tooltip("Motor sesi maksimum pitch")]
    [SerializeField] private float engineMaxPitch = 2f;
    bool isEngineSoundPlay = false;
    private float engineSpeed;
    private bool isCarStop;
    public bool isActive;


    //isActive booluna tekerlekleri hareket ettirdi�imden scriptten ula��yorum.
    //e�er isActive true ise tekerlekler inputa g�re d�n�yor.
    private void OnEnable()
    {
        isActive = true;
    }

    private void OnDisable()
    {
        isActive = false;
    }


    private void Start()
    {
        /* 
         Oyun ba�lad��� zaman bize hareket anlam�nda s�k�nt� ��kartmamas� i�in k�re ve araba colliderlar�n� koydu�umuz
         objeleri null parent yap�yoruz.
         defaultDrag de�i�keninde motor olarak kullanaca��m�z k�renin drag�n� al�yoruz.
         */
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;
        defaultDrag = sphereRB.drag;

        audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = engineMinPitch;
    }

    private void Update()
    {
        Movement();
        Turn();
        WheelTurn();

        if (Mathf.Abs(speedInput) <= 0)
        {
            CarSoundSet(false);
            isCarStop = true;
        }

        if (engineSpeed != 0)
        {
            audioSource.pitch = engineSpeed;
        }

        this.transform.position = sphereRB.transform.position;
    }



    public void CarSoundSet(bool value)
    {
        /*
         Ba�ka scriptlerden eri�erek araban�n sesini a��p kapatmay� ayarlamak i�in b�yle bir metot yazd�m.
         true ise araba ileri gidiyor demek ve ses buna g�re de�i�iyor.
        false ise araba idle sesine gidiyor.
        idle sesindeyken yava��a motor pitchini d���r�yor.
         */
        if (value == true)
        {
            audioSource.clip = gasSound;
            if (!audioSource.isPlaying)
            {
                engineSpeed = engineMinPitch;
                audioSource.Play();
                isCarStop = false;
            }
        }
        else if (value == false)
        {
            audioSource.clip = idleSound;
            if (audioSource.pitch > .6f)
            {
                audioSource.pitch -= .6f * Time.deltaTime;
            }
            else if (audioSource.pitch <= .6f)
            {
                audioSource.pitch = .6f;
            }
            
            engineSpeed = 0;
            if (!isCarStop)
            {
                audioSource.pitch = 1;
                audioSource.Play();

            }
        }
    }

    private void WheelTurn()
    {
        /*
         Arabay� d�nd�r�rken tekerlerinde d�nmesi i�in en turninput de�erimizi kullan�yoruz
         */
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * wheelMaxTurn), leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * wheelMaxTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
    }

    private void Turn()
    {
        /*
            Araba sadece yerde ise d�n�� yapmas�na izin veriyorum.
            d�n��leri yava�latmak i�in turnstrength de�eri d���r�lebilir.
         */
        if (isGrounded)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrenght * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
    }

    private void Movement()
    {
        speedInput = 0;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }



        if (engineSpeed < engineMinPitch)
        {
            engineSpeed = engineMinPitch;
        }

        else if (engineSpeed > engineMaxPitch)
        {
            engineSpeed = engineMaxPitch;
        }
        else
        {
            if (speedInput != 0)
            {
                //input oldu�u s�rece engine speed de�erini artt�r�yorum bu da motor sesini etkiliyor.
                engineSpeed += .1f * Time.deltaTime;
            }

        }

        turnInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        RaycastHit rayHit;

        //raypoint kullanmak yerinde direkt objesimizin transformunu da kullanabiliriz ama bu �ekilde daha garanti oluyor.

        if (Physics.Raycast(rayPoint.position, -this.transform.up, out rayHit, groundRayLenght, GroundLayer))
        {
            isGrounded = true;

            Quaternion rotateTo = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, alignToGroundTime * Time.deltaTime);
        }


        if (isGrounded)
        {
            sphereRB.drag = defaultDrag;
            if (Mathf.Abs(speedInput) > 0)
            {
                //E�er araba yerde ve kullan�c� araban�n ilerlemesi i�in input giriyorsa
                sphereRB.AddForce(transform.forward * speedInput, ForceMode.Acceleration);
            }
        }
        else
        {
            //E�er araba yerde de�il ise arabaya yer�ekimi uyguluyoruz.
            sphereRB.drag = airDrag;
            sphereRB.AddForce(transform.up * -gravity * 1000f);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayPoint.position, -transform.up * groundRayLenght);

        carRB.MoveRotation(transform.rotation);
    }
}
