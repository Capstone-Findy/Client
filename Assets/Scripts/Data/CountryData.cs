using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CountryData", menuName = "GameData/CountryData")]
public class CountryData : ScriptableObject
{
    public string countryName;
    public Sprite background;
    public List<StageSlot> stagesSlots = new List<StageSlot>(5);
}

[System.Serializable]
public class StageSlot
{
    public StageData stage;
    public Vector2 normlizedPos;
}