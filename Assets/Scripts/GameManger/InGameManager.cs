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
    public StageData currentStage;
    public bool isGameOver = false;
    public float actualPlayTime = 0f;

    [Header("Slider")]
    public Slider timeSlider;
    private float totalTime = 60f;
    public float currentTime;

    [Header("UI")]
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameVictoryPanel;
    [SerializeField]
    private GameObject itemUsagePanel;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private TextMeshProUGUI answerCountText;
    [SerializeField]
    private TextMeshProUGUI FirstClearTimeText;
    [SerializeField]
    private TextMeshProUGUI NonFirstClearTimeText;
    [SerializeField]
    private TextMeshProUGUI usedHintCountText;
    [SerializeField]
    private TextMeshProUGUI usedTimeAddCountText;
    [SerializeField]
    private TextMeshProUGUI usedOverlapCountText;
    [SerializeField]
    private TextMeshProUGUI usedGambleCountText;
    [SerializeField]
    private TextMeshProUGUI timeLeftText;

    void Start()
    {
        currentTime = totalTime;
        timeSlider.maxValue = totalTime;
        timeSlider.value = currentTime;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

    private void LoadNextStage()
    {
        // TODO : 다음 스테이지 이동 구현 및 패널 제외한 인게임은 터치 불가하도록 설정
    }

    private void GameOver()
    {
        isGameOver = true;
        timeSlider.value = 0;
        int foundCount = touchManager.GetFoundAnswerCount();

        if (foundCount == currentStage.totalAnswerCount)
        {
            gameVictoryPanel.SetActive(true);

            if (!DataManager.HasClearTime())
            {
                // TODO : 로그인 시스템 추가 이후 UploadClearTimeToServer로 함수 변경
                DataManager.SaveClearTime(actualPlayTime);
                FirstClearTimeText.text = $"최초 시도 걸린 시간 : {actualPlayTime:F2}초";
            }
            else
            {
                float firstTime = DataManager.GetClearTime();
                FirstClearTimeText.text = $"최초 시도 걸린 시간 : {firstTime:F2}초";
                NonFirstClearTimeText.text = $"이번 시도 걸린 시간 : {actualPlayTime:F2}초";
            }
        }
        else
        {
            gameOverPanel.SetActive(true);
            answerCountText.text = $"찾은 정답 개수 : {foundCount}개";
        }
        itemUsagePanel.SetActive(true);
        ShowUsedItem();

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
        SceneManager.LoadScene("GameScene");
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
}
