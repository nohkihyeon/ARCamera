using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class HumanBodyTrackerUI : Singleton<HumanBodyTrackerUI>
{
    HumanBoneController humanBoneController;
    int Count = 0;
    int compareNum = 0;

   [SerializeField]
    [Tooltip("The Skeleton prefab to be controlled.")]
    private GameObject skeletonPrefab;


    [SerializeField]
    [Range(-10.0f, 10.0f)]
    private float skeletonOffsetX = 0;

    [Range(-10.0f, 10.0f)]
    [SerializeField]
    private float skeletonOffsetY = 0;

    [Range(-10.0f, 10.0f)]
    [SerializeField]
    private float skeletonOffsetZ = 0;

    [SerializeField]
    [Tooltip("The ARHumanBodyManager which will produce body tracking events.")]
    private ARHumanBodyManager humanBodyManager;
    
    /*=============================================================================================================================*/

    [SerializeField]
    [Tooltip("The Human Body Tracker text used for debugging purposes.")]
    public Text humanBodyTrackerText;

    [SerializeField]
    [Tooltip("The Human Body text used for debugging purposes.")]
    public Text humanBodyText;

    [SerializeField]
    [Tooltip("The Human bone controller used for debugging purposes.")]
    public Text humanBoneControllerText;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private Button UISelectDismiss;

    [SerializeField]
    private Button toggleOptionsButton;

    [SerializeField]
    private Button selectButton;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private GameObject UISelect;

    [SerializeField]
    private Toggle toggleDebug;

    [SerializeField]
    private Text debugText;

    [SerializeField]
    private Toggle toggleupdate;

    bool toggleBool = true;

    const int k_NumSkeletonJoints = 91; // 조인트 수를 나타내는 값


    //오브젝트 좌표 저장할 때 사용할 버튼(테스트 용도)
    [SerializeField]
    private Button CaptureButton;

    //만약 캡쳐 버튼을 눌렀을 때 오브젝트가 생성되어 있는지 판단하기 위한 변수
    bool IsAdded = false;

    //캡쳐하기 전 OnHumanBodiesChanged내의 UI문구가 뜨지 않도록 예외 처리하기 위한 불 변수
    bool ButtonClicked = false;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();




    private Dictionary<TrackableId, HumanBoneController> skeletonTracker2 = new Dictionary<TrackableId, HumanBoneController>();

    Transform[] OriginalSkeleton = new Transform[k_NumSkeletonJoints];
    StoreTransform[] originalSkeleton = new StoreTransform[k_NumSkeletonJoints];
    //HumanBoneController OriginalSkeleton;

    Transform[] NewSkeleton = new Transform[k_NumSkeletonJoints];


    public ARHumanBodyManager HumanBodyManagers
    {
        get { return humanBodyManager; }
        set { humanBodyManager = value; }
    }

    public GameObject SkeletonPrefab
    {
        get { return skeletonPrefab; }
        set { skeletonPrefab = value; }
    }

    void OnEnable()
    {
        Debug.Assert(humanBodyManager != null, "Human body manager is required.");
        humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        if (humanBodyTrackerText == null || humanBodyTrackerText == null || humanBoneControllerText == null)
        {
            Debug.LogError($"{gameObject.name} is missing UI connections in inspector...");

        }
    }

    void OnDisable()
    {
        if (humanBodyManager != null)
            humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
    }

    void Awake()
    {
        dismissButton.onClick.AddListener(Dismiss);
        UISelectDismiss.onClick.AddListener(SelectDismiss);
        toggleOptionsButton.onClick.AddListener(ToggleOptions);
        toggleDebug.onValueChanged.AddListener(ToggleDebugging);
        toggleupdate.onValueChanged.AddListener(ToggleBoolFunc);
        selectButton.onClick.AddListener(SelectEnableButton);

        //좌표 저장 시 사용할 버튼 활성화
        CaptureButton.onClick.AddListener(CaptureFunction);
    }
    //좌표 저장 시 사용할 함수(테스트 용도)

    private int CompareAlgorithm()
    {
        compareNum = 0;
        for (int i=0; i < k_NumSkeletonJoints; i++)
        {
           
        }
        return compareNum;
    }
    
    private void CaptureFunction()
    {
        if (IsAdded == true)
        {
            // 확장클래스를 활용해서 위치, 회전, 크기를 Deep copy
            for (int i=0; i < k_NumSkeletonJoints; i++)
            {
                originalSkeleton[i] = humanBoneController.m_BoneMapping[i].Save().Position().Rotation().Scale();
            }
            
            ButtonClicked = true;
        }
        else
        {
            Debug.LogWarning("Not Saved the Object");
        }

    }
    private void Dismiss() => welcomePanel.SetActive(false);
    private void SelectDismiss() => UISelect.SetActive(false);
    private void SelectEnableButton() => UISelect.SetActive(true);

    private void ToggleDebugging(bool value)
    {
        ToggleText(humanBodyText);
        ToggleText(humanBodyTrackerText);
        ToggleText(humanBoneControllerText);

        if (value == true)
            debugText.GetComponentInChildren<Text>().text = "Debug Off";
        else
            debugText.GetComponentInChildren<Text>().text = "Debug On";
    }

    private void ToggleText(Text text) => text.gameObject.SetActive(!text.gameObject.activeSelf);

    private void ToggleOptions()
    {
        Count++;
        if (ButtonClicked == true)
        {
            //1
            //유사도 비교 알고리즘 사용(향후 수정해야 한다.)
            //CompareAlgorithm();
            //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"SavedPositionArray " + Count.ToString() + " : (" + SavedPositionArray[Count, 0] +
            //    ", " + SavedPositionArray[Count, 1] +
            //    ", " + SavedPositionArray[Count, 2] + ")";
            //HumanBodyTrackerUI.Instance.humanBodyText.text = $"실시간Skeleton " + Count.ToString() + " : " + humanBoneController.m_BoneMapping[Count].position;

            HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"originalSkeleton[" + Count.ToString() + "] : (" + originalSkeleton[Count].position.x +
                ", " + originalSkeleton[Count].position.y +
                ", " + originalSkeleton[Count].position.z + ")";
            HumanBodyTrackerUI.Instance.humanBodyText.text = $"humanBoneController[" + Count.ToString() + "] : " + humanBoneController.m_BoneMapping[Count].position;

            HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $" Index :  {(float)compareNum / 91}" + " //Threshold : " + compareNum + "/91";
        }
        if (Count % 2 == 0)
        {
            //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"options.activeSelf";
            toggleOptionsButton.GetComponentInChildren<Text>().text = "Record\nMode";
            options.SetActive(false);
        }
        else if(Count % 2 == 1)
        {
            //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"!options.activeSelf";
            toggleOptionsButton.GetComponentInChildren<Text>().text = "X";
            options.SetActive(true);

        }
    }
    private void ToggleBoolFunc(bool value)
    {
        if (value == true)
        {
            toggleBool = true;
        }
        else if (value == false)
        {
            toggleBool = false;
        }
    }


    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
    {
        if (toggleBool == false)
        {
            //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"{this.gameObject.name} Position: {this.gameObject.transform.position}\n" +
            //            $"LocalPosition: {this.gameObject.transform.localPosition}";
            //HumanBodyTrackerUI.Instance.humanBodyText.text = $" toggle.activeSelf : {toggleupdate.gameObject.activeSelf}";
        }

        if (toggleBool == true)
        {
            foreach (var humanBody in eventArgs.added)
            {
                if (!skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
                {
                    Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
                    var newSkeletonGO = Instantiate(skeletonPrefab, humanBody.transform);

                    humanBoneController = newSkeletonGO.GetComponent<HumanBoneController>();

                    // add an offset just when the human body is added
                    humanBoneController.transform.position = humanBoneController.transform.position +
                        new Vector3(skeletonOffsetX, skeletonOffsetY, skeletonOffsetZ);

                    skeletonTracker.Add(humanBody.trackableId, humanBoneController);
                }

                humanBoneController.InitializeSkeletonJoints();
                humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
                IsAdded = true;


                //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"{this.gameObject.name} {humanBoneController.name} Position: {humanBoneController.transform.position}\n" +
                //$"LocalPosition: {humanBoneController.transform.localPosition}";
                //HumanBodyTrackerUI.Instance.humanBodyText.text = $" toggle.activeSelf : {toggleupdate.gameObject.activeSelf}";
                //HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $" buttonClicked :  {ButtonClicked}";
                HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $" Index :  {(float)compareNum / 91}" + " //Threshold : " + compareNum + "/91";
            }

        }
        // true 버튼을 눌렀을때만 실행
        if (toggleBool == true)
        {
            foreach (var humanBody in eventArgs.updated)
            {
                if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
                {
                    humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
                }

                //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"{this.gameObject.name} Position: {this.gameObject.transform.position}\n" +
                //         $"LocalPosition: {this.gameObject.transform.localPosition}";
                //HumanBodyTrackerUI.Instance.humanBodyText.text = $" toggle.activeSelf : {toggleupdate.gameObject.activeSelf}";
                //HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $" buttonClicked :  {ButtonClicked}";
                HumanBodyTrackerUI.Instance.humanBoneControllerText.text = $" Index :  {(float)compareNum / 91}" + " //Threshold : " + compareNum + "/91";



            }
        }

        foreach (var humanBody in eventArgs.removed)
        {
            Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                Destroy(humanBoneController.gameObject);
                skeletonTracker.Remove(humanBody.trackableId);
            }
        }

        //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"{this.gameObject.name} Position: {this.gameObject.transform.position}\n" +
        //    $"LocalPosition: {this.gameObject.transform.localPosition}";
    }

}

public class StoreTransform
{
    private Transform m_Transform;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public StoreTransform(Transform aTransform)
    {
        m_Transform = aTransform;
    }
    public StoreTransform LocalPosition()
    {
        position = m_Transform.localPosition;
        return this;
    }
    public StoreTransform Position()
    {
        position = m_Transform.position;
        return this;
    }
    public StoreTransform LocalRotation()
    {
        rotation = m_Transform.localRotation;
        return this;
    }
    public StoreTransform Rotation()
    {
        rotation = m_Transform.rotation;
        return this;
    }
    public StoreTransform Scale()
    {
        localScale = m_Transform.localScale;
        return this;
    }
    public StoreTransform AllLocal()
    {
        return LocalPosition().LocalRotation().Scale();
    }
    public StoreTransform AllWorld()
    {
        return Position().Rotation().Scale();
    }
}




public static class TransformSerializationExtension
{
    public static StoreTransform Save(this Transform aTransform)
    {
        return new StoreTransform(aTransform);
    }


}