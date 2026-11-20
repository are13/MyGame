using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{

    // public Sprite defaultIcon;
    public Image icon;
    public Image selected;
    public TextMeshProUGUI amount;
    public ItemStorer itemStorer;
    public int idx;
    public Action<int> notifySelect;
    public Action<int> notifyClick;

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
        itemStorer.onContainerChanged.AddListener(updateDisplay);
        this.itemStorer = itemStorer;
        this.idx = idx;
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        // print("entered");
        notifySelect?.Invoke(idx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        notifyClick?.Invoke(idx);
    }
}
