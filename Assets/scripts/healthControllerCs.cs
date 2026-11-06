using System;
using System.Collections.Generic;
using UnityEngine;

public class healthControllerCs : MonoBehaviour
{

    public Transform healthController;
    public GameObject heartFab;
    public int MAX_HEARTS = 7;
    int currentHealth;

    List<GameObject> hearts = new List<GameObject>();

    void Start()
    {

        for (int i = 0; i < MAX_HEARTS; i++)
        {
            hearts.Add(Instantiate(heartFab,healthController));
        }
        
        currentHealth = MAX_HEARTS;
        changeHealth(currentHealth);
    }

    void updateMaxHealth(int maxHealth)
    {
        
        if(maxHealth == MAX_HEARTS) return;

        if(maxHealth < MAX_HEARTS)
        {
            
            MAX_HEARTS = maxHealth;
            changeHealth((currentHealth < MAX_HEARTS)?currentHealth:MAX_HEARTS);
            return;
        }

        int d = maxHealth - MAX_HEARTS;
        for(int i = 0 ; i < d ; i++)
        {
            
            hearts.Add(Instantiate(heartFab,healthController));
        }
    }


    public void changeHealth(float new_health)
    {

        print("-------------");   
        int temp = Mathf.CeilToInt(new_health);
        print(temp);
        currentHealth = (temp <= MAX_HEARTS) ? temp : MAX_HEARTS;
        for (int i = 0; i < hearts.Count; i++)
        {
            
            if (i < currentHealth)
            {
                hearts[i].SetActive(true);
            }else hearts[i].SetActive(false);
        }
    }
}
