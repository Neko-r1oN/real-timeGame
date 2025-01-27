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
    //ユーザーID入力フィールド(デバッグ用)
    [SerializeField] InputField debug;

    //名前フィールド
    [SerializeField] GameObject nameField;
    [SerializeField] Text nameText;

    //登録画面
    [SerializeField] GameObject registUI;

    //登録判定
    [SerializeField] GameObject registFalse;

    //ボタン関係
    [SerializeField] GameObject dummyButton;
    [SerializeField] GameObject startButton;

    [SerializeField] GameObject ConnnectText;
    //判定用変数
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

            if (!isConnecting)
            {
                //未記入の場合
                if (nameText.text.Length < 3 || nameText.text.Length >= 8)
                {
                    dummyButton.SetActive(true);
                    startButton.SetActive(false);
                }
                //入力欄に文字が入力されている場合
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
        //デバッグだったら
        if (debug.text != "")
        {
            bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));


            if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);

            else Debug.Log("データ取れませんでした");
        }
        
        else
        {
            //ファイルがローカルに保存されていたら
            if (isSuccess)
            {//タップでゲームシーン
                Initiate.Fade("GameScene", Color.black, 1.0f);
            }

            else
            {//登録UI表示
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
