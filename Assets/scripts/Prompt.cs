

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{

    // public GameObject optionPrefab;
    public Option buttonPrefab;
    public Player player;
    // public List<Option> options;
    List<Option> options;
    int selected = 0;


    void Awake()
    {

        // options = new List<Option>();s
    }

    public void execute()
    {

        if (transform.GetChild(selected) != null)
        {

            // print("clicked");
            transform.GetChild(selected).GetComponent<Option>().onClick(player);
            // print(transform.GetChild(selected).GetComponent<Option>().action);
        }
    }

    public void updateNearBy(List<IInteractable> interactables)
    {

        removeAll();

        foreach (IInteractable interactable in interactables)
        {

            var temp = Instantiate<Option>(buttonPrefab, transform);
            temp.init((p) =>
            {

                var res = interactable.interact(player);
                removeAll();
                updateDialogOption(res);
            }, interactable.getId());
            options.Add(temp);
        }
    }

    public void updateSelected(int dSelected)
    {
        int newSelected = selected + dSelected;
        if (newSelected == selected || newSelected < 0 || newSelected >= transform.childCount) return;
        options[selected].select(false);
        options[newSelected].select(true);
        selected = newSelected;
    }

    public void updateDialogOption(List<Option> options)
    {

        foreach (Option option in options)
        {

            var temp = Instantiate(option, transform, false);
            temp.action = option.action;
            temp.transform.localScale = Vector3.one;

            // print("aaaaaaaa");
        }

    }

    public void addOptions(String text, UnityAction action)
    {

        // var temp = Instantiate(buttonPrefab,transform,false);
        // temp.GetComponent<TextMeshProUGUI>().text = text;
        // temp.GetComponent<Button>().onClick.AddListener(action);
    }

    public void removeAll()
    {

        foreach (Option option in options)
        {
            Destroy(option.gameObject);
        }

        options.Clear();
    }
    void Start()
    {


    }
}