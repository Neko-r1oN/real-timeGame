using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float speed;
    private GameObject[] targets;
    private bool isSwitch = false;

    private float[] PlayersPos;

    //アニメーション
    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 6.0f;        // 移動速度


    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;

    private GameObject closeEnemy;

    private void Start()
    {
        // タグを使って画面上の全ての敵の情報を取得
        targets = GameObject.FindGameObjectsWithTag("Player");

        rigidbody = GetComponent<Rigidbody>();
        fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        // 「初期値」の設定
        float closeDist = 1000;

        //ステージ上に存在するプレイヤーの情報を取得
        foreach (GameObject t in targets)
        {
            // コンソール画面での確認用コード
            //print(Vector3.Distance(transform.position, t.transform.position));

            // このオブジェクト（砲弾）と敵までの距離を計測
            float tDist = Vector3.Distance(transform.position, t.transform.position);

            // もしも「初期値」よりも「計測した敵までの距離」の方が近いならば、
            if (closeDist > tDist)
            {
                // 「closeDist」を「tDist（その敵までの距離）」に置き換える。
                // これを繰り返すことで、一番近い敵を見つけ出すことができる。
                closeDist = tDist;

                // 一番近い敵の情報をcloseEnemyという変数に格納する（★）
                closeEnemy = t;
            }
        }

        // 砲弾が生成されて0.5秒後に、一番近い敵に向かって移動を開始する。
        Invoke("SwitchOn", 0.5f);
    }

    void Update()
    {
        if (isSwitch)
        {
            float step = speed * Time.deltaTime;

            // ★で得られたcloseEnemyを目的地として設定する。
            //transform.position = Vector3.MoveTowards(transform.position, closeEnemy.transform.position, step);
        }

        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

        /*float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x == 0)
        {
            direction = DIRECTION_TYPE.STOP;
        }else if(x > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;
        }else if(x< 0)
        {
            direction= DIRECTION_TYPE.LEFT;
        }*/

        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            // プレイヤーの位置(transform.position)の更新
            // 移動方向ベクトル(velocity)を足し込みます
            transform.position += velocity;
        }
    }

    private void FixedUpdate()
    {
        /*switch (direction)
        {
            case DIRECTION_TYPE.STOP:

                break;

            case DIRECTION_TYPE.RIGHT:

                break;

            case DIRECTION_TYPE.LEFT:

                break;
        }
        rigidbody.velocity = new Vector3(speed,rigidbody.velocity.y,rigidbody.velocity.x);*/
    }

    void SwitchOn()
    {
        isSwitch = true;
    }
}
