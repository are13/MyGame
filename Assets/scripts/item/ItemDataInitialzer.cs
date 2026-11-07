

using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataInitializer : MonoBehaviour
{

    public static Dictionary<String, Item> read = new Dictionary<string, Item>();

    void Awake()
    {

        read.Clear();
        itemLoadData[] itemDatas = Resources.LoadAll<itemLoadData>("items");
        print($"{itemDatas.Length} items loaded");

        foreach (itemLoadData data in itemDatas)
        {
            switch (data.id)
            {

                case "iced_frog":
                    {

                        construt(typeof(IcedFrog), data);
                        break;
                    }

                case "speed_scroll":
                    {

                        construt(typeof(ScrollSpeed), data);
                        break;
                    }

                case "level1_key":
                    {

                        construt(typeof(Level1Key), data);
                        break;
                    }
            }


        }
    }
    void construt(Type type, itemLoadData data)
    {

        // print("con");
        read.Add(data.id, (Item)Activator.CreateInstance(type, new System.Object[] { data.id, data.preFab, data.icon, data.displayName, data.description, data.maxStackNum, data.type }));
        // print($"{read[data.id]}");
    }
}