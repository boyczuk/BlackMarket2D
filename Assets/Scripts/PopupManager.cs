using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public RecruitableNPC[] recruitableNPCs;
    public Transform npcListContainer;
    public GameObject npcButtonPrefab;
    public TextMeshProUGUI npcDescriptionText;
    public Button confirmButton;
    public Image npcMugshotImage;

    private GangDataManager gangDataManager;
    private CriminalOrganization criminalOrganization;
    private RecruitableNPC selectedNPC;
    public GameObject gangMemberDisplayPrefab;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();
        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            criminalOrganization = gangDataManager.criminalOrganization;
            DisplayExistingGangMembers();
        }

        PopulateNPCList();
        confirmButton.onClick.AddListener(RecruitSelectedNPC);
        confirmButton.interactable = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    void DisplayExistingGangMembers()
    {
        if (criminalOrganization == null)
            return;

        if (
            criminalOrganization.boss != null
            && !string.IsNullOrEmpty(criminalOrganization.boss.name)
        )
        {
            DisplayRecruitedGangMember(criminalOrganization.boss);
        }

        foreach (var underboss in criminalOrganization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
            {
                DisplayRecruitedGangMember(underboss);
            }
        }

        foreach (var lieutenant in criminalOrganization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
            {
                DisplayRecruitedGangMember(lieutenant);
            }
        }

        foreach (var soldier in criminalOrganization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
            {
                DisplayRecruitedGangMember(soldier);
            }
        }
    }

    void PopulateNPCList()
    {
        foreach (RecruitableNPC npc in recruitableNPCs)
        {
            GameObject npcButton = Instantiate(npcButtonPrefab, npcListContainer);
            TextMeshProUGUI buttonText = npcButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = npc.npcName;

            Button buttonComponent = npcButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => SelectNPC(npc));
        }
    }

    void SelectNPC(RecruitableNPC npc)
    {
        selectedNPC = npc;
        npcDescriptionText.text = npc.description;
        npcMugshotImage.sprite = npc.npcMugshot;
        confirmButton.interactable = true;
    }

    void RecruitSelectedNPC()
    {
        if (selectedNPC != null)
        {
            GangMember newMember = new GangMember(
                selectedNPC.npcName,
                "Soldier",
                selectedNPC.npcMugshot,
                Vector3.zero
            );

            criminalOrganization.AddMember(newMember);
            gangDataManager.SaveGangData(criminalOrganization);

            GameObject npcInstance = Instantiate(
                selectedNPC.npcPrefab,
                Vector3.zero,
                Quaternion.identity
            );

            DisplayRecruitedGangMember(newMember);

            ClosePopup();
        }
    }

    void DisplayRecruitedGangMember(GangMember recruitedNPC)
    {
        Transform gangMembersContainer = GameObject.Find("GangMembersPanel")?.transform;
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

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
