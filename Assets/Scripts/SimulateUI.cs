using Firebase.Auth;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FullSerializer;
using Firebase.Database;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine.Video;

public class SimulateUI : MonoBehaviour
{
    [Serializable]
    public class User
    {
        public string uid;
        public string name;
        public string email;
        public string serialCode;
    }

    [SerializeField]
    private CanvasGroup canvasGroup = null;

    [SerializeField]
    private TMP_Text ampmText = null;

    [SerializeField]
    private TMP_Text timeText = null;

    [SerializeField]
    private TMP_Text nameText = null;

    [SerializeField]
    private TaskElement[] elements;

    [SerializeField]
    private Button settingButton = null;

    [SerializeField]
    private StartUI startUI = null;

    [SerializeField]
    private SignalElement[] signalElements = null;

    [SerializeField]
    private YesNoPopup signalerPopup = null;

    [SerializeField]
    private YesNoPopup plannerPopup = null;

    [SerializeField]
    private SignalElement emptyElement = null;

    [SerializeField]
    private VideoPlayer videoPlayer = null;

    private string url = "https://abigail-c7d46-default-rtdb.firebaseio.com/";
    private WorkSpace currentWorkspace = null;
    private Coroutine updateRoutine = null;

    private static fsSerializer serializer = new fsSerializer();
    private List<User> users = new List<User>();

    private string[] signals = new string[] {
        "신호 없음",
        "운전자 호출",
        "주권사용",
        "보권사용",
        "운전방향지시",
        "위로 올리기",
        "천천히 조금씩 위로 올리기",
        "아래로 내리기",
        "천천히 조금씩 아래로 내리기",
        "수평이동",
        "물건걸기",
        "정지",
        "비상정지",
        "작업완료",
        "뒤집기",
        "천천히 이동",
        "기다려라",
        "신호불명",
        "기중기의 이상발생"
    };

    private async Task LoadDatabaseAsync()
    {
        users.Clear();

        CollectionReference serialCodesRef = FirebaseFirestore.DefaultInstance.Collection("users");
        await serialCodesRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();

                var user = new User() { email = documentDictionary["email"].ToString(),
                                        name = documentDictionary["name"].ToString(),
                                        uid = documentDictionary["uid"].ToString() };

                users.Add(user);
            }

