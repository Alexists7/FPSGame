using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void Use();
    public virtual void Reload()
    {
        //made virtual so that Item class can have access
    }
    public virtual void OutOfAmmo()
    {
        //made virtual so that Item class can have access
    }

    public AudioConfigScriptableObject audioConfig;
    public GameObject bulletImpactPrefab;
}
