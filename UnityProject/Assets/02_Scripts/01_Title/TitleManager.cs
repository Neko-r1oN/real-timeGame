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

    //ユーザーID入力フィールド(デバッグ用)
    [SerializeField] InputField debug;


    //名前フィールド
    [SerializeField] GameObject nameField;
    [SerializeField] Text nameText;

    //登録判定
    [SerializeField] GameObject registTrue;
    [SerializeField] GameObject registFalse;

    //ボタン関係
    [SerializeField] GameObject dummyButton;
    [SerializeField] GameObject startButton;

    //判定用変数
    private bool isClick;
    bool isSuccess;

    // Start is called before the first frame update
    void Start()
    {
        registTrue.SetActive(false);
        registFalse.SetActive(false);

        nameText.text = "";
        isClick = true;

        //ユーザーモデルを取得
        // userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //ローカルのユーザーデータ取得
        isSuccess = UserModel.Instance.LoadUserData();

        nameField.SetActive(true);

        dummyButton.SetActive(true);
        startButton.SetActive(false);

       

    }

    // Update is called once per frame
    void Update()
    {
        //端末にアカウント情報があった場合
        if (isSuccess)
        {//初回登録用のボタンを非表示

            dummyButton.SetActive(false);
            startButton.SetActive(true);
            nameField.SetActive(false);
        }
        //情報がなかった場合
        else
        {
            //入力欄に文字が入力されている場合
            if (nameText.text != "" || debug.text != "")
            {
                dummyButton.SetActive(false);
                startButton.SetActive(true);
            }
            //未記入の場合
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

            else Debug.Log("データ取れませんでした");
        }
        else
        {
            if (isSuccess)
            {//初回登録用のボタンを非表示
                Initiate.Fade("GameScene", Color.black, 1.0f);
            }
        }
       /*
      
            if (debug.text != "")
            {
                bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));


                if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);

                else Debug.Log("データ取れませんでした");
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
