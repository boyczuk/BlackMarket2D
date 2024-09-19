using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;

    private Vector2 movementDirection;
    private float timer;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomDirection();
        timer = changeDirectionTime;
    }

    // Update is called once per frame
    void Update()
    {
        MoveNPC();
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SetRandomDirection();
            timer = changeDirectionTime;
        }
    }

    void SetRandomDirection()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void MoveNPC() {
        rb.velocity = movementDirection * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            SetRandomDirection();
        }
    }
}
