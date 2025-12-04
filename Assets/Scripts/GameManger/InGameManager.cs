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

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameVictoryPanel;
    [SerializeField] private GameObject itemUsagePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI answerCountText;
    [SerializeField] private TextMeshProUGUI remainCountText;
    [SerializeField] private TextMeshProUGUI FirstClearTimeText;
    [SerializeField] private TextMeshProUGUI NonFirstClearTimeText;
    [SerializeField] private TextMeshProUGUI usedHintCountText;
    [SerializeField] private TextMeshProUGUI usedTimeAddCountText;
    [SerializeField] private TextMeshProUGUI usedOverlapCountText;
    [SerializeField] private TextMeshProUGUI usedGambleCountText;
    [SerializeField] private TextMeshProUGUI remainHintCountText;
    [SerializeField] private TextMeshProUGUI remainTimeAddCountText;
    [SerializeField] private TextMeshProUGUI remainOverlapCountText;
    [SerializeField] private TextMeshProUGUI remainGambleCountText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI moveToMainText;
    [SerializeField] private TextMeshProUGUI stageScoreText;

    void Start()
    {
        currentStage = GameManager.instance.selectedStage;
        touchManager.currentStage = GameManager.instance.selectedStage;
    
        if (originalImage != null && wrongImage != null && currentStage != null)
        {
            originalImage.sprite = currentStage.originalImage;
            wrongImage.sprite = currentStage.wrongImage;
        }

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
            ///// 추후 제거 (위치 탐색 코드)/////
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                originalImage.rectTransform, 
                pos,                 
                null,                     
                out localPoint             
            );

            Debug.Log($"Clicked Local Coords: new Vector2({localPoint.x}f, {localPoint.y}f)");
            /////-----/////
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
        isGameOver = true;
        timeSlider.value = 0;
        SoundManager.instance.StopBGM();

        itemUsagePanel.SetActive(true);
        ShowUsedItem();

        int gameId = currentStage != null ? currentStage.gameId : 0;
        int foundCount = touchManager.GetFoundAnswerCount();
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
            onSuccess: () =>
            {
                Debug.Log("게임 결과 서버 업로드 완료.");
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"게임 결과 업로드 실패: {msg}");
            }
        );        

        if (foundCount == currentStage.totalAnswerCount)
        {
            gameVictoryPanel.SetActive(true);
            SoundManager.instance.PlaySFX(SoundType.SFX_GameWin);

            var country = GameManager.instance.selectedCountry;
            int countryIndex = GameManager.instance.GetCountryIndex(country);
            var stages = country.stagesSlots;
            int currentIndex = stages.FindIndex(slot => slot.stage == currentStage);
            Debug.Log($"[Debug] CountryIndex: {countryIndex}, GameID: {gameId}"); // 1. 값 확인
            if(countryIndex >= 0 && countryIndex < countryCodes.Length)
            {
                string countryCode = countryCodes[countryIndex];

                if(gameId > 0)
                {
                    Debug.Log("[Debug] 점수 요청 시작"); // 2. 요청 진입 확인
                    DataManager.instance.GetGameScore(countryCode, gameId,
                        onSuccess: (score) =>
                        {
                            Debug.Log($"[Debug] 점수 로드 성공: {score}"); // 3. 성공 콜백 확인
                            if (stageScoreText != null)
                                stageScoreText.text = $"획득 점수: {score}점";
                        },
                        onError: (code, msg) =>
                        {
                            Debug.LogError($"[Debug] 점수 로드 실패: {msg}"); // 5. 실패 콜백 확인
                            if (stageScoreText != null)
                                stageScoreText.text = "점수 로드 실패";
                        }
                    );
                }
                else
                {
                    Debug.LogWarning("[Debug] countryIndex가 범위를 벗어남");
                }
            }

            if (country != null)
            {
                DataManager.UnlockNextStage(country.countryName, currentIndex);
            }

            bool isLastStageOfCountry = (currentIndex == stages.Count - 1);

            if(isLastStageOfCountry)
            {
                int currentCountryIndex = GameManager.instance.GetCountryIndex(country);
                if(currentCountryIndex != - 1)
                {
                    DataManager.UnlockNextCountry(currentCountryIndex);
                }
            }
            
            if (currentIndex >= stages.Count - 1) moveToMainText.text = "메인으로 이동";

            if(country != null)
            {
                float? prevClearTime = DataManager.GetLocalBestClearTime(country.countryName, currentIndex);
                
                if(prevClearTime.HasValue)
                {
                    float bestTime = prevClearTime.Value;
                    if(actualPlayTime < bestTime)
                    {
                        DataManager.SaveLocalBestClearTime(country.countryName, currentIndex, actualPlayTime);

                        if (FirstClearTimeText != null)
                            FirstClearTimeText.text = $"최고 기록 달성! : {actualPlayTime:F2}초";
                        if (NonFirstClearTimeText != null)
                            NonFirstClearTimeText.text = $"이전 최고 기록 : {bestTime:F2}초";
                    }
                    else
                    {
                        if (FirstClearTimeText != null)
                            FirstClearTimeText.text = $"최고 기록 : {bestTime:F2}초";
                        if (NonFirstClearTimeText != null)
                            NonFirstClearTimeText.text = $"이번 시도 기록 : {actualPlayTime:F2}초";
                    }
                }
                else
                {
                    DataManager.SaveLocalBestClearTime(country.countryName, currentIndex, actualPlayTime);

                    if (FirstClearTimeText != null)
                        FirstClearTimeText.text = $"첫 클리어 기록 : {actualPlayTime:F2}초";
                    if (NonFirstClearTimeText != null)
                        NonFirstClearTimeText.text = string.Empty;
                }
            }
        }
        else
        {
            int remainCountAnswer = currentStage.totalAnswerCount - foundCount;
            gameOverPanel.SetActive(true);
            SoundManager.instance.PlaySFX(SoundType.SFX_GameOver);

            answerCountText.text = $"찾은 정답 개수 : {foundCount}개";
            remainCountText.text = $"남은 정답 개수 : {remainCountAnswer}개";
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
        GameManager.instance.LoadScene("StageSelectScene", false);
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
        int hintUsed = itemManager.GetUsedHintCount();
        int timeAddUsed = itemManager.GetUsedTimeAddCount();
        int overlapUsed = itemManager.GetUsedOverlapCount();
        int gambleUsed = itemManager.GetUsedGambleCount();

        usedHintCountText.text = $"{hintUsed}";
        usedTimeAddCountText.text = $"{timeAddUsed}";
        usedOverlapCountText.text = $"{overlapUsed}";
        usedGambleCountText.text = $"{gambleUsed}";
    }

    private void ShowLeftItemCount()
    {
        int leftHintCount = itemManager.hintItemCount;
        int leftTimeAddCount = itemManager.timeAddItemCount;
        int leftOverlapCount = itemManager.overlapItemCount;
        int leftGambleCount = itemManager.gambleItemCount;

        remainHintCountText.text = $"{leftHintCount}";
        remainTimeAddCountText.text = $"{leftTimeAddCount}";
        remainOverlapCountText.text = $"{leftOverlapCount}";
        remainGambleCountText.text = $"{leftGambleCount}";
    }
}

