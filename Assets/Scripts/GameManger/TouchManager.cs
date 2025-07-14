using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TouchManager : MonoBehaviour
{
    public StageData currentStage;
    public RectTransform playArea;
    private List<AnswerPair> foundAnswer;
    public TextMeshProUGUI resultText; // 임시 텍스트(추후에 삭제)

    private void Awake()
    {
        foundAnswer = new List<AnswerPair>();   
    }

    public void CheckAnswer(Vector2 screenPos)
    {
        if (CheckInImage(playArea, screenPos, true) || CheckInImage(playArea, screenPos, false))
        {
                // TODO : 텍스트 관련 삭제 후 정답 체크 시 빨간 동그라미 표시(영구 유지)
                resultText.text = "Correct!";
                resultText.gameObject.SetActive(true);
                StartCoroutine("HideResultText");
        }
    }

    private bool CheckInImage(RectTransform imageRect, Vector2 screenPos, bool isOriginal)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, screenPos, null, out localPoint);

        foreach (var answer in currentStage.answerPairs)
        {
            if (IsAlreadyFound(answer)) continue;
            Vector2 target = isOriginal ? answer.originalImagePos : answer.wrongImagePos;

            if (Vector2.Distance(localPoint, target) <= currentStage.correctRange)
            {
                foundAnswer.Add(answer);
                return true;
            }
        }
        return false;
    }

    private bool IsAlreadyFound(AnswerPair answer)
    {
        return foundAnswer.Contains(answer);
    }


    // TODO : 추후에 삭제
    private IEnumerator HideResultText()
    {
        yield return new WaitForSeconds(1f);
        resultText.gameObject.SetActive(false);
    }
}
