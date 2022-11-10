using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField]
    private GameObject[] actives;

    [SerializeField]
    private GameObject[] deactives;

    private void Awake()
    {
        foreach (var obj in deactives)
        {
            obj.SetActive(false);
        }

        foreach (var obj in actives)
        {
            obj.SetActive(true);
        }
    }
}
