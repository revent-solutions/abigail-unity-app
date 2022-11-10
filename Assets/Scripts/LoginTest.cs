using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginTest : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text resultText;
    public Button signInButton;
    public Button signUpButton;

    public string text;
    public string email;
    public string password;

    private FirebaseAuth firebaseAuth;

    private void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    private void Update()
    {
        resultText.text = text;
    }

    public void OnClickSignInButton()
    {
        email = emailField.text;
        password = passwordField.text;

        firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                text = "가입 취소";
                return;
            }
            if (task.IsFaulted)
            {
                text = "가입 실패";
                return;
            }

            text = "가입 성공";
        });
    }

    public void OnClickSignUpButton()
    {
        email = emailField.text;
        password = passwordField.text;

        firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                text = "로그인 취소";
                return;
            }
            if (task.IsFaulted)
            {
                text = "로그인 실패";
                return;
            }

            text = "로그인 성공";
        });
    }
}
