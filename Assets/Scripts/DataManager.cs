using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; set; }

    public bool IsAutoLogin { get; set; } = false;
    public int CameraIndex { get; set; } = 0;

    private void Awake()
    {
        Instance = this;

        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetString("IsAutoLogin", IsAutoLogin.ToString());

        PlayerPrefs.Save();
    }

    public void Load()
    {
        IsAutoLogin = bool.Parse(PlayerPrefs.GetString("IsAutoLogin", "false"));
    }
}
