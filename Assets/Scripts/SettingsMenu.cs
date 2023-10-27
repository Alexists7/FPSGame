using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Text placeHolderText;
    [SerializeField] TMP_InputField sensText;
    [SerializeField] Button quitGameButton;

    PlayerController controller;
    float mouseSens;

    void Start()
    {
        controller = GetComponentInParent<PlayerController>();
        mouseSens = controller.GetMouseSensitivity();

        sensSlider.value = mouseSens;
        placeHolderText.text = sensSlider.value.ToString();
    }
    void Update()
    {
        CheckForEscPress();
        UpdateSliderValue();
        CheckForUserSensInput();
        CheckQuitGame();
    }

    void OnValueChanged(float newValue)
    {
        mouseSens = newValue;
        controller.SetMouseSensitivity(mouseSens);
    }

    void CheckForUserSensInput()
    {
        sensSlider.onValueChanged.AddListener(delegate { OnValueChanged(sensSlider.value); });

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            float.TryParse(sensText.text, out mouseSens);

            if (mouseSens > 0f && mouseSens <= 20f)
            {
                sensSlider.value = mouseSens;
                controller.SetMouseSensitivity(mouseSens);
                sensText.text = "";
            }
            else
            {
                sensSlider.value = 4f;
                sensText.text = "";
            }
        }
    }

    void UpdateSliderValue() 
    {
        placeHolderText.text = (Mathf.Round(sensSlider.value * 100.0f) * 0.01f).ToString();
        
    }

    void CheckForEscPress()
    {    
        if(settingsMenu.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                settingsMenu.SetActive(true);
            }
        }
        else if (settingsMenu.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
                settingsMenu.SetActive(false);
            }
        }
    }

    void CheckQuitGame()
    {
        quitGameButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }
}
