using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private BoxCollider2D doorCollider;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            OpenDoor();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        doorCollider.enabled = false;
    }

    void CloseDoor()
    {
        doorCollider.enabled = true;
    }
}
