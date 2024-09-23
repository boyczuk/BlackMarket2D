using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public void ClosePopup() {
        gameObject.SetActive(false);
    }
}
