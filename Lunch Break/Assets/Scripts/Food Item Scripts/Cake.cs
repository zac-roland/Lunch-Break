﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : InventoryItemBase
{
    public override string Name
    {
        get
        {
            return "Cake";
        }
    }

    public override void OnUse()
    {
        // TODO: Do something with the object
        base.OnUse();
    }
}