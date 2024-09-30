using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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

    private List<RecruitableNPC> recruitedGangMembers = new List<RecruitableNPC>();
    private RecruitableNPC selectedNPC;

    void Start()
    {
        PopulateNPCList();
        confirmButton.onClick.AddListener(RecruitSelectedNPC);
        confirmButton.interactable = false;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
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
        Debug.Log("Selected NPC: " + npc.npcName);
    }

    void RecruitSelectedNPC()
    {
        if (selectedNPC != null)
        {
            recruitedGangMembers.Add(selectedNPC);
            Instantiate(selectedNPC.npcPrefab, Vector3.zero, Quaternion.identity);
            DisplayRecruitedGangMember(selectedNPC);
            Debug.Log(selectedNPC.npcName + " has been recruited!");
            ClosePopup();
        }
    }

    void DisplayRecruitedGangMember(RecruitableNPC recruitedNPC)
    {
        Debug.Log("Displaying recruited NPC: " + recruitedNPC.npcName);

        GameObject gangMemberDisplay = Instantiate(gangMemberDisplayPrefab, gangMembersContainer);

        TextMeshProUGUI nameText = gangMemberDisplay.transform.Find("GangMemberNameText").GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = recruitedNPC.npcName;
        }
        else
        {
            Debug.LogError("Could not find TextMeshProUGUI component on GangMemberNameText");
        }

        Image portraitImage = gangMemberDisplay.transform.Find("PortraitImage").GetComponent<Image>();
        if (portraitImage != null)
        {
            portraitImage.sprite = recruitedNPC.npcMugshot;
        }
        else
        {
            Debug.LogError("Could not find Image component on PortraitImage");
        }
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
