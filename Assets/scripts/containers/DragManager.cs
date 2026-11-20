

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{

    public Image ghost;
    DragData dragging;

    void Awake()
    {
        ghost.gameObject.SetActive(false);
        dragging = null;
    }
    public void startDragging(ItemStorer source, int idx, ItemStack itemStack)
    {

        ghost.gameObject.SetActive(true);
        if (dragging != null)
        {
            if (source.getItem(idx) == null)
            {
                source.setItem(idx, itemStack);
                dragging = null;
                ghost.gameObject.SetActive(false);
                return;
            }
            else
            {
                source.setItem(idx, dragging.itemStack);
                dragging.source.setItem(dragging.idx, itemStack);
                ghost.sprite = itemStack.item.icon;
                return;
            }
        }
        else if (itemStack != null)
        {

            dragging = new DragData(source, idx, itemStack);
            dragging.source.setItem(idx, null);
            ghost.sprite = itemStack.item.icon;
        }else ghost.gameObject.SetActive(false);
    }

    public bool isDragging => dragging != null;
    public DragData getDragging => dragging;
    public void endDragging(ItemStorer source)
    {

        if (dragging != null && source == dragging.source)
        {
            dragging.source.setItem(dragging.idx, dragging.itemStack);
            dragging = null;
            ghost.gameObject.SetActive(false);
        }
    }

    void Update()
    {

        if (ghost == null) return;
        ghost.rectTransform.position = Mouse.current.position.value;
    }

    // public void moveGhost(Vector2 vector2)
    // {

    //     if(ghost == null) return;
    //     ghost.rectTransform.position = vector2;
    // }

}

public class DragData
{

    public ItemStorer source;
    public int idx;
    public ItemStack itemStack;

    public DragData(ItemStorer storer, int idx, ItemStack itemStack)
    {

        source = storer;
        this.idx = idx;
        this.itemStack = itemStack;
    }
}