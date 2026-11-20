using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DragManager : MonoBehaviour
{

    public Image ghost;
    public TextMeshProUGUI ghostAmount;   // 可选：幽灵数量文字

    DragData dragging;

    public bool isDragging => dragging != null;
    public DragData getDragging => dragging;

    void Awake()
    {
        if (ghost != null) ghost.gameObject.SetActive(false);
        dragging = null;
    }

    public void startDragging(ItemStorer store, int idx, ItemStack itemStack = null)
    {
        print(store.maxCount);
        if (store == null || !store.isVaildSlot(idx)) return;

        if (dragging == null)
        {
            PickUp(store, idx);
            return;
        }

        PlaceInto(store, idx);
    }

    void PickUp(ItemStorer store, int idx)
    {
        var stack = store.getItem(idx);
        if (stack == null || stack.amount <= 0) return;  

        dragging = new DragData(store, idx, stack);

        store.setItem(idx, null);

        ShowGhost();
    }

    void PlaceInto(ItemStorer store, int idx)
    {
        var target = store.getItem(idx);

        // 1) 目标空格：放下手上的整堆
        if (target == null)
        {
            store.setItem(idx, dragging.itemStack);  
            ClearDragging();
            return;
        }

        // 2) 同种物品：合并，装不下的继续拿在手上
        if (target.item.Equals(dragging.itemStack.item))
        {
            int space = target.item.maxStackNum - target.amount;
            int move = Mathf.Min(space, dragging.itemStack.amount);

            target.amount += move;
            dragging.itemStack.amount -= move;

            store.notify(idx);

            if (dragging.itemStack.amount <= 0) ClearDragging();
            else RefreshGhost();
            return;
        }

        ItemStorer homeStore = dragging.homeStore;
        int homeIdx = dragging.homeIdx;

        store.setItem(idx, dragging.itemStack);    
        dragging = new DragData(homeStore, homeIdx, target);

        ShowGhost();
    }

    public void ReturnHeld()
    {
        if (dragging == null) return;

        var store = dragging.homeStore;
        int idx = dragging.homeIdx;

        if (store != null && store.isVaildSlot(idx) && store.getItem(idx) == null)
        {
            store.setItem(idx, dragging.itemStack);
            ClearDragging();
            return;
        }

        if (store != null)
        {
            for (int i = 0; i < store.maxCount; i++)
            {
                if (store.getItem(i) != null) continue;

                store.setItem(i, dragging.itemStack);
                ClearDragging();
                return;
            }
        }

    }

    public void endDragging(ItemStorer source)
    {
        if (dragging == null) return;
        if (source == null || source == dragging.homeStore) ReturnHeld();
    }

    void ShowGhost()
    {
        if (ghost == null) return;

        ghost.gameObject.SetActive(true);
        RefreshGhost();
    }

    void RefreshGhost()
    {
        if (dragging == null || ghost == null) return;

        ghost.sprite = dragging.itemStack.item.icon;

    }

    void ClearDragging()
    {
        dragging = null;

        if (ghost != null) ghost.gameObject.SetActive(false);
    }

    void Update()
    {
        if (dragging == null || ghost == null) return;
        if (Mouse.current == null) return;

        ghost.rectTransform.position = Mouse.current.position.value;
    }

}

public class DragData
{

    public ItemStorer homeStore;
    public int homeIdx;
    public ItemStack itemStack;

    public DragData(ItemStorer store, int idx, ItemStack itemStack)
    {

        homeStore = store;
        homeIdx = idx;
        this.itemStack = itemStack;
    }

    // 兼容旧字段名
    public ItemStorer source => homeStore;
    public int idx => homeIdx;
}
