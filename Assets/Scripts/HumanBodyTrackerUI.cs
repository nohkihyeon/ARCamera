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
    public Toggle toggleDebug;

    [SerializeField]
    private Text debugText;

    [SerializeField]
    private Button ObjectControllerButton;

    [SerializeField]
    private Toggle toggletracking;

    bool toggleBool = true;
    bool AutoTakePic = false;

    public GameObject CompareTextPanel;

    public Text CompareText;

    const int k_NumSkeletonJoints = 91; // 조인트 수를 나타내는 값


    //오브젝트 좌표 저장할 때 사용할 버튼(테스트 용도)
    [SerializeField]
    private Button CaptureButton;

    //만약 캡쳐 버튼을 눌렀을 때 오브젝트가 생성되어 있는지 판단하기 위한 변수
    bool IsAdded = false;

    //캡쳐하기 전 OnHumanBodiesChanged내의 UI문구가 뜨지 않도록 예외 처리하기 위한 불 변수
    bool ButtonClicked = false;

    private Dictionary<TrackableId, HumanBoneController> skeletonTracker = new Dictionary<TrackableId, HumanBoneController>();

    StoreTransform[] OriginalSkeleton = new StoreTransform[k_NumSkeletonJoints];

    Transform[] Children;

    StoreTransform[] StoredSkeleton = new StoreTransform[k_NumSkeletonJoints];

    bool[] PercentOfMatch = new bool[k_NumSkeletonJoints];

    public GameObject UISelect_Content;

    public GameObject CapturedPoseCharacter;
    public int CaptureCount;

    public GameObject TestPoseCharacter;

    //GameObject SelectedPose;

    public Text text1;
    public Text TestText2;


    int Xposition = 150;
    int XCount = 1;
    int Yposition = 1300;
    int YCount = 1;

    public bool CaptureUse;

    bool CaptureJudge;
    bool ControllerState;
    bool UISelectState;


    public GameObject[] PositionArray = new GameObject[9];

    bool result = false;

    public Text tex;



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

    private void Start()
    {
        CaptureUse = true;
        CaptureJudge = false;
        CaptureCount = 1;
        ControllerState = false;
        UISelectState = false;
        CompareTextPanel.SetActive(false);
    }
    private void Update()
    {
        bool IsSelect = GameObject.Find("Canvas").GetComponent<UISelector>().isSelected;
        //bool result = false;
        if (IsSelect)
        {
            result = CompareAlgorithm();
            CompareText.text = compareNum.ToString() + "%";
        }
        // text1.text = "알고리즘 결과는 " + result.ToString();
        if (AutoTakePic && CompareAlgorithm())
        {
            GameObject.Find("Canvas").GetComponent<UISelector>().SelectedPose.SetActive(false);
            GameObject.Find("AR Camera").GetComponent<CameraCapture>().TakePhoto();
            CaptureJudge = true;
            toggleDebug.isOn = false;
        }
        if (CaptureJudge)
        {
            GameObject.Find("Canvas").GetComponent<UISelector>().SelectedPose.SetActive(true);
            CaptureJudge = false;
        }

    }

    void Awake()
    {
        dismissButton.onClick.AddListener(Dismiss);
        toggleOptionsButton.onClick.AddListener(ToggleOptions);
        toggleDebug.onValueChanged.AddListener(ToggleDebugging);
        ObjectControllerButton.onClick.AddListener(ObjectController);
        selectButton.onClick.AddListener(UISelectSetFunc);
        toggletracking.onValueChanged.AddListener(SkeletonOnOff);

        //좌표 저장 시 사용할 버튼 활성화
        CaptureButton.onClick.AddListener(CaptureFunction);
    }
    // 두 오브젝트를 비교하는 함수
    private bool CompareAlgorithm()
    {
        compareNum = 0;
        bool CompareResult;

        GameObject.Find("Canvas").GetComponent<UISelector>().InitializeTransform(humanBoneController);

        Transform[] SelectedPoseInfor = GameObject.Find("Canvas").GetComponent<UISelector>().SelectPoseInfor;

        Vector3 DistanceHeapPot = PositionCompare(SelectedPoseInfor[1].position, humanBoneController.m_BoneMapping[1].position);

        //StoredSkeleton에 정확히 저장 후 StoredSkeleton을 m_BoneMapping과 일치한 좌표로 옮기는 반복문
        for (int i = 0; i < k_NumSkeletonJoints; i++)
        {
            StoredSkeleton[i] = SelectedPoseInfor[i].Save().AllWorld();
            StoredSkeleton[i].position = -1 * (DistanceHeapPot - SelectedPoseInfor[i].position);
            Vector3 JointDistance = StoredSkeleton[i].position - humanBoneController.m_BoneMapping[i].position;

            if (JointDistance.sqrMagnitude < (0.01f)) PercentOfMatch[i] = true;
            else PercentOfMatch[i] = false;
        }

        foreach (bool MatchResult in PercentOfMatch)
        {
            if (MatchResult == true) compareNum++;
            else continue;
        }

        if (compareNum >= 70) CompareResult = true;
        else CompareResult = false;

        //TestText2.text = "Percent : " + compareNum.ToString() +"/ "+ humanBoneController.transform.localScale;

        return CompareResult;
    }

    // 기존의 OriginalSkeleton과 humanBoneController.m_BoneMapping의 좌표 차이를 구해서 그 차이를 반환하는 함수
    private Vector3 PositionCompare(Vector3 StoreTransformPosition, Vector3 TransformPosition)
    {
        return StoreTransformPosition - TransformPosition;
    }


    private void CaptureFunction()
    {

        if (IsAdded == true && CaptureCount < 10 && CaptureUse == true)
        {
            GameObject.Find("AlertPanel").GetComponent<AlertMessageController>().popUpMessage("포즈 저장 완료!");
            CapturedPoseCharacter = Instantiate(SkeletonPrefab);
            Children = CapturedPoseCharacter.GetComponentsInChildren<Transform>();

            // 확장클래스를 활용해서 위치, 회전, 크기를 Deep copy
            for (int i = 0; i < k_NumSkeletonJoints; i++)
            {
                OriginalSkeleton[i] = humanBoneController.m_BoneMapping[i].Save().AllWorld();
            }
            HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"access Children";
            GameObject.Find("Canvas").GetComponent<UISelector>().SetObjectTransform(CapturedPoseCharacter.transform, CaptureCount);
            GameObject.Find("Canvas").GetComponent<UISelector>().PoseList[CaptureCount] = CapturedPoseCharacter;


            CreateStoredSkeleton(Children);
            TranslateCapturedPoseCharacter();
            CaptureCount++;


            ButtonClicked = true;
            //HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"CaputuredPosedCharacter transform\n" + CapturedPoseCharacter.transform;
            CapturedPoseCharacter.layer = 8;
        }
        else
        {
            Debug.LogWarning("Not Saved the Object");
        }

        // CapturePose();
    }

    private void TranslateCapturedPoseCharacter()
    {
        CapturedPoseCharacter.transform.localScale += new Vector3(200, 200, 200);
        CapturedPoseCharacter.transform.position = PositionArray[CaptureCount - 1].transform.position;
        tex.text = CaptureCount.ToString();

        CapturedPoseCharacter.transform.rotation = PositionArray[0].transform.rotation;
    }

    private void CreateStoredSkeleton(Transform[] trans)
    {
        int ChildCount = 0;
        int IndexCount = 0;
        int JointIndex = 0;
        HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"In the Creat~ Children";

        foreach (var child in trans)
        {
            if (child.name == transform.name)
                return;
            else if (ChildCount < 7)
            {
                JointIndex = GameObject.Find("AR Human Body Tracker").GetComponent<FindJointNumber>().GetJoint("Root");
                child.transform.position = OriginalSkeleton[JointIndex].position;
                child.transform.rotation = OriginalSkeleton[JointIndex].rotation;
            }
            else if (ChildCount >= 7 && IndexCount < 91)
            {

                JointIndex = GameObject.Find("AR Human Body Tracker").GetComponent<FindJointNumber>().GetJoint(child.name);
                child.transform.position = OriginalSkeleton[JointIndex].position;
                child.transform.rotation = OriginalSkeleton[JointIndex].rotation;
                IndexCount++;
                HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"JointIndex : " + JointIndex;

            }
            if (JointIndex == 46)
            {
                ChildCount++;
                return;
            }
            child.gameObject.layer = 8;
            ChildCount++;
        }
    }


    private void SkeletonOnOff(bool value)
    {
        if (toggleBool == true)
        {
            if (value == true)
            {
                CaptureUse = true;
                humanBoneController.gameObject.SetActive(CaptureUse);
            }
            else if (value == false)
            {
                CaptureUse = false;
                humanBoneController.gameObject.SetActive(CaptureUse);
            }

            else
                Debug.Log("Not Instantiate HumanboneController");
        }
    }
    private void Dismiss() => welcomePanel.SetActive(false);

    private void UISelectSetFunc()
    {
        UISelectState = !UISelectState;
        UISelect.SetActive(UISelectState);
    }

    private void ToggleDebugging(bool value)
    {
        // ToggleText(humanBodyText);
        ToggleText(humanBodyTrackerText);
        ToggleText(humanBoneControllerText);

        if (value == true)
        {
            CompareTextPanel.SetActive(true);
            ObjectControllerButton.gameObject.SetActive(false);
            debugText.GetComponentInChildren<Text>().text = "Compare 중";
            AutoTakePic = true;

        }
        else
        {
            AutoTakePic = false;
            debugText.GetComponentInChildren<Text>().text = "자동촬영";
            ObjectControllerButton.gameObject.SetActive(true);
        }

    }

    private void ToggleText(Text text) => text.gameObject.SetActive(!text.gameObject.activeSelf);

    private void ToggleOptions()
    {
        Count++;
        if (Count % 2 == 0)
        {
            options.SetActive(false);
        }
        else if (Count % 2 == 1)
        {
            options.SetActive(true);
        }
    }
    private void ObjectController()
    {
        ControllerState = !ControllerState;
        GameObject.Find("Canvas").GetComponent<UISelector>().PoseRotator.gameObject.SetActive(ControllerState);
        GameObject.Find("Canvas").GetComponent<UISelector>().PoseScaler.gameObject.SetActive(ControllerState);
    }


    void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs eventArgs)
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
        }


        // true 버튼을 눌렀을때만 실행

        foreach (var humanBody in eventArgs.updated)
        {
            if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
            {
                humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);

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
        //if (toggleBool == false)
        //{

        //}

        //if (toggleBool == true)
        //{
        //    foreach (var humanBody in eventArgs.added)
        //    {
        //        if (!skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
        //        {
        //            Debug.Log($"Adding a new skeleton [{humanBody.trackableId}].");
        //            var newSkeletonGO = Instantiate(skeletonPrefab, humanBody.transform);

        //            humanBoneController = newSkeletonGO.GetComponent<HumanBoneController>();

        //            // add an offset just when the human body is added
        //            humanBoneController.transform.position = humanBoneController.transform.position +
        //                new Vector3(skeletonOffsetX, skeletonOffsetY, skeletonOffsetZ);

        //            skeletonTracker.Add(humanBody.trackableId, humanBoneController);
        //        }

        //        humanBoneController.InitializeSkeletonJoints();
        //        humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);
        //        IsAdded = true;
        //    }

        //}
        //// true 버튼을 눌렀을때만 실행
        //if (toggleBool == true)
        //{
        //    foreach (var humanBody in eventArgs.updated)
        //    {
        //        if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
        //        {
        //            humanBoneController.ApplyBodyPose(humanBody, Vector3.zero);

        //        }
        //    }
        //}

        //foreach (var humanBody in eventArgs.removed)
        //{
        //    Debug.Log($"Removing a skeleton [{humanBody.trackableId}].");
        //    if (skeletonTracker.TryGetValue(humanBody.trackableId, out humanBoneController))
        //    {
        //        Destroy(humanBoneController.gameObject);
        //        skeletonTracker.Remove(humanBody.trackableId);
        //    }
        //}
    }
}





