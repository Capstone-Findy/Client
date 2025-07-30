using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TouchManager : MonoBehaviour
{
    [Header("GameManager")]

    [SerializeField]
    private InGameManager inGameManager;
    [SerializeField]
    private ItemManager itemManager;

    [Header("Stage Information")]
    public StageData currentStage;
    public RectTransform originalImageArea;
    public RectTransform wrongImageArea;
    private List<Vector2> foundAnswer;
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

        if (resultOriginal == CheckResult.Correct)
        {
            Vector2 answerPos = GetAnswerPosition(screenPos, originalImageArea);
            itemManager.CreateHintMarker(originalImageArea, answerPos);
            itemManager.CreateHintMarker(wrongImageArea, answerPos);
        }
        else if (resultWrong == CheckResult.Correct)
        {
            Vector2 answerPos = GetAnswerPosition(screenPos, wrongImageArea);
            itemManager.CreateHintMarker(originalImageArea, answerPos);
            itemManager.CreateHintMarker(wrongImageArea, answerPos);
        }
        else if (resultOriginal == CheckResult.AlreadyFound || resultWrong == CheckResult.AlreadyFound)
        {
            // TODO : UI 추가 -> 화면에 텍스트로 이미 찾은 곳임을 알림
        }
        else
        {
            // TODO : 틀린 곳 체크 시 X 표시(1초 유지)
            isChecking = true;
            inGameManager.currentTime -= 3f;
            Debug.Log("Wrong!!!");
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

    public bool IsAlreadyFound(Vector2 answer)
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

    public void ForceAddFoundAnswer(Vector2 answer)
    {
        if (!foundAnswer.Contains(answer))
        {
            foundAnswer.Add(answer);
        }
    }

    private Vector2 GetAnswerPosition(Vector2 screenPos, RectTransform imageRect)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, screenPos, null, out localPoint);

        foreach (var answer in currentStage.answerPos)
        {
            if (Vector2.Distance(localPoint, answer) <= currentStage.correctRange)
            {
                return answer;
            }
        }

        return Vector2.zero;
    }
}
