using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ObjectTouchEvent : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The Human Body text used for debugging purposes.")]
    public Text humanBodyText;
    private int layerMask;
    
   
    void Start()
    {
        // 레이어 인덱스가 8인 레이어마스크 변수에 대입
        // 8만큼 Bit Shift
        humanBodyText.gameObject.SetActive(true);
        layerMask = 1 << 8;
        
    }
    // public static Camera MainCamera;
    // Update is called once per frame d
    void Update()
    {
        ClickDetect();
    }

    private void ClickDetect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("GetMouseButton0");
            
            Debug.Log("Camera " + Camera.main.gameObject);
            humanBodyText.GetComponentInChildren<Text>().text = "클릭 되기 전 \n충돌체 이름 : "  + "\n충돌거리 : " ;
           
            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.Log(Physics.Raycast(ray, out hit_info, Mathf.Infinity));
            // Debug.Log(ray);
             
            if (Physics.Raycast(ray, out hit_info, Mathf.Infinity))
            {
                Debug.Log("충돌체 이름 : " + hit_info.collider.gameObject.name);
                Debug.Log("충돌거리 : " + hit_info.distance);
                Debug.Log("법선벡터 : " + hit_info.normal);
                humanBodyText.GetComponentInChildren<Text>().text = "충돌체 이름 : " + hit_info.collider.gameObject.name + "\n충돌거리 : " + hit_info.distance + "\n법선벡터 : " + hit_info.normal;
            }
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
            
        }
    }
}