//StoreTransform 클래스 부분
//=====================================================================================================================
public class StoreTransform
{
    private Transform m_Transform;
    public Vector3 position;
    public Vector3 localPosition;
    public Quaternion rotation;
    public Quaternion localRotation;
    public Vector3 localScale;

    private StoreTransform m_StoreTransform;

    public StoreTransform(Transform aTransform)
    {
        m_Transform = aTransform;
    }
    public StoreTransform LocalPosition()
    {
        localPosition = m_Transform.localPosition;
        return this;
    }
    public StoreTransform Position()
    {
        position = m_Transform.position;
        return this;
    }
    public StoreTransform LocalRotation()
    {
        localRotation = m_Transform.localRotation;
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


    public StoreTransform(StoreTransform aTransform)
    {
        m_StoreTransform = aTransform;
    }
    public StoreTransform StoreLocalPosition()
    {
        localPosition = m_StoreTransform.localPosition;
        return this;
    }
    public StoreTransform StorePosition()
    {
        position = m_StoreTransform.position;
        return this;
    }
    public StoreTransform StoreLocalRotation()
    {
        rotation = m_StoreTransform.localRotation;
        return this;
    }
    public StoreTransform StoreRotation()
    {
        rotation = m_StoreTransform.rotation;
        return this;
    }
    public StoreTransform StoreScale()
    {
        localScale = m_StoreTransform.localScale;
        return this;
    }
    public StoreTransform StoreAllLocal()
    {
        return StoreLocalPosition().StoreLocalRotation().StoreScale();
    }
    public StoreTransform StoreAllWorld()
    {
        return StorePosition().StoreRotation().StoreScale();
    }
}




public static class TransformSerializationExtension
{
    public static StoreTransform Save(this Transform aTransform)
    {
        return new StoreTransform(aTransform);
    }

    public static StoreTransform StoreSave(this StoreTransform aTransform)
    {
        return new StoreTransform(aTransform);
    }
}