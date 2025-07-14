using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnswerPair
{
    public Vector2 originalImagePos;
    public Vector2 wrongImagePos;
}

[CreateAssetMenu(fileName = "StageData", menuName = "GameData/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public List<AnswerPair> answerPairs;
    public float correctRange = 40f; // 터치 허용 반경
}