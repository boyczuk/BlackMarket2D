using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public bool move = true;
    public float HP;
    public float maxHP;
    public float moveSpeed;
    public float Damage;
    public GameObject HPBar;
    private float moveTime;
    
    void Start()
    {
        UpdateHPBar();
    }

    void Update()
    {
        if (move){
            transform.Translate(-Vector3.right * moveSpeed  * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //Checking collision tag/name
        if (other.transform.name == "Bullet"){
            StartCoroutine(DamageTakeAnimation());
            HP = HP - other.transform.GetComponent<Bullet>().Damage;
            if (HP<=0) StartCoroutine(Dead());
            GameObject.Destroy(other.gameObject);
            UpdateHPBar();
        }
        if (other.transform.name == "Player"){
            other.transform.GetComponent<PlayerControl>().HP -= Damage;
            other.transform.GetComponent<PlayerControl>().UpdateHPBar();
            other.transform.GetComponent<PlayerControl>().DamageTake();
            StartCoroutine(Dead());
        }
    }

    IEnumerator DamageTakeAnimation(){
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void UpdateHPBar(){
        float xScale = HP/maxHP;
        if (HP<=0) xScale = 0;
        HPBar.transform.localScale = new Vector3(xScale, 
                                    HPBar.transform.localScale.y, HPBar.transform.localScale.z);
    }

    IEnumerator Dead(){
        GetComponent<BoxCollider2D>().enabled = false;
        move = false;

        float time = 0.5f;
        float elapsedTime = 0;
        Vector2 startingPos = transform.position;
        Vector2 finalPos = startingPos;
        finalPos.y = finalPos.y+2f;
            
        while (elapsedTime < time)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f, time - elapsedTime);
            transform.position = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GameObject.Destroy(transform.gameObject);
    }
}
