////////////////////////////////////////////////////////////////////////////
///
///  タイトル画面(ユーザー処理)スクリプト
///  Author : 川口京佑  2025.01/28
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

    [SerializeField] GameObject ConnnectText;     //接続中UI
    //判定用変数
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

        //ローカルのユーザーデータを取得
        isSuccess = UserModel.Instance.LoadUserData();

        nameField.SetActive(true);
        dummyButton.SetActive(true);
        startButton.SetActive(false);
    }

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
                if (nameText.text.Length < 1 || nameText.text.Length >= 8)
                {
                    dummyButton.SetActive(true);
                    startButton.SetActive(false);
                }
                //入力欄に文字が入力されている場合
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

    //画面タッチ関数
    public async void TapScreen()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
        );

        //デバッグIDが入力されていたら
        if (debug.text != "")
        {
            bool isGet = await UserModel.Instance.GetUserInfoAsync(int.Parse(debug.text));    //情報取得

            if (isGet) Initiate.Fade("GameScene", Color.black, 1.0f);
            else Debug.Log("データ取れませんでした");
        }
        else
        {
            //ファイルがローカルに保存されていたら
            if (isSuccess) Initiate.Fade("GameScene", Color.black, 1.0f);     //画面遷移
            else registUI.SetActive(true);     //初回登録UI表示
        }
    }

    //ユーザー登録処理
    public async void OnClickRegist()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
        );

        isConnecting = true;
        ConnnectText.SetActive(true);
        startButton.SetActive(false);

        //登録処理
        bool isRegist = await UserModel.Instance.RegistUserAsync(nameText.text);

        //登録成功した場合
        if (isRegist == true)
        {
            registUI.SetActive(false);
            Initiate.Fade("GameScene", Color.black, 1.0f);
        }
        //登録失敗した場合
        else if (isRegist == false)
        {
            startButton.SetActive(true);
            registFalse.SetActive(true);
            isConnecting = false;
            ConnnectText.SetActive(false);
        }
    }
}
