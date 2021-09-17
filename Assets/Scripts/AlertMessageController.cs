using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AlertMessageController : MonoBehaviour
{   
    [SerializeField]
    public GameObject AlertPanel;
    [SerializeField]
    public Text AlertText;

    [SerializeField]
    public GameObject StartPoint;
    
    [SerializeField]
    public GameObject EndPoint;

    public void popUpMessage(string msg)
    {
        AlertText.text = msg;
        LeanTween.move(AlertPanel, StartPoint.transform.position , 0.2f).setOnComplete(Destroy);
    }
    void Destroy()
    {
        LeanTween.move(AlertPanel, EndPoint.transform.position, 1f).setDelay(1f);
    }
}
