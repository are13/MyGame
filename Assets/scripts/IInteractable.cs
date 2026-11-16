

using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    
    string getId();
    DialogNode interact(Player player);
}