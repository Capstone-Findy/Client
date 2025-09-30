using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CountryData", menuName = "GameData/CountryData")]
public class CountryData : ScriptableObject
{
    public string countryName;
    public Sprite background;
    public Vector2 backgroundSize = new Vector2(1920, 1080);
    public List<StageSlot> stagesSlots = new List<StageSlot>(5);
    public List<ArrowSlot> arrowSlots = new List<ArrowSlot>(4);
}

[System.Serializable]
public class StageSlot
{
    public StageData stage;
    public Vector2 StageImagePos;
}

[System.Serializable]
public class ArrowSlot
{
    public Vector2 ArrowImagePos;
    public float rotation;
}