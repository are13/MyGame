

using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using UnityEngine;

public class SpawnData
{

    public GameObject entity;
    public int maxAmount;
    public int minAmount;
    public float chance;
    public bool isPosLocked;
    public List<Vector2> spawnPoints;
    public SpawnTime spawnTime;

    public SpawnData(GameObject entity,List<Vector2> spawnPoints, SpawnTime spawnTime = SpawnTime.LOAD, bool isPosLocked = false, int minAmount = 0, int maxAmount = 1, float chance = 1)
    {

        this.entity = entity;
        this.spawnTime = spawnTime;
        this.isPosLocked = isPosLocked;
        this.minAmount = minAmount;
        this.maxAmount = maxAmount;
        this.chance = chance;
        this.spawnPoints = spawnPoints;
    }
}

public enum SpawnTime
{

    LOAD, DURING, AFTER
}