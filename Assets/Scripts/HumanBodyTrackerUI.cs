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
    private Toggle toggleDebug;

    [SerializeField]
    private Text debugText;

    [SerializeField]
    private Toggle toggleupdate;

    [SerializeField]
    private Toggle toggletracking;

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

    //public GameObject Position1;
    //public GameObject Position2;
    //public GameObject Position3;


    public GameObject[] PositionArray = new GameObject[9];


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
        CaptureCount = 1;
        //PositionArray[0] = Position1;
        //PositionArray[1] = Position2;
        //PositionArray[2] = Position3;
    }
    void Awake()
    {
        dismissButton.onClick.AddListener(Dismiss);
        UISelectDismiss.onClick.AddListener(SelectDismiss);
        toggleOptionsButton.onClick.AddListener(ToggleOptions);
        toggleDebug.onValueChanged.AddListener(ToggleDebugging);
        toggleupdate.onValueChanged.AddListener(ToggleBoolFunc);
        selectButton.onClick.AddListener(SelectEnableButton);
        toggletracking.onValueChanged.AddListener(SkeletonOnOff);

        //좌표 저장 시 사용할 버튼 활성화
        CaptureButton.onClick.AddListener(CaptureFunction);
    }
    // 두 오브젝트를 비교하는 함수
    private bool CompareAlgorithm()
    {
        compareNum = 0;
        bool CompareResult;

        Vector3 DistanceHeapPot = PositionCompare(OriginalSkeleton[1].position, humanBoneController.m_BoneMapping[1].position);

        //StoredSkeleton에 정확히 저장 후 StoredSkeleton을 m_BoneMapping과 일치한 좌표로 옮기는 반복문
        for (int i = 0; i < k_NumSkeletonJoints; i++)
        {
            StoredSkeleton[i] = OriginalSkeleton[i].StoreSave().StoreAllWorld();
            StoredSkeleton[i].position = -1 * (DistanceHeapPot - OriginalSkeleton[i].position);
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

        return CompareResult;
    }

    // 기존의 OriginalSkeleton과 humanBoneController.m_BoneMapping의 좌표 차이를 구해서 그 차이를 반환하는 함수
    private Vector3 PositionCompare(Vector3 StoreTransformPosition, Vector3 TransformPosition)
    {
        return StoreTransformPosition - TransformPosition;
    }

    private void CaptureFunction()
    {
        //TestObject = Instantiate(SkeletonPrefab, new Vector3(178, 1337, -373), Quaternion.identity);
        //TestObject.transform.localScale = new Vector3(200,200,200);

        //CapturedPoseCharacter = Instantiate(SkeletonPrefab);
        //Children = CapturedPoseCharacter.GetComponentsInChildren<Transform>();
        //CapturedPoseCharacter.layer = 8;
        //CapturedPoseCharacter.transform.localScale = new Vector3(200, 200, 200);
        //CapturedPoseCharacter.transform.position = new Vector3(0, 800, -400);

        if (IsAdded == true && CaptureCount < 10)
        {

            CapturedPoseCharacter = Instantiate(SkeletonPrefab);
            //GameObject.Find("Canvas").GetComponent<UISelector>().SetObjectTransform(CapturedPoseCharacter, CaptureCount);


            Children = CapturedPoseCharacter.GetComponentsInChildren<Transform>();
            //CapturedPoseCharacter.layer = 8;

            // 확장클래스를 활용해서 위치, 회전, 크기를 Deep copy
            for (int i = 0; i < k_NumSkeletonJoints; i++)
            {
                OriginalSkeleton[i] = humanBoneController.m_BoneMapping[i].Save().AllWorld();
            }
            HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"access Children";
            GameObject.Find("Canvas").GetComponent<UISelector>().SetObjectTransform(CapturedPoseCharacter.transform, CaptureCount);
            //Transform PPPPP = GameObject.Find("Canvas").GetComponent<UISelector>().ObjectPosition[CaptureCount];
            //text1.text = PPPPP.position.ToString() + "////"+ humanBoneController.transform.position.ToString() + humanBoneController.transform.localScale.ToString();
            GameObject.Find("Canvas").GetComponent<UISelector>().PoseList[CaptureCount] = CapturedPoseCharacter;


            CreateStoredSkeleton(Children);
            TranslateCapturedPoseCharacter();
            //CreateStoredSkeleton(Children2);

            //Transform PPPPP = GameObject.Find("Canvas").GetComponent<UISelector>().ObjectPosition[CaptureCount];
            text1.text = CapturedPoseCharacter.transform.position.ToString() + "////" + CapturedPoseCharacter.transform.localScale.ToString();
            //TestText2.text = CapturedPoseCharacter.transform.localPosition.ToString() + " //// " + Position1.transform.position.ToString();

            ButtonClicked = true;
            // CapturedPoseCharacter.transform.parent = BasicObject.transform;
            HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"CaputuredPosedCharacter transform\n" + CapturedPoseCharacter.transform;
            //CapturedPoseCharacter.transform.position = new Vector3(0.05f, -2.45f, 0);
            //CapturedPoseCharacter.layer = 8;
            //GameObject.Find("Canvas").GetComponent<UISelector>().AddFunc(CapturedPoseCharacter);
            CapturedPoseCharacter.layer = 8;
            CaptureCount++;
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
        //CapturedPoseCharacter.transform.position = new Vector3(Xposition, Yposition, -400);
        CapturedPoseCharacter.transform.position = PositionArray[CaptureCount - 1].transform.position;
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

                //JointIndex = HumanBoneController.instance.GetJoint(child.name);
                JointIndex = GameObject.Find("AR Human Body Tracker").GetComponent<FindJointNumber>().GetJoint(child.name);
                child.transform.position = OriginalSkeleton[JointIndex].position;
                child.transform.rotation = OriginalSkeleton[JointIndex].rotation;
                IndexCount++;
                HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"JointIndex : " + JointIndex;

            }
            if (JointIndex == 46)
            {
                ChildCount++;
                // HumanBodyTrackerUI.Instance.humanBodyText.text = $" NotEndforeach, ChildCount = " + ChildCount + ", IndexCount : " + IndexCount;
                return;
            }
            child.gameObject.layer = 8;
            ChildCount++;
        }
        // HumanBodyTrackerUI.Instance.humanBodyText.text = $" Endforeach";
    }


    private void SkeletonOnOff(bool value)
    {
        if (toggleBool == true)
        {
            if (value == true)
                humanBoneController.gameObject.SetActive(true);
            else if (value == false)
            {
                humanBoneController.gameObject.SetActive(false);
            }

            else
                Debug.Log("Not Instantiate HumanboneController");
        }
    }
    private void Dismiss() => welcomePanel.SetActive(false);
    private void SelectDismiss() => UISelect.SetActive(false);
    private void SelectEnableButton() => UISelect.SetActive(true);

    private void ToggleDebugging(bool value)
    {
        // ToggleText(humanBodyText);
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
            CompareAlgorithm();
            HumanBodyTrackerUI.Instance.humanBodyTrackerText.text = $"OriginalSkeleton[" + Count.ToString() + "] : (" + OriginalSkeleton[Count].position.x +
                ", " + OriginalSkeleton[Count].position.y +
                ", " + OriginalSkeleton[Count].position.z + ")";

        }
        if (Count % 2 == 0)
        {
            toggleOptionsButton.GetComponentInChildren<Text>().text = "Record\nMode";
            options.SetActive(false);
        }
        else if (Count % 2 == 1)
        {
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