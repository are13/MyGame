using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

    public static string path = "sprite/prefabs/";

    public List<SpawnData> candidates = new List<SpawnData>();
    public List<GameObject> prePlaced;
    public List<GameObject> doors;
    public List<Vector2> startPoint = new List<Vector2>(4);
    public BoxCollider2D rb;
    public int total;
    int left;
    float process = 0f;
    bool isFinished = false;
    // public List<GameObject> lockedEntity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        
    }

    void Start()
    {
        left = total;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFinished && collision.gameObject.CompareTag("Player"))
        {

            for (int i = 0; i < candidates.Count; i++)
            {

                if (candidates[i].spawnTime == SpawnTime.LOAD)
                {

                    if (candidates[i].isPosLocked)
                    {
                        // print("qqqqqqqqqqqqq");
                        Instantiate(candidates[i].entity, candidates[i].spawnPoints[0], Quaternion.identity).GetComponent<LivingEntity>().onEntityDie.AddListener(UpdateProcess);
                    }
                }
            }
            
            EnterWay way;
            float detect = 3f;
            if(Mathf.Abs(rb.bounds.min.x-collision.bounds.max.x) < detect) way = EnterWay.LEFT;
            else if(Mathf.Abs(rb.bounds.max.x-collision.bounds.min.x) < detect) way = EnterWay.UP;
            else if(Mathf.Abs(rb.bounds.min.y - collision.bounds.max.y) < detect) way = EnterWay.DOWN;
            else way = EnterWay.DOWN;


            collision.transform.position = startPoint[(int)way];
            print(way);

            for (int i = 0; i < prePlaced.Count; i++)
            {

                if (prePlaced[i] != null)
                {
                    
                    prePlaced[i].SetActive(true);
                    prePlaced[i].GetComponent<LivingEntity>().onEntityDie.AddListener(UpdateProcess);
                }
                    
                // print("---");

            }
            foreach (GameObject door in doors)
            {

                door.SetActive(true);
            }
        }
    }

    public void UpdateProcess()
    {
        
        left--;
        process = (total-left)/total;
        // print("die");
        if(process >= 1)
        {
            
            onFinishedZone();
        }
    }

    public void onFinishedZone()
    {

        isFinished = true;

        foreach (GameObject door in doors)
        {

            door.SetActive(false);
        }
    }

    public void onPlayerDie()
    {
        
        foreach (GameObject door in doors)
        {

            door.SetActive(false);
        }
    }
}

enum EnterWay
{
    UP=0,RIGHT=1,DOWN=2,LEFT=3
}