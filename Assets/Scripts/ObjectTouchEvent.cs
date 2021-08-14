using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ObjectTouchEvent : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The Human Body text used for debugging purposes.")]
    public Text humanBodyText;

    void Start()
    {
        humanBodyText.gameObject.SetActive(true);
    }
    // public static Camera MainCamera;
    // Update is called once per frame
    void Update()
    {
        ClickDetect();
    }

    private void ClickDetect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GetMouseButton0");
            RaycastHit hit_info;
            Debug.Log("Camera " + Camera.main.gameObject);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit_info, Mathf.Infinity))
            {
                Debug.Log("충돌체 이름 : " + hit_info.collider.gameObject.name);
                Debug.Log("충돌거리 : " + hit_info.distance);
                Debug.Log("법선벡터 : " + hit_info.normal);
                humanBodyText.GetComponentInChildren<Text>().text = "충돌체 이름 : " + hit_info.collider.gameObject.name + "\n충돌거리 : " + hit_info.distance + "\n법선벡터 : " + hit_info.normal;
            }
        }
    }
}
