using TMPro;
using UnityEngine;

public class HealthPromot : MonoBehaviour
{

    TextMeshProUGUI text;

    float maxHealth;
    float currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        text = GetComponent<TextMeshProUGUI>();
    }

    public void init(float maxHealth,float currentHealth)
    {
        
        this.maxHealth = maxHealth;

        updateHealth(currentHealth,0);
    }

    public void updateHealth(float newCurrentHealth,float dt)
    {
        
        if(currentHealth == newCurrentHealth) return;

        currentHealth = newCurrentHealth;
        text.text = currentHealth + " / " + maxHealth;
    }
}
