using System;
using System.Collections;
using System.Collections.Generic;
using EmergencyRoulette;
using UnityEngine;

[ExcelAsset (AssetPath = "Resources/ScriptableObjects")]
public class ExcelTest : ScriptableObject
{
	public List<TestItem> Sheet1;
}
