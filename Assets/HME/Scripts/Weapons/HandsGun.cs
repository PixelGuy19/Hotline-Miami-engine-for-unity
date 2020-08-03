using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsGun : GunBase
{
    protected override void Use()
    {
        base.Use();
        Owner.SoundPlayer.PlaySound("Punch");
    }
}