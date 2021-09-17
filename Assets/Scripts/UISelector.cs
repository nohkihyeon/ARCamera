
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    public Dropdown UISelectorDd;
    public GameObject ReferenceObject;
    public GameObject[] PoseList = new GameObject[10];
    public GameObject SelectedPose;
    GameObject DummyPose;
    public Button DestroyButton;
    //public Button ConfirmButton;

    public GameObject UISelect;

    Dropdown.OptionData OpList;


    public Transform[] ObjectPosition = new Transform[10];

    public GameObject ARPosition;

    public GameObject test1;

    [SerializeField]
    private Button CaptureButton;

    public Camera ARCamera;

    public Slider PoseScaler;
    public Slider PoseRotator;

    Vector2 startPos;
    Vector2 direction;

    bool isMoving;
    public bool isSelected;
    [SerializeField]
    private Button ButtonOne;
    [SerializeField]
    private Button ButtonTwo;
    [SerializeField]
    private Button ButtonThree;
    [SerializeField]
    private Button ButtonFour;
    [SerializeField]
    private Button ButtonFive;
    [SerializeField]
    private Button ButtonSix;
    [SerializeField]
    private Button ButtonSeven;
    [SerializeField]
    private Button ButtonEight;
    [SerializeField]
    private Button ButtonNine;

    Vector3 OriginalSize;
    Quaternion OriginalRotation;
    const int k_NumSkeletonJoints = 91;

    public bool UIStateInfor;
    //public Text TestText2;

    public Transform[] SelectPoseInfor = new Transform[k_NumSkeletonJoints];
    Transform[] PoseTransform;

    //public Text TestText1;
    int number;
    bool PoseSelectState;
    // Start is called before the first frame update
    void Start()
    {
        UIStateInfor = false;
        InitDropDown();
        ButtonOne.onClick.AddListener(UiSelectButtonClicked);
        ButtonTwo.onClick.AddListener(UiSelectButtonClicked);
        ButtonThree.onClick.AddListener(UiSelectButtonClicked);
        ButtonFour.onClick.AddListener(UiSelectButtonClicked);
        ButtonFive.onClick.AddListener(UiSelectButtonClicked);
        ButtonSix.onClick.AddListener(UiSelectButtonClicked);
        ButtonSeven.onClick.AddListener(UiSelectButtonClicked);
        ButtonEight.onClick.AddListener(UiSelectButtonClicked);
        ButtonNine.onClick.AddListener(UiSelectButtonClicked);
        //ARCamera.cullingMask = ~(1 << 8);
    }
    private void UiSelectButtonClicked()
    {
        var go = EventSystem.current.currentSelectedGameObject;
        if (go != null)
        {
            Debug.Log("Clicked on : " + go.name);
            int num = (int)char.GetNumericValue(go.name.ToCharArray()[7]);
            SelectPoseFunc(num);
            GameObject.Find("AlertPanel").GetComponent<AlertMessageController>().popUpMessage("선택하신 포즈로 생성되었어요!");
        }

        else
            Debug.Log("currentSelect Gameobject is null");

    }

    private void Awake()
    {
        UISelectorDd.onValueChanged.AddListener(SelectPoseFunc);
        DestroyButton.onClick.AddListener(DestroySelectedPose);
        CaptureButton.onClick.AddListener(AddFunc);
        PoseScaler.onValueChanged.AddListener(ControllPoseScaler);
        PoseRotator.onValueChanged.AddListener(ControllPoseRotator);

    }

    // Update is called once per frame
    void Update()
    {
        //TestText2.text = StateTest.ToString();
        TouchControll();
        if (!PoseSelectState)
        {
            isSelected = true;
            //InitializeTransform(SelectedPose);

        }
        else
        {
            isSelected = false;

        }
    }

    void SelectPoseFunc(int index)
    {
        if (index == 0)
        {
            Debug.Log("Zero can't use");
        }
        else
        {
            if (PoseSelectState)
            {
                SelectedPose = Instantiate(PoseList[index]);

                SelectedPose.transform.localScale = SelectedPose.transform.localScale - new Vector3(200, 200, 200);
                SelectedPose.transform.position = ARPosition.transform.position;
                SelectedPose.transform.rotation = ARCamera.transform.rotation;
                PoseSelectState = false;
                //UISelectorDd.gameObject.SetActive(PoseSelectState);
                DestroyButton.gameObject.SetActive(!PoseSelectState);
                UISelect.gameObject.SetActive(PoseSelectState);
                //SelectedPose.gameObject.SetActive(true);
                SelectedPose.gameObject.layer = 0;
                UIStateInfor = true;


                DummyPose = Instantiate(PoseList[index]);
                DummyPose.transform.localScale = DummyPose.transform.localScale - new Vector3(200, 200, 200);
                DummyPose.transform.position = ARPosition.transform.position;
                DummyPose.transform.rotation = ARCamera.transform.rotation;
                DummyPose.gameObject.SetActive(false);

                OriginalSize = SelectedPose.transform.localScale;
                OriginalRotation = SelectedPose.transform.rotation;

                //PoseTransform = SelectedPose.GetComponentsInChildren<Transform>();
                //InitPoseTransform(PoseTransform);
                //InitializeTransform(SelectedPose);

                ResetPoseScaler();
                ResetPoseRotator();
            }
        }
    }

    public void InitializeTransform(HumanBoneController human)
    {

        DummyPose.transform.position = SelectedPose.transform.position;
        DummyPose.transform.rotation = human.transform.rotation;
        DummyPose.transform.localScale = new Vector3(1, 1, 1);

        PoseTransform = DummyPose.GetComponentsInChildren<Transform>();
        InitPoseTransform(PoseTransform);

    }
    public void SetTrue()
    {
        SelectedPose.SetActive(true);
    }
    public void SetFalse()
    {
        SelectedPose.SetActive(false);
    }
    private void InitPoseTransform(Transform[] trans)
    {
        int ChildCount = 0;
        int IndexCount = 0;
        int JointIndex = 0;


        foreach (var child in trans)
        {
            if (child.name == transform.name)
                return;

            else if (ChildCount >= 7 && IndexCount < 91)
            {

                JointIndex = GameObject.Find("AR Human Body Tracker").GetComponent<FindJointNumber>().GetJoint(child.name);
                SelectPoseInfor[JointIndex] = child.transform;
                IndexCount++;

            }
            if (JointIndex == 46)
            {
                ChildCount++;
                return;
            }
            ChildCount++;
        }
    }

    public void SetObjectTransform(Transform input, int CaptureCount)
    {
        ObjectPosition[CaptureCount] = input;
    }
    public void DestroySelectedPose()
    {
        Destroy(SelectedPose);
        Destroy(DummyPose);
        PoseSelectState = true;
        DestroyButton.gameObject.SetActive(!PoseSelectState);
        //UISelectorDd.gameObject.SetActive(PoseSelectState);
        UIStateInfor = false;
    }
    void InitDropDown()
    {
        number = 1;
        PoseSelectState = true;
        for (int i = 0; i < PoseList.Length; i++)
        {
            PoseList[i] = null;
        }
        UISelectorDd.options.Clear();
        OpList = new Dropdown.OptionData();
        OpList.text = "포즈를 선택하세요.";
        UISelectorDd.options.Add(OpList);

        DestroyButton.gameObject.SetActive(false);
    }

    public void AddFunc()
    {
        if (number < 10)
        {
            //PoseList[number] = ReferenceObject.transform.GetComponent<HumanBodyTrackerUI>().CapturedPoseCharacter;
            OpList = new Dropdown.OptionData();
            OpList.text = number.ToString();
            UISelectorDd.options.Add(OpList);
            number++;
        }
    }
    void TouchControll()
    {
        //if(Input.GetTouch(0).ga == "")
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            isMoving = true;
            startPos = touch.position;

        }
        if (touch.phase == TouchPhase.Moved)
        {
            SelectedPose.transform.Translate(touch.deltaPosition * Time.deltaTime * 0.1f);
        }
        if (touch.phase == TouchPhase.Ended)
        {
            isMoving = false;
            startPos = touch.position;
        }

    }
    private void ResetPoseScaler()
    {
        //PoseScaler.value = (int)SelectedPose.transform.position.y;
        PoseScaler.value = (int)OriginalSize.y;
        PoseScaler.maxValue = (int)OriginalSize.y + 6;
        PoseScaler.minValue = 0;
    }
    private void ControllPoseScaler(float value)
    {
        //if (value + OriginalSize.x < 0 || value + OriginalSize.y < 0 || value + OriginalSize.z < 0)
        //    TouchedObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        //else
        //    TouchedObject.transform.localScale = OriginalSize + new Vector3(value, value, value);
        //SelectedPose.transform.localScale = OriginalSize + new Vector3(value, value, value);
        SelectedPose.transform.localScale = OriginalSize * value;
    }
    private void ResetPoseRotator()
    {
        PoseRotator.value = OriginalRotation.y;
        PoseRotator.maxValue = OriginalRotation.y + 180;
        PoseRotator.minValue = OriginalRotation.y - 180;
    }
    private void ControllPoseRotator(float value)
    {
        SelectedPose.transform.rotation = Quaternion.Euler(OriginalRotation.x, OriginalRotation.y + value, OriginalRotation.z);
    }
}
