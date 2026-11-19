


using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    
    public IContainer container;
    public bool isOpenAnyContainer => container == null;

    public void open(IContainer opens)
    {
        
        if(opens == null) return;
        if (container != null)
        {
            close();
        }
        container = opens;
        opens.openContainer();
        GameManager.manager.player.setInteracting(state.CONTAINER);
        GameManager.manager.shouldFrozen(false);


    }

    public void close()
    {
        
        if(container == null) return;
        print("ent");
        container.closeContainer();
        GameManager.manager.shouldResume();
        GameManager.manager.player.setInteracting(state.NONE);
        container = null;
    }

}