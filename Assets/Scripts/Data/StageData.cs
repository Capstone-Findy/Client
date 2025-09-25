using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "GameData/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public Sprite originalImage;
    public Sprite wrongImage;
    public Sprite thumbnail;
    public List<Vector2> answerPos;
    public float correctRange = 40f; // 터치 허용 반경
    public int totalAnswerCount;
}