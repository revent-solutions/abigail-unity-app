using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameInputField = null;

    [SerializeField]
    private TMP_InputField idInputField = null;

    [SerializeField]
    private TMP_InputField passwordInputField = null;

    [SerializeField]
    private TMP_InputField againPasswordInputField = null;

    [SerializeField]
    private TMP_Text statusText = null;

    [SerializeField]
    private Button signUpButton = null;

    [SerializeField]
    private LoginUI loginUI = null;

    private FirebaseAuth firebaseAuth;
    private string str = string.Empty;
    private bool isSignupButtonClicked = false;

    private void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    private void Update()
    {
        statusText.text = str;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            loginUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private async void OnClickSignUpButton()
    {
        if (isSignupButtonClicked)
        {
            return;
        }

        isSignupButtonClicked = true;

        str = string.Empty;

        if (string.IsNullOrEmpty(nameInputField.text))
        {
            str = "이름을 입력해주세요.";
            isSignupButtonClicked = false;
            return;
        }

        if (string.IsNullOrEmpty(idInputField.text))
        {
            str = "이메일을 입력해주세요.";
            isSignupButtonClicked = false;
            return;
        }

        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            str = "비밀번호를 입력해주세요.";
            isSignupButtonClicked = false;
            return;
        }

        if (againPasswordInputField.text != passwordInputField.text)
        {
            str = "비밀번호가 일치하지 않습니다.";
            isSignupButtonClicked = false;
            return;
        }

        var email = idInputField.text;
        var password = passwordInputField.text;
        var isCompleted = false;

        await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                isSignupButtonClicked = false;
                str = "가입 취소";
                return;
            }
            if (task.IsFaulted)
            {
                isSignupButtonClicked = false;
                str = "가입 실패, 입력 정보를 확인해주세요.";
                return;
            }

            task.Result.UpdateUserProfileAsync(new UserProfile() { DisplayName = nameInputField.text });
            isCompleted = true;
        });


        if (isCompleted)
        {
            isSignupButtonClicked = false;
            loginUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        signUpButton.onClick.AddListener(OnClickSignUpButton);

        str = string.Empty;
        statusText.text = string.Empty;
        nameInputField.text = string.Empty;
        idInputField.text = string.Empty;
        passwordInputField.text = string.Empty;
        againPasswordInputField.text = string.Empty;

        isSignupButtonClicked = false;
    }

    private void OnDisable()
    {
        signUpButton.onClick.RemoveListener(OnClickSignUpButton);
    }
}
