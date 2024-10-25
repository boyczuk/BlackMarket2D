using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public bool spawn;
    public float SpawnRate;
    public GameObject Enemy;
    private float timer = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn){
            timer = timer + Time.deltaTime;
            if (timer>=SpawnRate){
                timer = 0;
                GameObject Enemyclone = Instantiate(Enemy, transform.position, Enemy.transform.rotation);
            }
        }
    }
}
