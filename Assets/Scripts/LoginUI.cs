using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idInputfield = null;

    [SerializeField]
    private TMP_InputField passwordInputfield = null;

    [SerializeField]
    private Toggle autoLoginToggle = null;

    [SerializeField]
    private Button loginButton = null;

    [SerializeField]
    private Button signUpButton = null;

    [SerializeField]
    private SignUpUI signUpUI = null;

    [SerializeField]
    private StartUI startUI = null;

    private FirebaseAuth firebaseAuth;
    private bool isLoginButtonClicked = false;

    private void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;

        DataManager.Instance.Load();
        autoLoginToggle.isOn = DataManager.Instance.IsAutoLogin;

        if (DataManager.Instance.IsAutoLogin)
        {
            OnClickLoginButton();
        }
    }

    private async void OnClickLoginButton()
    {
        if (isLoginButtonClicked)
        {
            return;
        }

        isLoginButtonClicked = true;

        var isCompleted = false;

        await firebaseAuth.SignInWithEmailAndPasswordAsync(idInputfield.text, passwordInputfield.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                isLoginButtonClicked = false;
                Debug.Log("Login Canceled");
                return;
            }
            if (task.IsFaulted)
            {
                isLoginButtonClicked = false;
                Debug.Log("Login Fault");
                return;
            }

            DataManager.Instance.IsAutoLogin = autoLoginToggle.isOn;
            DataManager.Instance.Save();

            isCompleted = true;
        });

        if (isCompleted)
        {
            isLoginButtonClicked = false;
            startUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private void OnClickSignUpButton()
    {
        this.gameObject.SetActive(false);
        signUpUI.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        firebaseAuth.SignOut();

        loginButton.onClick.AddListener(OnClickLoginButton);
        signUpButton.onClick.AddListener(OnClickSignUpButton);

        isLoginButtonClicked = false;
    }

    private void OnDisable()
    {
        signUpButton.onClick.RemoveListener(OnClickSignUpButton);
        loginButton.onClick.RemoveListener(OnClickLoginButton);
    }
}
