using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CriminalOrganization
{
    public GangMember boss;
    public List<GangMember> underbosses = new List<GangMember>();
    public List<GangMember> lieutenants = new List<GangMember>();
    public List<GangMember> soldiers = new List<GangMember>();
    public List<GangMember> kickedOutMembers = new List<GangMember>();

    public CriminalOrganization()
    {
        boss = null;
        underbosses = new List<GangMember>();
        lieutenants = new List<GangMember>();
        soldiers = new List<GangMember>();
    }

    // Method to get all members
    public List<GangMember> GetAllMembers()
    {
        List<GangMember> allMembers = new List<GangMember>();

        if (boss != null)
        {
            allMembers.Add(boss);
        }

        allMembers.AddRange(underbosses);
        allMembers.AddRange(lieutenants);
        allMembers.AddRange(soldiers);

        return allMembers;
    }

    public void AddMember(GangMember newMember)
    {
        switch (newMember.role)
        {
            case "Boss":
                boss = newMember;
                break;
            case "Underboss":
                underbosses.Add(newMember);
                break;
            case "Lieutenant":
                lieutenants.Add(newMember);
                break;
            case "Soldier":
                soldiers.Add(newMember);
                break;
            default:
                soldiers.Add(newMember); // Default to soldier if role is unspecified
                break;
        }
    }
}
