
using System;
using UnityEngine;

public class LootData
{
    public Item item;
    public int maxAmount;
    public int minAmount;
    public float chance;

    public LootData(Item item,float chance,int minAmount=1 , int maxAmount=1)
    {
        
        this.item = item;
        this.chance = chance;
        this.maxAmount = maxAmount;
        this.minAmount = minAmount;
    }

}