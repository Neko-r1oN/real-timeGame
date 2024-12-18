using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnomation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    private string animState;

    private PlayerManager player;
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    float degStop;

    void Start()
    {
        //GameObject parentObject = GameObject.Find("MyPlay");

        anim = this.gameObject.GetComponent<Animator>();

        player = GetComponentInParent<PlayerManager>();//一番上の親についているコンポーネントを取得する


        joyStick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();


       
    }

    void Update()
    {

        //Move();

        switch (player.animState)
        {
            case 0://アイドル状態

                //anim.SetBool(animState, false); //アニメーション停止
                break;
            case 1:    //ダッシュ状態
                
                animState = "isDash";
                break;
            case 2:    //ジャンプ状態
                animState = "isJump";

                break;
            case 3:    //キャッチ
                animState = "isCatch";
                break;
            case 4:    //投げる
                animState = "isThrow"; //wait→walkへ
                break;
            case 5:   //ダウン
                animState = "isDown";
                break;
            default:

                animState = "";
                break;
        }

        if(animState != null)
        {
            if (player.animState == 2)
            {
                SetTrigger();
            }
            else
            {
                SetBool();
            }
        }
    }
        
   

    public void SetTrigger()
    {

        anim.SetTrigger(animState); //wait→walkへ
    }
    public void SetBool()
    {

        anim.SetBool(animState,true); //wait→walkへ
    }
}
