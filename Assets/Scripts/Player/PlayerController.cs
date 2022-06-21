using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 9.5f;
    [SerializeField] private float jumpHeight = 5;
    [Tooltip("Karakterin havadaki hareketini kýstýlar.")]
    [SerializeField] private float airMultiplier = .6f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    private float moveSpeed = 0f;
    private bool isRunning;
    private bool canRun;
    private Vector3 moveInput;
    private float yVelocity;
    private bool isPressRunButton;


    [Header("Stamina Settings")]
    [Tooltip("Kosmak icin gerekli minumum stamina")]
    [SerializeField] private float minRunStamina = 40;
    [SerializeField] private float staminaAmount = 100f;
    [SerializeField] Slider staminaSlider;
    private Stamina playerStamina;


    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private float groundCheckRadius = .2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Gravity")]
    [SerializeField] private float gravity = 14f;

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject model;
    [SerializeField] Canvas playerCanvas;
    private CharacterController characterController;
    private Rigidbody rb;

    [Header("Interactable")]
    bool isPlayerNearInteractable;
    GameObject interactableCar;

    #region Unity Methods
    private void Awake()
    {
        characterController = this.GetComponent<CharacterController>();
        rb = this.GetComponent<Rigidbody>();
        playerStamina = new Stamina(staminaAmount);
    }

    private void Start()
    {
        staminaSlider.maxValue = playerStamina._maxStamina;
        staminaSlider.minValue = 0f;
    }

    private void Update()
    {
        GetMoveInput();
        Movement();
        Jump();
        ApplyGravity();
        StaminaHandler();
        InteractWithCar();
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.INTERACTABLE_TAG))
        {
            playerCanvas.enabled = true;
            isPlayerNearInteractable = true;
            interactableCar = other.gameObject;
        }
        else
        {
            playerCanvas.enabled = false;
            isPlayerNearInteractable = false;
            interactableCar = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagManager.INTERACTABLE_TAG))
        {
            playerCanvas.enabled = false;
            isPlayerNearInteractable = false;
            interactableCar = null;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
    }

    #endregion

    #region Movement
    private void Movement()
    {
        /*
            Oyuncunun sol shift tuþuna basýp basmadýðýný ve herhangi bir movement inputu girip girmediðini kontrol ediyoruz.
            moveSpeed deðiþkenini bir container olarak kullanýyorum ana hareket kodu bu deðiþkeni kullanýyor 
            bu yöntem sayesinde duplicate kod kullanýmýný azaltmayý amaçlýyorum.
            Walk,Run,Idle metotlarý moveSpeed deðiþkenimi kontrol etmek için gerekli.
         */
        if ((!isPressRunButton || !canRun) && moveInput != Vector3.zero)
        {
            Walk();
        }
        else if (isPressRunButton && moveInput != Vector3.zero)
        {
            if (canRun)
            {
                Run();
            }

        }
        else if (moveInput == Vector3.zero)
        {
            Idle();
        }


        //inputu normalized yapmamýn sebebi karakterin çapraz ilerlerken daha hýzlý gitmesini engellemek
        Vector3 moveVector = moveInput.normalized * moveSpeed;
        moveVector.y = yVelocity;

        if (isGrounded)
        {
            moveVector = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveVector;
            characterController.Move(moveVector * Time.deltaTime);
        }
        else
        {
            moveVector = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveVector;
            characterController.Move(moveVector * airMultiplier * Time.deltaTime);
        }
        anim.SetFloat("MoveSpeed", moveSpeed);
        model.transform.rotation = cameraTransform.rotation;
    }

    private void Idle()
    {
        moveSpeed = 0.0f;
        isRunning = false;
        playerStamina.StaminaIncrease();
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        isRunning = true;
    }

    void StaminaHandler()
    {
        if (isPressRunButton && moveInput != Vector3.zero)
        {
            isRunning = true;
            playerStamina.StaminaDecrease(playerStamina.runCostPerSecond);

            if (playerStamina.GetStamina() < 0)
            {
                playerStamina.SetStamina(0);
            }

            if (playerStamina.GetStamina() <= 0)
            {
                canRun = false;
            }
            else if (playerStamina.GetStamina() >= minRunStamina)
            {
                canRun = true;
            }
        }

        staminaSlider.value = playerStamina.GetStamina();
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        isRunning = false;
        playerStamina.StaminaIncrease();
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            yVelocity = jumpHeight;
            anim.SetTrigger("isJump");
        }
    }

    private void GetMoveInput()
    {
        float zValue = Input.GetAxisRaw("Vertical");
        float xValue = Input.GetAxisRaw("Horizontal");
        isPressRunButton = Input.GetKey(runKey);
        moveInput = new Vector3(xValue, 0, zValue);
    }
    #endregion

    #region Gravity and Ground Check

    private void ApplyGravity()
    {
        //Eðer oyuncu yerde deðilse yer çekimi uygulamaya baþklýyoruz
        if (!isGrounded)
        {
            yVelocity -= gravity * Time.deltaTime;
            Vector3 gravityVelocity = new Vector3(0, yVelocity, 0);
            characterController.Move(gravityVelocity * Time.deltaTime);
        }
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPosition.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    }

    #endregion

    void InteractWithCar()
    {
        if (isPlayerNearInteractable && Input.GetKeyDown(interactKey))
        {
            if (interactableCar != null)
            {
                interactableCar.GetComponent<CarInteract>().Interact(this.gameObject);
                playerCanvas.enabled = false;
            }
        }
    }
}
