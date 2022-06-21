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


    //isActive booluna tekerlekleri hareket ettirdiðimden scriptten ulaþýyorum.
    //eðer isActive true ise tekerlekler inputa göre dönüyor.
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
         Oyun baþladýðý zaman bize hareket anlamýnda sýkýntý çýkartmamasý için küre ve araba colliderlarýný koyduðumuz
         objeleri null parent yapýyoruz.
         defaultDrag deðiþkeninde motor olarak kullanacaðýmýz kürenin dragýný alýyoruz.
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
         Baþka scriptlerden eriþerek arabanýn sesini açýp kapatmayý ayarlamak için böyle bir metot yazdým.
         true ise araba ileri gidiyor demek ve ses buna göre deðiþiyor.
        false ise araba idle sesine gidiyor.
        idle sesindeyken yavaþça motor pitchini düþürüyor.
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
         Arabayý döndürürken tekerlerinde dönmesi için en turninput deðerimizi kullanýyoruz
         */
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * wheelMaxTurn), leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * wheelMaxTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
    }

    private void Turn()
    {
        /*
            Araba sadece yerde ise dönüþ yapmasýna izin veriyorum.
            dönüþleri yavaþlatmak için turnstrength deðeri düþürülebilir.
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
                //input olduðu sürece engine speed deðerini arttýrýyorum bu da motor sesini etkiliyor.
                engineSpeed += .1f * Time.deltaTime;
            }

        }

        turnInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        RaycastHit rayHit;

        //raypoint kullanmak yerinde direkt objesimizin transformunu da kullanabiliriz ama bu þekilde daha garanti oluyor.

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
                //Eðer araba yerde ve kullanýcý arabanýn ilerlemesi için input giriyorsa
                sphereRB.AddForce(transform.forward * speedInput, ForceMode.Acceleration);
            }
        }
        else
        {
            //Eðer araba yerde deðil ise arabaya yerçekimi uyguluyoruz.
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
