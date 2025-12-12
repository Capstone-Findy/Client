using System.Collections.Generic;
using System.Collections;
using Findy.Define;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [Header("GameManager")]
    public TouchManager touchManager;
    public ItemManager itemManager;

    [Header("Game Information")]
    [SerializeField] private Image originalImage; 
    [SerializeField] private Image wrongImage;
    public StageData currentStage;
    public bool isGameOver = false;
    public float actualPlayTime = 0f;
    private string[] countryCodes = { "Korea", "Japan", "China", "USA", "France" };

    [Header("Slider")]
    public Slider timeSlider;
    private float totalTime = 60f;
    public float currentTime;

    [Header("UI Groups")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameVictoryPanel;
    [SerializeField] private GameObject itemUsagePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject nextStageButton;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI answerCountText;
    [SerializeField] private TextMeshProUGUI remainCountText;
    [SerializeField] private TextMeshProUGUI FirstClearTimeText;
    [SerializeField] private TextMeshProUGUI NonFirstClearTimeText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI moveToMainText;
    [SerializeField] private TextMeshProUGUI stageScoreText;
    
    [Header("Item UI Texts")]
    [SerializeField] private TextMeshProUGUI usedHintCountText;
    [SerializeField] private TextMeshProUGUI usedTimeAddCountText;
    [SerializeField] private TextMeshProUGUI usedOverlapCountText;
    [SerializeField] private TextMeshProUGUI usedGambleCountText;
    [SerializeField] private TextMeshProUGUI remainHintCountText;
    [SerializeField] private TextMeshProUGUI remainTimeAddCountText;
    [SerializeField] private TextMeshProUGUI remainOverlapCountText;
    [SerializeField] private TextMeshProUGUI remainGambleCountText;

    [Header("Remain Icons")]
    [SerializeField] private GameObject remainIconPrefab;
    [SerializeField] private Transform iconRow1;
    [SerializeField] private Transform iconRow2;
    private List<GameObject> remainIcons = new List<GameObject>();

    [Header("Custom UI")]
    [SerializeField] private Image victoryAnswerUI;
    [SerializeField] private Image failAnswerUI;

    void Start()
    {
        currentStage = GameManager.instance.selectedStage;
        touchManager.currentStage = GameManager.instance.selectedStage;
    
        if (originalImage != null && wrongImage != null && currentStage != null)
        {
            originalImage.sprite = currentStage.originalImage;
            wrongImage.sprite = currentStage.wrongImage;
        }

        if (victoryAnswerUI != null) victoryAnswerUI.gameObject.SetActive(false);
        if (failAnswerUI != null) failAnswerUI.gameObject.SetActive(false);

        StartCoroutine(InitRemainIconsRoutine());

        currentTime = totalTime;
        timeSlider.maxValue = totalTime;
        timeSlider.value = currentTime;
    }

    void Update()
    {
        ShowLeftItemCount();
        
        if (Input.GetMouseButtonDown(0) && !pausePanel.activeInHierarchy
        && !gameOverPanel.activeInHierarchy && !gameVictoryPanel.activeInHierarchy)
        {
            Vector2 pos = Input.mousePosition;
            touchManager.CheckAnswer(pos);

            if (touchManager.GetFoundAnswerCount() == currentStage.totalAnswerCount)
            {
                GameOver();
            }
        }

        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            actualPlayTime += Time.deltaTime;
            timeSlider.value = currentTime;
            timeLeftText.text = currentTime.ToString("F0");
        }
        else
        {
            timeLeftText.text = "0";
            currentTime = 0;
            GameOver();
        }
    }
    private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        timeSlider.value = 0;
        SoundManager.instance.StopBGM();

        int gameId = currentStage != null ? currentStage.gameId : 0;
        int foundCount = touchManager.GetFoundAnswerCount();
        int totalCount = currentStage != null ? currentStage.totalAnswerCount : 0;
        bool isCustomMode = (gameId == -1);

        ProcessItems(isCustomMode);

        ProcessServerUpload(isCustomMode, gameId, foundCount);

        if (foundCount == totalCount)
        {
            HandleVictory(isCustomMode, gameId);
        }
        else
        {
            HandleDefeat(isCustomMode, foundCount, totalCount);
        }
    }
    private void ProcessItems(bool isCustomMode)
    {
        if (!isCustomMode)
        {
            itemUsagePanel.SetActive(true);
            ShowUsedItem();
            ConsumeUsedItems();
        }
        else
        {
            itemUsagePanel.SetActive(false);
        }
    }

    private void ProcessServerUpload(bool isCustomMode, int gameId, int foundCount)
    {
        if (isCustomMode) return;

        int remainingTime = Mathf.FloorToInt(currentTime);
        var resultData = new GameResultDto
        {
            gameId = gameId,
            remainTime = remainingTime < 0 ? 0 : remainingTime,
            correct = foundCount,
            item1 = itemManager.GetUsedHintCount(),
            item2 = itemManager.GetUsedTimeAddCount(),
            item3 = itemManager.GetUsedOverlapCount(),
            item4 = itemManager.GetUsedOverlapCount()
        };

        DataManager.instance.UploadGameResult(resultData,
            onSuccess: () => Debug.Log("게임 결과 서버 업로드 완료."),
            onError: (code, msg) => Debug.LogError($"게임 결과 업로드 실패: {msg}")
        );
    }

    private void HandleVictory(bool isCustomMode, int gameId)
    {
        gameVictoryPanel.SetActive(true);
        SoundManager.instance.PlaySFX(SoundType.SFX_GameWin);

        if (isCustomMode)
        {
            if (nextStageButton != null) nextStageButton.SetActive(false);
            HandleCustomVictory();
        }
        else
        {
            if (nextStageButton != null) nextStageButton.SetActive(true);
            HandleNormalVictory(gameId);
        }
    }

    private void HandleDefeat(bool isCustomMode, int foundCount, int totalCount)
    {
        gameOverPanel.SetActive(true);
        SoundManager.instance.PlaySFX(SoundType.SFX_GameOver);

        int remainCount = totalCount - foundCount;
        answerCountText.text = $"{foundCount}";
        remainCountText.text = $"{remainCount}";

        SetAnswerImageUI(isCustomMode, failAnswerUI);
    }
    private void HandleCustomVictory()
    {
        SetAnswerImageUI(true, victoryAnswerUI);

        if (moveToMainText != null) moveToMainText.text = "메인으로 이동";
        if (stageScoreText != null) stageScoreText.text = "Custom Stage";

        if (currentStage != null && !string.IsNullOrEmpty(currentStage.customId))
        {
            UpdateCustomBestRecord(currentStage.customId);
        }
        else
        {
            if (FirstClearTimeText != null) FirstClearTimeText.text = $"클리어 기록 : {actualPlayTime:F2}초";
            if (NonFirstClearTimeText != null) NonFirstClearTimeText.text = "갤러리에 저장하면 기록이 남습니다.";
        }
    }

    private void UpdateCustomBestRecord(string customId)
    {
        string key = $"CustomBest_{customId}";
        float bestTime = PlayerPrefs.GetFloat(key, float.MaxValue);

        if (actualPlayTime < bestTime)
        {
            PlayerPrefs.SetFloat(key, actualPlayTime);
            PlayerPrefs.Save();

            if (FirstClearTimeText != null) FirstClearTimeText.text = $"최고 기록 달성! : {actualPlayTime:F2}초";
            if (NonFirstClearTimeText != null)
            {
                if (bestTime == float.MaxValue) NonFirstClearTimeText.text = "";
                else NonFirstClearTimeText.text = $"이전 최고 기록 : {bestTime:F2}초";
            }
        }
        else
        {
            if (FirstClearTimeText != null) FirstClearTimeText.text = $"최고 기록 : {bestTime:F2}초";
            if (NonFirstClearTimeText != null) NonFirstClearTimeText.text = $"이번 시도 기록 : {actualPlayTime:F2}초";
        }
    }
    private void HandleNormalVictory(int gameId)
    {
        SetAnswerImageUI(false, victoryAnswerUI);

        var country = GameManager.instance.selectedCountry;
        if (country == null) return;

        int countryIndex = GameManager.instance.GetCountryIndex(country);
        var stages = country.stagesSlots;
        int currentIndex = stages.FindIndex(slot => slot.stage == currentStage);

        if (countryIndex >= 0 && countryIndex < countryCodes.Length && gameId > 0)
        {
            string countryCode = countryCodes[countryIndex];
            DataManager.instance.GetGameScore(countryCode, gameId,
                onSuccess: (score) => { if (stageScoreText != null) stageScoreText.text = $"획득 점수: {score}점"; },
                onError: (code, msg) => { if (stageScoreText != null) stageScoreText.text = "점수 로드 실패"; }
            );
        }

        DataManager.UnlockNextStage(country.countryName, currentIndex);

        bool isLastStage = (currentIndex == stages.Count - 1);

        if (isLastStage)
        {
            int currentCountryIndex = GameManager.instance.GetCountryIndex(country);
            if (currentCountryIndex != -1) DataManager.UnlockNextCountry(currentCountryIndex);
            
            if (moveToMainText != null) moveToMainText.text = "메인으로 이동";

            if (nextStageButton != null) nextStageButton.SetActive(false);
        }
        else
        {
            if (nextStageButton != null) nextStageButton.SetActive(true);
        }
        UpdateNormalBestRecord(country.countryName, currentIndex);
    }

    private void UpdateNormalBestRecord(string countryName, int stageIndex)
    {
        float? prevClearTime = DataManager.GetLocalBestClearTime(countryName, stageIndex);
        if (prevClearTime.HasValue)
        {
            float bestTime = prevClearTime.Value;
            if (actualPlayTime < bestTime)
            {
                DataManager.SaveLocalBestClearTime(countryName, stageIndex, actualPlayTime);
                if (FirstClearTimeText != null) FirstClearTimeText.text = $"최고 기록 달성! : {actualPlayTime:F2}초";
                if (NonFirstClearTimeText != null) NonFirstClearTimeText.text = $"이전 최고 기록 : {bestTime:F2}초";
            }
            else
            {
                if (FirstClearTimeText != null) FirstClearTimeText.text = $"최고 기록 : {bestTime:F2}초";
                if (NonFirstClearTimeText != null) NonFirstClearTimeText.text = $"이번 시도 기록 : {actualPlayTime:F2}초";
            }
        }
        else
        {
            DataManager.SaveLocalBestClearTime(countryName, stageIndex, actualPlayTime);
            if (FirstClearTimeText != null) FirstClearTimeText.text = $"첫 클리어 기록 : {actualPlayTime:F2}초";
            if (NonFirstClearTimeText != null) NonFirstClearTimeText.text = string.Empty;
        }
    }
    private void SetAnswerImageUI(bool show, Image targetUI)
    {
        if (targetUI == null) return;

        if (show && currentStage != null && currentStage.answerImage != null)
        {
            targetUI.gameObject.SetActive(true);
            targetUI.sprite = currentStage.answerImage;
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator InitRemainIconsRoutine()
    {
        if (currentStage == null || remainIconPrefab == null || iconRow1 == null || iconRow2 == null) yield break;        
        foreach(Transform child in iconRow1) Destroy(child.gameObject);
        foreach(Transform child in iconRow2) Destroy(child.gameObject);

        remainIcons.Clear();
        yield return null; 

        int totalCount = currentStage.totalAnswerCount;
        for(int i = 0; i < totalCount; i++)
        {
            Transform targetParent = (i < 7) ? iconRow1 : iconRow2;
            GameObject icon = Instantiate(remainIconPrefab, targetParent);
            icon.SetActive(true);
            remainIcons.Add(icon);
        }
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(iconRow1 as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(iconRow2 as RectTransform);
    }

    public void DecreaseRemainIcon()
    {
        if(remainIcons.Count > 0)
        {
            int lastIndex = remainIcons.Count - 1;
            GameObject iconToRemove = remainIcons[lastIndex];

            iconToRemove.SetActive(false);
            remainIcons.RemoveAt(lastIndex);
        }
    }
    
    public void LoadNextStage()
    {
        var country = GameManager.instance.selectedCountry;
        var stages = country.stagesSlots;
        int currentIndex = stages.FindIndex(slot => slot.stage == currentStage);

        if (currentIndex >= stages.Count - 1)
        {
            GameManager.instance.PopSceneHistory();
            GameManager.instance.LoadScene("StageSelectScene", false);
        }
        else
        {
            StageData nextStage = stages[currentIndex + 1].stage;
            GameManager.instance.SelectStage(nextStage);
            GameManager.instance.LoadScene(SceneManager.GetActiveScene().name, false);
        }
    }
    
    public void ReturnToMain()
    {
        GameManager.instance.PopSceneHistory();
        if(currentStage != null && currentStage.gameId == -1)
        {
            string targetScene = GameManager.instance.returnSceneName;
            if (string.IsNullOrEmpty(targetScene)) targetScene = "CustomMakeScene2";
            GameManager.instance.LoadScene(targetScene, false);
        }
        else
        {
            GameManager.instance.LoadScene("StageSelectScene", false);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ToggleMasterVolume(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
    }

    private void ShowUsedItem()
    {
        usedHintCountText.text = $"{itemManager.GetUsedHintCount()}";
        usedTimeAddCountText.text = $"{itemManager.GetUsedTimeAddCount()}";
        usedOverlapCountText.text = $"{itemManager.GetUsedOverlapCount()}";
        usedGambleCountText.text = $"{itemManager.GetUsedGambleCount()}";
    }

    private void ShowLeftItemCount()
    {
        remainHintCountText.text = $"{itemManager.hintItemCount}";
        remainTimeAddCountText.text = $"{itemManager.timeAddItemCount}";
        remainOverlapCountText.text = $"{itemManager.overlapItemCount}";
        remainGambleCountText.text = $"{itemManager.gambleItemCount}";
    }

    private void ConsumeUsedItems()
    {
        int usedHint = itemManager.GetUsedHintCount();
        int usedTime = itemManager.GetUsedTimeAddCount();
        int usedOverlap = itemManager.GetUsedOverlapCount();
        int usedGamble = itemManager.GetUsedGambleCount();

        var userData = GameManager.instance.currentUserData;

        if (usedHint > 0) { DataManager.instance.UpdateItem(1, -usedHint); if(userData != null) userData.item1 -= usedHint; }
        if (usedTime > 0) { DataManager.instance.UpdateItem(2, -usedTime); if(userData != null) userData.item2 -= usedTime; }
        if (usedOverlap > 0) { DataManager.instance.UpdateItem(3, -usedOverlap); if(userData != null) userData.item3 -= usedOverlap; }
        if (usedGamble > 0) { DataManager.instance.UpdateItem(4, -usedGamble); if(userData != null) userData.item4 -= usedGamble; }
    }
}