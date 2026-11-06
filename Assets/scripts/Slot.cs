using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    public Sprite defaultIcon;
    public Image icon;
    public Image selected;
    public TextMeshProUGUI amount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {


    }

    public void select(bool isSelected)
    {
        
        selected.gameObject.SetActive(isSelected);
    }

    public void updateDisplay(ItemStack itemStack)
    {

        if (itemStack == null || itemStack.amount <= 0)
        {

            icon.sprite = defaultIcon;
            return;
        }

        icon.sprite = Resources.Load<Sprite>(itemStack.item.icon);
        amount.text = (itemStack.amount>1)?itemStack.amount+"":"";
    }
}
