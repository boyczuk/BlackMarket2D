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
    public GameObject gangMemberDisplayPrefab;

    public GameObject recruitmentPanel;        
    public GameObject hierarchyPanel;           
    public Button viewRecruitmentButton;       
    public Button viewHierarchyButton;        
    public Button backButton;                   

    private GangDataManager gangDataManager;
    private CriminalOrganization criminalOrganization;
    private RecruitableNPC selectedNPC;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();
        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            criminalOrganization = gangDataManager.criminalOrganization;
        }

        PopulateNPCList();
        confirmButton.onClick.AddListener(RecruitSelectedNPC);
        confirmButton.interactable = false;

        viewRecruitmentButton.onClick.AddListener(ShowRecruitmentPanel);
        viewHierarchyButton.onClick.AddListener(ShowHierarchyPanel);
        backButton.onClick.AddListener(ShowBasePopup);

        recruitmentPanel.SetActive(false);
        hierarchyPanel.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    void ShowRecruitmentPanel()
    {
        recruitmentPanel.SetActive(true);
        hierarchyPanel.SetActive(false);
        backButton.gameObject.SetActive(true); 
    }

    void ShowHierarchyPanel()
    {
        recruitmentPanel.SetActive(false);
        hierarchyPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    void ShowBasePopup()
    {
        recruitmentPanel.SetActive(false);
        hierarchyPanel.SetActive(false);
        backButton.gameObject.SetActive(false);
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

            newMember.SetMugshot(selectedNPC.npcMugshot);

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
