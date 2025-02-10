////////////////////////////////////////////////////////////////////////////
///
///  �^�C�g�����(���[�U�[����)�X�N���v�g
///  Author : ������C  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using KanKikuchi.AudioManager;



public class TitleManager : MonoBehaviour
{
    //���[�U�[ID���̓t�B�[���h(�f�o�b�O�p)
    [SerializeField] InputField debug;

    //���O�t�B�[���h
    [SerializeField] GameObject nameField;    
    [SerializeField] Text nameText;

    //�o�^���
    [SerializeField] GameObject registUI;

    //�o�^����
    [SerializeField] GameObject registFalse;

    //�{�^���֌W
    [SerializeField] GameObject dummyButton;
    [SerializeField] GameObject startButton;

    [SerializeField] GameObject ConnnectText;     //�ڑ���UI
    //����p�ϐ�
    private bool isClick;
    bool isSuccess;
    bool isConnecting;
 
    void Start()
    {
        registUI.SetActive(false);
        isConnecting = false;

        ConnnectText.SetActive(false);

        nameText.text = "";
        isClick = true;

        //���[�J���̃��[�U�[�f�[�^���擾
        isSuccess = UserModel.Instance.LoadUserData();

        nameField.SetActive(true);
        dummyButton.SetActive(true);
        startButton.SetActive(false);
    }

    void Update()
    {
        //�[���ɃA�J�E���g��񂪂������ꍇ
        if (isSuccess)
        {//����o�^�p�̃{�^�����\��

            dummyButton.SetActive(false);
            startButton.SetActive(true);
            nameField.SetActive(false);
        }
        //��񂪂Ȃ������ꍇ
        else
        {
            if (!isConnecting)
            {
                //���L���̏ꍇ
                if (nameText.text.Length < 1 || nameText.text.Length >= 8)
                {
                    dummyButton.SetActive(true);
                    startButton.SetActive(false);
                }
                //���͗��ɕ��������͂���Ă���ꍇ
                else
                {
                    dummyButton.SetActive(false);
                    startButton.SetActive(true);
                } 
            }
        }
        if (isClick) return;
        isClick = true;
    }

    //��ʃ^�b�`�֐�
    public async void TapScreen()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //�Đ��������I�[�f�B�I�̃p�X
                  volumeRate: 1,                //���ʂ̔{��
                  delay: 0.0f,                     //�Đ������܂ł̒x������
                  pitch: 1,                     //�s�b�`
                  isLoop: false,                 //���[�v�Đ����邩
                  callback: null                //�Đ��I����̏���
        );

        //�f�o�b�OID�����͂���Ă�����
        if (debug.text != "")
        {
            bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));    //���擾

            if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);
            else Debug.Log("�f�[�^���܂���ł���");
        }
        else
        {
            //�t�@�C�������[�J���ɕۑ�����Ă�����
            if (isSuccess) Initiate.Fade("GameScene", Color.black, 1.0f);     //��ʑJ��
            else registUI.SetActive(true);     //����o�^UI�\��
        }
    }

    //���[�U�[�o�^����
    public async void OnClickRegist()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //�Đ��������I�[�f�B�I�̃p�X
                  volumeRate: 1,                //���ʂ̔{��
                  delay: 0.0f,                     //�Đ������܂ł̒x������
                  pitch: 1,                     //�s�b�`
                  isLoop: false,                 //���[�v�Đ����邩
                  callback: null                //�Đ��I����̏���
        );

        isConnecting = true;
        ConnnectText.SetActive(true);
        startButton.SetActive(false);

        //�o�^����
        bool isRegist = await UserModel.Instance.RegistUserAsync(nameText.text);

        //�o�^���������ꍇ
        if (isRegist == true)
        {
            registUI.SetActive(false);
            Initiate.Fade("GameScene", Color.black, 1.0f);
        }
        //�o�^���s�����ꍇ
        else if (isRegist == false)
        {
            startButton.SetActive(true);
            registFalse.SetActive(true);
            isConnecting = false;
            ConnnectText.SetActive(false);
        }
    }
}
