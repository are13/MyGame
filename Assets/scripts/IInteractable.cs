

using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    
    string getId();
    List<Option> interact(Player player);
}