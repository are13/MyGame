

using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    
    int getId();
    String getDisplayName();
    void interact(Player player);
}