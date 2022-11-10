using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskElement : MonoBehaviour
{
    public Image checkImage = null;
    public TMP_Text taskNameText = null;

    public Color checkColor;
    public Color uncheckColor;

    public void Check()
    {
        checkImage.gameObject.SetActive(true);
        taskNameText.color = checkColor;
        taskNameText.fontStyle = FontStyles.Strikethrough;
    }

    public void Uncheck()
    {
        checkImage.gameObject.SetActive(false);
        taskNameText.color = uncheckColor;
        taskNameText.fontStyle = FontStyles.Normal;
    }
}
