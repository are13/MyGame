using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    public Sprite defaultIcon;
    public Image icon;
    public Image selected;
    public TextMeshProUGUI amount;
    public ItemStorer itemStorer;
    public int idx;

    public void select(bool isSelected)
    {
        
        selected.gameObject.SetActive(isSelected);
    }

    public void bind(ItemStorer itemStorer, int idx)
    {
        if (this.itemStorer != null)
        {
            itemStorer.onContainerChanged.RemoveListener(updateDisplay);
        }
    }

    public void updateDisplay(int idx)
    {

        var temp = itemStorer.getItem(idx);
        if (temp == null)
        {

            icon.gameObject.SetActive(false);
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = temp.item.icon;
        amount.text = (temp.amount>1)?temp.amount+"":"";
    }
}
