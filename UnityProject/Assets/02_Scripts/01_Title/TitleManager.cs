using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;



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

    [SerializeField] GameObject ConnnectText;
    //����p�ϐ�
    private bool isClick;
    bool isSuccess;
    bool isConnecting;
    // Start is called before the first frame update
    void Start()
    {
        registUI.SetActive(false);
        isConnecting = false;

        ConnnectText.SetActive(false);

        nameText.text = "";
        isClick = true;

        //���[�U�[���f�����擾
        // userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //���[�J���̃��[�U�[�f�[�^�擾
        isSuccess = UserModel.Instance.LoadUserData();

        nameField.SetActive(true);

        dummyButton.SetActive(true);
        startButton.SetActive(false);

       

    }

    // Update is called once per frame
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
                if (nameText.text.Length < 3 || nameText.text.Length >= 8)
                {
                    dummyButton.SetActive(true);
                    startButton.SetActive(false);
                }
                //���͗��ɕ��������͂���Ă���ꍇ
                else/* if (nameText.text != "" || debug.text != "")*/
                {
                    dummyButton.SetActive(false);
                    startButton.SetActive(true);
                }
                
            }
        }

        if (isClick) return;

        isClick = true;


       

    }
    public async void TapScreen()
    {
        //�f�o�b�O��������
        if (debug.text != "")
        {
            bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));


            if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);

            else Debug.Log("�f�[�^���܂���ł���");
        }
        
        else
        {
            //�t�@�C�������[�J���ɕۑ�����Ă�����
            if (isSuccess)
            {//�^�b�v�ŃQ�[���V�[��
                Initiate.Fade("GameScene", Color.black, 1.0f);
            }

            else
            {//�o�^UI�\��
                registUI.SetActive(true);
            }
        }
    }
    public async void OnClickRegist()
    {
        isConnecting = true;
        ConnnectText.SetActive(true);
        startButton.SetActive(false);
        bool isRegist = await UserModel.Instance.RegistUserAsync(nameText.text);



        Debug.Log(isRegist);

        if (isRegist == true)
        {
            registUI.SetActive(false);
            Initiate.Fade("GameScene", Color.black, 1.0f);
        }
        else if (isRegist == false)
        {
            startButton.SetActive(true);
            registFalse.SetActive(true);
            isConnecting = false;
            ConnnectText.SetActive(false);
        }
    }
}
