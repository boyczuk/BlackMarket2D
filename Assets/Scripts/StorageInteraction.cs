using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageInteraction : MonoBehaviour, IPointerClickHandler
{
    public GameObject storagePopup;

    void Update()
    {
        if (storagePopup != null && storagePopup.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // Detect left-click
        {
            if (storagePopup != null)
            {
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
}
