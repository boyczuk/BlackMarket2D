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
    public Button closeButton;
    public GameObject hierarchyMemberPrefab; // Add this for member prefab

    private GangDataManager gangDataManager;
    private CriminalOrganization criminalOrganization;
    private RecruitableNPC selectedNPC;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();
        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            criminalOrganization = gangDataManager.criminalOrganization;
            DisplayGangHierarchy(); // Add this to display hierarchy on load if needed
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
        closeButton.gameObject.SetActive(true);
    }

    void ShowHierarchyPanel()
    {
        recruitmentPanel.SetActive(false);
        hierarchyPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
        DisplayGangHierarchy(); // Call hierarchy display when showing hierarchy panel
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

    void DisplayGangHierarchy()
    {
        if (hierarchyPanel == null)
            return;

        // Clear existing elements in the hierarchy panel
        foreach (Transform child in hierarchyPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Start with the boss
        if (criminalOrganization != null && criminalOrganization.boss != null)
        {
            CreateHierarchyEntry(criminalOrganization.boss, hierarchyPanel.transform, 0, 0);
        }

        // Display other ranks in the hierarchy
        DisplayMembersInPyramid(criminalOrganization.underbosses.ToArray(), 1);
        DisplayMembersInPyramid(criminalOrganization.lieutenants.ToArray(), 2);
        DisplayMembersInPyramid(criminalOrganization.soldiers.ToArray(), 3);
    }

    void DisplayMembersInPyramid(GangMember[] members, int row)
    {
        if (members == null || members.Length == 0)
            return;

        int columns = members.Length;
        float xOffset = 200f; // Adjust for horizontal spacing

        for (int i = 0; i < columns; i++)
        {
            float xPosition = (i - (columns - 1) / 2.0f) * xOffset; // Center the members horizontally
            CreateHierarchyEntry(members[i], hierarchyPanel.transform, row, xPosition);
        }
    }

    void CreateHierarchyEntry(GangMember member, Transform parent, int row, float xOffset)
    {
        if (member == null) return; // Check if the member is not null

        GameObject memberDisplay = Instantiate(hierarchyMemberPrefab, parent);
        memberDisplay.transform.localPosition = new Vector3(xOffset, -row * 55f + -5f, 0);

        TextMeshProUGUI memberText = memberDisplay
            .transform.Find("MemberText")
            .GetComponent<TextMeshProUGUI>();
        if (memberText != null)
        {
            memberText.text = $"{member.name}\n{member.role}";
        }

        Image memberImage = memberDisplay.transform.Find("Image").GetComponent<Image>();
        if (memberImage != null)
        {
            memberImage.sprite = member.mugshot;
        }
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
