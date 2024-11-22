using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StorageInteraction : MonoBehaviour, IPointerClickHandler
{
    public GameObject storagePopup;
    public Transform weaponListContainer;
    public GameObject weaponDisplayPrefab;

    private GangDataManager gangDataManager;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();

        if (storagePopup != null)
        {
            storagePopup.SetActive(false);
        }
    }

    void Update()
    {
        if (storagePopup != null && storagePopup.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (storagePopup != null)
            {
                PopulateStorage();
                storagePopup.SetActive(true);
                Debug.Log("Storage popup opened.");
            }
        }
    }

    public void ClosePopup()
    {
        Debug.Log("Close button clicked.");
        if (storagePopup != null)
        {
            storagePopup.SetActive(false);
            Debug.Log("Storage popup closed.");
        }
    }

    private void PopulateStorage()
    {
        if (gangDataManager == null || weaponListContainer == null || weaponDisplayPrefab == null)
        {
            Debug.LogWarning("PopulateStorage: Missing references.");
            return;
        }

        foreach (Transform child in weaponListContainer)
        {
            Destroy(child.gameObject);
        }

        List<Weapon> ownedWeapons = gangDataManager.playerGang.ownedWeapons;

        foreach (Weapon weapon in ownedWeapons)
        {
            GameObject weaponDisplay = Instantiate(weaponDisplayPrefab, weaponListContainer);

            TextMeshProUGUI weaponNameText = weaponDisplay.GetComponentInChildren<TextMeshProUGUI>();
            if (weaponNameText != null)
            {
                weaponNameText.text = $"{weapon.weaponName}";
            }

            Image weaponIcon = weaponDisplay.GetComponentInChildren<Image>();
            if (weaponIcon != null)
            {
                weaponIcon.sprite = weapon.weaponIcon;
            }
        }

        Debug.Log("Storage populated with weapons.");
    }
}
