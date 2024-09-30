using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CriminalOrganization
{
    public GangMember boss;
    public List<GangMember> underbosses = new List<GangMember>();
    public List<GangMember> lieutenants = new List<GangMember>();
    public List<GangMember> soldiers = new List<GangMember>();

    public void AddMember(GangMember member){
        switch(member.role){
            case "Boss":
                boss = member;
                break;
            case "Underboss":
                underbosses.Add(member);
                break;
            case "Lieutenant":
                underbosses.Add(member);
                break;
            case "Soldier":
                underbosses.Add(member);
                break;
        }
    }
}
