using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
 
 public class RenderTextureRaycast : MonoBehaviour, IPointerClickHandler
 {
     [SerializeField] protected Camera UICamera;
     [SerializeField] protected RectTransform RawImageRectTrans;
     [SerializeField] protected Camera RenderToTextureCamera;
     
     public void OnPointerClick(PointerEventData eventData)
     {
         Debug.Log("OnPointerClick");
         Vector2 localPoint;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImageRectTrans, eventData.position, UICamera, out localPoint);
         Vector2 normalizedPoint = Rect.PointToNormalized(RawImageRectTrans.rect, localPoint);
         var renderRay = RenderToTextureCamera.ViewportPointToRay(normalizedPoint);
         if (Physics.Raycast(renderRay, out var raycastHit))
         {
             Debug.Log("Hit: " + raycastHit.collider.gameObject.name);
         }
         else
         {
             Debug.Log("No hit object");
         }
          Debug.DrawRay(renderRay.origin, renderRay.direction * 1000f, Color.blue);
     }
 }