using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HouseData
{
    public bool isDone;
    public Dictionary<string, bool> houseInteractableDataDict = new Dictionary<string, bool>();
}
