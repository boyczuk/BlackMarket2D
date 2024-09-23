using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaptopInteraction : MonoBehaviour
{

    public GameObject recruitmentPopup;

    void OnMouseDown() {
        if (recruitmentPopup != null) {
            recruitmentPopup.SetActive(true);
        }
    }
}
