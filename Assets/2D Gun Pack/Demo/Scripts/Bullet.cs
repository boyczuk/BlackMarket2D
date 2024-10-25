using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletSpeed;
    public float Damage;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 force = transform.right * BulletSpeed;
        rb.AddForce(force, ForceMode2D.Impulse  );
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.name == "Ground"){
            GameObject.Destroy(gameObject);
        }
    }
}
