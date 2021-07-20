﻿// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;

// public class TouchEvent : MonoBehaviour 
// {
//     int Count;
//     int TouchObjectNumber; //추후에 레이캐스트를 이용해서 포즈를 선택했을 시 각 포즈가 가지고 있는 고유 번호를 할당
//     GameObject TouchedObject;

//     Vector2 startPos;
//     Vector2 direction;

//     public Slider PoseScaler;
//     public Slider PoseRotator;

//     Vector3 OriginalSize;
//     Quaternion OriginalRotation;

//     //GraphicRaycaster m_Raycaster;
//     //PointerEventData m_PointerEventData;
//     //EventSystem m_EventSystem;
//     bool isMoving;

//     // Start is called before the first frame update
//     void Start()
//     {

//         //m_Raycaster = GetComponent<GraphicRaycaster>();
//         //m_EventSystem = GetComponent<EventSystem>();
//         isMoving = false;
//         TouchedObject = GameObject.Find("AR Human Body Tracker").GetComponent<HumanBodyTrackerUI>().CapturedPoseCharacter;
//         OriginalSize = TouchedObject.transform.localScale;
//         OriginalRotation = TouchedObject.transform.rotation;
//         ResetPoseScaler();
//         ResetPoseRotator();
//     }
//     private void Awake()
//     {
//         PoseScaler.onValueChanged.AddListener(ControllPoseScaler);
//         PoseRotator.onValueChanged.AddListener(ControllPoseRotator);
//     }
//     // Update is called once per frame
//     void Update()
//     {
//         TouchedObject = GameObject.Find("AR Human Body Tracker").GetComponent<HumanBodyTrackerUI>().CapturedPoseCharacter;
//         TouchControll();
//         //SelectPoseFunc();
//     }
//     void TouchControll()
//     {
//         Touch touch = Input.GetTouch(0);
//         if (touch.phase == TouchPhase.Began)
//         {
//             isMoving = true;
//             startPos = touch.position;

//         }
//         if(touch.phase == TouchPhase.Moved)
//         {
//             TouchedObject.transform.Translate(touch.deltaPosition * Time.deltaTime * 0.1f);
//         }
//         if (touch.phase == TouchPhase.Ended)
//         {
//             isMoving = false;
//             startPos = touch.position;
//         }
//     }
//     //void SelectPoseFunc()
//     //{
//     //    m_PointerEventData = new PointerEventData(m_EventSystem);
//     //    m_PointerEventData.position = Input.mousePosition;

//     //    List<RaycastResult> results = new List<RaycastResult>();

//     //    m_Raycaster.Raycast(m_PointerEventData, results);

//     //    foreach (RaycastResult result in results)
//     //    {
//     //        if(result.gameObject.name == "CapturedPoseCharacter")
//     //        {
//     //            result.gameObject.transform.Translate(Vector3.back * Time.deltaTime * 1);
//     //        }
//     //    }
//     //}
//     private void ResetPoseScaler()
//     {
//         PoseScaler.value = (int)OriginalSize.y + 1;
//         PoseScaler.maxValue = (int)OriginalSize.y + 6;
//         PoseScaler.minValue = (int)OriginalSize.y - 4;
//     }
//     private void ControllPoseScaler(float value)
//     {
//         if (value + OriginalSize.x < 0 || value + OriginalSize.y < 0 || value + OriginalSize.z < 0)
//             TouchedObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
//         else
//             TouchedObject.transform.localScale = OriginalSize + new Vector3(value, value, value);
//     }
//     private void ResetPoseRotator()
//     {
//         PoseRotator.value = 0;
//         PoseRotator.maxValue = 180;
//         PoseRotator.minValue = -180;
//     }
//     private void ControllPoseRotator(float value)
//     {
//         TouchedObject.transform.rotation =  Quaternion.Euler(OriginalRotation.x, OriginalRotation.y + value, OriginalRotation.z);
//     }
// }
