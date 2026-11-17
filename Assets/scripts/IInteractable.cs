

using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    
    string getId();
    void interact(Player player);
}