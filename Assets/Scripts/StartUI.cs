using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField]
    private Button loginButton = null;

    [SerializeField]
    private Button logoutButton = null;

    [SerializeField]
    private Button activateSerialCodeButton = null;

    [SerializeField]
    private TMP_Text userDisplayNameText = null;

    [SerializeField]
    private TMP_Text serialCodeText = null;

    [SerializeField]
    private Button startButton = null;

    [SerializeField]
    private LoginUI loginUI = null;

    [SerializeField]
    private SerialCodeUI serialCodeUI = null;

    [SerializeField]
    private SimulateUI simulateUI = null;

    private FirebaseAuth firebaseAuth;

    private void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;

        if (DataManager.Instance.IsAutoLogin == false)
        {
            FirebaseAuth.DefaultInstance.SignOut();
            logoutButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            return;
        }
    }

    private void OnClickLoginButton()
    {
        loginUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnClickLogoutButton()
    {
        loginUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnClickActivateSerialCodeButton()
    {
        serialCodeUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnClickStartButton()
    {
        if (firebaseAuth.CurrentUser == null || serialCodeText.text == "시리얼 코드 없음")
        {
            return;
        }

        simulateUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private async void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        loginButton.onClick.AddListener(OnClickLoginButton);
        logoutButton.onClick.AddListener(OnClickLogoutButton);
        activateSerialCodeButton.onClick.AddListener(OnClickActivateSerialCodeButton);

        serialCodeText.text = "시리얼 코드 없음";
        activateSerialCodeButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);

        userDisplayNameText.text = "유저 정보 없음";

        if (firebaseAuth.CurrentUser != null)
        {
            DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("users").Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            var snapshot = await docRef.GetSnapshotAsync();
            var dict = snapshot.ToDictionary();

            if (dict != null && dict.ContainsKey("serialCode") && string.IsNullOrEmpty(dict["serialCode"].ToString()) == false)
            {
                serialCodeText.text = dict["serialCode"].ToString();
                activateSerialCodeButton.gameObject.SetActive(false);
                startButton.gameObject.SetActive(true);
            }

            logoutButton.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(false);

            userDisplayNameText.text = firebaseAuth.CurrentUser.DisplayName;

            return;
        }

        logoutButton.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        activateSerialCodeButton.onClick.RemoveListener(OnClickActivateSerialCodeButton);
        logoutButton.onClick.RemoveListener(OnClickLogoutButton);
        loginButton.onClick.RemoveListener(OnClickLoginButton);
        startButton.onClick.RemoveListener(OnClickStartButton);
    }
}
