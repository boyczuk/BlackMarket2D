using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public RecruitableNPC[] recruitableNPCs;
    public Transform npcListContainer;
    public GameObject npcButtonPrefab;
    public TextMeshProUGUI npcDescriptionText;
    public Button confirmButton;
    public Image npcMugshotImage;

    public Transform gangMembersContainer;
    public GameObject gangMemberDisplayPrefab;

    private CriminalOrganization criminalOrganization = new CriminalOrganization();
    private GangDataManager gangDataManager;
    private RecruitableNPC selectedNPC;

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

            if (gangDataManager != null)
            {
                gangDataManager.SaveGangData(criminalOrganization);
            }

            DisplayRecruitedGangMember(selectedNPC);
            ClosePopup();
        }
    }

    void DisplayRecruitedGangMember(RecruitableNPC recruitedNPC)
    {
        GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);
        TextMeshProUGUI nameText = gangMemberDisplay.transform.Find("GangMemberNameText").GetComponent<TextMeshProUGUI>();
        
        if (nameText != null)
        {
            nameText.text = recruitedNPC.npcName;
        }

        Image portraitImage = gangMemberDisplay.transform.Find("PortraitImage").GetComponent<Image>();
        
        if (portraitImage != null)
        {
            portraitImage.sprite = recruitedNPC.npcMugshot;
        }
    }

    void DisplayExistingGangMembers(){
        if (criminalOrganization == null) return;

        if (criminalOrganization.boss != null && !string.IsNullOrEmpty(criminalOrganization.boss.name))
        {
            DisplayGangMember(criminalOrganization.boss);
        }

        foreach (var underboss in criminalOrganization.underbosses)
        {
            if (!string.IsNullOrEmpty(underboss.name))
                DisplayGangMember(underboss);
        }

        foreach (var lieutenant in criminalOrganization.lieutenants)
        {
            if (!string.IsNullOrEmpty(lieutenant.name))
                DisplayGangMember(lieutenant);
        }

        foreach (var soldier in criminalOrganization.soldiers)
        {
            if (!string.IsNullOrEmpty(soldier.name))
                DisplayGangMember(soldier);
        }
    }


    void DisplayGangMember(GangMember member)
    {
        GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);
        TextMeshProUGUI nameText = gangMemberDisplay.transform.Find("GangMemberNameText").GetComponent<TextMeshProUGUI>();
        
        if (nameText != null)
        {
            nameText.text = member.name;
        }

        Image portraitImage = gangMemberDisplay.transform.Find("PortraitImage").GetComponent<Image>();
        
        if (portraitImage != null)
        {
            portraitImage.sprite = member.mugshot;
        }
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
