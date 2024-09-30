using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GangMember {
    public string name;
    public string role;
    public Sprite mugshot;
    public Vector3 position;

    public GangMember(string name, string role, Sprite mugshot, Vector3 position){
        this.name = name;
        this.role = role;
        this.mugshot = mugshot;
        this.position = position;
    }
}