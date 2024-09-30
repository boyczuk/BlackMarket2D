using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class GangDataManager : MonoBehaviour
{
    private string savePath;
    public CriminalOrganization criminalOrganization;
    public GameObject gangMemberDisplayPrefab;

    void Start()
    {
        savePath = Application.persistentDataPath + "/gangData.json";
        LoadGangData();
        DisplayExistingGangMembers();
        SpawnGangMembersInWorld();
    }

    public void SaveGangData(CriminalOrganization organization)
    {
        string json = JsonUtility.ToJson(organization, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadGangData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            criminalOrganization = JsonUtility.FromJson<CriminalOrganization>(json);
        }
        else
        {
            criminalOrganization = new CriminalOrganization();
        }
    }

    void DisplayExistingGangMembers()
    {
        if (criminalOrganization == null) return;

        if (criminalOrganization.boss != null && !string.IsNullOrEmpty(criminalOrganization.boss.name))
        {
            criminalOrganization.boss.LoadMugshot();
            DisplayRecruitedGangMember(criminalOrganization.boss);
        }

        foreach (var underboss in criminalOrganization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                underboss.LoadMugshot();
                DisplayRecruitedGangMember(underboss);
            }
        }

        foreach (var lieutenant in criminalOrganization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                lieutenant.LoadMugshot();
                DisplayRecruitedGangMember(lieutenant);
            }
        }

        foreach (var soldier in criminalOrganization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                soldier.LoadMugshot();
                DisplayRecruitedGangMember(soldier);
            }
        }
    }

    void SpawnGangMembersInWorld()
    {
        if (criminalOrganization == null) return;

        if (criminalOrganization.boss != null && !string.IsNullOrEmpty(criminalOrganization.boss.name))
        {
            SpawnMemberInWorld(criminalOrganization.boss);
        }

        foreach (var underboss in criminalOrganization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                SpawnMemberInWorld(underboss);
            }
        }

        foreach (var lieutenant in criminalOrganization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                SpawnMemberInWorld(lieutenant);
            }
        }

        foreach (var soldier in criminalOrganization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                SpawnMemberInWorld(soldier);
            }
        }
    }

    void SpawnMemberInWorld(GangMember member)
    {
        GameObject npcPrefab = Resources.Load<GameObject>("Prefabs/NPC");
        if (npcPrefab != null)
        {
            GameObject npcInstance = Instantiate(npcPrefab, member.position, Quaternion.identity);
            npcInstance.name = member.name;
        }
    }

    void DisplayRecruitedGangMember(GangMember recruitedNPC)
    {
        Transform gangMembersContainer = GameObject.Find("GangMembersPanel")?.transform;
        GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);

        TextMeshProUGUI nameText = gangMemberDisplay.transform.Find("GangMemberNameText").GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = recruitedNPC.name;
        }

        Image portraitImage = gangMemberDisplay.transform.Find("PortraitImage").GetComponent<Image>();
        if (portraitImage != null)
        {
            portraitImage.sprite = recruitedNPC.mugshot;
        }
    }
}
