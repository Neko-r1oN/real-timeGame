using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase charaCam;

    [SerializeField] GameObject roomMenu;

    [SerializeField] GameObject controller;
    // Start is called before the first frame update
    void Start()
    {
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;

        roomMenu.SetActive(false);
        controller.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCharaChange()
    {
        //�J�����D��x�ݒ�
        menuCam.Priority = 0;
        charaCam.Priority = 50;
    }
    public void OnClickBack()
    {
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }

    public void OnClickButtle()
    {
        //�J�����D��x�ݒ�
        roomMenu.SetActive(true);
        controller.SetActive(true);
        menuCam.Priority = 0;
        charaCam.Priority = 0;
    }

    public void OnClickButtleBack()
    {
        //�J�����D��x�ݒ�
        controller.SetActive(false);
        roomMenu.SetActive(false);
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
