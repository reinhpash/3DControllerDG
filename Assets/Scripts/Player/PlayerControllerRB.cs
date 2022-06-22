using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerRB : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] Transform cameraTransform;
    float verticalInput;
    float horizontalInput;
    Vector3 movementDirection;

    private float moveSpeed;
    [SerializeField] private float walkSpeed = 6.0f;
    [SerializeField] private float runSpeed = 9.5f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private Stamina playerStamina;
    [SerializeField] private float staminaAmount = 100f;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private float groundCheckRadius = .2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    bool tryJump;

    [SerializeField]private Animator anim;
    [SerializeField] private Transform model;

    [SerializeField] Canvas playerCanvas;
    [Header("Interactable")]
    bool isPlayerNearInteractable;
    GameObject interactableCar;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    enum PlayerStates
    {
        Walk,
        Run,
        Idle
    }

    private PlayerStates currentState;

    private bool canRun = true;
    [SerializeField] private float minRunStamina = 40f;
    [SerializeField] private Slider staminaSlider;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        coll = this.GetComponent<Collider>();
        playerStamina = new Stamina(staminaAmount);

        Cursor.lockState = CursorLockMode.Locked;
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

    private void Update()
    {
        MovementInput();
        HandlePlayerStates();
        StaminaHandler();
        InteractWithCar();

        anim.SetFloat("MoveSpeed", moveSpeed);

        if (Input.GetButtonDown("Jump"))
        {
            //inputlarý updateden alýyoruz
            tryJump = true;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        anim.SetTrigger("isJump");
        //tryJump boolunu false yapýyoruz ki sürekli bu fonksiyon çalýþmasýn.
        tryJump = false;
    }

    void StaminaHandler()
    {
        if (currentState == PlayerStates.Run)
        {
            playerStamina.StaminaDecrease(playerStamina.runCostPerSecond);


            if (playerStamina.GetStamina() < 0)
            {
                playerStamina.SetStamina(0);
            }

            if (playerStamina.GetStamina() <= 0)
            {
                canRun = false;
            }
        }

        if (currentState == PlayerStates.Walk)
        {
            playerStamina.StaminaIncrease();
        }

        if (currentState == PlayerStates.Idle)
        {
            playerStamina.StaminaIncrease();
        }

        if (playerStamina.GetStamina() >= minRunStamina)
        {
            canRun = true;
        }

        staminaSlider.value = playerStamina.GetStamina();
    }

    private void HandlePlayerStates()
    {
        if ((!Input.GetKey(runKey) || !canRun) && movementDirection != Vector3.zero)
        {
            currentState = PlayerStates.Walk;
        }
        else if (Input.GetKeyDown(runKey) && movementDirection != Vector3.zero)
        {
            if (canRun)
            {
                currentState = PlayerStates.Run;
            }
            
        }
        else if (movementDirection == Vector3.zero)
        {
            currentState = PlayerStates.Idle;
        }
    }

    private void FixedUpdate()
    {
        Movement();
        CheckIsGrounded();

        if (tryJump && isGrounded)
        {
            Jump();
        }
    }

    private void MovementInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        movementDirection = new Vector3(horizontalInput, 0, verticalInput);
    }

    private void Movement()
    {
        if (currentState == PlayerStates.Idle)
        {
            moveSpeed = 0;
        }
        else if (currentState == PlayerStates.Walk)
        {
            moveSpeed = walkSpeed;
        }
        else if (currentState == PlayerStates.Run)
        {
            if (canRun)
            {
                moveSpeed = runSpeed;
            }
        }

        Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput).normalized * moveSpeed;

        moveVector.y = rb.velocity.y;
        //Kontorlcü içerisine koyduðumuz modelin rotasyonunu kamera rotasyonuna eþitliyoruz.
        //Böylece model kameranýn baktýðý yere bakýyor.
        model.transform.rotation = cameraTransform.rotation;
        rb.velocity = RotatePlayerWithCamera(moveVector);
    }

    private Vector3 RotatePlayerWithCamera(Vector3 moveVector)
    {
        var playerRotation = Quaternion.Euler(0, cameraTransform.rotation.y, 0);
        moveVector = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveVector;
        return moveVector;
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPosition.position, groundCheckRadius, groundLayer);
    }

    void InteractWithCar()
    {
        if (isPlayerNearInteractable && Input.GetKeyDown(interactKey))
        {
            if (interactableCar != null)
            {
                interactableCar.GetComponent<CarInteract>().Interact(this.gameObject, this.GetComponent<PlayerControllerRB>());
                playerCanvas.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
    }
}
