using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TouchManager : MonoBehaviour
{
    public StageData currentStage;
    public RectTransform originalImageArea;
    public RectTransform wrongImageArea;
    private List<Vector2> foundAnswer;
    public TextMeshProUGUI resultText; // 임시 텍스트(추후에 삭제)

    private bool isChecking = false;

    private enum CheckResult { None, Correct, AlreadyFound }

    private void Awake()
    {
        foundAnswer = new List<Vector2>();
    }

    public void CheckAnswer(Vector2 screenPos)
    {
        if (isChecking) return;
        if (!IsPointerInsideRect(originalImageArea, screenPos) && !IsPointerInsideRect(wrongImageArea, screenPos)) return;


        var resultOriginal = CheckInImage(originalImageArea, screenPos);
        var resultWrong = CheckInImage(wrongImageArea, screenPos);

        if (resultOriginal == CheckResult.Correct || resultWrong == CheckResult.Correct)
        {
            // TODO : 텍스트 관련 삭제 후 정답 체크 시 빨간 동그라미 표시(영구 유지)
            resultText.text = "Correct!";
            resultText.gameObject.SetActive(true);
            StartCoroutine("HideResultText");
        }
        else if (resultOriginal == CheckResult.AlreadyFound || resultWrong == CheckResult.AlreadyFound)
        {
            //Debug.Log("이미 찾은 곳입니다.");
        }
        else
        {
            // TODO : 텍스트 관련 삭제 후 틀린 곳 체크 시 남은 시간 차감 및 X 표시(1초 유지)
            isChecking = true;
            resultText.text = "Wrong!";
            resultText.gameObject.SetActive(true);
            StartCoroutine("HideResultText");
            StartCoroutine("CheckingAfterDelay");
        }
    }

    private CheckResult CheckInImage(RectTransform imageRect, Vector2 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, screenPos, null, out localPoint);

        foreach (var answer in currentStage.answerPos)
        {
            if (Vector2.Distance(localPoint, answer) <= currentStage.correctRange)
            {
                if (IsAlreadyFound(answer))
                {
                    return CheckResult.AlreadyFound;
                }
                foundAnswer.Add(answer);
                return CheckResult.Correct;
            }
        }
        return CheckResult.None;
    }

    private bool IsPointerInsideRect(RectTransform rectTransform, Vector2 screenPos)
    {
        if (rectTransform == null) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPos, null);
    }

    private bool IsAlreadyFound(Vector2 answer)
    {
        return foundAnswer.Contains(answer);
    }

    public int GetFoundAnswerCount()
    {
        return foundAnswer.Count;
    }

    private IEnumerator CheckingAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        isChecking = false;
    }
    // TODO : 추후에 삭제
    private IEnumerator HideResultText()
    {
        yield return new WaitForSeconds(1f);
        resultText.gameObject.SetActive(false);
    }
}
