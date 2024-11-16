using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenu : MonoBehaviour
{
    public GameObject rightClickMenu;
    public Button moveButton;
    public TextMeshProUGUI npcNameDisplay;

    private GameObject selectedNPC;
    private bool moveCommandActive = false;
    private bool punchCommandActive = false;
    private bool shootCommandActive = false;
    private GameObject attackingNPC;

    public Button punchButton;
    public Button shootButton;

    private void Start()
    {
        rightClickMenu.SetActive(false);
        npcNameDisplay.gameObject.SetActive(false);
        moveButton.onClick.AddListener(OnMoveButtonClicked);
        punchButton.onClick.AddListener(OnPunchButtonClicked);
        shootButton.onClick.AddListener(onShootButtonClicked);
    }

    void Update()
    {
        // Detect right-click for context menu
        if (
            Input.GetMouseButtonDown(1)
            && !moveCommandActive
            && !punchCommandActive
            && !shootCommandActive
        )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                NPCMovement npcMovement = hit.collider.GetComponent<NPCMovement>();
                if (npcMovement != null && npcMovement.isInPlayerGang) // Check if NPC is in the player's gang
                {
                    selectedNPC = hit.collider.gameObject;
                    ShowMenu(hit.collider.transform.position);
                }
            }
        }

        // Check if hovering over an NPC to show name
        Ray hoverRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hoverHit = Physics2D.Raycast(hoverRay.origin, hoverRay.direction);
        if (hoverHit.collider != null && hoverHit.collider.CompareTag("NPC"))
        {
            NPCMovement npcMovement = hoverHit.collider.GetComponent<NPCMovement>();
            if (npcMovement != null && npcMovement.isInPlayerGang)
            {
                DisplayNPCName(hoverHit.collider.transform.position, hoverHit.collider.name);
                npcNameDisplay.gameObject.SetActive(true);
            }
        }
        else
        {
            npcNameDisplay.gameObject.SetActive(false);
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

        // Combat targeting logic for Punch and Shoot
        if ((punchCommandActive || shootCommandActive) && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                NPCMovement targetNPC = hit.collider.GetComponent<NPCMovement>();
                if (targetNPC != null) // Ensure a valid NPC is clicked
                {
                    if (punchCommandActive)
                    {
                        ExecutePunch(attackingNPC, targetNPC.gameObject);
                    }
                    else if (shootCommandActive)
                    {
                        ExecuteShoot(attackingNPC, targetNPC.gameObject);
                    }
                }
            }

            // Reset combat state after the action
            punchCommandActive = false;
            shootCommandActive = false;
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
        Debug.Log(
            $"Showing menu at screen position: {screenPosition}, local position: {localPoint}"
        );
    }

    void ExecutePunch(GameObject attacker, GameObject target) {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null) {
            int punchDamage = 10;
            targetHealth.TakeDamage(punchDamage);
            Debug.Log($"{attacker.name} punched {target.name} for {punchDamage} damage.");
        }
    }

    void ExecuteShoot(GameObject attacker, GameObject target) {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null) {
            int gunDamage = 50;
            targetHealth.TakeDamage(gunDamage);
            Debug.Log($"{attacker.name} shot {target.name} for {gunDamage} damage.");
        }
    }

    void OnPunchButtonClicked()
    {
        punchCommandActive = true;
        attackingNPC = selectedNPC;
        rightClickMenu.SetActive(false);
        Debug.Log("Punch command active. Select a target.");
    }

    void onShootButtonClicked()
    {
        shootCommandActive = true;
        attackingNPC = selectedNPC;
        rightClickMenu.SetActive(false);
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
        npcNameDisplay.text = npcName;
    }

    private bool IsClickInsideUI(GameObject uiElement)
    {
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            Camera.main
        );
    }
}
