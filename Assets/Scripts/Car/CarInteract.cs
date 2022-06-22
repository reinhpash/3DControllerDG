using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInteract : MonoBehaviour
{
    [SerializeField] private GameObject carCam;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject playerCam;
    [SerializeField] private GameObject playerSpawnPos;
    [SerializeField] private bool isCarActive;
    public KeyCode exitButton = KeyCode.E;

    private PlayerControllerRB moveScript;


    //Bu scriptte humanoid karakterimizin kullan�labilir olan bir arabayla olan etkile�imini ayarl�yoruz

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(TagManager.PLAYER_TAG);
    }

    public void Interact(GameObject player)
    {
        ActiveCar();
        DeactivePlayer(player);
        StartCoroutine(setActiveCar());

    }
    public void Interact(GameObject player, PlayerControllerRB rb)
    {
        ActiveCar();
        DeactivePlayer(player, rb);
        moveScript = rb;
        StartCoroutine(setActiveCar());

    }

    IEnumerator setActiveCar()
    {
        yield return new WaitForSeconds(.5f);
        isCarActive = true;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (Input.GetKeyDown(exitButton) && isCarActive)
        {
            DeactiveCar();
            if (moveScript == null)
            {
                ActivatePlayer(player);
            }
            else
            {
                ActivatePlayer(player,moveScript);
            }
        }
    }

    private void ActiveCar()
    {
        this.gameObject.GetComponent<CarController>().enabled = true;
        this.GetComponent<CarController>().CarSoundSet(true);
        this.GetComponent<AudioSource>().enabled = true;
        carCam.SetActive(true);
    }

    private void DeactiveCar()
    {
        this.gameObject.GetComponent<CarController>().enabled = false;
        carCam.SetActive(false);
        this.GetComponent<CarController>().CarSoundSet(false);
        this.GetComponent<AudioSource>().enabled = false;
        isCarActive = false;
    }

    private void DeactivePlayer(GameObject player)
    {
        /*player.GetComponent<CharacterController>().enabled = false;*/
        player.GetComponent<PlayerController>().enabled = false;
        playerHud.SetActive(false);
        playerCam.SetActive(false);
        player.SetActive(false);
    }

    private void ActivatePlayer(GameObject player)
    {
        /*player.GetComponent<CharacterController>().enabled = true;*/
        player.GetComponent<PlayerController>().enabled = true;
        playerHud.SetActive(true);
        playerCam.SetActive(true);
        player.SetActive(true);
        player.transform.position = playerSpawnPos.transform.position;
    }
    private void DeactivePlayer(GameObject player, PlayerControllerRB rb)
    {
        /*player.GetComponent<CharacterController>().enabled = false;*/
        rb.enabled = false;
        playerHud.SetActive(false);
        playerCam.SetActive(false);
        player.SetActive(false);
    }

    private void ActivatePlayer(GameObject player, PlayerControllerRB rb)
    {
        /*player.GetComponent<CharacterController>().enabled = true;*/
        rb.enabled = true;
        playerHud.SetActive(true);
        playerCam.SetActive(true);
        player.SetActive(true);
        player.transform.position = playerSpawnPos.transform.position;
    }
}
