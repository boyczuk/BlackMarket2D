using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecruitableNPC
{
    public string npcName;
    public GameObject npcPrefab;
    public string description;
    public Sprite npcMugshot;
    // Here will be more info like affiliations (incase of bad blood with other gangs)
}
