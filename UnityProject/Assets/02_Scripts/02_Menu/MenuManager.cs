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
        //�J�����D��x�ݒ�
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
        menuCanvas.SetActive(true);

        //�J�����D��x�ݒ�
        roomMenu.SetActive(true);
        roomName.text = "lobby";     //�f�t�H���g�̓��r�[�s
        
        //controller.SetActive(true);
        menuCam.Priority = 0;
        charaCam.Priority = 0;
    }

    public void OnClickLeave()
    {
        Debug.Log("�ޏo������");
        //standbyUI.SetActive(false);
    }

    public void OnClickButtleBack()
    {
        menuCanvas.SetActive(false);
        //�J�����D��x�ݒ�
        // controller.SetActive(false);
        roomMenu.SetActive(false);
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
