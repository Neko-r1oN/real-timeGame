using Cinemachine;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase charaCam;

    [SerializeField] InputField roomName;

    [SerializeField] GameObject menuCanvas;

    [SerializeField] GameObject optionCanvas;

    [SerializeField] GameObject roomMenu;

    [SerializeField] GameObject controller;

    [SerializeField] GameObject standbyUI;
    // Start is called before the first frame update
    void Start()
    {
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;

        menuCanvas.SetActive(false);
        roomMenu.SetActive(false);
        //controller.SetActive(false);
        optionCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickOption()
    {
        optionCanvas.SetActive(true);
    }
    public void OnClickOptionBack()
    {
        optionCanvas.SetActive(false);
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
        menuCanvas.SetActive(true);

        //カメラ優先度設定
        roomMenu.SetActive(true);
        roomName.text = "lobby";     //デフォルトはロビー行
        
        //controller.SetActive(true);
        menuCam.Priority = 0;
        charaCam.Priority = 0;
    }

    public void OnClickLeave()
    {
        Debug.Log("退出おした");
        //standbyUI.SetActive(false);
    }

    public void OnClickButtleBack()
    {
        menuCanvas.SetActive(false);
        //カメラ優先度設定
        // controller.SetActive(false);
        roomMenu.SetActive(false);
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
