using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SerialCodeUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField serialCodeInputField = null;

    [SerializeField]
    private TMP_Text statusText = null;

    [SerializeField]
    private Button activateButton = null;

    [SerializeField]
    private StartUI startUI = null;

    private List<string> serialCodes = new List<string>();

    private FirebaseFirestore db;
    private bool isButtonClicked = false;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    private void LoadDatabase()
    {
        CollectionReference serialCodesRef = db.Collection("meta");
        serialCodesRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                
                var arr = ((IEnumerable)documentDictionary["serial-codes"]).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

                serialCodes.AddRange(arr);
            }

            Debug.Log("Read all data from the users collection.");
            
        });
    }

    private void OnClickActivateButton()
    {
        LoadDatabase();

        if (isButtonClicked)
        {
            return;
        }

        isButtonClicked = true;

        if (string.IsNullOrEmpty(serialCodeInputField.text))
        {
            isButtonClicked = false;
            statusText.text = "시리얼 코드를 입력해주세요.";
            return;
        }

        if (serialCodes.Contains(serialCodeInputField.text) == false)
        {
            isButtonClicked = false;
            statusText.text = "시리얼 코드가 유효하지 않습니다.";
            return;
        }

        DocumentReference docRef = db.Collection("users").Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId);

        Dictionary<string, object> user = new Dictionary<string, object>
        {
                { "uid", FirebaseAuth.DefaultInstance.CurrentUser.UserId },
                { "name",  FirebaseAuth.DefaultInstance.CurrentUser.DisplayName },
                { "email", FirebaseAuth.DefaultInstance.CurrentUser.Email },
                { "serialCode", serialCodeInputField.text },
        };

        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            startUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            isButtonClicked = false;
        });
    }

    private void OnEnable()
    {
        statusText.text = string.Empty;
        isButtonClicked = false;

        LoadDatabase();

        activateButton.onClick.AddListener(OnClickActivateButton);
    }

    private void OnDisable()
    {
        activateButton.onClick.RemoveListener(OnClickActivateButton);
    }
}
