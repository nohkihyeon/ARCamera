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
    public AudioSource SutterSound;
    bool State;

    //public AudioClip CameraSound;

    private Camera Camera
    {
        get
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }

    [SerializeField]
    private Camera _camera;
    void Awake()
    {
        TakePhotoButton.onClick.AddListener(PhotoFunc);
    }
    private void Start()
    {
        State = false;
    }
    private void Update()
    {
        if (State)
        {
            ActiveObject();
        }    
    }

    private void PhotoFunc()
    {
        bool Identify = GameObject.Find("Canvas").GetComponent<UISelector>().UIStateInfor;
        if (State == false)
        {

            if (Identify)
                GameObject.Find("Canvas").GetComponent<UISelector>().SelectedPose.SetActive(State);
            State = !State;
            TakePhoto();
        }
    }

    private void ActiveObject()
    {
        GameObject.Find("Canvas").GetComponent<UISelector>().SelectedPose.SetActive(State);
        State = !State;
    }

    public void TakePhoto()
    {
        Debug.Log("TakePhoto()" + fileCounter);
        Canvas.SetActive(false);
        Invoke("Screenshot", 0.4f);
    }

    public void setActiveTrue()
    {
        Canvas.SetActive(true);
        GameObject.Find("AlertPanel").GetComponent<AlertMessageController>().popUpMessage("자동촬영 되었습니다!");

    }
    
    public void Screenshot()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string name = "AR Camera" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") +fileCounter + ".png";
        NativeGallery.SaveImageToGallery(bytes, "AR Camera", name);
        fileCounter++;
        Destroy(texture);

        SutterSound.Play();

        Invoke("setActiveTrue", 1f);
        Debug.Log("Canvas True");
    }
}
