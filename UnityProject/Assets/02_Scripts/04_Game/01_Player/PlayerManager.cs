////////////////////////////////////////////////////////////////////////////
///
///  プレイヤーマネージャースクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

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
    //データ制御用
    RoomModel roomModel;
    GameDirector gameDirector;
    UserModel userModel;

    private int MAX_PLAYER = 4;       //最大プレイ人数

    //カーソル関連
    private string enemyTag = "Enemy";           //取得タグ名(敵)
    private GameObject searchNearEnemy;          //最も近い敵座標
    private GameObject cursor;                   //照準カーソル

    //取得タグ関連
    private string ballTag = "EasyBall";         //取得タグ名(ボール)
    private string enemyBallTag = "BallPlayer";  //取得タグ名(ボール所持者)
    private GameObject searchNearBall;           //最も近いボール座標

    //プレイヤー状態関連
    private bool isHaveBall;       //ボールを所持しているか
    private bool isGround;         //地面に触れているか
    private bool isJump;           //ジャンプしているか
    private bool isCatch;          //キャッチ状態であるか
    private bool isDash;           //ダッシュ状態であるか
    private bool isThrow;          //投げ状態であるか
    private bool isFeint;          //フェイント状態
    private bool isDown;           //ダウン状態
    private bool isDead;           //死亡状態

    //ゲームバランス関連
    public float velosity = 13f;              //ジャンプの強さ
    public float ballSpeed = 24.0f;           //ボールの速さ
    public float knockBack = 12.0f;           //
    public float catchDelay = 0.6f;           //キャッチ有効時間
    public float haveCatchDelay = 0.6f;       //キャッチディレイ
    public float throwDelay = 0.6f;           //玉発射ディレイ
    public float downTime = 4.0f;             //ダウン時間

    GameObject ballPrefab;

    private bool isInit = false;

    [SerializeField] private Vector3 velocity;              // 移動方向
    [SerializeField] private float moveSpeed = 4.5f;        // 移動速度

    //UI関連
    Button jumpButton;
    Button catchButton;
    Button throwButton;
    Button feintButton;

    GameObject catchbtn;    //表示切替用
    GameObject throwbtn;    //表示切替用
    GameObject feintbtn;    //表示切替用

    FixedJoystick fixedJoystick;    //JoyStick
    Rigidbody rigidbody;
    CapsuleCollider hitBox;

    //プレイヤーアニメーション
    PlayerAnimation playerAnim;
    private bool isLeft;     //画像の向き

    //HP変数
    public int maxHp = 3;    //HP最大値
    private int hp;           //HP現在値

    private int damage = 1;   //ダメージ量(現状固定)

    //リザルトスコア用
    private int throwNum;    //投げた回数
    private int hitNum;      //当てた回数
    private int catchNum;    //キャッチした回数
    private bool isLast;     //最後の生存者か


    private bool isCalledOnce; //初回呼び出し用

    private void Start()
    {
        //コンポーネント取得
        playerAnim = this.gameObject.transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>();
        //ユーザーモデルを取得
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        hp = maxHp; //HP設定

        //プレイヤー状態
        isCatch = false;      //キャッチ状態
        isLeft = false;       //向いている方向
        isHaveBall = false;   //ボール所持状態
        isDash = false;       //ダッシュ状態
        isGround = false;     //接地状態
        isJump = false;       //ジャンプ状態
        isThrow = false;      //投げ状態
        isFeint = false;      //フェイント状態
        isDown = false;       //ダウン状態
        isDead = false;       //死亡状態
        isLast = true;        //最後のプレイヤーか

        isCalledOnce = false; //一度のみ実行用

        //ボールプレハブ取得
        ballPrefab = (GameObject)Resources.Load("Ball");
        //カーソルオブジェクトを取得
        cursor = GameObject.Find("Cursor");

        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearEnemy = EnemySerch(enemyTag);

        //ボールタグ取得
        searchNearBall = EnemySerch(ballTag);

        //ボールが取得できなかった場合
        if (searchNearBall == null) searchNearBall = EnemySerch(enemyBallTag);  //近くの敵を取得
        

        rigidbody = GetComponent<Rigidbody>();
        hitBox = GetComponent<CapsuleCollider>();

        //コントローラー
        //移動スティック
        fixedJoystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        //ジャンプボタン
        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();
        jumpButton.onClick.AddListener(() => OnClickJump());
        //キャッチボタン
        catchButton = GameObject.Find("CatchButton").GetComponent<Button>();
        catchButton.onClick.AddListener(() => OnClickCatch());
        catchbtn = GameObject.Find("CatchButton");
        //フェイントボタン
        feintButton = GameObject.Find("FeintButton").GetComponent<Button>();
        feintButton.onClick.AddListener(() => OnClickFeint());
        feintbtn = GameObject.Find("FeintButton");
        //投げるボタン
        throwButton = GameObject.Find("ThrowButton").GetComponent<Button>();
        throwButton.onClick.AddListener(() => OnClickThrow());
        throwbtn = GameObject.Find("ThrowButton");

        //ルームモデルの取得
        roomModel = GameObject.Find("RoomModel").GetComponent<RoomModel>();
        //GameDirectorの取得
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        //戦闘情報
        throwNum = 0;
        hitNum = 0;
        catchNum = 0;

        //ボールorボール所持者が取得できた場合
        if (searchNearBall)
        {
            if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x) isLeft = true;  //画像を左向きに
            else isLeft = false;  //右向きに
        }
        else isLeft = true;   //デフォルト
    }

    void Update()
    {
        //体力が0の場合
        if (!isDead && hp <= 0)
        {
            StopAllCoroutines();
            isLast = false;
            isDead = true;
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);
            //死亡処理
            DeadUser();
        }

        //最後の一人になった場合
        if (!gameDirector.isDead && gameDirector.deadNum >= MAX_PLAYER - 1 && this.isLast)
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
        if (searchNearEnemy)
        {
            if (isHaveBall) transform.LookAt(searchNearEnemy.transform);  //敵の方向を向く
            else this.gameObject.transform.eulerAngles = Vector3.zero;    //デフォルト
        }

        //ボールタグ取得
        searchNearBall = EnemySerch(ballTag);
        if (searchNearBall = null) searchNearBall = EnemySerch(enemyBallTag);   //ボール所持者を取得

        //向きチェック
        Move();

        //向き判定更新
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<AngleManager>().isLeft = this.isLeft;

        //ボタン表示非表示
        if (isHaveBall) catchbtn.SetActive(false);
        else if (!isHaveBall) catchbtn.SetActive(true);

        //フィールドに敵プレイヤーが存在している場合
        if (searchNearEnemy != null)
        {
            if (isHaveBall)
            {
                cursor.SetActive(true);
                //カーソルを最も近い敵の座標に移動
                cursor.transform.DOMove(searchNearEnemy.gameObject.transform.position, 0.1f).SetEase(Ease.Linear);
            }
        }

        if (isDead) return;   //死んでいたら以下の処理を実行しない

        //ボール所持状態
        if (isHaveBall)
        {
            //フェイント状態
            if (isFeint)
            {
                //ジャンプフェイント
                if (isJump)
                {
                    isJump = false;
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRFEINT);
                }
                //通常フェイント
                else
                {
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);
                }
            }
            else
            {
                //投げる
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
                    //接地かつジャンプ状態でないときのみ移動
                    if (isDash && isGround && !isJump)
                    {
                        //ダッシュアニメーション
                        if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_DASH);
                    }
                    else if (isCatch)
                    {
                        //キャッチアニメーション
                        if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);
                    }
                    else if (!isFeint)
                    {
                        //アイドル(待機)アニメーション
                        if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_IDLE);
                    }
                    //ジャンプ
                    if (isJump && !isDash && !isFeint)
                    {
                        if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_JUMP);
                    }
                }
            }
        }
        //ボール非所持状態
        else
        {
            if (isDown)
            {
                //ジャンプ状態だったら
                if (isDead)
                {
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DEAD);
                }
                else
                {
                    playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);
                }
            }
            else
            {
                if (isFeint)
                {
                    //ジャンプフェイント
                    if (isJump)
                    {
                        isJump = false;
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRFEINT);
                    }
                    //通常フェイント
                    else
                    {
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);
                    }
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
                    //接地かつジャンプ状態でないときのみ移動
                    if (isDash && isGround && !isJump)
                    {
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DASH);
                    }
                    else if (isCatch)
                    {
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
                    }
                    else
                    {
                        //アイドル(待機)アニメーション
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
                    }
                    if (isJump && !isDash)
                    {
                        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.JUMP);
                    }
                }
            }
        }

        //接地しているか判定
        if (isGround == true)
        {
            //ジャンプボタンが押されたか
            if (isJump == true)
            {
                if (isDead) return;

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

        //移動処理
        if (isCatch || isDown || isThrow || isDead || isFeint) return;  //いずれかの条件を満たしていたら移動不能

        Vector3 move = (Camera.main.transform.forward * fixedJoystick.Vertical + Camera.main.transform.right * fixedJoystick.Horizontal) * moveSpeed;
        move.y = rigidbody.velocity.y;
        rigidbody.velocity = move;
    }

    /// <summary>
    /// ダッシュアニメーションチェック
    /// </summary>
    public void Move()
    {
        //いずれかの状態だった場合
        if (isDead || isDown || isCatch || isFeint || isThrow) return;

        float dx = fixedJoystick.Horizontal; //joystickの水平方向の動きの値、-1~1の値を取得
        float dy = fixedJoystick.Vertical;   //joystickの垂直方向の動きの値、-1~1の値を取得

        float rad = Mathf.Atan2(dx - 0, dy - 0); //　 原点(0,0)と点（dx,dy)の距離から角度をとってくれる関数
        float deg = rad * Mathf.Rad2Deg; //radianからdegreenに変換

        //移動パッドが動かされている場合
        if (deg != 0)
        {
            //ジャンプ中は向き固定
            if (isJump) return;
            else
            {
                isDash = true;
                //右向きに
                if (deg > 0) isLeft = false;
                //左向きに
                else if (deg < 0) isLeft = true;
            }
        }
        //移動パッドが触られていない状態
        else
        {
            isDash = false;
            //ボールタグ取得
            searchNearBall = EnemySerch(ballTag);
            //ボール所持者プレイヤータグ取得
            if (searchNearBall == null) searchNearBall = EnemySerch(enemyBallTag);

            //ボールを持っている場合
            if (isHaveBall)
            {
                if (searchNearEnemy)
                {
                    if (searchNearEnemy.gameObject.transform.position.x < this.gameObject.transform.position.x) isLeft = true;  ///画像を右向きに
                    else isLeft = false;   //左向きに
                }
            }
            //ボールorボール所持者が取得できた場合
            if (searchNearBall)
            {
                if (searchNearBall.gameObject.transform.position.x < this.gameObject.transform.position.x) isLeft = true;  ///画像を右向きに
                else isLeft = false;   //左向きに
            }
        }
    }

    //ジャンプボタン関数
    public void OnClickJump()
    {
        //既にダウン or ジャンプ中
        if(isJump || isDown)return;

        SEManager.Instance.Play(
           audioPath: SEPath.JUMP,      //再生したいオーディオのパス
           volumeRate: 1,                //音量の倍率
           delay: 0,                     //再生されるまでの遅延時間
           pitch: 1,                     //ピッチ
           isLoop: false,                 //ループ再生するか
           callback: null                //再生終了後の処理
        );
        
        isDash = false;
        isJump = true;
    }

    //キャッチボタン関数
    public void OnClickCatch()
    {
        if (isDown || isDead) return;

        isDash = false;
        isCatch = true;

        //キャッチアニメーション
        playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.CATCH);
        Debug.Log("キャッチ");

        //キャッチ状態状態解除
        Invoke("isCatchOut", catchDelay);
    }

    //キャッチ状態解除関数
    void isCatchOut()
    {
        isCatch = false;
        Debug.Log("キャッチ解除");
    }
    //ボール投げボタン関数
    void OnClickThrow()
    {
        if (isDown || isDead || !isHaveBall) return;
        if (isFeint) StopAllCoroutines();

        throwbtn.SetActive(false);

        isFeint = false;
        isThrow = true;

        StartCoroutine(IsThrowOut());
        StartCoroutine(Shot());
    }

    //フェイント(投げるふり)処理
    public void OnClickFeint()
    {
        if (!isHaveBall) return;

        isFeint = true;

        if (!isJump) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.FEINT);
        else if(isJump)  playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.AIRFEINT);
        
        StartCoroutine(IsThrowOut());
    }
    //フェイントリセット
    IEnumerator IsThrowOut()
    {
        yield return new WaitForSeconds(throwDelay);//１秒待つ
       
        isFeint = false;
        isThrow = false;

        if (isHaveBall) playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_IDLE);
        else playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);
    }

    //ボール発射処理
    IEnumerator Shot()
    {
        //ボールを持っている状態のみ実行
        if (isHaveBall)
        {
             yield return new WaitForSeconds(0.2f);   //ボール発射タイミング
            //当たり判定変更(ボールと重ならないように)
            hitBox.isTrigger = true;
            //マスタークライアントに変更
            roomModel.isMaster = true;

            GameObject newbullet = Instantiate(ballPrefab, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity); //弾を生成
            Rigidbody bulletRigidbody = newbullet.GetComponent<Rigidbody>();

            bulletRigidbody.velocity = (transform.forward * ballSpeed); //キャラクターが向いている方向に弾に力を加える

            ThrowBall();

            //ボール所持状態を解除する
            StartCoroutine(ChangeThrowHitBox());
        }
        else Debug.Log("玉持ってないよ");
    }

    void OnCollisionEnter(Collision other)
    {
        //着地を検出したので着地状態を書き換え
        if (!isGround)
        {
            if (other.gameObject.tag == "Wall") return;   //カベは例外

            SEManager.Instance.Play(
                audioPath: SEPath.JUMPED,      //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 0.0f,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );

            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.IDLE);

            isJump = false;
            isGround = true;
        }

        //場外オブジェクト
        if (other.gameObject.tag == "Warp") this.gameObject.transform.position = new Vector3(0.0f,0.7f,-0.6f);    //ステージ中央にワープ
        
        //ボールオブジェクト
        if (other.gameObject.tag == "Ball")
        {
            //ダウン中or死んでたらリターン
            if (isDown || isDead) return;

            //キャッチ状態でボールに触ったら
            if (isCatch)
            {
                SEManager.Instance.Play(
                    audioPath: SEPath.CATCH,      //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0.0f,                     //再生されるまでの遅延時間
                    pitch: 1,                     //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                //再生終了後の処理
                );

                //キャッチアニメーション
                playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.HAVE_CATCH);
                Debug.Log("キャッチ");

                CancelInvoke();
                //キャッチ状態状態解除
                Invoke("isCatchOut", haveCatchDelay);

                Destroy(other.gameObject);    //ボール削除
                Debug.Log("キャッチ成功");

                catchNum++;
                GetBall();
            }
            //キャッチ状態じゃなかったら
            else HitBall();        
        }

        //ダメージ性のない状態のボール
        if (other.gameObject.tag == "EasyBall")
        {
            //ダウン中だったら
            if (isDown || isDead ) return;

            Debug.Log("Easy取得");

            //ボール獲得
            GetBall();
        }
    }

    //ボール取得処理
    private async void GetBall()
    {
        //ボール所持者重複チェック
        GameObject ballChack = EnemySerch(enemyBallTag);

        if (ballChack) return;

        //取得者のIDを通知
        await roomModel.GetBallAsync(roomModel.ConnectionId);

        Debug.Log("所持者いない");
        //ボール所持者重複チェック
        GameObject ball = EnemySerch(ballTag);
        Destroy(ball);    //ボール削除

        StartCoroutine(ChangeGetHitBox());

        //ボール所持者変更
        gameDirector.getUserId = roomModel.ConnectionId;
        Debug.Log("ボール取得");

        SEManager.Instance.Play(
               audioPath: SEPath.GET,      //再生したいオーディオのパス
               volumeRate: 1,                //音量の倍率
               delay: 0,                     //再生されるまでの遅延時間
               pitch: 1,                     //ピッチ
               isLoop: false,                 //ループ再生するか
               callback: null                //再生終了後の処理
               );

        throwbtn.SetActive(true);

        //マスタークライアントに変更
        roomModel.isMaster = true;
    }
    //ボール取得処理
    private async void ThrowBall()
    {
        throwNum++;

        SEManager.Instance.Play(
                    audioPath: SEPath.THROW,      //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0.0f,                     //再生されるまでの遅延時間
                    pitch: 1,                     //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                //再生終了後の処理
                );

        Vector3 test = new Vector3();
        //ボール情報
        var throwData = new ThrowData()
        {
            ConnectionId = roomModel.ConnectionId,            //接続ID
            ThorwPos = this.gameObject.transform.position,    //投げたプレイヤーの座標
            //GoalPos = searchNearEnemy.transform.eulerAngles,    //目標座標
            GoalPos = test,    //目標座標

        };

        //ボール発射通知
        await roomModel.ThrowBallAsync(throwData);
       
    }

    //当たり判定変更
    IEnumerator ChangeGetHitBox()
    {
        //持っていない状態だったら
        if (!isHaveBall)
        {
            hitBox.isTrigger = true;
            isHaveBall = true;
            Debug.Log("所持に変更");
            yield break; // ここでコルーチン終了  
        }
    }

    //当たり判定変更
    IEnumerator ChangeThrowHitBox()
    {
        Debug.Log("非所持に変更");
        this.gameObject.transform.eulerAngles = Vector3.zero;   //キャラ回転
        isHaveBall = false;
        yield return new WaitForSeconds(0.1f);   //ボールの軌道妨害防止
        hitBox.isTrigger = false;
    }

    //ボールヒット処理
    private async void HitBall()
    {
        //自分が投げた球は例外
        if (gameDirector.getUserId == roomModel.ConnectionId) return;
        

        //ポップアップの方向を上向きのベクトルに設定
        Vector3 jump_vector = Vector3.up;
        //ポップアップの速度を計算
        Vector3 jump_velocity = jump_vector * 5.0f;
        //ポップアップの速度を設定
        rigidbody.velocity = jump_velocity;


        //HPがまだある場合
        if (hp > 0)
        {
            Debug.Log("ヒット");
            Debug.Log("残り体力:" + hp);

            //ダウン処理
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);

            //ヒットデータ
            var hitData = new HitData()
            {
                ConnectionId = roomModel.ConnectionId,              //当てられたユーザーのID
                EnemyId = gameDirector.enemyId,                     //当てたユーザーのID
                DamagedHP = this.hp,                                //当てられたユーザーのHP
                Point = GetPoint(),                                 //獲得ポイント
            };
            //ヒット通知
            await roomModel.HitBallAsync(hitData);

            //自身のHP減少
            this.hp--;
        }
        else
        {
            DeadUser();
            //ダウン処理
            playerAnim.SetAnim(PlayerAnimation.ANIM_STATE.DOWN);
        }
        //ダウン処理
        DownUser();
    }

    //ダウン処理
    private async void DownUser()
    {
        isDown = true;
        await roomModel.DownUserAsync(roomModel.ConnectionId);   //ダウン状態通知

        StartCoroutine(DownBack());
    }
    //ダウン復帰
    IEnumerator DownBack()
    {
        //死んでいない場合のみ
        if (!isDead)
        {
            yield return new WaitForSeconds(downTime);//指定秒数待つ
            Debug.Log("自機回復");
            isDown = false;
            //回復通知
            DownBackUser();
        }
    }

    //ダウン復帰処理
    private async void DownBackUser()
    {
        await roomModel.DownBackUserAsync(roomModel.ConnectionId);   //ダウン復帰通知
    }

    //ユーザー死亡処理
    private async void DeadUser()
    {
        isDead = true;
        this.gameObject.tag = "Dead";
        //操作不能に
        Destroy(fixedJoystick);
    
        //死亡データ
        var deadData = new DeadData()
        {
            ConnectionId = roomModel.ConnectionId,    //ユーザーID
            Name = userModel.userName,                //ユーザー名
            Point = gameDirector.point,               //ユーザー獲得ポイント
            Time = (int)gameDirector.time,            //生存時間
            ThrowNum = throwNum,                      //投げた回数
            HitNum = gameDirector.hitNum,             //当てた回数
            CatchNum = catchNum,                      //キャッチした回数
            JoinOrder = gameDirector.JoinNum,         //プレイヤー番号
            IsLast = this.isLast,
        };
        //死亡通知
        await roomModel.DeadUserAsync(deadData,gameDirector.deadNum);
    }

    //ポイント取得関数
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

    //指定したタグの最で近いオブジェクトを取得
    private GameObject EnemySerch(string getTagName)
    {
        //最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;
        //検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;
        //tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(getTagName);

        //指定したタグのオブジェクトが存在しなかった場合
        if (objs.Length == 0) return searchTargetObj;
        
        //距離チェック
        foreach (GameObject obj in objs)
        {
            //objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            //nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {
                //nearDistanceを更新
                nearDistance = distance;
                //searchTargetObjを更新
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
