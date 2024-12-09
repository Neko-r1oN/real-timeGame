using DG.Tweening;
using Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    RoomModel roomModel;

    private string tagName = "Enemy";        // インスペクターで変更可能

    private GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
    private GameObject cursor;

    public bool isHaveBall;       //ボールを所持しているか

    public bool isGround { get; set; }         //地面に触れているか
    public bool isClickJump;
    public float velosity = 4.5f;

    public int animState { get; set; }

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 6.0f;        // 移動速度

    Button jumpButton;
    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;

    private GameObject closeEnemy;

    private void Start()
    {
        //カーソルオブジェクトを取得
        cursor = GameObject.Find("Cursor");

        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = EnemySerch();

       
        animState = 0;

        //玉所持情報初期化
        isHaveBall = false;

        isGround = false;
        isClickJump = false;

        rigidbody = GetComponent<Rigidbody>();
        fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();
        jumpButton.onClick.AddListener(() => OnClickJump());


       
        //ルームモデルの取得
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
        

    }

    void Update()
    {
        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

       
        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
        velocity = velocity.normalized * moveSpeed * Time.deltaTime;

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
            animState = 1;    //ダッシュ

            // プレイヤーの位置(transform.position)の更新
            // 移動方向ベクトル(velocity)を足し込みます
            transform.position += velocity;
        }


        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = EnemySerch();

        //フィールドに敵プレイヤーが存在している場合
        if (searchNearObj != null)
        {
            //カーソルを最も近い敵の座標に移動
            cursor.transform.DOMove(searchNearObj.gameObject.transform.position, 0.1f).SetEase(Ease.Linear);
        }
        //フィールドに敵プレイヤーが存在していない場合
        else
        {
            //カーソルを初期位置に移動
            cursor.transform.DOMove(new Vector3(10.714f, -1.94f,12.87f), 0.1f).SetEase(Ease.Linear);
        }

        //着地しているかを判定
        if (isGround == true)
        {
            //スペースキーが押されているかを判定
            if (isClickJump == true)
            {
                

                //ジャンプの方向を上向きのベクトルに設定
                Vector3 jump_vector = Vector3.up;
                //ジャンプの速度を計算
                Vector3 jump_velocity = jump_vector * velosity;

                //上向きの速度を設定
                rigidbody.velocity = jump_velocity;
                //地面から離れるので着地状態を書き換え
                isGround = false;
            }
        }

    }

    public void OnClickJump()
    {
        animState = 2;
        isClickJump = true;
    }
    void OnCollisionEnter(Collision other)
    {
        animState = 0;
        //着地を検出したので着地状態を書き換え
        isGround = true;
        isClickJump = false;

        if (other.gameObject.tag == "Clear")
        {
            // 全ユーザーにゲーム終了通知
            FinishGame();
        }

    }
    
    // ゲーム終了通知送信処理
    private async void FinishGame()
    {
        await roomModel.FinishGameAsync();
    }



    /// <summary>
    /// 指定されたタグの中で最も近いものを取得
    /// </summary>
    /// <returns></returns>
    private GameObject EnemySerch()
    {

        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;

        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;

        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objsから１つずつobj変数に取り出す
        foreach (GameObject obj in objs)
        {

            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistanceを更新
                nearDistance = distance;

                // searchTargetObjを更新
                searchTargetObj = obj;
            }
        }

        //最も近かったオブジェクトを返す
        return searchTargetObj;
    }

}
