using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase charaCam;

    [SerializeField] GameObject roomMenu;
    // Start is called before the first frame update
    void Start()
    {
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;

        roomMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCharaChange()
    {
        //カメラ優先度設定
        menuCam.Priority = 0;
        charaCam.Priority = 50;
    }
    public void OnClickBack()
    {
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }

    public void OnClickButtle()
    {
        //カメラ優先度設定
        roomMenu.SetActive(true);
        menuCam.Priority = 0;
        charaCam.Priority = 0;
    }

    public void OnClickButtleBack()
    {
        //カメラ優先度設定
        roomMenu.SetActive(false);
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
