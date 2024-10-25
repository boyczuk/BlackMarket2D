using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Controler : MonoBehaviour
{
    [Header("UI_Text Properties")]
    public GameObject UI_Gun;
    public GameObject UI_Type;
    public GameObject UI_Name;
    public GameObject UI_Total;

    [Header("Sprites")]
    public Sprite[] AssaultRifle;
    public Sprite[] SMG;
    public Sprite[] MG;
    public Sprite[] SniperRifle;
    public Sprite[] Shotgun;
    public Sprite[] Pistol;
    public Sprite[] OtherGun;
    public Sprite[] Grenade;
    private Sprite[] CurrentSprite;

    private int currentType;
    private int currentGun;

    void Start() {
        CurrentSprite = AssaultRifle;
    }
    public void ButtonHandler(){
        GameObject buttonobject = EventSystem.current.currentSelectedGameObject;
        string buttonname = buttonobject.name;
        switch (buttonname){
            case "UI_BackGun":
            currentGun = currentGun-1;
            if(currentGun<0) currentGun = CurrentSprite.Length-1;
            break;
            case "UI_NextGun":
            currentGun = currentGun+1;
            if(currentGun>=CurrentSprite.Length) currentGun = 0;
            break;
            case "UI_NextType":
            currentType = currentType+1;
            if(currentType>=8) currentType = 0;
            currentGun = 0;
            break;
            case "UI_BackType":
            currentType = currentType-1;
            if(currentType<0) currentType = 7;
            currentGun = 0;
            break;
            default:
            break;            
        }
        RefreshUI();
    }

    void RefreshUI(){
        string TypeString = "";
        switch(currentType){
            case 0:
            TypeString = "10 Assault Rifle";
            CurrentSprite = AssaultRifle;
            break;
            case 1:
            TypeString = "10 SMG";
            CurrentSprite = SMG;
            break;
            case 2:
            TypeString = "10 MG";
            CurrentSprite = MG;
            break;
            case 3:
            TypeString = "8 Sniper Rifle";
            CurrentSprite = SniperRifle;
            break;
            case 4:
            TypeString = "5 Shotgun";
            CurrentSprite = Shotgun;
            break;
            case 5:
            TypeString = "10 Pistol";
            CurrentSprite = Pistol;
            break;
            case 6:
            TypeString = "3 Other Gun";
            CurrentSprite = OtherGun;
            break;
            case 7:
            TypeString = "5 Grenade";
            CurrentSprite = Grenade;
            break;
        }

        UI_Type.GetComponent<Text>().text = TypeString;
        string gunname = CurrentSprite[currentGun].name;
        gunname = gunname.Substring(gunname.IndexOf(" "));
        UI_Name.GetComponent<Text>().text = gunname;
        UI_Gun.GetComponent<Image>().sprite = CurrentSprite[currentGun];
        if (currentType!=7){
            UI_Gun.GetComponent<RectTransform>().sizeDelta = new Vector2 (CurrentSprite[currentGun].rect.size.x*2,CurrentSprite[currentGun].rect.size.y*2);
        }else{
            UI_Gun.GetComponent<RectTransform>().sizeDelta = new Vector2 (CurrentSprite[currentGun].rect.size.x*1.5f,CurrentSprite[currentGun].rect.size.y*1.5f);
        }
        UI_Total.GetComponent<Text>().text = (currentGun+1) + "/" + CurrentSprite.Length;
    }
}
