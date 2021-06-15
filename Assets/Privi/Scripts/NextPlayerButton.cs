using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPlayerButton : MonoBehaviour
{
    [SerializeField] Text text;
    private void Start()
    {
        if(text == null)
        {
            text = GetComponentInChildren<Text>();
        }
        onNextPlayerController();
        GameManager.Instance.OnSwitchPlayerControllerEvent += onNextPlayerController;
    }

    private void onNextPlayerController()
    {
        text.text = GameManager.Instance.NextControllerName;
    }
}
