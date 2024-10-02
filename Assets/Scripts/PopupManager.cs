using System.Collections.Generic;
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
    public GameObject hierarchyMemberPrefab;

    public GameObject profilePanelPrefab;
    private GameObject activeProfilePanel;

    private GangDataManager gangDataManager;
    private CriminalOrganization criminalOrganization;
    private RecruitableNPC selectedNPC;

    void Start()
    {
        gangDataManager = FindObjectOfType<GangDataManager>();
        if (gangDataManager != null && gangDataManager.criminalOrganization != null)
        {
            criminalOrganization = gangDataManager.criminalOrganization;
            DisplayGangHierarchy();
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
        closeButton.gameObject.SetActive(true);
        DisplayGangHierarchy();
    }

    void ShowBasePopup()
    {
        recruitmentPanel.SetActive(false);
        hierarchyPanel.SetActive(false);
        backButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);
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

        foreach (Transform child in hierarchyPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (criminalOrganization != null && criminalOrganization.boss != null)
        {
            CreateHierarchyEntry(criminalOrganization.boss, hierarchyPanel.transform, 0, 0);
        }

        DisplayMembersInPyramid(criminalOrganization.underbosses.ToArray(), 1);
        DisplayMembersInPyramid(criminalOrganization.lieutenants.ToArray(), 2);
        DisplayMembersInPyramid(criminalOrganization.soldiers.ToArray(), 3);
    }

    void DisplayMembersInPyramid(GangMember[] members, int row)
    {
        if (members == null || members.Length == 0)
            return;

        int columns = members.Length;
        float xOffset = 200f;

        for (int i = 0; i < columns; i++)
        {
            float xPosition = (i - (columns - 1) / 2.0f) * xOffset;
            CreateHierarchyEntry(members[i], hierarchyPanel.transform, row, xPosition);
        }
    }

    void CreateHierarchyEntry(GangMember member, Transform parent, int row, float xOffset)
    {
        if (member == null)
            return;

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

        Button memberButton = memberDisplay.GetComponent<Button>();
        if (memberButton != null)
        {
            memberButton.onClick.AddListener(() => DisplayMemberProfile(member));
        }
    }

    void DisplayMemberProfile(GangMember member)
    {
        if (activeProfilePanel != null)
        {
            Destroy(activeProfilePanel);
        }

        activeProfilePanel = Instantiate(profilePanelPrefab, transform.parent);

        RectTransform rectTransform = activeProfilePanel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        TextMeshProUGUI nameText = activeProfilePanel
            .transform.Find("NameText")
            .GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = member.name;
        }

        Image mugshotImage = activeProfilePanel
            .transform.Find("MugshotImage")
            .GetComponent<Image>();
        if (mugshotImage != null)
        {
            mugshotImage.sprite = member.mugshot;
        }

        TMP_Dropdown roleDropdown = activeProfilePanel
            .transform.Find("RoleDropdown")
            .GetComponent<TMP_Dropdown>();
        if (roleDropdown != null)
        {
            roleDropdown.ClearOptions();
            roleDropdown.AddOptions(
                new List<string> { "Boss", "Underboss", "Lieutenant", "Soldier" }
            );
            roleDropdown.value = roleDropdown.options.FindIndex(option =>
                option.text == member.role
            );

            roleDropdown.onValueChanged.AddListener(
                delegate
                {
                    string selectedRole = roleDropdown.options[roleDropdown.value].text;
                    ChangeMemberRole(member, selectedRole);
                }
            );
        }

        // Find and set up the Kick Out button
        Button kickOutButton = activeProfilePanel
            .transform.Find("KickOutButton")
            .GetComponent<Button>();
        if (kickOutButton != null)
        {
            kickOutButton.onClick.RemoveAllListeners();
            kickOutButton.onClick.AddListener(() => KickOutMember(member));
        }

        Debug.Log($"Displaying profile for: {member.name}");
    }

    void KickOutMember(GangMember member)
    {
        Debug.Log($"Kicking out member: {member.name}");

        // Remove from the appropriate list
        if (criminalOrganization.boss == member)
        {
            criminalOrganization.boss = null;
        }
        else if (criminalOrganization.underbosses.Contains(member))
        {
            criminalOrganization.underbosses.Remove(member);
        }
        else if (criminalOrganization.lieutenants.Contains(member))
        {
            criminalOrganization.lieutenants.Remove(member);
        }
        else if (criminalOrganization.soldiers.Contains(member))
        {
            criminalOrganization.soldiers.Remove(member);
        }

        // Add to kicked out list
        member.kickedOut = true;
        criminalOrganization.kickedOutMembers.Add(member);

        // Save the updated organization data
        gangDataManager.SaveGangData(criminalOrganization);

        // Close profile and update hierarchy
        if (activeProfilePanel != null)
        {
            Destroy(activeProfilePanel);
        }

        DisplayGangHierarchy();
    }

    void ChangeMemberRole(GangMember member, string newRole)
    {
        switch (member.role)
        {
            case "Boss":
                criminalOrganization.boss = null;
                break;
            case "Underboss":
                criminalOrganization.underbosses.Remove(member);
                break;
            case "Lieutenant":
                criminalOrganization.lieutenants.Remove(member);
                break;
            case "Soldier":
                criminalOrganization.soldiers.Remove(member);
                break;
        }

        member.role = newRole;
        switch (newRole)
        {
            case "Boss":
                criminalOrganization.boss = member;
                break;
            case "Underboss":
                criminalOrganization.underbosses.Add(member);
                break;
            case "Lieutenant":
                criminalOrganization.lieutenants.Add(member);
                break;
            case "Soldier":
                criminalOrganization.soldiers.Add(member);
                break;
        }

        gangDataManager.SaveGangData(criminalOrganization);

        DisplayGangHierarchy();

        Debug.Log($"Changed role of {member.name} to {newRole}");
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
