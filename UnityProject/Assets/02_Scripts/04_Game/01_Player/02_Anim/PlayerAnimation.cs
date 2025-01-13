using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator = null;

    private FixedJoystick joyStick;

    //private PlayerManager player;
    //private EnemyManager enemy;
  

    //アニメ状態
    public enum ANIM_STATE
    {
        IDLE = 0,             //停止中
        DASH = 5,                 //ダッシュ中
        JUMP = 10,                 //ジャンプ中
        CATCH = 15,                //キャッチ中
        THROW = 30,                //投げ中
        AIRTHROW = 35,             //空中投げ中
        DOWN = 80,                 //ダウン中
        DEAD = 90,                 //死亡中
    }

    //アニメステートを初期値に設定
    ANIM_STATE anim_State = ANIM_STATE.IDLE;

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();

    }

    void Update()
    {
        //自機だった場合
        if (GetComponentInParent<PlayerManager>())
        {
            //animator.SetInteger("animation", (int)ANIM_STATE.IDLE);
        }
        //自機だった場合
        if (GetComponentInParent<EnemyManager>()) 
        {

        }
    }

    /// <summary>
    /// アニメーションID取得
    /// </summary>
    /// <returns>アニメーションID</returns>
   public int GetAnimId()
    {
        return animator.GetInteger("animation");
    }

    public void SetAnim(ANIM_STATE animId)
    {
        animator.SetInteger("animation", (int)animId);
    }

    //敵アニメーション関数
    public void SetEnemyAnim(int id)
    {
        animator.SetInteger("animation", id);
    }
}
