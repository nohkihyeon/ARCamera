using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ObjectTouchEvent : MonoBehaviour
{
    [SerializeField]
    public Dropdown UISelectorDropdown;
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
    
    [SerializeField]
    [Tooltip("The Human Body text used for debugging purposes.")]
    public Text humanBodyText;
    
   
    void Start()
    {
        humanBodyText.gameObject.SetActive(true);
        ButtonOne.onClick.AddListener(ObjectOne);
        ButtonTwo.onClick.AddListener(ObjectTwo);
        ButtonThree.onClick.AddListener(ObjectThree);
        ButtonFour.onClick.AddListener(ObjectFour);
        ButtonFive.onClick.AddListener(ObjectFive);
        ButtonSix.onClick.AddListener(ObjectSix);
        ButtonSeven.onClick.AddListener(ObjectSeven);
        ButtonEight.onClick.AddListener(ObjectEight);
        ButtonNine.onClick.AddListener(ObjectNine);
        
        
    }
    void Update()
    {

    }

    private void ObjectOne()
    {
        Debug.Log("obj1");
        UISelectorDropdown.value = 1;
        humanBodyText.GetComponentInChildren<Text>().text = "obj1";
    }
    private void ObjectTwo()
    {
        UISelectorDropdown.value = 2;
        Debug.Log("obj2");
    }
    private void ObjectThree()
    {
        UISelectorDropdown.value = 3;
        Debug.Log("obj3");
    }
    private void ObjectFour()
    {
        UISelectorDropdown.value = 4;
        Debug.Log("obj4");
    }
    private void ObjectFive()
    {
        UISelectorDropdown.value = 5;
        Debug.Log("obj5");
    }
    private void ObjectSix()
    {
        UISelectorDropdown.value = 6;
        Debug.Log("obj6");
    }
    private void ObjectSeven()
    {
        UISelectorDropdown.value = 7;
        Debug.Log("obj7");
    }
    private void ObjectEight()
    {
        UISelectorDropdown.value = 8;
        Debug.Log("obj8");
    }
    private void ObjectNine()
    {
        UISelectorDropdown.value = 9;
        Debug.Log("obj9");
    }
}
