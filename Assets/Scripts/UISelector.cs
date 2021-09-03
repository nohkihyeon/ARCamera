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
    GameObject SelectedPose;
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
    bool isSelected;
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

    //public Text TestText1;
    int number;
    bool PoseSelectState;
    // Start is called before the first frame update
    void Start()
    {
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
    }
    private void UiSelectButtonClicked()
    {
        var go = EventSystem.current.currentSelectedGameObject;
        if(go !=null)
        {
            Debug.Log("Clicked on : " + go.name);
            int num = (int)char.GetNumericValue(go.name.ToCharArray()[7]);
            SelectPoseFunc(num);
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
        TouchControll();
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
                //SelectedPose.transform.position -= SelectedPose.transform.position;
                SelectedPose.transform.localScale = SelectedPose.transform.localScale - new Vector3(200, 200, 200);
                SelectedPose.transform.position = ARPosition.transform.position;
                SelectedPose.transform.rotation = ARCamera.transform.rotation;
                SelectedPose.gameObject.layer = 0;
                PoseSelectState = false;
                UISelectorDd.gameObject.SetActive(PoseSelectState);
                DestroyButton.gameObject.SetActive(!PoseSelectState);
                UISelect.gameObject.SetActive(PoseSelectState);
                SelectedPose.gameObject.SetActive(true);

                OriginalSize = SelectedPose.transform.localScale;
                OriginalRotation = SelectedPose.transform.rotation;

                ResetPoseScaler();
                ResetPoseRotator();
            }
        }
    }
    public void SetObjectTransform(Transform input, int CaptureCount)
    {
        ObjectPosition[CaptureCount] = input;
    }
    public void DestroySelectedPose()
    {
        Destroy(SelectedPose);
        PoseSelectState = true;
        DestroyButton.gameObject.SetActive(!PoseSelectState);
        UISelectorDd.gameObject.SetActive(PoseSelectState);
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
            //SelectedPose.transform.Translate(touch.deltaPosition + new Vector2 (SelectedPose.transform.position.x, SelectedPose.transform.position.y));
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
