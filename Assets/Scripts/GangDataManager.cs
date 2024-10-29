using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GangDataManager : MonoBehaviour
{
    private string playerSavePath;
    private string enemySavePath;
    public CriminalOrganization playerGang;
    public CriminalOrganization enemyGang;
    public GameObject gangMemberDisplayPrefab;

    void Start()
    {
        playerSavePath = Application.persistentDataPath + "/gangData.json";
        enemySavePath = Application.persistentDataPath + "/enemyGangData.json";

        LoadPlayerGangData();
        LoadEnemyGangData();

        DisplayExistingGangMembers(playerGang, true);
        DisplayExistingGangMembers(enemyGang, false);

        SpawnGangMembersInWorld(playerGang, true);
        SpawnGangMembersInWorld(enemyGang, false);
    }

    public void SaveGangData(CriminalOrganization organization, bool isPlayerGang)
    {
        string path = isPlayerGang ? playerSavePath : enemySavePath;
        string json = JsonUtility.ToJson(organization, true);
        File.WriteAllText(path, json);
    }

    public void LoadPlayerGangData()
    {
        playerGang = LoadGangData(playerSavePath) ?? CreateDefaultOrganization();
        SaveGangData(playerGang, true);
    }

    public void LoadEnemyGangData()
    {
        enemyGang = LoadGangData(enemySavePath) ?? CreateDefaultEnemyGang();
        SaveGangData(enemyGang, false);
    }

    private CriminalOrganization LoadGangData(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonUtility.FromJson<CriminalOrganization>(json);
            }
        }
        return null;
    }

    private CriminalOrganization CreateDefaultOrganization()
    {
        CriminalOrganization org = new CriminalOrganization();
        GangMember defaultBoss = new GangMember("Default Boss", "Boss", null, Vector3.zero, "NPC");
        org.boss = defaultBoss;
        return org;
    }

    private CriminalOrganization CreateDefaultEnemyGang()
    {
        CriminalOrganization org = new CriminalOrganization();

        GangMember enemyBoss = new GangMember(
            "Raul “Razor” Sanchez",
            "Boss",
            null,
            new Vector3(10, -5, 0),
            "EnemyNPCBoss"
        );
        org.boss = enemyBoss;

        GangMember enemyHitman = new GangMember(
            "Leon Vega",
            "Hitman",
            null,
            new Vector3(12, -6, 0),
            "EnemyNPCHitman"
        );
        org.underbosses.Add(enemyHitman);

        GangMember enemyCapo = new GangMember(
            "Miguel “Shorty” Rivera",
            "Capo",
            null,
            new Vector3(11, -7, 0),
            "EnemyNPCCapo"
        );
        org.lieutenants.Add(enemyCapo);

        return org;
    }

    void DisplayExistingGangMembers(CriminalOrganization organization, bool isPlayerGang)
    {
        if (organization == null)
            return;

        if (organization.boss != null && !string.IsNullOrEmpty(organization.boss.name))
        {
            organization.boss.LoadMugshot();
            DisplayRecruitedGangMember(organization.boss, isPlayerGang);
        }

        foreach (var underboss in organization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                underboss.LoadMugshot();
                DisplayRecruitedGangMember(underboss, isPlayerGang);
            }
        }

        foreach (var lieutenant in organization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                lieutenant.LoadMugshot();
                DisplayRecruitedGangMember(lieutenant, isPlayerGang);
            }
        }

        foreach (var soldier in organization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                soldier.LoadMugshot();
                DisplayRecruitedGangMember(soldier, isPlayerGang);
            }
        }
    }

    void SpawnGangMembersInWorld(CriminalOrganization organization, bool isPlayerControlled)
    {
        if (organization == null)
            return;

        if (organization.boss != null && !string.IsNullOrEmpty(organization.boss.name))
        {
            SpawnMemberInWorld(organization.boss, isPlayerControlled);
        }

        foreach (var underboss in organization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                SpawnMemberInWorld(underboss, isPlayerControlled);
            }
        }

        foreach (var lieutenant in organization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                SpawnMemberInWorld(lieutenant, isPlayerControlled);
            }
        }

        foreach (var soldier in organization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                SpawnMemberInWorld(soldier, isPlayerControlled);
            }
        }
    }

    void SpawnMemberInWorld(GangMember member, bool isPlayerControlled)
    {
        if (string.IsNullOrEmpty(member.npcPrefabName))
        {
            Debug.LogWarning(
                "Missing npcPrefabName for member: " + member.name + ". Assigning default prefab."
            );
            member.npcPrefabName = "NPC";
        }

        GameObject npcPrefab = Resources.Load<GameObject>("Prefabs/" + member.npcPrefabName);
        if (npcPrefab != null)
        {
            GameObject npcInstance = Instantiate(npcPrefab, member.position, Quaternion.identity);
            npcInstance.name = member.name;

            NPCMovement npcMovement = npcInstance.GetComponent<NPCMovement>();
            if (npcMovement != null)
            {
                npcMovement.isInPlayerGang = isPlayerControlled; // Set based on gang membership
                npcMovement.isPlayerControlled = isPlayerControlled; // Tracks if NPC is following player commands
            }
        }
    }

    void DisplayRecruitedGangMember(GangMember recruitedNPC, bool isPlayerGang)
    {
        Transform gangMembersContainer = GameObject
            .Find(isPlayerGang ? "GangMembersPanel" : "EnemyGangPanel")
            ?.transform;
        GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);

        TextMeshProUGUI nameText = gangMemberDisplay
            .transform.Find("GangMemberNameText")
            .GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = recruitedNPC.name;
        }

        Image portraitImage = gangMemberDisplay
            .transform.Find("PortraitImage")
            .GetComponent<Image>();
        if (portraitImage != null)
        {
            portraitImage.sprite = recruitedNPC.mugshot;
        }
    }
}
