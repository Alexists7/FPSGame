using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")] 
public class GunInfo : ItemInfo
{
    public float damage;
    public int ammoCount;
    public int totalCount;
    public int magCapacity;
    public int totalCapacity;

    void OnDisable()
    {
        ammoCount = magCapacity;
        totalCount = totalCapacity;
    }
}
