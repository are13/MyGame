

using System.Collections.Generic;
using UnityEngine;

public class Prompt : MonoBehaviour
{

    public GameObject optionPrefab;
    // public List<Option> options;
    int selected = 0;


    void Awake()
    {

        // options = new List<Option>();s
    }

    public void execute(Player player)
    {

        if (transform.GetChild(selected) != null)
        {

            // print("clicked");
            transform.GetChild(selected).GetComponent<Option>().onClick(player);
            // print(transform.GetChild(selected).GetComponent<Option>().action);
        }
    }

    public void updateSelected(int dSelected)
    {
        int newSelected = selected + dSelected;
        if (newSelected == selected || newSelected < 0 || newSelected >= transform.childCount) return;
        transform.GetChild(newSelected).GetComponent<Option>().select(true);
        transform.GetChild(selected).GetComponent<Option>().select(false);
        selected = newSelected;
    }

    public void addOptions(List<Option> options)
    {

        foreach (Option option in options)
        {

            option.setId(transform.childCount + 1);
            var temp = Instantiate(option, transform, false);
            temp.action = option.action;
            temp.transform.localScale = Vector3.one;
        }


    }

    public void removeOptions(List<Option> options)
    {
        HashSet<Option> options1 = new HashSet<Option>(options);
        for (int j = transform.childCount - 1; j >= 0; j--)
        {

            Transform target = transform.GetChild(j);
            if (options1.Contains(target.GetComponent<Option>()))
            {

                Destroy(target.gameObject);
            }
        }
    }
    void Start()
    {


    }
}