            Debug.Log("Read all data from the users collection.");

        });
    }


    private void Update()
    {
        if (DateTime.Now.Hour >= 12)
        {
            ampmText.text = "PM";
        }
        else
        {
            ampmText.text = "AM";
        }

        timeText.text = $"{DateTime.Now.ToString("hh:mm")}";
    }

    private IEnumerator UpdateWorkspaceRoutine()
    {
        yield return new WaitUntil(() => currentWorkspace != null);

        while (true)
        {
            yield return new WaitForSeconds(2f);

            var targetUrl = $"{url}{currentWorkspace.id}.json";
            RestClient.Get(targetUrl, (ex, res) => {
                var target = JsonUtility.FromJson<WorkSpace>(res.Text);

                OnUpdateWorkspace(target);
            });
        }
    }

    private async void OnUpdateWorkspace(WorkSpace newWorkspace)
    {
        await LoadDatabaseAsync();

        var targetUrl = $"{url}{currentWorkspace.id}.json";

        /*
        // 접속을 받았을 때
        if (currentWorkspace.isPlanner == false && string.IsNullOrEmpty(newWorkspace.planner) == false && newWorkspace.isPlanner == false)
        {
            canvasGroup.alpha = 0f;
            plannerPopup.gameObject.SetActive(true);
            plannerPopup.yesButton.onClick.RemoveAllListeners();
            plannerPopup.noButton.onClick.RemoveAllListeners();

            plannerPopup.SetName(users.Where(elem => elem.uid == newWorkspace.planner).FirstOrDefault().name, "플래너");

            plannerPopup.yesButton.onClick.AddListener(() => {
                currentWorkspace.planner = newWorkspace.planner;
                currentWorkspace.isPlanner = true;
                plannerPopup.gameObject.SetActive(false);

                if (signalerPopup.gameObject.activeSelf == false && plannerPopup.gameObject.activeSelf == false)
                {
                    canvasGroup.alpha = 1f;
                }
            });

            plannerPopup.noButton.onClick.AddListener(() => {
                currentWorkspace.planner = newWorkspace.planner;
                currentWorkspace.isPlanner = false;
                plannerPopup.gameObject.SetActive(false);

                if (signalerPopup.gameObject.activeSelf == false && plannerPopup.gameObject.activeSelf == false)
                {
                    canvasGroup.alpha = 1f;
                }
            });
        }
        */

        /*
        if (currentWorkspace.isSignaler == false && string.IsNullOrEmpty(newWorkspace.signaler) == false && newWorkspace.isSignaler == false)
        {
            canvasGroup.alpha = 0f;
            var isPlannerPopupOpened = plannerPopup.gameObject.activeSelf;

            plannerPopup.gameObject.SetActive(false);
            signalerPopup.gameObject.SetActive(true);
            signalerPopup.yesButton.onClick.RemoveAllListeners();
            signalerPopup.noButton.onClick.RemoveAllListeners();

            signalerPopup.SetName(users.Where(elem => elem.uid == newWorkspace.signaler).FirstOrDefault().name, "신호수");

            signalerPopup.yesButton.onClick.AddListener(() => {
                currentWorkspace.signaler = newWorkspace.signaler;
                currentWorkspace.isSignaler = true;
                signalerPopup.gameObject.SetActive(false);
                plannerPopup.gameObject.SetActive(isPlannerPopupOpened);

                if (signalerPopup.gameObject.activeSelf == false && plannerPopup.gameObject.activeSelf == false)
                {
                    canvasGroup.alpha = 1f;
                }
            });

            signalerPopup.noButton.onClick.AddListener(() => {
                currentWorkspace.signaler = newWorkspace.signaler;
                currentWorkspace.isSignaler = false;
                signalerPopup.gameObject.SetActive(false);
                plannerPopup.gameObject.SetActive(isPlannerPopupOpened);

                if (signalerPopup.gameObject.activeSelf == false && plannerPopup.gameObject.activeSelf == false)
                {
                    canvasGroup.alpha = 1f;
                }
            });

            // Popup 띄우기, 위랑 다른 거
            currentWorkspace.signaler = newWorkspace.signaler;
            currentWorkspace.isSignaler = newWorkspace.isSignaler;
        }
        */

        currentWorkspace.planner = newWorkspace.planner;
        currentWorkspace.isPlanner = newWorkspace.isPlanner;
        currentWorkspace.signaler = newWorkspace.signaler;
        currentWorkspace.isSignaler = newWorkspace.isSignaler;

        // 신호를 받았을 때 
        signalElements.ToList().ForEach(elem => elem.gameObject.SetActive(false));
        emptyElement.gameObject.SetActive(false);


        if (newWorkspace.signal <= 0)
        {
            signalElements[10].gameObject.SetActive(true);
        }
        else
        {
            if (newWorkspace.signal <= signalElements.Length)
            {
                signalElements[newWorkspace.signal - 1].gameObject.SetActive(true);
            }
            else if (newWorkspace.signal < signals.Length)
            {
                emptyElement.gameObject.SetActive(true);
                emptyElement.GetComponentInChildren<TMP_Text>().text = signals[newWorkspace.signal];
            }
            else
            {
                signalElements[10].gameObject.SetActive(true);
            }
        }

        currentWorkspace.signal = newWorkspace.signal;


        // Task를 업데이트할 때
        for(int i = 0; i < 3; i++)
        {
            if (i >= newWorkspace.tasks.Count(elem => string.IsNullOrEmpty(elem) == false))
            {
                elements[i].gameObject.SetActive(false);
                continue;
            }

            elements[i].gameObject.SetActive(true);

            elements[i].taskNameText.text = newWorkspace.tasks[i];

            if (newWorkspace.istasks[i])
            {
                elements[i].Check();
            }
            else
            {
                elements[i].Uncheck();
            }
        }

        currentWorkspace.tasks = newWorkspace.tasks;
        currentWorkspace.istasks = newWorkspace.istasks;

        RestClient.Put(targetUrl, currentWorkspace);
    }

    private void ConnectToWorkspace()
    {
        currentWorkspace = null;

        // Find workspace
        var targetUrl = $"{url}workspace.json";

        RestClient.Get(targetUrl, (ex, res) => {
            
            var data = fsJsonParser.Parse(res.Text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, WorkSpace>), ref deserialized);

            var workSpaces = deserialized as Dictionary<string, WorkSpace>;
            OnResponse(workSpaces);
        });
    }

    private void OnResponse(Dictionary<string, WorkSpace> res)
    {
        if (res != null && res.Any(elem => elem.Value.Uid == FirebaseAuth.DefaultInstance.CurrentUser.UserId))
        {
            currentWorkspace = res.Where(elem => elem.Value.Uid == FirebaseAuth.DefaultInstance.CurrentUser.UserId).FirstOrDefault().Value;

            RestClient.Put($"{url}{currentWorkspace.id}.json", currentWorkspace);

            return;
        }

        // Create worksapce
        currentWorkspace = new WorkSpace();
        currentWorkspace.Uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        currentWorkspace.id = $"ws-{currentWorkspace.Uid}";

        var targetUrl = $"{url}{currentWorkspace.id}.json";
        RestClient.Put(targetUrl, currentWorkspace);
    }

    private void DisconnectWorkspace()
    {
        currentWorkspace = null;
    }

    private void OnClickSettingButton()
    {
        DisconnectWorkspace();

        startUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        foreach (var elem in elements)
        {
            elem.gameObject.SetActive(false);
        }

        emptyElement.gameObject.SetActive(false);
        signalElements[10].gameObject.SetActive(true);

        currentWorkspace = null;
        videoPlayer.Play();

        settingButton.onClick.AddListener(OnClickSettingButton);

        ConnectToWorkspace();
        nameText.text = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;

        updateRoutine = StartCoroutine(UpdateWorkspaceRoutine());
    }

    private void OnDisable()
    {
        videoPlayer.Pause();

        settingButton.onClick.RemoveListener(OnClickSettingButton);

        if (updateRoutine != null)
        {
            StopCoroutine(updateRoutine);
            updateRoutine = null;
        }
    }
}
