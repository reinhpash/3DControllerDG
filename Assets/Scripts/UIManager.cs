using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject escMenu;
    [SerializeField] AudioListener listener;
    public Image muteIcon;

    public Sprite muteImage;
    public Sprite unMuteImage;

    private bool isMuted = false;

    bool isActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = !isActive;
            escMenu.SetActive(isActive);

            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
        }
    }

    public void OnClickMuteButton()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            muteIcon.sprite = muteImage;
            listener.enabled = false;
        }
        else
        {
            muteIcon.sprite = unMuteImage;
            listener.enabled = true;
        }
    }

    public void OnClickClose()
    {
        escMenu.SetActive(false);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }


}
