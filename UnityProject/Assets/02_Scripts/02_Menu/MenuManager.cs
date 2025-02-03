////////////////////////////////////////////////////////////////////////////
///
///  ���j���[UI�}�l�[�W���[�X�N���v�g
///  Author : ������C  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using Cinemachine;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase menuCam;     //���j���[��ʃJ����
    public CinemachineVirtualCameraBase charaCam;    //�L�����N�^�[�I����ʃJ����

    [SerializeField] private InputField roomName;     //���[����
    [SerializeField] private GameObject menuCanvas;   //���j���[�L�����o�X
    [SerializeField] private GameObject optionCanvas; //�ݒ�L�����o�X
    [SerializeField] private GameObject tutorialCanvas; //��������L�����o�X
    [SerializeField] private GameObject roomMenu;     //���[�����j���[

    void Start()
    {
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;

        menuCanvas.SetActive(false);
        roomMenu.SetActive(false);
        optionCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
    }

    //�ݒ�{�^���p�֐�
    public void OnClickOption()
    {
        optionCanvas.SetActive(true);
    }
    //�ݒ�����p�֐�
    public void OnClickOptionBack()
    {
        optionCanvas.SetActive(false);
    }
    //�����т����{�^���p�֐�
    public void OnClickTutorial()
    {
        tutorialCanvas.SetActive(true);
    }
    //�����т��������p�֐�
    public void OnClickTutorialBack()
    {
        tutorialCanvas.SetActive(false);
    }
    //�L�����N�^�[�ύX�p�֐�
    public void OnClickCharaChange()
    {
        //�J�����D��x�ݒ�
        menuCam.Priority = 0;
        charaCam.Priority = 50;
    }
    //�L�����N�^�[�ύX�����p�֐�
    public void OnClickBack()
    { 
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
    //�v���C�x�[�g�}�b�`�{�^���p�֐�
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
    //�ޏo
    public void OnClickLeave()
    {
        Debug.Log("�ޏo������");
    }
    //�o�g�������p�ϐ�
    public void OnClickButtleBack()
    {
        menuCanvas.SetActive(false);
        //�J�����D��x�ݒ�
        roomMenu.SetActive(false);
        //�J�����D��x�ݒ�
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
