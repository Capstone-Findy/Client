using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    private EncodeImageHelper imageEncoder;
    [Header("Button")]
    public Button SignUpButton;
    public Button LoginButton;

    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField LoginEmailInput;
    public InputField LoginPasswordInput;
    public Toggle RememberMeToggle;

    [Header("SignUp Panel")]
    public GameObject SignUpPanel;
    public InputField SignUpNameInput;
    public InputField SignUpEmailInput;
    public InputField SignUpPasswordInput;
    public Image ProfileImage;
    private Texture2D selectedProfileTexture;
    public GameObject SuccessPanel;
    public GameObject SuccessSignUpPanel;

    void Start()
    {
        if (imageEncoder == null)
        {
            imageEncoder = FindObjectOfType<EncodeImageHelper>();
        }
        if (SignUpButton != null)
                SignUpButton.onClick.AddListener(ShowSignUpPanel);
            
        if (LoginButton != null)
            LoginButton.onClick.AddListener(ShowLoginPanel);
    }

    public void ShowLoginPanel()
    {
        LoginPanel.SetActive(true);
        SignUpPanel.SetActive(false);    

        if(selectedProfileTexture != null) Destroy(selectedProfileTexture);
        selectedProfileTexture = null;
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
    public void CloseSuccessPanel()
    {
        SuccessPanel.SetActive(false);
    }

    public void OnClickLoginSubmit()
    {
        string email = LoginEmailInput.text;
        string password = LoginPasswordInput.text;
        bool rememberMe = (RememberMeToggle != null) ? RememberMeToggle.isOn : false;

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
        string file = "Dummy_Data";
        
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            // TODO : 필수항목을 입력해달라는 팝업 띄우기
            return;
        }

        // string base64File = "";
        // if(selectedProfileTexture != null && imageEncoder != null)
        // {
        //     base64File = imageEncoder.EncodeTextureToBase64(selectedProfileTexture);
        // }

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

    public void OnClickSendValidationEmail()
    {
        string email = SignUpEmailInput.text;
        DataManager.instance.SendValidationEmail(email,
            onSuccess: () =>
            {
                SuccessPanel.SetActive(true);
                Debug.Log("인증 메일 발송 성공! 메일함을 확인해주세요.");
            },
            onError: (code, msg) =>
            {
                Debug.LogError($"인증 메일 발송 실패: {msg}");
            });
    }

    public void OnClickSelectProfileImage()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if(path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 256, false, true);
                if(texture != null)
                {
                    if(selectedProfileTexture != null) Destroy(selectedProfileTexture);
                    selectedProfileTexture = texture;
                    Sprite newSprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    ProfileImage.sprite = newSprite;
                    ProfileImage.preserveAspect = true;
                }
            }
        }, "프로필 사진을 선택하세요.", "image/*");
    }

    private void OnDestroy()
    {
        if(selectedProfileTexture != null)
        {
            Destroy(selectedProfileTexture);
        }
    }
}
