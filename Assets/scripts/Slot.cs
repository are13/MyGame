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

    public void bind(ItemStorer storer, int slotIndex)
    {
        if (itemStorer != null)
        {
            itemStorer.onContainerChanged.RemoveListener(updateDisplay);
        }

        itemStorer = storer;
        idx = slotIndex;

        if (itemStorer != null)
        {
            itemStorer.onContainerChanged.AddListener(updateDisplay);
        }

        updateDisplay(idx);
    }

    public void updateDisplay(int changedIdx)
    {
        if (itemStorer == null)
        {
            SetEmpty();
            return;
        }

        if (changedIdx >= 0 && changedIdx != idx) return;

        var temp = itemStorer.getItem(idx);
        if (temp == null || temp.amount <= 0)
        {
            SetEmpty();
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = temp.item.icon;
        amount.text = (temp.amount > 1) ? temp.amount + "" : "";
    }

    void SetEmpty()
    {
        icon.gameObject.SetActive(false);
        amount.text = "";
    }

    void OnDestroy()
    {
        if (itemStorer != null)
        {
            itemStorer.onContainerChanged.RemoveListener(updateDisplay);
        }
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (GameManager.manager != null && GameManager.manager.dragManager != null)
            {
                GameManager.manager.dragManager.ReturnHeld();
            }
            return;
        }

        notifyClick?.Invoke(idx);
    }
}
