// ***********added**************
// 选项列表 UI。统一管理"附近 NPC 列表"与"对话选项列表"，并向 DialogController
// 暴露 addOption / resetSelection / beginDialog / endDialog。
// 选项行直接克隆 Inspector 上指定的 optionPrefab，不再做模板解析与运行时兜底。
// ***********added**************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Prompt : MonoBehaviour
{

    public Option optionPrefab;
    public Player player;
    // public List<Option> options;
    List<Option> options = new List<Option>();
    int selected = 0;

    // ***********added**************
    readonly List<IInteractable> nearby = new List<IInteractable>();
    bool isInDialog;

    public int nearbyCount
    {
        get { return nearby.Count; }
    }
    // ***********added**************

    void Awake()
    {

        // options = new List<Option>();s
        // **************changed*****************
        options = new List<Option>();
        if (player == null) player = GetComponentInParent<Player>();
    }

    public void execute()
    {
        // **************changed*****************
        if (options == null || options.Count == 0) return;
        if (selected < 0 || selected >= options.Count) selected = 0;

        // **************changed*****************
        Player who = player != null ? player : UnityEngine.Object.FindFirstObjectByType<Player>();
        options[selected].onClick(who);
    }

    public void updateNearBy(List<IInteractable> interactables)
    {

        removeAll();
        nearby.Clear();
        if (interactables != null)
        {
            foreach (var interactable in interactables)
            {
                if (!nearby.Contains(interactable))
                {
                    nearby.Add(interactable);
                }
            }
        }

        if (isInDialog) return;

        foreach (IInteractable target in nearby)
        {
            // IInteractable interactable = target;
            addOption(labelOf(target), (p) => interactWith(target));
        }
        resetSelection();
    }

    void interactWith(IInteractable interactable)
    {
        if (DialogController.isShowing) return;

        removeAll();

        var start = interactable.interact(player);
        if (start != null) updateDialog(start);
    }

    void updateDialog(DialogNode node)
    {
        

    }

    string labelOf(IInteractable interactable)
    {
        string label = "";
        if (interactable == null)
        {
            return "Talk";
        }
        else
        {
            
            label = interactable.getId();
        }
        if (string.IsNullOrEmpty(label)) label = interactable.GetType().Name;
        // if (string.IsNullOrEmpty(label) && interactable is Component component) label = component.name;
        // if (string.IsNullOrEmpty(label)) label = "Talk";
        return label;
    }

    public Option addOption(string text, Action<Player> action)
    {
        if (options == null) options = new List<Option>();

        if (optionPrefab == null)
        {
            Debug.LogError("[Prompt] optionPrefab 未赋值：请在 Inspector 的 Prompt 组件上指定选项 prefab。");
            return null;
        }

        Option temp = Instantiate(optionPrefab, transform, false);
        temp.gameObject.SetActive(true);
        temp.init(action, text);
        options.Add(temp);
        return temp;
    }

    public void resetSelection()
    {
        if (options == null || options.Count == 0) return;
        selected = 0;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] != null) options[i].select(i == selected);
        }
    }

    public void beginDialog()
    {
        isInDialog = true;
        removeAll();
    }

    public void endDialog()
    {
        isInDialog = false;
        removeAll();
        updateNearBy(new List<IInteractable>(nearby));
    }
    // ***********added**************

    public void updateSelected(int dSelected)
    {
        // **************changed*****************
        if (options == null || options.Count == 0) return;
        int newSelected = Mathf.Clamp(selected + dSelected, 0, options.Count - 1);
        if (newSelected == selected) return;
        if (options[selected] != null) options[selected].select(false);
        if (options[newSelected] != null) options[newSelected].select(true);
        selected = newSelected;
    }

    public void updateDialogOption(List<Option> dialogOptions)
    {

        // **************changed*****************
        if (dialogOptions == null) return;
        if (options == null) options = new List<Option>();

        foreach (Option option in dialogOptions)
        {
            if (option == null) continue;

            Option temp = Instantiate(option, transform, false);
            temp.transform.localScale = Vector3.one;
            temp.gameObject.SetActive(true);
            temp.action = option.action;
            options.Add(temp);

            // print("aaaaaaaa");
        }
        resetSelection();
    }

    public void addOptions(String text, UnityAction action)
    {

        // var temp = Instantiate(buttonPrefab,transform,false);
        // temp.GetComponent<TextMeshProUGUI>().text = text;
        // temp.GetComponent<Button>().onClick.AddListener(action);
    }

    public void removeAll()
    {

        if (options == null) return;
        print($"{options.Count} options to remove");
        foreach (Option option in options)
        {
            print("invoked");
            if (option == null) continue;
            // option.transform.SetParent(null, false);
            print($"{nearbyCount} npcs nearby.");
            option.transform.SetParent(null);
            Destroy(option.gameObject);
        }

        options.Clear();
        selected = 0;
    }
    void Start()
    {


    }
}
