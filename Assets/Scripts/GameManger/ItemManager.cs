using System.Collections;
using UnityEngine.UI;
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
    private GameObject wrongMarkerPrefab;
    [SerializeField]
    private RectTransform originalImageArea;
    [SerializeField]
    private RectTransform wrongImageArea;
    [SerializeField]
    private Image overlayBackground;
    [SerializeField]
    private GameObject coinAnimPrefab;
    [SerializeField]
    private Transform uiCanvas;
    [SerializeField]
    private Sprite coinFrontSprite;
    [SerializeField]
    private Sprite coinBackSprite;
    [SerializeField]
    private float coinSpinDuration = 0.8f;
    [SerializeField]
    private float coinResultDuration = 0.5f;
    [SerializeField]
    private Sprite frontSentense;
    [SerializeField]
    private Sprite backSentense;

    [Header("Control")]
    private bool isUsed = false;

    [Header("Data")]
    // TODO : 사용자마다 다른 아이템 개수 및 서버에 존재하는 아이템 개수를 갖고와 초기화
    public int hintItemCount = 5;
    public int timeAddItemCount = 5;
    public int overlapItemCount = 5;
    public int gambleItemCount = 5;


    /*
    Main Item Function
    */
    public void FindOne()
    {
        if (hintItemCount <= 0)
        {
            Debug.Log("보유한 아이템이 없습니다.");
            return;
        }
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
        hintItemCount--;
    }

    public void AddTimeItem()
    {
        if (timeAddItemCount <= 0)
        {
            Debug.Log("보유한 아이템이 없습니다.");
            return;
        }
        if (inGameManager.currentTime < maxTime)
        {
            inGameManager.currentTime += timeBonus;

            if (inGameManager.currentTime > maxTime)
            {
                inGameManager.currentTime = maxTime;
            }
            inGameManager.timeSlider.value = inGameManager.currentTime;
        }
        timeAddItemCount--;
    }

    public void ScreenOverlap()
    {
        if (overlapItemCount <= 0)
        {
            Debug.Log("보유한 아이템이 없습니다.");
            return;
        }
        if (!isUsed)
        {
            StartCoroutine("OverlapCoroutine");
        }
    }

    private IEnumerator OverlapCoroutine()
    {

        isUsed = true;
        overlapItemCount--;

        Vector2 originalPos1 = originalImageArea.anchoredPosition;
        Vector2 originalPos2 = wrongImageArea.anchoredPosition;
        Vector2 center = new Vector2(0, 200);

        Image originalImg = originalImageArea.GetComponent<Image>();
        Image wrongImg = wrongImageArea.GetComponent<Image>();

        float duration = 0.8f; // 이미지 이동속도
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            originalImageArea.anchoredPosition = Vector2.Lerp(originalPos1, center, t);
            wrongImageArea.anchoredPosition = Vector2.Lerp(originalPos2, center, t);

            Color origColor = originalImg.color;
            Color wrongColor = wrongImg.color;
            origColor.a = Mathf.Lerp(1f, 0.5f, t);
            wrongColor.a = Mathf.Lerp(1f, 0.5f, t);

            originalImg.color = origColor;
            wrongImg.color = wrongColor;

            yield return null;

        }

        overlayBackground.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        overlayBackground.gameObject.SetActive(false);

        originalImageArea.anchoredPosition = originalPos1;
        wrongImageArea.anchoredPosition = originalPos2;

        Color resetOrigColor = originalImg.color;
        Color resetWrongColor = wrongImg.color;

        resetOrigColor.a = 1f;
        resetWrongColor.a = 1f;

        originalImg.color = resetOrigColor;
        wrongImg.color = resetWrongColor;
        isUsed = false;
    }

    public void Gamble()
    {
        if (gambleItemCount <= 0)
        {
            Debug.Log("보유한 아이템이 없습니다.");
            return;
        }
        float chance = Random.value;

        gambleItemCount--;
        bool showBack = chance >= 0.5f;
        ShowGambleAnimation(showBack);
        return;
    }


    /*
    Utility
    */
    public void CreateHintMarker(RectTransform imageArea, Vector2 localPos)
    {
        GameObject marker = Instantiate(hintMarkerPrefab, imageArea);
        marker.transform.localScale = Vector3.one;
        marker.GetComponent<RectTransform>().anchoredPosition = localPos;
        Destroy(marker, 2f);
    }

    public void CreateWrongMarker(RectTransform imageArea, Vector2 localPos)
    {
        GameObject marker = Instantiate(wrongMarkerPrefab, imageArea);
        marker.transform.localScale = Vector3.one;
        marker.GetComponent<RectTransform>().anchoredPosition = localPos;
        Destroy(marker, 1f);
    }

    private void ShowGambleAnimation(bool showBack)
    {
        StartCoroutine(ShowGambleAnimationRoutine(showBack));
    }

    private IEnumerator ShowGambleAnimationRoutine(bool showBack)
    {
        GameObject coinFx = Instantiate(coinAnimPrefab, uiCanvas);
        var rect = coinFx.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(0, 200);
        }

        var img = coinFx.GetComponent<Image>();
        var anim = coinFx.GetComponent<Animator>();

        if (anim != null)
        {
            anim.Play("CoinToss", 0, 0f);
        }
        yield return new WaitForSeconds(coinSpinDuration);

        if (!showBack)
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
        if (anim != null)
        {
            anim.enabled = false;
        }
        if (img != null)
        {
            img.sprite = showBack ? coinBackSprite : coinFrontSprite;
        }

        GameObject labelGO = new GameObject("CoinResultLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        var labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.SetParent(coinFx.transform, false);
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot     = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = new Vector2(0f, 250f); 

        var labelImage = labelGO.GetComponent<Image>();
        labelImage.raycastTarget = false;
        labelImage.preserveAspect = true;
        labelImage.sprite = showBack ? backSentense : frontSentense; 
        labelRect.sizeDelta = new Vector2(500, 500);
        yield return new WaitForSeconds(coinResultDuration);
        Destroy(coinFx);
    }

    /*
    Getter
    */
    public int GetUsedHintCount() => 5 - hintItemCount;
    public int GetUsedTimeAddCount() => 5 - timeAddItemCount;
    public int GetUsedOverlapCount() => 5 - overlapItemCount;
    public int GetUsedGambleCount() => 5 - gambleItemCount;

}
