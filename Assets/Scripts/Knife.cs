using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Knife : Item
{
    public abstract override void Use();
    public AudioConfigScriptableObject audioConfig;
}
