
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootData
{
    public itemLoadData item;
    public int maxAmount = 1;
    public int minAmount = 1;
    [Range(0,1)]
    public float chance = 1;

    // public LootData(Item item,float chance,int minAmount=1 , int maxAmount=1)
    // {
        
    //     this.item = item;
    //     this.chance = chance;
    //     this.maxAmount = maxAmount;
    //     this.minAmount = minAmount;
    // }

}

[CreateAssetMenu(fileName = "NewLootTable",menuName = "Game/LootTable")]
public class LootTable : ScriptableObject
{
    
    public List<LootData> loots = new List<LootData>();
}