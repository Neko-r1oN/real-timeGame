using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;



public class TitleManager : MonoBehaviour
{

    
    //[SerializeField] UserModel userModel;

    //���[�U�[ID���̓t�B�[���h(�f�o�b�O�p)
    [SerializeField] InputField debug;


    //���O�t�B�[���h
    [SerializeField] GameObject nameField;
    [SerializeField] Text nameText;

    //�o�^����
    [SerializeField] GameObject registTrue;
    [SerializeField] GameObject registFalse;

    //�{�^���֌W
    [SerializeField] GameObject dummyButton;
    [SerializeField] GameObject startButton;

    //����p�ϐ�
    private bool isClick;
    bool isSuccess;

    // Start is called before the first frame update
    void Start()
    {
        registTrue.SetActive(false);
        registFalse.SetActive(false);

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
            //���͗��ɕ��������͂���Ă���ꍇ
            if (nameText.text != "" || debug.text != "")
            {
                dummyButton.SetActive(false);
                startButton.SetActive(true);
            }
            //���L���̏ꍇ
            else
            {
                dummyButton.SetActive(true);
                startButton.SetActive(false);
            }
        }

        if (isClick) return;

        isClick = true;


    }

    public async void OnClickRegist()
    {
        if (debug.text != "")
        {
            bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));


            if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);

            else Debug.Log("�f�[�^���܂���ł���");
        }
        else
        {
            if (isSuccess)
            {//����o�^�p�̃{�^�����\��
                Initiate.Fade("GameScene", Color.black, 1.0f);
            }
        }
       /*
      
            if (debug.text != "")
            {
                bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));


                if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);

                else Debug.Log("�f�[�^���܂���ł���");
            }
            else
            {
                bool isRegist = await UserModel.Instance.RegistUserAsync(nameText.text);



                Debug.Log(isRegist);

                if (isRegist == true)
                {
                    registFalse.SetActive(false);
                    registTrue.SetActive(true);
                    Initiate.Fade("GameScene", Color.black, 1.0f);
                }
                else if (isRegist == false)
                {
                    registTrue.SetActive(false);
                    registFalse.SetActive(true);
                }

            }*/
        
    }
}
