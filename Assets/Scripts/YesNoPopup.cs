using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesNoPopup : MonoBehaviour
{
    public Button yesButton = null;
    public Button noButton = null;
    public TMP_Text text = null;

    public void SetName(string name, string type)
    {
        text.text = $"{name}({type})의 참여를\n수락하시겠습니까?";
    }
}
