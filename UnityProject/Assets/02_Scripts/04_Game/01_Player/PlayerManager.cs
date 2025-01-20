using DG.Tweening;
using MessagePack;
using MessagePack.Resolvers;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerManager : MonoBehaviour
{

    RoomModel roomModel;
    GameDirector gameDirector;
    UserModel userModel;

    private int MAX_PLAYER = 4;

    //カーソル関連
    private string enemyTag = "Enemy";           //取得タグ名(敵)
    private GameObject searchNearEnemy;          //最も近い敵座標
    private GameObject cursor;

    //カーソル関連
    private string ballTag = "EasyBall";           //取得タグ名(ボール)
    private string enemyBallTag = "BallPlayer";     //取得タグ名(ボール所持者)
    private GameObject searchNearBall;          //最も近い敵座標

    //プレイヤー状態関連
    public bool isHaveBall;       //ボールを所持しているか
    public bool isGround;         //地面に触れているか
    public bool isJump;           //ジャンプしているか
    public bool isCatch;          //キャッチ状態であるか
    public bool isDash;           //ダッシュ状態であるか
    public bool isThrow;          //投げ状態であるか
    public bool isFeint;

    public float velosity = 13f;              //ジャンプの強さ
    public float ballSpeed = 18.0f;           //ボールの速さ
    public float knockBack = 4.0f;
    public float catchDelay = 0.6f;
    public float throwDelay = 0.6f;
    GameObject ballPrefab;



    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 6.0f;        // 移動速度

    //UI関連
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;    //表示切替用

    FixedJoystick fixedJoystick;    //JoyStick
    Rigidbody rigidbody;


    //プレイヤーアニメーション
    PlayerAnimation playerAnim;
    private bool isLeft;     //画像の向き

    //HP変数
    public int maxHp = 5;    //HP最大値
    public int hp;           //HP現在値

    public int damage = 1;   //ダメージ量(現状固定)

    //リザルトスコア用
    private int throwNum;
    public int hitNum;
    private int catchNum;
    private bool isLast;     //最後まで生き残
    private bool isDead;

    private bool isCalledOnce;

    private void Start()
    {

          
        //ユーザーモデルを取得
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //HP設定
        hp = maxHp;

        //プレイヤー状態
        isCatch = false;      //キャッチ状態
        isLeft = false;       //向いている方向
        isHaveBall = false;   //ボール所持状態
        isDash = false;       //ダッシュ状態
        isGround = false;     //接地状態
        isJump = false;       //ジャンプ状態
        isThrow = false;      //投げ状態
        isFeint = false;

        //ボール発射用プレハブ取得
        ballPrefab = (GameObject)Resources.Load("Ball");

        //カーソルオブジェクトを取得
        cursor = GameObject.Find("Cursor");

        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearEnemy = EnemySerch(enemyTag);

        //ボールタグ取得
        searchNearBall = EnemySerch(ballTag);
        Debug.Log(searchNearBall);
        if(searchNearBall == null)
        {
            searchNearBall = EnemySerch(enemyBallTag);
            Debug.Log(searchNearBall);
        }

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
        feintButton.onClick.AddListener(() => OnClickFeint());

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

        throwNum = 0;
        hitNum = 0;
        catchNum = 0;

        //ボールタグ取得
        searchNearBall = EnemySerch(ballTag);
        if (searchNearBall == null)
        {
            //ボール所持者プレイヤータグ取得
            searchNearBall = EnemySerch(enemyBallTag);

        }

        //ボールorボール所持者が取得できた場合
        if (searchNearBall)
        {
            if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x)
            {
                isLeft = true;
            }
            else
            {
                isLeft = false;
            }
        }
        else
        {
            isLeft = true;
        }


        isLast = true;

        isCalledOnce = false;
        isDead = false;
    }

    void Update()
    {
        //HPがなくなった場合
        if (!isDead && hp <= 0)
        {
            isLast = false;
            Debug.Log("デッティ");
            //死亡処理
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DEAD);

            //操作不能
            Destroy(fixedJoystick);
            //プレイヤー死亡通知
            DeadUser();
            isDead = true;
            isDead = true;

        }
        
        //最後の一人になった場合
        if (!gameDirector.isDead && gameDirector.deadNum >= MAX_PLAYER-1 && this.isLast)
        {
            Debug.Log("全員しんだからじぶんもしぬ");
            if (!isCalledOnce)
            {
                DeadUser();
                isCalledOnce = true;
            }
        }

        //一番近い敵の座標取得
        searchNearEnemy = EnemySerch(enemyTag);

        //ボールタグ取得
        searchNearBall = EnemySerch(ballTag);
        //ボールが無かったら
        if (searchNearBall = null)
        {
            //ボール所持者を取得
            searchNearBall = EnemySerch(enemyBallTag);
        }

        //移動処理
        if (!isCatch)
        {
            Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal) * moveSpeed;

            move.y = rigidbody.velocity.y;

            rigidbody.velocity = move;
        }

        //向きチェック
        Move();

        //向き判定更新
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<AngleManager>().isLeft = this.isLeft;

        if (isHaveBall)
        {
            catchbtn.SetActive(false);

        } else if (!isHaveBall)
        {
            catchbtn.SetActive(true);

        }

        //キャッチ状態か投げ状態でない場合のみ移動できる
        if (!isCatch || !isThrow)
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

            }

        }

        //自分から一番近い敵を取得
        if (searchNearEnemy)
        {
            transform.LookAt(searchNearEnemy.transform);
        }

        //フィールドに敵プレイヤーが存在している場合
        if (searchNearEnemy != null)
        {
            cursor.SetActive(true);
            //カーソルを最も近い敵の座標に移動
            cursor.transform.DOMove(searchNearEnemy.gameObject.transform.position, 0.1f).SetEase(Ease.Linear);
        }
        //フィールドに敵プレイヤーが存在していない場合
        else
        {
            cursor.SetActive(false);
            //カーソルを見えない位置に移動
            cursor.transform.DOMove(new Vector3(10.714f, -1.94f, 12.87f), 0.1f).SetEase(Ease.Linear);
        }


        if (roomModel.isMaster)
        {
            //ロックオンカーソル送信

           
        }
        
        if (isThrow)
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
        }

        else
        {
            if (isFeint)
            {
                //ジャンプ状態だったら
                if (isJump)
                {
                    isJump = false;
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRFEINT);
                }
                else
                {
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);
                }
                
            }
            else
            {
                //接地かつジャンプ状態でないときのみ移動
                if (isDash && isGround && !isJump)
                {
                    //ダッシュアニメーション
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_DASH);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DASH);

                }
                else if (isCatch)
                {
                    //キャッチアニメーション
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
                }
                else
                {
                    //アイドル(待機)アニメーション
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_IDLE);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
                }

                //ジャンプ
                if (isJump && !isDash)
                {
                    if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_JUMP);

                    else if (!isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.JUMP);
                }
            }
        }
      

       

        //接地しているか判定
        if (isGround == true)
        {
            //ジャンプボタンが押されたか
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



    /// <summary>
    /// ダッシュアニメーションチェック
    /// </summary>
    public void Move()
    {
        //死んでいない場合
        if (!isDead)
        {
            //キャッチ状態でない場合
            if (!isCatch)
            {
                float dx = fixedJoystick.Horizontal; //joystickの水平方向の動きの値、-1~1の値をとります
                float dy = fixedJoystick.Vertical; //joystickの垂直方向の動きの値、-1~1の値をとります

                float rad = Mathf.Atan2(dx - 0, dy - 0); //　 原点(0,0)と点（dx,dy)の距離から角度をとってくれる便利な関数

                float deg = rad * Mathf.Rad2Deg; //radianからdegreenに変換します


                //移動パッドが動かされている場合
                if (deg != 0)
                {
                    isDash = true;

                    //右向きに
                    if (deg > 0) isLeft = false;

                    //左向きに
                    else if (deg < 0) isLeft = true;

                }
                //移動パッドが触られていない状態
                else
                {
                    isDash = false;

                    if (/*isDash &&*/ !isJump)
                    {
                        isDash = false;
                    }

                    //ボールタグ取得
                    searchNearBall = EnemySerch(ballTag);
                    if (searchNearBall == null)
                    {
                        //ボール所持者プレイヤータグ取得
                        searchNearBall = EnemySerch(enemyBallTag);

                    }

                    //ボールorボール所持者が取得できた場合
                    if (searchNearBall)
                    {
                        if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x)
                        {
                            //Debug.Log("ボールより右に居る");
                            isLeft = true;
                        }
                        else
                        {
                            //Debug.Log("ボールより左に居る");
                            isLeft = false;
                        }
                    }
                }
                
            }
        }

    }

    public void OnClickJump()
    {
        SEManager.Instance.Play(
           audioPath: SEPath.JUMP,      //再生したいオーディオのパス
           volumeRate: 1,                //音量の倍率
           delay: 0,                     //再生されるまでの遅延時間
           pitch: 1,                     //ピッチ
           isLoop: false,                 //ループ再生するか
           callback: null                //再生終了後の処理
       );
        
        Debug.Log("ジャンプ");
        isDash = false;
        isJump = true;
    }

    public void OnClickCatch()
    {
        isDash = false;
        isCatch = true;

        //キャッチアニメーション
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
        Debug.Log("キャッチ");


        //キャッチ状態状態解除
        Invoke("isCatchOut", catchDelay);
    }

    void isCatchOut()
    {
        isCatch = false;
        Debug.Log("キャッチ解除");
    }
    public void OnClickThrow()
    {
        if (!isDead)
        {
            isThrow = true;



            StartCoroutine(IsThrowOut());
            Shot();
        }
    }

    //フェイント(投げるふり)処理
    public void OnClickFeint()
    {
        Debug.Log("feint");
        isFeint = true;
        //フェイントアニメーション
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);

        

        StartCoroutine(IsThrowOut());
    }
    //フェイントリセット
    IEnumerator IsThrowOut()
    {
        yield return new WaitForSeconds(throwDelay);//１秒待つ
        Debug.Log("投げ状態リセット");
        //プレイヤー状態リセット
        isFeint = false;
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
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
            SEManager.Instance.Play(
                audioPath: SEPath.JUMPED,      //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 0.6f,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );

            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);

            isJump = false;
            isGround = true;
        }

        //クリア判定オブジェクト(デバッグ用)
        if (other.gameObject.tag == "Clear")
        {
            DeadUser();
        }

        //ボールオブジェクト
        if (other.gameObject.tag == "Ball")
        {
            //キャッチ状態でボールに触ったら
            if (isCatch)
            {
                SEManager.Instance.Play(
                    audioPath: SEPath.CATCH,      //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0.6f,                     //再生されるまでの遅延時間
                    pitch: 1,                     //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                //再生終了後の処理
                );
                //ノックバック
                rigidbody.AddForce(-transform.forward * knockBack, ForceMode.VelocityChange);

                //キャッチアニメーション
                playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);
                Debug.Log("キャッチ");

                CancelInvoke();
                //キャッチ状態状態解除
                Invoke("isCatchOut", 0.8f);

                //ボール所持状態にする
                isHaveBall = true;
                Destroy(other.gameObject);    //ボール削除
                Debug.Log("キャッチ成功");

                catchNum++;
                GetBall();
            }
            //キャッチ状態じゃなかったら
            else
            {
                HitBall();

            }

        }

        //ダメージ性のない状態のボール
        if (other.gameObject.tag == "EasyBall")
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
        isHaveBall = true;
        //ボール所持者変更
        gameDirector.getUserId = roomModel.ConnectionId;
        Debug.Log("ボール取得");

        //取得者のIDを通知
        await roomModel.GetBallAsync(roomModel.ConnectionId);
    }
    //ボール取得処理
    private async void ThrowBall()
    {
        throwNum++;
        
        // 追加
        //rigidbody.AddForce(-transform.forward * 3f, ForceMode.VelocityChange);

        SEManager.Instance.Play(
                    audioPath: SEPath.THROW,      //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0.6f,                     //再生されるまでの遅延時間
                    pitch: 1,                     //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                //再生終了後の処理
                );

        //ボール情報
        var throwData = new ThrowData()
        {
            ConnectionId = roomModel.ConnectionId,            //接続ID
            ThorwPos = this.gameObject.transform.position,    //投げたプレイヤーの座標
            GoalPos = searchNearEnemy.transform.eulerAngles,    //目標座標

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

        //ノックバック
        rigidbody.AddForce(-transform.forward * knockBack, ForceMode.VelocityChange);


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

            //自身のHP減少
            this.hp--;

        }
       
    }


    /// <summary>
    /// 死亡通知
    /// </summary>
    private async void DeadUser()
    {
        this.gameObject.tag = "Dead";

        var deadData = new DeadData()
        {
            ConnectionId = roomModel.ConnectionId,    //ユーザーID
            Name = userModel.userName,                //ユーザー名
            Point = gameDirector.point,               //ユーザー獲得ポイント
            Time = (int)gameDirector.time,            //生存時間
            ThrowNum = throwNum,                      //投げた回数
            HitNum = gameDirector.hitNum,                          //当てた回数
            CatchNum = catchNum,                      //キャッチした回数
            JoinOrder = gameDirector.JoinNum,         //プレイヤー番号
            IsLast = this.isLast,
        };

        await roomModel.DeadUserAsync(deadData,gameDirector.deadNum);
    }

    private int GetPoint()
    {
        //当てた敵と自身の距離を取得
        float point = Vector3.Distance(searchNearEnemy.transform.position, this.gameObject.transform.position);

        //小数点以下切り上げ
        Mathf.Ceil(point);
        Debug.Log("得点:" + point);

        //整数化してreturn
        return (int)point;
    }

    /// <summary>
    /// 指定されたタグの中で最も近いものを取得
    /// </summary>
    /// <returns></returns>
    private GameObject EnemySerch(string getTagName)
    {

        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;

        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;

        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(getTagName);

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


   

    //プレイヤーの向き取得(敵用)
    public bool GetAngle()
    {
        return isLeft;
    }
}
