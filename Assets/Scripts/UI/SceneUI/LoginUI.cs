using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField LoginEmailInput;
    public InputField LoginPasswordInput;

    [Header("SignUp Panel")]
    public GameObject SignUpPanel;
    public InputField SignUpNameInput;
    public InputField SignUpEmailInput;
    public InputField SignUpPasswordInput;

    public void ShowLoginPanel()
    {
        LoginPanel.SetActive(true);
        SignUpPanel.SetActive(false);    
    }

    public void ShowSignUpPanel()
    {
        SignUpPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        LoginPanel.SetActive(false);
        SignUpPanel.SetActive(false);
    }

    public void OnClickLoginSubmit()
    {
        string email = LoginEmailInput.text;
        string password = LoginPasswordInput.text;
        bool rememberMe = false;

        if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            // TODO : 필수항목을 입력해달라는 팝업띄우기
            return;
        }

        DataManager.instance.Login(email, password, rememberMe,
            onSuccess: () =>
            {
                Debug.Log("로그인 성공!");
                CloseAllPanels();
                SceneManager.LoadScene("MainScene");
            },
            onError : (code, msg) =>
            {
                // TODO : 로그인 실패 메시지 팝업 띄우기
                Debug.LogError($"로그인 실패: {code}, {msg}");
            });
    }

    public void OnClickSignUpSubmit()
    {
        string name = SignUpNameInput.text;
        string email = SignUpEmailInput.text;
        string password = SignUpPasswordInput.text;
        string file = "default_file_data";  // 임시 더미 데이터 할당

        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            // TODO : 필수항목을 입력해달라는 팝업 띄우기
            return;
        }

        DataManager.instance.SignUp(name, file, email, password,
            onSuccess: () =>
            {
                // TODO : 회원가입 성공 팝업 띄우기
                Debug.Log("회원가입 성공!");
                CloseAllPanels();
            },
            onError: (code, msg) =>
            {
                // TODO : 회원가입 실패 메시지 팝업 띄우기
                Debug.LogError($"회원가입 실패: {code}, {msg}");
            });
    }
}
