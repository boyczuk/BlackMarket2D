using UnityEngine;
using System.IO;
using TMPro; // Add this for TextMeshProUGUI
using UnityEngine.UI; // Add this for Image

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
        if (criminalOrganization == null)
            return;

        if (criminalOrganization.boss != null && !string.IsNullOrEmpty(criminalOrganization.boss.name))
        {
            DisplayRecruitedGangMember(criminalOrganization.boss);
            InstantiateNPC(criminalOrganization.boss);
        }

        foreach (var underboss in criminalOrganization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                DisplayRecruitedGangMember(underboss);
                InstantiateNPC(underboss);
            }
        }

        foreach (var lieutenant in criminalOrganization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                DisplayRecruitedGangMember(lieutenant);
                InstantiateNPC(lieutenant);
            }
        }

        foreach (var soldier in criminalOrganization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                DisplayRecruitedGangMember(soldier);
                InstantiateNPC(soldier);
            }
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

    void InstantiateNPC(GangMember recruitedNPC)
    {
        GameObject npcPrefab = Resources.Load<GameObject>("Prefabs/NPC"); // Assuming NPC.prefab is stored in a 'Resources/Prefabs' folder
        if (npcPrefab != null)
        {
            Instantiate(npcPrefab, recruitedNPC.position, Quaternion.identity);
        }
    }
}
