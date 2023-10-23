using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void Use();
    public abstract void Reload();
    public abstract void OutOfAmmo();

    public AudioConfigScriptableObject audioConfig;
    public GameObject bulletImpactPrefab;
}
