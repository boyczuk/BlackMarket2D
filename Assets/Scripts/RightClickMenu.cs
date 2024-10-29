using UnityEngine;
using TMPro;
using UnityEngine.UI; // Make sure you include this for TextMeshPro

public class RightClickMenu : MonoBehaviour
{
    public GameObject rightClickMenu;
    public Button moveButton;
    public TextMeshProUGUI npcNameDisplay; // Now directly the TextMeshPro object

    private GameObject selectedNPC;
    private bool moveCommandActive = false;

    private void Start()
    {
        rightClickMenu.SetActive(false);
        npcNameDisplay.gameObject.SetActive(false); // Ensure the name display is hidden initially
        moveButton.onClick.AddListener(OnMoveButtonClicked);
    }

    void Update()
    {
        // Detect right-click for context menu
        if (Input.GetMouseButtonDown(1) && !moveCommandActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                selectedNPC = hit.collider.gameObject;
                ShowMenu(hit.collider.transform.position);
            }
        }

        // Check if hovering over an NPC to show name
        Ray hoverRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hoverHit = Physics2D.Raycast(hoverRay.origin, hoverRay.direction);
        if (hoverHit.collider != null && hoverHit.collider.CompareTag("NPC"))
        {
            DisplayNPCName(hoverHit.collider.transform.position, hoverHit.collider.name);
            npcNameDisplay.gameObject.SetActive(true); // Enable display on hover
        }
        else
        {
            npcNameDisplay.gameObject.SetActive(false); // Disable display when not hovering
        }

        // Move NPC to position logic
        if (moveCommandActive && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveNPCToPosition(mousePos);
        }
        else if (!moveCommandActive && rightClickMenu.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsClickInsideUI(rightClickMenu) && !IsClickInsideUI(moveButton.gameObject))
            {
                rightClickMenu.SetActive(false);
            }
        }
    }

    void ShowMenu(Vector2 npcPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(npcPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rightClickMenu.transform.parent.GetComponent<RectTransform>(),
            screenPosition,
            Camera.main,
            out Vector2 localPoint
        );

        RectTransform rectTransform = rightClickMenu.GetComponent<RectTransform>();
        rectTransform.localPosition = localPoint;

        rightClickMenu.SetActive(true);
        Debug.Log($"Showing menu at screen position: {screenPosition}, local position: {localPoint}");
    }

    void OnMoveButtonClicked()
    {
        moveCommandActive = true;
        Debug.Log("Move button clicked. Awaiting map click to set destination.");
    }

    void MoveNPCToPosition(Vector2 position)
    {
        if (selectedNPC != null)
        {
            NPCMovement npcMovement = selectedNPC.GetComponent<NPCMovement>();
            if (npcMovement != null)
            {
                npcMovement.SetTargetPosition(position);
            }

            rightClickMenu.SetActive(false);
            moveCommandActive = false;
        }
    }

    void DisplayNPCName(Vector2 npcPosition, string npcName)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(npcPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            npcNameDisplay.transform.parent.GetComponent<RectTransform>(),
            screenPosition,
            Camera.main,
            out Vector2 localPoint
        );

        RectTransform rectTransform = npcNameDisplay.GetComponent<RectTransform>();
        rectTransform.localPosition = localPoint;
        npcNameDisplay.text = npcName; // Set the NPC's name directly on the TextMeshPro object
    }

    private bool IsClickInsideUI(GameObject uiElement)
    {
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main);
    }
}
