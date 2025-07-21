using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [Header("GameManager")]
    public TouchManager touchManager;

    [Header("Game Information")]
    public StageData currentStage;
    public bool isGameOver = false;

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
    private TextMeshProUGUI answerCountText;
    [SerializeField]
    private TextMeshProUGUI FirstClearTimeText;
    [SerializeField]
    private TextMeshProUGUI NonFirstClearTimeText;

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
            timeSlider.value = currentTime;
        }
        else
        {
            currentTime = 0;
            GameOver();
        }
    }

    private void GoToNextStage()
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

            float usedTime = totalTime - currentTime;

            if (!DataManager.HasClearTime())
            {
                // TODO : 로그인 시스템 추가 이후 UploadClearTimeToServer로 함수 변경
                DataManager.SaveClearTime(usedTime);
                FirstClearTimeText.text = $"최초 시도 걸린 시간 : {usedTime:F2}초";
            }
            else
            {
                float firstTime = DataManager.GetClearTime();
                FirstClearTimeText.text = $"최초 시도 걸린 시간 : {firstTime:F2}초";
                NonFirstClearTimeText.text = $"이번 시도 걸린 시간 : {usedTime:F2}초";
            }
        }
        else
        {
            gameOverPanel.SetActive(true);
            answerCountText.text = $"찾은 정답 개수 : {foundCount}개";
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("GameScene");
    }
}
