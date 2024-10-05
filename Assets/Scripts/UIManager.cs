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
    }

    public void ToggleBuildingTab()
    {
        bool isActive = !buildingTabPanel.activeSelf;
        buildingTabPanel.SetActive(isActive);

        if (!isActive)
        {
            wallPlacer.StopPlacingObject();
        }
    }
}
