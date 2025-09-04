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

    [Header("Stage Information")]
    [SerializeField]
    private GameObject correctImgPrefab;
    [SerializeField]
    private GameObject wrongImgPrefab;

    [Header("UI")]
    [SerializeField] private RectTransform uiRoot; // Canvas 또는 Canvas 하위의 UI 루트

    private enum CheckResult { None, Correct, AlreadyFound }

    private void Awake()
    {
        foundAnswer = new List<Vector2>();
        if (uiRoot == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) uiRoot = canvas.GetComponent<RectTransform>();
        }
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
            ShowCurStateImage(correctImgPrefab);
        }
        else if (resultWrong == CheckResult.Correct)
        {
            Vector2 answerPos = GetAnswerPosition(screenPos, wrongImageArea);
            itemManager.CreateHintMarker(originalImageArea, answerPos);
            itemManager.CreateHintMarker(wrongImageArea, answerPos);
            ShowCurStateImage(correctImgPrefab);
        }
        else if (resultOriginal == CheckResult.AlreadyFound || resultWrong == CheckResult.AlreadyFound)
        {
            // TODO : UI 추가 -> 화면에 텍스트로 이미 찾은 곳임을 알림
            Debug.Log("이미 찾은곳 입니다.");
        }
        else
        {
            Vector2 localPosOriginal;
            Vector2 localPosWrong;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(originalImageArea, screenPos, null, out localPosOriginal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(wrongImageArea, screenPos, null, out localPosWrong);

            itemManager.CreateWrongMarker(originalImageArea, localPosOriginal);
            itemManager.CreateWrongMarker(wrongImageArea, localPosWrong);

            isChecking = true;
            inGameManager.currentTime -= 3f;
            ShowCurStateImage(wrongImgPrefab);
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
    private void ShowCurStateImage(GameObject imgPrefab)
    {
        GameObject obj = Instantiate(imgPrefab, uiRoot);
        var rt = obj.GetComponent<RectTransform>();
        obj.SetActive(true);
        rt.anchoredPosition = new Vector2(0, 200);
        Destroy(obj, 1f);
    }
}
