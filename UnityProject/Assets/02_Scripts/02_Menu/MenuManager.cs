////////////////////////////////////////////////////////////////////////////
///
///  メニューUIマネージャースクリプト
///  Author : 川口京佑  2025.01/28
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
    public CinemachineVirtualCameraBase menuCam;     //メニュー画面カメラ
    public CinemachineVirtualCameraBase charaCam;    //キャラクター選択画面カメラ

    [SerializeField] private InputField roomName;     //ルーム名
    [SerializeField] private GameObject menuCanvas;   //メニューキャンバス
    [SerializeField] private GameObject optionCanvas; //設定キャンバス
    [SerializeField] private GameObject tutorialCanvas; //操作説明キャンバス
    [SerializeField] private GameObject roomMenu;     //ルームメニュー

    void Start()
    {
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;

        menuCanvas.SetActive(false);
        roomMenu.SetActive(false);
        optionCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
    }

    //設定ボタン用関数
    public void OnClickOption()
    {
        optionCanvas.SetActive(true);
    }
    //設定解除用関数
    public void OnClickOptionBack()
    {
        optionCanvas.SetActive(false);
    }
    //あそびかたボタン用関数
    public void OnClickTutorial()
    {
        tutorialCanvas.SetActive(true);
    }
    //あそびかた解除用関数
    public void OnClickTutorialBack()
    {
        tutorialCanvas.SetActive(false);
    }
    //キャラクター変更用関数
    public void OnClickCharaChange()
    {
        //カメラ優先度設定
        menuCam.Priority = 0;
        charaCam.Priority = 50;
    }
    //キャラクター変更解除用関数
    public void OnClickBack()
    { 
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
    //プライベートマッチボタン用関数
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
    //退出
    public void OnClickLeave()
    {
        Debug.Log("退出おした");
    }
    //バトル解除用変数
    public void OnClickButtleBack()
    {
        menuCanvas.SetActive(false);
        //カメラ優先度設定
        roomMenu.SetActive(false);
        //カメラ優先度設定
        menuCam.Priority = 50;
        charaCam.Priority = 0;
    }
}
