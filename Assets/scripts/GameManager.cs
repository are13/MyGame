using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager manager;

    [Header("Events")]
    public UnityEvent onGameFrozen;
    public UnityEvent onGameResume;

    [Header("Transition")]
    public Image fading;
    public float transitionTime = 0.5f;
    
    [Header("Managers")]
    public Prompt promptManager;
    public DialogManager dialogManager;
    public ContainerManager containerManager;
    public DragManager dragManager;
    public ItemEntityManager itemEntityManager;

    public GameObject frozenUI;

    [Header("Player Instance")]
    public Player player;

    public bool isFrozen;

    void Awake()
    {
        
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(manager != this)
        {
            
            Destroy(gameObject);
        }

        var temp = fading.color;
        temp.a = 0;
        fading.color = temp;
        dialogManager.resetBgs();
    }

    public void shouldFrozen(bool isShowFrozenUI)
    {
        onGameFrozen?.Invoke();
        frozenUI.SetActive(isShowFrozenUI);
        isFrozen = true;
    }

    public void shouldResume()
    {
        onGameResume?.Invoke();
        frozenUI.SetActive(false);
        isFrozen = false;
        print($"is frozen : {isFrozen}");
    }

    public void transformScene(String name)
    {
        
        StartCoroutine(FadeAndLoad(name));
    }

    private IEnumerator FadeAndLoad(String name)
    {
        
        yield return StartCoroutine(Fade(0f,1f));

        AsyncOperation loading = SceneManager.LoadSceneAsync(name);

        while (!loading.isDone)
        {
            
            yield return null;
        }

        yield return null;

        yield return StartCoroutine(Fade(1f,0f));
    }

    private IEnumerator Fade(float from,float to)
    {
        
        float timer = 0f;
        var temp = fading.color;
        while(timer < transitionTime)
        {
            
            timer += Time.unscaledDeltaTime;
            temp.a = Mathf.Lerp(from,to,timer/transitionTime);
            fading.color = temp;
            yield return null;
        }

        temp.a = to;
        fading.color = temp;
    }

    public void spawnItem(LootData loot, Vector2 pos)
    {

        print($"passed {pos}");
        var temp = Instantiate<GameObject>(loot.item.preFab,pos,Quaternion.identity);
        temp.GetComponent<ItemData>().create(new ItemStack(ItemDataInitializer.read[loot.item.id],loot.minAmount));
        itemEntityManager.addNewItemEntity(new ItemEntity(temp,pos)); 
    }

    public void pickItem(GameObject loot)
    {
        
        
    }

    public void startGame()
    {
        
        SceneManager.LoadScene("KosuzuHome");

    }
}
