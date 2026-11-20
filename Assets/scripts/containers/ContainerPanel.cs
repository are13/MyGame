

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ContainerPanel : MonoBehaviour
{
    
    public int old_selected = 0;
    public GameObject slotPrefab;
    public bool hasInit = false;
    public UnityEvent<int> onSelectionClicked;

    public virtual void init(int size)
    {
        

    }
}