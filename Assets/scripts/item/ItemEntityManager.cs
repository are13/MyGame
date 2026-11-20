

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEntityManager : MonoBehaviour
{
    
    public List<ItemEntity> itemEntities;

    void Awake()
    {
        
        itemEntities = new List<ItemEntity>();
    }

    public void addNewItemEntity(ItemEntity itemEntity)
    {
        
        print($"player {GameManager.manager.player.transform.position}");
        print($"item {itemEntity.entity.transform.position}");
        itemEntities.Add(itemEntity);
    }

    public void removeItemEntity(GameObject gameObject)
    {
        
        for (int i = itemEntities.Count - 1; i >= 0; i--)
        {
            Destroy(itemEntities[i].entity);
            itemEntities.RemoveAt(i);
        }
    }


}

[Serializable]
public class ItemEntity
{
    
    public GameObject entity;           //ItemData in the object
    public Vector2 position; 

    public ItemEntity(GameObject instance , Vector2 pos)
    {
        entity = instance;
        position = pos;
    }
}