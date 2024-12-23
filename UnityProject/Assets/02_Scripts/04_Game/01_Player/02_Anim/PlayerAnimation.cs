using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    private string animState;   //アニメステート
    private bool animJudge;     //アニメ判定

    private PlayerManager player;
    private EnemyManager enemy;
  
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    float degStop;


    void Start()
    {
        //GameObject parentObject = GameObject.Find("MyPlay");

        anim = this.gameObject.GetComponent<Animator>();

        player = GetComponentInParent<PlayerManager>();//一番上の親についているコンポーネントを取得する

        //
        if(player == null)
        {
            enemy = GetComponentInParent<EnemyManager>();//一番上の親についているコンポーネントを取得する

        }


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

      
    }
        
   

    public void SetTrigger()
    {

        anim.SetTrigger(animState); //wait→walkへ
    }
    public void SetBool()
    {

        anim.SetBool(animState,true); //wait→walkへ
    }

     
    public void ResetJump()
    {
        anim.SetBool("isJump",false);
    }
    public void ResetAnim()
    {
        anim.SetBool(animState,false);
        Debug.Log("キャンセル呼ばれた");

        Debug.Log(animState);

    }

   public int GetAnimId()
    {
        return 0;

    }
    public void SetAnim(int animName)
    {

    }
}
