using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerAnomation : MonoBehaviour
{
    private Animator anim = null;

    private FixedJoystick joyStick;

    float degStop;

    void Start()
    {
        GameObject parentObject = GameObject.Find("MyPlay");

        anim = parentObject.transform.GetChild(0).GetComponent<Animator>();
        
        joyStick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

    }

    void Update()
    {

        Move();

       
        // anim.SetBool("isDash", false);


        // anim.SetBool("isDash", true);

    }
    public  void Move()
    {

        float dx = joyStick.Horizontal; //joystickの水平方向の動きの値、-1~1の値をとります
        float dy = joyStick.Vertical; //joystickの垂直方向の動きの値、-1~1の値をとります

        float rad = Mathf.Atan2(dx - 0, dy - 0); //　 原点(0,0)と点（dx,dy)の距離から角度をとってくれる便利な関数

        float deg = rad * Mathf.Rad2Deg; //radianからdegreenに変換します

       

        if (deg != 0) //joystickの原点と(dx,dy)の２点がなす角度が０ではないとき = joystickを動かしている時
        {
            anim.SetBool("isDash", true); //wait→walkへ

            degStop = deg; //停止前のプレイヤーの向きを保存
        }
        else //joystickの原点と(dx,dy)の２点がなす角度が０の時 = joystickが止まっている時
        {
            anim.SetBool("isDash", false); //walk→waitへ

        }
    }
}
