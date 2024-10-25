using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header ("Player Setting")]
    public bool dead;
    public float HP;
    public float maxHP;
    [Header ("Gun Setting")]
    public float Firerate;
    public float BulletSpeed;
    public float Damage;
    public int Ammo;
    public int maxAmmo;
    public float ReloadSpeed;
    [Header ("Script Setting")]
    public GameObject HPBar;
    public GameObject bullet;
    public GameObject bulletDrop;
    public GameObject GunObject;
    public GameObject FireEffect;
    public Transform FireSpot;
    public GameObject InfoText;
    private bool fire = true;
    private Vector2 GunObject_oldposition;
    void Start()
    {
        GunObject_oldposition = GunObject.transform.localPosition;
        InfoText.GetComponent<TextMesh>().text = Ammo + "/" + maxAmmo;
    }

    void Update()
    {
        if (!dead){
        var mousePos = Input.mousePosition;
    
        mousePos.z = 10.0f;
        Vector3 World_mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        GunObject.transform.right= World_mousePos - GunObject.transform.position;

        if (Input.GetMouseButton(0) && fire && Ammo>=0){
            Ammo = Ammo-1;
            if(Ammo<0){
                StartCoroutine(ReloadAmmo());
            }else{
                StartCoroutine(FireAnimation());
                StartCoroutine(RecoilAnimation());
                StartCoroutine(Shoot(GunObject.transform.rotation));
                InfoText.GetComponent<TextMesh>().text = Ammo + "/" + maxAmmo;
            }
        }
        }
    }

    IEnumerator Shoot(Quaternion bulletRot){
        //Add rotation for Accurate
        bulletRot.z = bulletRot.z + Random.Range(-0.01f,0.01f);
        fire = false;
        //create bullet prefab
        GameObject bulletclone = Instantiate(bullet,FireSpot.position,bulletRot);
        bulletclone.GetComponent<Bullet>().BulletSpeed = BulletSpeed;
        bulletclone.GetComponent<Bullet>().Damage = Damage;
        bulletclone.name = "Bullet";
        GameObject.Destroy(bulletclone, 5f);

        //create dropping bullet prefab
        Vector2 DropSpot = FireSpot.position;
        DropSpot.x = DropSpot.x - 0.2f;
        GameObject bulletDropclone = Instantiate(bulletDrop,DropSpot,bulletRot);
        GameObject.Destroy(bulletDropclone, 2f);
        yield return new WaitForSeconds(Firerate);
        fire = true;
    }

    IEnumerator ReloadAmmo(){
        InfoText.GetComponent<TextMesh>().text = "Reloading..";
        yield return new WaitForSeconds(ReloadSpeed);
        Ammo = maxAmmo;
        fire = true;
        InfoText.GetComponent<TextMesh>().text = Ammo + "/" + maxAmmo;
    }

    IEnumerator RecoilAnimation(){
        float time = 0.1f;
        float elapsedTime = 0;
        Vector2 startingPos = GunObject_oldposition;
        Vector2 finalPos = startingPos;
        finalPos.x = finalPos.x-0.2f;
        while (elapsedTime < time)
        {
            GunObject.transform.localPosition = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GunObject.transform.localPosition = GunObject_oldposition;
    }

    IEnumerator FireAnimation(){
        FireEffect.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        FireEffect.GetComponent<SpriteRenderer>().enabled = false;

    }

    
    public void UpdateHPBar(){
        float xScale = HP/maxHP;
        if (HP<=0){
            xScale = 0;
            StartCoroutine(Dead());
        }
        HPBar.transform.localScale = new Vector3(xScale, 
                                    HPBar.transform.localScale.y, HPBar.transform.localScale.z);
    }
    
    IEnumerator Dead(){
        GetComponent<BoxCollider2D>().enabled = false;
        dead = true;
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().spawn = false;

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

    public void DamageTake(){
        StartCoroutine(DamageTakeAnimation());
    }

    IEnumerator DamageTakeAnimation(){
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

}
