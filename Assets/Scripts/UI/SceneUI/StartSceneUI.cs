using UnityEngine;

public class StartSceneUI : MonoBehaviour
{
    private bool isFlowStarted = false;

    void Update()
    {
        if(isFlowStarted || DataManager.instance == null) return;

        if(Input.GetMouseButtonDown(0))
        {
            isFlowStarted = true;
            StartLoginFlow();
        }
    }

    void StartLoginFlow()
    {
        if(DataManager.instance.HasRefreshToken())
        {
            TryAutoLogin();
        }
        else
        {
            GameManager.instance.LoadScene("LoginScene");
        }
    }

    private void TryAutoLogin()
    {
       DataManager.instance.RefreshToken(
            onSuccess: () =>
            {
                DataManager.instance.GetUserInfo(
                    onSuccess: (userData) => {
                        GameManager.instance.currentUserData = userData;
                        GameManager.instance.LoadScene("MainScene");
                    },
                    onError: (code, msg) => {
                        Debug.LogError($"자동 로그인 성공 후 유저 정보 로드 실패: {msg}. 로그인 씬으로 이동.");
                        GameManager.instance.LoadScene("LoginScene");
                    }
                );
            },
            onError: (code, msg) =>
            {
                Debug.LogWarning($"자동 로그인 실패 ({msg}). 일반 로그인 화면으로 이동.");
                DataManager.instance.ClearJwt();
                DataManager.instance.ClearRefreshToken();
                GameManager.instance.LoadScene("LoginScene");
            }
        ); 
    }    
}
