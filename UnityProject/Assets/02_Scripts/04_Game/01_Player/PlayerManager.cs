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

    private string tagName = "Enemy";          //インスペクターで変更可能

    private GameObject searchNearObj;          //最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
    private GameObject cursor;

    public bool isHaveBall { get; set; }       //ボールを所持しているか

    public bool isGround { get; set; }         //地面に触れているか
    public bool isClickJump;

    public bool isCatch { get; set; }

    public float velosity = 6.0f;

    public int animState { get; set; }

    GameObject ballPrefab;
    public float ballSpeed = 15.0f;
    

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 6.0f;        // 移動速度

    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;

    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;

    private GameObject closeEnemy;

    private void Start()
    {
        //ボールプレハブ取得
        ballPrefab = (GameObject)Resources.Load("Ball");

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

        //ジャンプボタン
        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();
        jumpButton.onClick.AddListener(() => OnClickJump());

        //キャッチボタン
        catchButton = GameObject.Find("CatchButton").GetComponent<Button>();
        catchButton.onClick.AddListener(() => OnClickCatch());

        //フェイントボタン
        feintButton = GameObject.Find("FeintButton").GetComponent<Button>();
        catchButton.onClick.AddListener(() => OnClickFeint());

        //投げるボタン
        throwButton = GameObject.Find("ThrowButton").GetComponent<Button>();
        throwButton.onClick.AddListener(() => OnClickThrow());


        catchbtn = GameObject.Find("CatchButton");

        //ルームモデルの取得
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();

        //プレイヤー移動を呼び出し、以降は0.02fごとに実行
        InvokeRepeating(nameof(SendData), 0.0f, 0.02f);
    }

    void Update()
    {
        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

       // Move();

        if (isHaveBall)
        {
            catchbtn.SetActive(false);
            
        }else if(!isHaveBall)
        {
            catchbtn.SetActive(true);
          
        }

        //キャッチ状態でない場合のみ移動できる
        if (isCatch != true)
        {
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
            else
            {
                animState=0;
            }

        }
        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = EnemySerch();

        if (searchNearObj)
        {
            transform.LookAt(searchNearObj.transform);
        }

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


   

        public void Move()
    {

        float dx = fixedJoystick.Horizontal; //joystickの水平方向の動きの値、-1~1の値をとります
        float dy = fixedJoystick.Vertical; //joystickの垂直方向の動きの値、-1~1の値をとります

        float rad = Mathf.Atan2(dx - 0, dy - 0); //　 原点(0,0)と点（dx,dy)の距離から角度をとってくれる便利な関数

        float deg = rad * Mathf.Rad2Deg; //radianからdegreenに変換します



        if (deg != 0) //joystickの原点と(dx,dy)の２点がなす角度が０ではないとき = joystickを動かしている時
        {
            animState = 1; //wait→walkへ

            //degStop = deg; //停止前のプレイヤーの向きを保存
        }
        else //joystickの原点と(dx,dy)の２点がなす角度が０の時 = joystickが止まっている時
        {
            animState = 0; //walk→waitへ

        }
    }

    public void OnClickJump()
    {
        animState = 2;
        isClickJump = true;
    }

    public void OnClickCatch()
    {
        isCatch = true;
        Debug.Log("キャッチ");
        animState = 3;
        //yield return new WaitForSeconds(1);

        isCatch = false;
        Debug.Log("キャッチ解除");

    }

    public void OnClickThrow()
    {
        //ジャンプ状態だったら
        if (isClickJump)
        {
            //ジャンプアニメーション
            animState = 4;
        }
        else
        {
            animState = 4;
        }
        Shot();
    }

    //フェイント(投げるふり)処理
    public void OnClickFeint()
    {
        animState = 4;
    }



    void Shot()
    {

        // shotObj.GetComponent<Rigidbody>().velocity = transform.forward * ballSpeed;

        if (isHaveBall)
        {

            GameObject newbullet = Instantiate(ballPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity); //弾を生成
            Rigidbody bulletRigidbody = newbullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (transform.forward * ballSpeed); //キャラクターが向いている方向に弾に力を加える
            //Destroy(newbullet, 10); //10秒後に弾を消す

            //ボール所持状態を解除する
            isHaveBall = false;
        }
        else
        {
            Debug.Log("玉持ってないよ");
        }
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

        if (other.gameObject.tag == "Ball")
        {
            //キャッチ状態でボールに触ったら
            if(isCatch)
            {
                //ボール所持状態にする
                isHaveBall = true;
                Destroy(other.gameObject);    //ボール削除
                Debug.Log("キャッチ成功");
            }
            //キャッチ状態じゃなかったら
            else
            {
                //ダウン処理
            }
        }
        if(other.gameObject.tag == "EasyBall")
        {
            //ボール所持状態にする
            isHaveBall = true;
            Destroy(other.gameObject);    //ボール削除
            Debug.Log("ゲット");
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
