using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class CameraCapture : MonoBehaviour
{
    [SerializeField]
    private Button TakePhotoButton;
    [SerializeField]
    private GameObject Canvas;
    public int fileCounter;
    public KeyCode screenshotKey;

    private Camera Camera
    {
        get
        {
            if (!_camera)
            {
                // _camera = Camera.main;
                _camera = Camera.main;
            }
            return _camera;
        }
    }

    [SerializeField]
    private Camera _camera;
    void Awake()
    {
        TakePhotoButton.onClick.AddListener(TakePhoto);
    }

    private void TakePhoto()
    {
        Debug.Log("TakePhoto()" + fileCounter);
        // Capture();
        Canvas.SetActive(false);
        Screenshot();
    }
    
    private void Screenshot()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string name = "AR Camera" + System.DateTime.Now.ToString("yyyy-mm-dd_HH-mm-ss") +fileCounter + ".png";
        NativeGallery.SaveImageToGallery(bytes, "AR Camera", name);
        fileCounter++;
        Destroy(texture);
        Canvas.SetActive(true);
        Debug.Log("Canvas True");
    }
}