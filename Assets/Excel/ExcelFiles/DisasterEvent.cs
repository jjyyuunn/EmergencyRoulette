using System;
using System.Collections;
using System.Collections.Generic;
using EmergencyRoulette;
using UnityEngine;

[ExcelAsset(AssetPath = "Resources/ScriptableObjects")]
public class DisasterEvent : ScriptableObject
{
	public List<DisasterEventItem> Sheet1;
}
