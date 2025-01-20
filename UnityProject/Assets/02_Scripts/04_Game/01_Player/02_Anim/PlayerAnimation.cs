using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator = null;

    private FixedJoystick joyStick;

    public bool isLeft;

    //private PlayerManager player;
    //private EnemyManager enemy;
  

    //アニメ状態
    public enum ANIM_STATE
    {
        IDLE = 0,                  //待機中
        HAVE_IDLE = 1,             //待機中(所持)
        DASH = 5,                  //ダッシュ中
        HAVE_DASH = 6,             //ダッシュ中(所持)
        JUMP = 10,                 //ジャンプ中
        HAVE_JUMP = 11,            //ジャンプ中(所持)
        CATCH = 15,                //キャッチ中
        HAVE_CATCH = 16,           //キャッチ中(所持)
        FEINT = 30,                //投げる(フェイント)
        THROW = 31,                //投げる(所持)
        AIRTHROW = 35,             //空中投げ中
        AIRFEINT = 36,             //空中投げ中(フェイント)
        DOWN = 80,                 //ダウン中
        DEAD = 90,                 //死亡中
    }

    //アニメステートを初期値に設定
    ANIM_STATE anim_State = ANIM_STATE.IDLE;

    //コンポーネント付与処理
    public void Init()
    {
        animator = this.gameObject.GetComponent<Animator>();

        isLeft = false;
    }

    void Update()
    {
        //キャラクターの向き切り替え
        if (isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = false;

        }
        if (!isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = true;
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

    //敵アニメーション反映関数
    public void SetEnemyAnim(int id)
    {

        animator.SetInteger("animation", id);
    }

    //敵の向き反映関数
    public bool GetAngle()
    {
        return isLeft;
    }
}
