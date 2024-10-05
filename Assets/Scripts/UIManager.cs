using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildingTabPanel;
    public WallPlacer wallPlacer;

    void Start()
    {
        buildingTabPanel.SetActive(false);

        if (wallPlacer == null)
        {
            wallPlacer = FindObjectOfType<WallPlacer>();
            if (wallPlacer == null)
            {
                Debug.LogError("WallPlacer script not found!");
            }
        }
    }

    public void ToggleBuildingTab()
    {
        bool isActive = !buildingTabPanel.activeSelf;
        buildingTabPanel.SetActive(isActive);

        if (!isActive && wallPlacer != null)
        {
            wallPlacer.StopPlacingObject();
        }
    }

    public void ToggleDeleteMode()
    {
        wallPlacer.StartDeletingObject();
    }
}
