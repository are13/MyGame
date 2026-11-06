using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIController : MonoBehaviour
{

    public Image image;
    public TextMeshProUGUI text;
    public LivingEntity entity;

    float maxHealth;
    float currentHp;

    void Awake()
    {
        // image = transform.Find("healthFill").GetComponent<Image>();
        // text = transform.Find("healthText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        entity.onEntityHealthChanged.AddListener(updateBar);
        entity.onEntityDie.AddListener(removeObj);
    }

    public void init(float maxHealth , float currentHp)
    {
        
        // print(maxHealth);
        this.maxHealth = maxHealth;
        // this.currentHp = currentHp;

        updateBar(currentHp,0);
    }

    // Update is called once per frame
    public void updateBar(float newHp, float delta)
    {


        if (currentHp != newHp)
        {

            currentHp = newHp;
            image.fillAmount = (maxHealth != 0) ? currentHp / maxHealth : 0;
            text.text = currentHp + " / " + maxHealth;
        }
    }

    public void removeObj()
    {
        // print("invoked");
        entity.onEntityHealthChanged.RemoveListener(updateBar);
        entity.onEntityDie.RemoveListener(removeObj);

        Destroy(gameObject);
    }
}
