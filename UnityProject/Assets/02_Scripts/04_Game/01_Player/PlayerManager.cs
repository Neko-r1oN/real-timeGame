using DG.Tweening;
using MessagePack;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{

    RoomModel roomModel;
    GameDirector gameDirector;

    private string tagName = "Enemy";          //インスペクターで変更可能

    private GameObject searchNearObj;          //最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
    private GameObject cursor;

    public bool isHaveBall { get; set; }       //ボールを所持しているか
    public bool isGround { get; set; }         //地面に触れているか
    public bool isJump;                        //ジャンプしているか
    public bool isCatch { get; set; }          //キャッチ状態であるか
    public bool isDash { get; set; }           //ダッシュ状態であるか

    public float velosity = 13f;              //ジャンプの強さ

   
    GameObject ballPrefab;
    public float ballSpeed = 18.0f;
    

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 6.0f;        // 移動速度


    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;

    FixedJoystick fixedJoystick;
    Rigidbody rigidbody;


    //プレイヤーアニメーション
    PlayerAnimation playerAnim;
    private bool isLeft;     //画像の向き

    

    //HP変数
    public int maxHp = 5;    //HP最大値
    public int hp;           //HP現在値

    public int damage = 1;   //ダメージ量(現状固定)

    private void Start()
    {
        //画像の向き
        isLeft = false;

        //キャッチ状態
        isCatch = false;

        //HP設定
        hp = maxHp;

        //プレイヤー状態初期化
        isHaveBall = false;
        isDash = false;
        //玉所持情報初期化
        isHaveBall = false;

        isGround = false;
        isJump = false;

        //ボール発射用プレハブ取得
        ballPrefab = (GameObject)Resources.Load("Ball");

        //カーソルオブジェクトを取得
        cursor = GameObject.Find("Cursor");

        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = EnemySerch();

       
        

        
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

        //GameDirectorの取得
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        //playerAnimation取得
        //playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        //向き判定更新
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<AngleManager>().isLeft = this.isLeft;


        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal)*moveSpeed;

        move.y = rigidbody.velocity.y;

        rigidbody.velocity = move;

        Move();

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
            
                // プレイヤーの位置(transform.position)の更新
                // 移動方向ベクトル(velocity)を足し込みます
                transform.position += velocity;
               
            }
            else
            {
               // animState=0;
            }

        }

        if (roomModel.isMaster)
        {
            //一番近い敵の座標取得
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
                //カーソルを見えない位置に移動
                cursor.transform.DOMove(new Vector3(10.714f, -1.94f, 12.87f), 0.1f).SetEase(Ease.Linear);
            }
        }

        //地面についた状態で移動
        if(isDash && isGround)
        {
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DASH);
        }
        else
        {
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
        }

        //ジャンプ
        if (isJump && !isDash)
        {
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.JUMP);
        }
        
        //着地しているかを判定
        if (isGround == true)
        {
            //スペースキーが押されているかを判定
            if (isJump == true)
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


        //移動パッドが動かされている場合
        if (deg != 0)
        {
            isDash = true;
            
        }
        //移動パッドが触られていない状態
        else 
        {
            isDash = false;

            if (/*isDash &&*/ !isJump)
            {
                isDash = false;
            }
        }

        //右向きに
        if (deg > 0) isLeft = false;

        //左向きに
        else if (deg < 0) isLeft = true;

    }

    public void OnClickJump()
    {
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.JUMP);
        Debug.Log("ジャンプ");
        isDash = false;
        isJump = true;
    }

    public void OnClickCatch()
    {
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);

        isDash = false;
        isCatch = true;
        Debug.Log("キャッチ");
        
       

        isCatch = false;
        Debug.Log("キャッチ解除");

    }

    public void OnClickThrow()
    {
        //ジャンプ状態だったら
        if (isJump)
        {
            //ジャンプ投げアニメーション
           
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRTHROW);
        }
        else
        {
            //投げアニメーション
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.THROW);
        }

        Invoke("isThrowOut", 2.0f);
        Shot();
    }

    //フェイント(投げるふり)処理
    public  void OnClickFeint()
    {
        //フェイントアニメーション
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.THROW);
        
        Invoke("isThrowOut", 1.0f);
    }


    /// <summary>
    /// ボール発射処理
    /// </summary>
    void Shot()
    {
        if (isHaveBall)
        {
            //マスタークライアントに変更
            roomModel.isMaster = true;

            GameObject newbullet = Instantiate(ballPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity); //弾を生成
            Rigidbody bulletRigidbody = newbullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (transform.forward * ballSpeed); //キャラクターが向いている方向に弾に力を加える

            ThrowBall();

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
      
        //着地を検出したので着地状態を書き換え
        if (!isGround)
        {
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);

            isJump = false;
            isGround = true;
        }
      
        //クリア判定オブジェクト(デバッグ用)
        if (other.gameObject.tag == "Clear")
        {
            // 全ユーザーにゲーム終了通知
            FinishGame();
        }

        //ボールオブジェクト
        if (other.gameObject.tag == "Ball")
        {
            //キャッチ状態でボールに触ったら
            if (isCatch)
            {
                //ボール所持状態にする
                isHaveBall = true;
                Destroy(other.gameObject);    //ボール削除
                Debug.Log("キャッチ成功");

                GetBall();
            }
            //キャッチ状態じゃなかったら
            else
            {
                HitBall();

            }
            
        }

        //ダメージ性のない状態のボール
        if(other.gameObject.tag == "EasyBall")
        {
            //ボール所持状態にする
            isHaveBall = true;
            Destroy(other.gameObject);    //ボール削除
            Debug.Log("ゲット");

            //ボール獲得
            GetBall();
        }

    }

    //ボール取得処理
    private async void GetBall()
    {
        //ボール所持者変更
        gameDirector.getUserId = roomModel.ConnectionId;
        Debug.Log("ボール取得");

        //取得者のIDを通知
        await roomModel.GetBallAsync(roomModel.ConnectionId);
    }
    //ボール取得処理
    private async void ThrowBall()
    {
        //ボール情報
        var throwData = new ThrowData()
        {
            ConnectionId = roomModel.ConnectionId,            //接続ID
            ThorwPos = this.gameObject.transform.position,    //投げたプレイヤーの座標
            GoalPos = searchNearObj.transform.eulerAngles,    //目標座標

        };

        //ボール発射通知
        await roomModel.ThrowBallAsync(throwData);

    }


    private async void HitBall()
    {

        //自分が投げた球はreturn
        if (gameDirector.getUserId == roomModel.ConnectionId) {
            Debug.Log("自打球");
            return;
           
        }
        //自身のHP減少
        this.hp--;

        //HPがまだある場合
        if (hp > 0)
        {
            Debug.Log("ヒット");
           

            Debug.Log("残り体力:" + hp);

            //ダウン処理
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);

            var hitData = new HitData()
            {
                ConnectionId = roomModel.ConnectionId,              //当てられたユーザーのID
                EnemyId = gameDirector.enemyId,                     //当てたユーザーのID
                DamagedHP = this.hp,                                //当てられたユーザーのHP
                Point = GetPoint(),                                 //獲得ポイント
            };

            await roomModel.HitBallAsync(hitData);

        }
        //HPがなくなった場合
        else if (hp <= 0)
        {
            Debug.Log("デッティ");
            //死亡処理
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DEAD);
        }


       
    }

    private int GetPoint()
    {
        //当てた敵と自身の距離を取得
        float point = Vector3.Distance(searchNearObj.transform.position, this.gameObject.transform.position);

        //小数点以下切り上げ
        Mathf.Ceil(point);
        Debug.Log("得点:" + point);

        //整数化してreturn
        return (int)point;
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


    //アニメーションリセット処理
    private void isThrowOut()
    {
        //playerAnim.SetBool("isThrow", false);
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
    }

    public bool GetAngle()
    {
        return isLeft;
    }
}
