using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildingTabPanel;

    // Start is called before the first frame update
    void Start()
    {
        buildingTabPanel.SetActive(false);
    }

    public void ToggleBuildingTab() {
        buildingTabPanel.SetActive(!buildingTabPanel.activeSelf);
    }
}
