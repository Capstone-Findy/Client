using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("GameManager")]
    [SerializeField]
    private InGameManager inGameManager;
    [SerializeField]
    private TouchManager touchManager;
    public StageData currentStage;

    [Header("TimeInfo")]
    private const float maxTime = 60f;
    private const float timeBonus = 5f;
    private const float timeMinus = 3f;

    [Header("UI")]
    [SerializeField]
    private GameObject hintMarkerPrefab;
    [SerializeField]
    private RectTransform originalImageArea;
    [SerializeField]
    private RectTransform wrongImageArea;


    /*
    Main Item Function
    */
    public void FindOne()
    {
        int remainingAnswer = currentStage.totalAnswerCount - touchManager.GetFoundAnswerCount();
        if (remainingAnswer <= 1)
        {
            Debug.Log("힌트 아이템을 사용할 수 없습니다.");
            return;
        }
        foreach (var answer in currentStage.answerPos)
            {
                if (!touchManager.IsAlreadyFound(answer))
                {
                    CreateHintMarker(originalImageArea, answer);
                    CreateHintMarker(wrongImageArea, answer);
                    touchManager.ForceAddFoundAnswer(answer);
                    break;
                }
            }
    }

    public void AddTimeItem()
    {
        if (inGameManager.currentTime < maxTime)
        {
            inGameManager.currentTime += timeBonus;

            if (inGameManager.currentTime > maxTime)
            {
                inGameManager.currentTime = maxTime;
            }
            inGameManager.timeSlider.value = inGameManager.currentTime;
        }
    }

    public void Gamble()
    {
        float chance = Random.value;

        if (chance < 0.5f)
        {
            if (inGameManager.currentTime < maxTime)
            {
                inGameManager.currentTime += timeBonus;
                if (inGameManager.currentTime > maxTime)
                {
                    inGameManager.currentTime = maxTime;
                }
            }
        }
        else
        {
            inGameManager.currentTime -= timeMinus;
            if (inGameManager.currentTime < 0f)
            {
                inGameManager.currentTime = 0f;
            }
        }
    }


    /*
    Function Helper
    */
    private void CreateHintMarker(RectTransform imageArea, Vector2 localPos)
    {
        GameObject marker = Instantiate(hintMarkerPrefab, imageArea);
        marker.transform.localScale = Vector3.one;
        marker.GetComponent<RectTransform>().anchoredPosition = localPos;
        Destroy(marker, 2f);
    }
}
