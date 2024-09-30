using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button saveGameButton;
    public Button settingsButton;
    public Button exitButton;

    private GangDataManager gangDataManager;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        resumeButton.onClick.AddListener(ResumeGame);
        saveGameButton.onClick.AddListener(SaveGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);

        gangDataManager = FindObjectOfType<GangDataManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void SaveGame()
    {
        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            foreach (var member in gangDataManager.criminalOrganization.GetAllMembers())
            {
                GameObject npcObject = GameObject.Find(member.name);
                if (npcObject != null)
                {
                    member.position = npcObject.transform.position;
                }
            }

            gangDataManager.SaveGangData(gangDataManager.criminalOrganization);
            Debug.Log("Game Saved!");
        }
    }

    void OpenSettings(){
        Debug.Log("Settings opened (placeholder)");
    }

    void ExitGame() {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
