using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GangDataManager gangDataManager;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();

        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            DisplayExistingGangMembers(gangDataManager.criminalOrganization);
        }
    }

    private void DisplayExistingGangMembers(CriminalOrganization organization)
    {
        if (organization.boss != null)
        {
            CreateGangMemberDisplay(organization.boss);
        }

        foreach (var underboss in organization.underbosses)
        {
            CreateGangMemberDisplay(underboss);
        }

        foreach (var lieutenant in organization.lieutenants)
        {
            CreateGangMemberDisplay(lieutenant);
        }

        foreach (var soldier in organization.soldiers)
        {
            CreateGangMemberDisplay(soldier);
        }
    }

    private void CreateGangMemberDisplay(GangMember member)
    {
        Transform gangMembersContainer = GameObject.Find("GangMembersPanel").transform;
        GameObject gangMemberDisplayPrefab = Resources.Load<GameObject>("Prefabs/GangMemberDisplay");

        if (gangMemberDisplayPrefab != null && gangMembersContainer != null)
        {
            GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);
            TextMeshProUGUI nameText = gangMemberDisplay.transform.Find("GangMemberNameText").GetComponent<TextMeshProUGUI>();
            nameText.text = member.name;
            Image portraitImage = gangMemberDisplay.transform.Find("PortraitImage").GetComponent<Image>();
            portraitImage.sprite = member.mugshot;
        }
    }
}
