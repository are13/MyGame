


using System.Collections.Generic;
using UnityEngine;

public interface IContainer
{
    
    void openContainer();

    void updateSelected(int dt);

    void updateDisplay(List<ItemStack> items);
}