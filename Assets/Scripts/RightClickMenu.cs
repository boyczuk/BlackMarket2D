using System.Collections;
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
                if (npcMovement != null && npcMovement.isInPlayerGang) 
                {
                    selectedNPC = hit.collider.gameObject;
                    ShowMenu(hit.collider.transform.position);
                }
            }
        }

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

        if ((punchCommandActive || shootCommandActive) && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                NPCMovement targetNPC = hit.collider.GetComponent<NPCMovement>();
                if (targetNPC != null)
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

    void ExecutePunch(GameObject attacker, GameObject target)
    {
        float requiredDistance = 1.5f; 
        NPCMovement npcMovement = attacker.GetComponent<NPCMovement>();

        if (npcMovement == null)
        {
            Debug.LogWarning($"Attacker {attacker.name} has no NPCMovement component.");
            return;
        }

        float currentDistance = Vector2.Distance(
            attacker.transform.position,
            target.transform.position
        );

        if (currentDistance > requiredDistance)
        {
            Debug.Log($"{attacker.name} is too far to punch {target.name}. Moving closer.");
            npcMovement.SetTargetPosition(target.transform.position);

            StartCoroutine(MonitorProximityAndPunch(attacker, target, requiredDistance));
        }
        else
        {
            PerformPunch(attacker, target);
        }
    }

    IEnumerator MonitorProximityAndPunch(
        GameObject attacker,
        GameObject target,
        float requiredDistance
    )
    {
        NPCMovement npcMovement = attacker.GetComponent<NPCMovement>();
        while (
            Vector2.Distance(attacker.transform.position, target.transform.position)
            > requiredDistance
        )
        {
            npcMovement.SetTargetPosition(target.transform.position);
            yield return null; 
        }

        PerformPunch(attacker, target);
    }

    void PerformPunch(GameObject attacker, GameObject target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            int punchDamage = 10; 
            targetHealth.TakeDamage(punchDamage);
            Debug.Log($"{attacker.name} punched {target.name} for {punchDamage} damage.");
        }
    }

    void ExecuteShoot(GameObject attacker, GameObject target)
    {
        float maxRange = 10f;
        float currentDistance = Vector2.Distance(
            attacker.transform.position,
            target.transform.position
        );

        if (currentDistance > maxRange)
        {
            Debug.Log($"{attacker.name} is too far to shoot {target.name}.");
            return;
        }

        Vector2 direction = (target.transform.position - attacker.transform.position).normalized;
        int wallLayerMask = LayerMask.GetMask("Walls");
        RaycastHit2D hit = Physics2D.Raycast(
            attacker.transform.position,
            direction,
            maxRange,
            wallLayerMask
        );

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            Debug.Log(
                $"{attacker.name} has no clear line of sight to {target.name}. Blocked by: {hit.collider.name}"
            );
            return;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
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
