using EmergencyRoulette;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset (AssetPath = "Resources/ScriptableObjects")]
public class ModuleTestExcel : ScriptableObject
{
    public List<ModuleDataItem> Sheet1;
}
