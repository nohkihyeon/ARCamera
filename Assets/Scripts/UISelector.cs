using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject test1;
    GameObject test2;

    [SerializeField]
    private Button CaptureButton;

    //public Text TestText1;
    int number;
    bool PoseSelectState;
    // Start is called before the first frame update
    void Start()
    {
        InitDropDown();
    }
    private void Awake()
    {
        UISelectorDd.onValueChanged.AddListener(SelectPoseFunc);
        DestroyButton.onClick.AddListener(DestroySelectedPose);
        CaptureButton.onClick.AddListener(AddFunc);
    }

    // Update is called once per frame
    void Update()
    {
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
                //SelectedPose.transform.position = ObjectPosition[index].position;
                SelectedPose.transform.position -= SelectedPose.transform.position;
                SelectedPose.transform.localScale = SelectedPose.transform.localScale - new Vector3(200, 200, 200);
                //TestText2.text = ObjectPosition[index].position.ToString() + " //// "+ SelectedPose.transform.position.ToString() + SelectedPose.transform.localScale.ToString();
                SelectedPose.gameObject.layer = 0;
                //SelectedPose.transform.position = new Vector3(0,0,0);
                //SelectedPose.transform.position = ObjectPosition[index].position;
                //SelectedPose.transform.localScale = new Vector3(1, 1, 1);
                //SelectedPose.transform.localScale = ObjectPosition[index].localScale;
                test2 = Instantiate(test1, ObjectPosition[index]);
                PoseSelectState = false;
                UISelectorDd.gameObject.SetActive(PoseSelectState);
                DestroyButton.gameObject.SetActive(!PoseSelectState);
                UISelect.gameObject.SetActive(PoseSelectState);
                SelectedPose.gameObject.SetActive(true);
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
    //public void AddFunc(GameObject input)
    //{
    //    if (number < 10)
    //    {
    //        PoseList[number] = input;
    //        PoseList[number].gameObject.layer = 0;
    //        OpList = new Dropdown.OptionData();
    //        OpList.text = number.ToString();
    //        UISelectorDd.options.Add(OpList);
    //        number++;
    //    }
    //}
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
}
