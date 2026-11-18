


using System.Collections.Generic;
using UnityEngine;

public interface IContainer
{
    
    void openContainer();

    void closeContainer();

    void updateSelected(int dt);

    void updateDisplay(List<ItemStack> items);
}