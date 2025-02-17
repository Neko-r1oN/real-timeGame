////////////////////////////////////////////////////////////////////////////
///
///  ゲーム処理スクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;                 //DOTweenを使うときはこのusingを入れる
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using KanKikuchi.AudioManager;
using UnityEngine.InputSystem.XR;
using MessagePack.Resolvers;
using Newtonsoft.Json.Linq;
using System.Linq;


/// <summary>
/// ゲームの流れを処理するクラス
/// </summary>
public class GameDirector : MonoBehaviour
{
    //プレイヤー生成用プレハブ
    [SerializeField] GameObject characterPrefab;
    //ルームモデル
    [SerializeField] RoomModel roomModel;

    //ルームデータ(表示用)
    [SerializeField] Text roomNameText;
    [SerializeField] Text userName;

    private UserModel userModel;

    //入室データ(プレイべート用)
    [SerializeField] Text roomName;

    [SerializeField] GameObject matchBtn;

    //プレイヤー生成親オブジェクト
    [SerializeField] private GameObject spawnObj;
    //プレイヤー生成位置
    [SerializeField] private Transform[] spawnPosList;

    SpriteRenderer charaNum;     //プレイヤー順番識別画像

    //ターゲット用カーソル
    [SerializeField] GameObject cursor;

    //入室UI
    [SerializeField] GameObject joinUI;
    //待機中UI
    [SerializeField] GameObject standByUI;
    [SerializeField] GameObject matchText;

    //ユーザー待機中UI生成親オブジェクト
    [SerializeField] private GameObject spawnUIObj;
    //メニューUI
    [SerializeField] GameObject menuCanvas;

    //ユーザースコア表示UIプレハブ
    [SerializeField] GameObject playerUIPrefab;
    //ユーザースコア表示UIオブジェクト
    [SerializeField] GameObject spawnPlayerUIObj;

    //ライフプレハブ
    [SerializeField] GameObject lifePrefab;

    //ユーザー別待機UIプレハブ
    [SerializeField] GameObject standUIPrefab;
    //スコア表示UI生成親オブジェクト
    [SerializeField] GameObject spawnStandUIObj;

    [SerializeField] GameObject controller;    //操作UI
    [SerializeField] GameObject leaveButton;   //切断UI  
    [SerializeField] GameObject ReadyButton;   //準備UI
    [SerializeField] GameObject disconnectUI;  //タイムアウトUI
    [SerializeField] GameObject errorUI;       //エラーUI
    
    private bool isStart;            //開始判定
    public float time;              //生存時間

    //入室ID保管用
    private Guid[] joinedId = new Guid[0];

    //ゲームUI
    [SerializeField] GameObject gameUI;

    [SerializeField] Sprite player1;
    [SerializeField] Sprite player2;
    [SerializeField] Sprite player3;
    [SerializeField] Sprite player4;

    //リザルトUIプレハブ
    [SerializeField] GameObject resultUIPrefab;
    //リザルトUI生成位置
    [SerializeField] GameObject resultUIPos;
    //リザルトUI生成位置
    [SerializeField] GameObject resultObj;

    //プレイヤー存在判定
    private bool isPlayer;
    //ボール存在判定
    private bool isBall;
    private bool isMatch;

    private bool isBallElsePlayer = false;

    GameObject ballObj;

    [SerializeField] GameObject ballPrefab;

    //マスタークライアント判定
    private bool isJoinFirst;

    //オブジェクト移動遷移補完時間
    private float dotweenTime = 0.1f;
    //サーバー通信時間
    private float commuTime = 0.02f;

    public Guid enemyId;     //敵ID保存用
    private int enemyPoint;
    public int point;
    public int hitNum;

    PlayerManager playerManager;

    //自滅防止用変数
    public Guid getUserId;

    public int deadNum;
    public bool isDead;
    public int JoinNum;

    //ゲーム状態
    private enum GAME_STATE
    {
        STOP = 0,             //停止中
        MATCHING ,         //マッチング中
        READY ,            //準備中
        READYED ,          //準備完了中
        START ,            //ゲーム中
        FINISH ,           //終了
        ERROR,                //エラー(切断)
    }

    GAME_STATE game_State = GAME_STATE.STOP;    //初期設定
    private int MAX_PLAYER = 4;             //最大プレイ人数

    //UI色変更用変数
    Color RedColor = new Color32(247, 33, 73, 255);
    Color BlueColor = new Color32(33, 112, 247, 255);
    Color GreenColor = new Color32(33, 247, 87, 255);
    Color YellowColor = new Color32(247, 247, 33, 255);

    //キャラクターリスト
    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    //待機UIリスト
    Dictionary<Guid, GameObject> standUIList = new Dictionary<Guid, GameObject>();
    //スコアUIリスト
    Dictionary<Guid, GameObject> scoreUIList = new Dictionary<Guid, GameObject>();
    //リザルトUIリスト
    Dictionary<Guid, GameObject> resultUIList = new Dictionary<Guid, GameObject>();

    //コルーチン
    Coroutine autoReady;     //自動準備用
    Coroutine timeOut;       //タイムアウト用

    void Awake()
    {
        //フレームレート設定
        Application.targetFrameRate = 60; //60FPS固定
    }
    async void Start()
    {
        isMatch = false;

        //メニューBGM
        BGMManager.Instance.Play(
            audioPath: BGMPath.MENU, //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                //再生されるまでの遅延時間
            pitch: 1,                //ピッチ
            isLoop: true,             //ループ再生するか
            allowsDuplicate: false             //他のBGMと重複して再生させるか
        );

        //歓声
        BGMManager.Instance.Play(
            audioPath: BGMPath.GUEST,         //再生したいオーディオのパス
            volumeRate: 0.5f,                    //音量の倍率
            delay: 0,                         //再生されるまでの遅延時間
            pitch: 1,                         //ピッチ
            isLoop: true,                     //ループ再生するか
            allowsDuplicate: true             //他のBGMと重複して再生させるか
        );
        //歓声
        BGMManager.Instance.Play(
            audioPath: BGMPath.GUEST,         //再生したいオーディオのパス
            volumeRate: 0.5f,                    //音量の倍率
            delay: 0.7f,                         //再生されるまでの遅延時間
            pitch: 1,                         //ピッチ
            isLoop: true,                     //ループ再生するか
            allowsDuplicate: true             //他のBGMと重複して再生させるか
        );

        enemyPoint = 0;
        point = 0;
        hitNum = 0;

       

        //通知を受け取った際に実行する関数を紐づけ
        roomModel.OnJoinedUser += this.OnJoinedUser;       //ユーザー入室
        roomModel.MatchedUser += this.MatchedUser;         //マッチング
        roomModel.LeavedUser += this.LeavedUser;           //ユーザー退出
        roomModel.MovedUser += this.MovedUser;             //ユーザー移動情報
        roomModel.MovedBall += this.MovedBall;             //ボール移動情報
        roomModel.ThrowedBall += this.ThrowedBall;         //ボール投げ
        roomModel.GetBall += this.GetBall;                 //ボール取得
        roomModel.HitBall += this.HitBall;                 //ボールヒット
        roomModel.MoveCursor += this.MovedCursor;          //カーソル移動
        roomModel.DownUser += this.DownUser;               //プレイヤーダウン
        roomModel.DownBackUser += this.DownBackUser;       //ダウン復帰
        roomModel.StandUser += this.Stand;                 //ユーザー準備スタンバイ
        roomModel.ReadyUser += this.ReadyUser;             //ユーザー準備完了
        roomModel.StartGameUser += this.GameStart;         //ゲーム開始
        roomModel.UserDead += this.DeadUser;               //ユーザー死亡
        roomModel.FinishGameUser += this.FinishGameUser;   //ゲーム終了

        isPlayer = false;   //自機判定
        isBall = false;     //ボール所持判定
        isJoinFirst = false;//フェイント判定
        isDead = false;     //死亡判定

        deadNum = 0;        //死亡人数

        cursor.SetActive(true);
        resultObj.SetActive(false);

        //ユーザーモデルを取得
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //ユーザーID表示
        if (userModel.userName != "")
        {
            Debug.Log(userModel.userName);
            userName.text = userModel.userName;
        }

        //サーバー接続
        await roomModel.ConnectAsync();

    }

    void Update()
    {
        //ゲーム開始常態か
        if (isStart) time += Time.deltaTime;
        

        //ゲーム状態
        switch (game_State)
        {
            case GAME_STATE.READY:
                standByUI.SetActive(true);
                break;
            case GAME_STATE.MATCHING:
                standByUI.SetActive(true);
                break;
            case GAME_STATE.READYED:
                standByUI.SetActive(true);
                break;
            case GAME_STATE.START:
                standByUI.SetActive(false);
                break;
            default:
                standByUI.SetActive(false);
                break;
        }

        //他のプレイヤーがボールを持っていたら
        if (isBallElsePlayer)
        {
            //ボール取得
            ballObj = GameObject.Find("Ball");
            //ボールが存在したら
            if (ballObj) Destroy(ballObj.gameObject);    //ボール削除
        }
    }

    //入室処理
    public async void JoinRoom()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
        );

        Debug.Log("ルーム名:" + roomName.text);
        Debug.Log("ユーザーID;" + userModel.userId);

        cursor.SetActive(true);

        game_State = GAME_STATE.MATCHING;    //ゲーム状態変更

        await roomModel.JoinAsync(roomName.text, userModel.userId);     //ルーム名とユーザーIDを渡して入室


        //同期通信呼び出し、以降は commuTime ごとに実行
        InvokeRepeating(nameof(SendData), 0.0f, commuTime);

        Debug.Log("入室完了");
    }

    //入室処理
    public async void JoinLobby()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
              );

        matchBtn.SetActive(false);
        menuCanvas.SetActive(true);
        cursor.SetActive(true);

        game_State = GAME_STATE.MATCHING;

        await roomModel.JoinLobbyAsync(userModel.userId);     //ユーザーIDを渡して入室

        //game_State = GAME_STATE.READY;

        Debug.Log("マッチング中");
    }

    //マッチングが成立したときの処理
    private async void MatchedUser(string roomName)
    {
        SEManager.Instance.Play(
                    audioPath: SEPath.MATCHED,      //再生したいオーディオのパス
                    volumeRate: 1,                //音量の倍率
                    delay: 0.0f,                     //再生されるまでの遅延時間
                    pitch: 1,                     //ピッチ
                    isLoop: false,                 //ループ再生するか
                    callback: null                //再生終了後の処理
        );

        await roomModel.LeaveAsync();     //ロビーから退出

        Debug.Log("マッチ:" + roomName);

        if (!isMatch)
        {
            //受け取ったユーザーIDをルーム名に渡して入室
            await roomModel.JoinAsync(roomName, userModel.userId);
            
            //同期通信呼び出し、以降は commuTime ごとに実行
            InvokeRepeating(nameof(SendData), 0.0f, commuTime);

            isMatch = true;   //多重入室防止用
        }
    }

    //ユーザーが入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.JOIN,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
        );

        if (joinedId != null)
{
            //重複チェック
            foreach (Guid id in joinedId)
            {
                //既に自分のIDで入室していた場合
                if (id == user.ConnectionId) return;
            }
        }

        //観戦用
        if (joinedId.Length > MAX_PLAYER)
        {
            joinUI.SetActive(false);
            standByUI.SetActive(false);
            controller.SetActive(false);

            game_State = GAME_STATE.START;

            return;
        }

        if(game_State != GAME_STATE.MATCHING)
      
        //マスターチェック
        roomModel.OnMasterCheck(user);

        //キャラクター生成
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //インスタンス生成
        //待機中UI生成
        GameObject standByCharaUI = Instantiate(standUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);
        //プレイヤースコアUI生成
        GameObject charaInfoUI = Instantiate(playerUIPrefab, Vector3.zero, Quaternion.identity, spawnPlayerUIObj.gameObject.transform);

       


        characterList[user.ConnectionId] = characterObject;     //フィールドで保持
                                                            
        characterList[user.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();    //コンポーネント付与
        standUIList[user.ConnectionId] = standByCharaUI;        //フィールドで保持

        scoreUIList[user.ConnectionId] = charaInfoUI;           //フィールドで保持

        //入室者ID保存
        Array.Resize(ref joinedId, joinedId.Length +1);
        joinedId[joinedId.Length - 1] = user.ConnectionId;
      
        //UIカラー
        Image standUIColor = standByCharaUI.gameObject.GetComponent<Image>();
        Image infoUIColor = charaInfoUI.transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<Image>();

        //プレイヤーNo取得(UI)
        Image number = standByCharaUI.transform.GetChild(1).gameObject.GetComponent<Image>();

        //プレイヤー名取得
        Text name = standByCharaUI.transform.GetChild(3).gameObject.GetComponent<Text>();
        name.text = user.UserData.Name;

        //プレイヤー情報UI取得(UI)
        Image scoreUINumber = charaInfoUI.transform.GetChild(3).gameObject.GetComponent<Image>();
        Text infoName = charaInfoUI.transform.GetChild(1).gameObject.GetComponent<Text>();
        infoName.text = user.UserData.Name;

        //自機区別テキスト表示
        GameObject child = characterObject.transform.GetChild(1).gameObject;

        charaNum = child.GetComponent<SpriteRenderer>();

        //プレイヤーナンバー画像差し替え
        switch (user.JoinOrder)
        {
            case 1:
                number.sprite = player1;
                scoreUINumber.sprite = player1;
                charaNum.sprite = player1;
                characterObject.name = "player1";
                standByCharaUI.name = "UI1";
                charaInfoUI.name = "UI1";
                standUIColor.color = RedColor;
                infoUIColor.color = RedColor;
                break;
            case 2:
                number.sprite = player2;
                scoreUINumber.sprite = player2;
                charaNum.sprite = player2;
                characterObject.name = "player2";
                standByCharaUI.name = "UI2";
                charaInfoUI.name = "UI2";

                standUIColor.color = BlueColor;
                infoUIColor.color = BlueColor;
                break;
            case 3:
                number.sprite = player3;
                scoreUINumber.sprite = player3;
                charaNum.sprite = player3;
                characterObject.name = "player3";
                standByCharaUI.name = "UI3";
                charaInfoUI.name = "UI3";

                standUIColor.color = GreenColor;
                infoUIColor.color = GreenColor;
                break;
            case 4:
               
                number.sprite = player4;
                scoreUINumber.sprite = player4;
                charaNum.sprite = player4;
                characterObject.name = "player4";
                standByCharaUI.name = "UI4";
                charaInfoUI.name = "UI4";

                standUIColor.color = YellowColor;
                infoUIColor.color = YellowColor;

                break;
            default:
                Debug.Log("観戦者");
                break;
        }

        //入室したユーザーのIDが自身のIDと一致した場合
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            //自機区別テキスト表示
            GameObject you = characterObject.transform.GetChild(1).transform.GetChild(0).gameObject;
            you.SetActive(true);

            JoinNum = user.JoinOrder;

            characterObject.name = "MyPlay";
            //自機用のスクリプト＆タグを追加
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            characterList[roomModel.ConnectionId].tag = "Player";

            //自機を証明
            isPlayer = true;

        }
        //一致しなかった場合
        else
        {
            string enemy = "Enemy";
            characterObject.name = enemy;
            characterObject.tag = "Enemy";
        }
       
        child.SetActive(true);
    }

    private void OnDestroy()
    {
        //紐づけを解除
        roomModel.OnJoinedUser -= this.OnJoinedUser;       //ユーザー入室
        roomModel.MatchedUser -= this.MatchedUser;         //マッチング
        roomModel.LeavedUser -= this.LeavedUser;           //ユーザー退出
        roomModel.MovedUser -= this.MovedUser;             //ユーザー移動情報
        roomModel.MovedBall -= this.MovedBall;             //ボール移動情報
        roomModel.ThrowedBall -= this.ThrowedBall;         //ボール投げ
        roomModel.GetBall -= this.GetBall;                 //ボール取得
        roomModel.HitBall -= this.HitBall;                 //ボールヒット
        roomModel.MoveCursor -= this.MovedCursor;          //カーソル移動
        roomModel.DownUser -= this.DownUser;               //プレイヤーダウン
        roomModel.DownBackUser -= this.DownBackUser;       //ダウン復帰
        roomModel.StandUser -= this.Stand;                 //ユーザー準備スタンバイ
        roomModel.ReadyUser -= this.ReadyUser;             //ユーザー準備完了
        roomModel.StartGameUser -= this.GameStart;         //ゲーム開始
        roomModel.UserDead -= this.DeadUser;               //ユーザー死亡
        roomModel.FinishGameUser -= this.FinishGameUser;   //ゲーム終了

    }

    //切断処理
    public async void DisConnectRoom()
    {
        SEManager.Instance.Play(
                  audioPath: SEPath.TAP,      //再生したいオーディオのパス
                  volumeRate: 1,              //音量の倍率
                  delay: 0.0f,                //再生されるまでの遅延時間
                  pitch: 1,                   //ピッチ
                  isLoop: false,              //ループ再生するか
                  callback: null              //再生終了後の処理
        );

        //入室ユーザーID破棄
        Array.Clear(joinedId, 0, joinedId.Length);

        matchBtn.SetActive(true);
        //同期通信解除
        CancelInvoke();

        isMatch = false;

        //退出処理
        await roomModel.LeaveAsync();

        standByUI.SetActive(false);
        menuCanvas.SetActive(false);

        game_State = GAME_STATE.STOP;

        //MagicOnion切断処理
        //await roomModel.DisConnectAsync();

        // プレイヤーオブジェクトの削除
        foreach (Transform player in spawnObj.transform)
        {
            Destroy(player.gameObject);
        }
        //UIオブジェクト削除
        foreach (Transform ui in spawnUIObj.transform)
        {
            Destroy(ui.gameObject);
        }
        //スコアUIオブジェクト削除
        foreach (Transform info in spawnPlayerUIObj.transform)
        {
            Destroy(info.gameObject);
        }

        isPlayer = false;
      
        Debug.Log("退出完了");
    }

    //ユーザーが退出したときの処理
    private async void LeavedUser(Guid connnectionId)
    {
        //退出ユーザーのオブジェクトが無かったら
        if (!characterList.ContainsKey(connnectionId))
        {
           return;
        }

        //エラー状態以外の場合のみ実行
        if (game_State != GAME_STATE.ERROR) joinedId = Array.FindAll(joinedId, i => i != connnectionId).ToArray();   //退出ユーザーIDを破棄
        
        //想定外の状態で切断が発生した場合
        if (game_State == GAME_STATE.READY || game_State == GAME_STATE.READYED || game_State == GAME_STATE.START)
        {
            game_State = GAME_STATE.ERROR;
            disconnectUI.SetActive(true);
        }

        //退出したプレイヤーのオブジェクト削除
        Destroy(characterList[connnectionId]);     //プレイヤー
        Destroy(standUIList[connnectionId]);       //待機UI
        Destroy(scoreUIList[connnectionId]);       //スコア表示UI

        //退出したプレイヤーをリスト削除
        characterList.Remove(connnectionId);       //プレイヤーリスト
        standUIList.Remove(connnectionId);         //待機UIリスト
        scoreUIList.Remove(connnectionId);         //スコア表示UIリスト

        Debug.Log("退出したユーザー番号:"+ connnectionId);

        //プレイヤー判定をリセット
        isPlayer = false;

        Debug.Log("退出ユーザーオブジェクト削除");

        isMatch = false;

        menuCanvas.SetActive(true);
    }

    //他クライアントにデータを送信する関数
    private async void SendData()
    {
        //通知を受けたユーザーのオブジェクトが存在しなかったら
        if (!characterList.ContainsKey(roomModel.ConnectionId)) return;

        //移動情報
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,       //接続ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //キャラ位置
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //キャラ回転
            Angle = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<AngleManager>().GetAngle(),          //キャラクターの向き
            AnimId = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().GetAnimId(),     //アニメーションID
        };

        //プレイヤー移動通知送信
        await roomModel.MoveAsync(moveData);

        //カーソル座標送信
        if (roomModel.isMaster) await roomModel.MoveCursorAsync(cursor.transform.position);

        //フィールド上のボール検索
        ballObj = GameObject.Find("Ball");

        if (!ballObj) return;
        //ボールがフィールドに存在していたら
        if (ballObj) isBall = true;

        //ボールが存在している & 自身がボールマスター
        if (isBall && roomModel.isMaster)
        {
            //ボール情報
            var moveBallData = new MoveData()
            {
                ConnectionId = roomModel.ConnectionId,      //接続ID
                Pos = ballObj.transform.position,           //ボール位置
                Rotate = ballObj.transform.eulerAngles,     //ボール回転
            };

            //ボール位置同期
            await roomModel.MoveBallAsync(moveBallData);
        }
    }

    //他ユーザーが移動したときの処理
    private async void MovedUser(MoveData moveData)
    {
        //送られてきたプレイヤーが存在していなかったら
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;    

        //コンポーネント付与
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();

        //Dotweenで移動補完
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Dotweenで回転補完
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);

        //アニメーション更新
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().SetEnemyAnim(moveData.AnimId);
        //キャラクターの向き更新
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<AngleManager>().SetAngle(moveData.Angle);
    } 

    //ボールが移動したときの処理
    private async void MovedBall(MoveData moveBallData)
    {
        //フィールドにボールが存在しなかったら
        if (!ballObj) return;

        //Dotweenで移動補完
        ballObj.transform.DOMove(moveBallData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Dotweenで回転補完
        ballObj.transform.DORotate(moveBallData.Rotate, dotweenTime).SetEase(Ease.Linear);
    }

    //カーソルが移動したときの処理
    private async void MovedCursor(Vector3 cursorPos)
    {
        //Dotweenで移動補完
        cursor.transform.DOMove(cursorPos, dotweenTime).SetEase(Ease.Linear);
    }

    //ボール発射処理
    private async void ThrowedBall(ThrowData throwData)
    {
        SEManager.Instance.Play(
            audioPath: SEPath.THROW,      //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );

        //タグ変更
        characterList[throwData.ConnectionId].gameObject.tag = "Enemy";
        //投げたユーザーのIDを保存
        enemyId = throwData.ConnectionId;

        isBallElsePlayer = false;

        //投げた座標に玉を生成
        Vector3 pos = characterList[throwData.ConnectionId].transform.position;     //投げたプレイヤーの座標取得
        GameObject newbullet = Instantiate(ballPrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity); //弾を生成
    }

    //ボール取得処理
    private async void GetBall(Guid getUserId)
    {

        isBallElsePlayer = true;
         
        //ボール削除
        ballObj = GameObject.Find("Ball");
        if (ballObj) Destroy(ballObj.gameObject);    //ボール削除

        SEManager.Instance.Play(
                  audioPath: SEPath.GET,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
              );

        Debug.Log(getUserId);

        //タグ変更
        characterList[getUserId].gameObject.tag = "BallPlayer";
        //取得者IDを更新
        this.getUserId = getUserId;

        
        Debug.Log(this.getUserId);
       
        Debug.Log("取得者ID更新");
        
        //保健用
        ballObj = GameObject.Find("Ball");
        if (ballObj) Destroy(ballObj.gameObject);    //ボール削除

        //マスタークライアントリセット
        roomModel.isMaster = false;

        //取得者のIDと自身のIDが一致した場合
        if(getUserId == roomModel.ConnectionId) roomModel.isMaster = true;
    }

    //ヒット処理
    public async void HitBall(HitData hitData)
    {
        SEManager.Instance.Play(
            audioPath: SEPath.HIT,        //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );

        SEManager.Instance.Play(
            audioPath: SEPath.GUEST_HIT,  //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );


        //残機リスト
        GameObject lifeList = scoreUIList[hitData.ConnectionId].transform.GetChild(5).gameObject;
        //残機(体力UI)削除
        Destroy(lifeList.transform.GetChild(0).gameObject);

        //ポイント反映
        Text playerPoint = scoreUIList[hitData.EnemyId].transform.GetChild(4).gameObject.GetComponent<Text>();

        enemyPoint = int.Parse(playerPoint.text);
        enemyPoint += hitData.Point;
        playerPoint.text = enemyPoint.ToString();

        //自身が当てたユーザー通知だった場合
        if (roomModel.ConnectionId == hitData.EnemyId)
        {
            point += hitData.Point;
            hitNum++;     //ヒット回数加算
        }
    }
    
    //プレイヤーダウン処理
    public async void DownUser(Guid downUserId)
    { 
        CapsuleCollider hitBox;      //当たり判定
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //コライダー取得
        hitBox.isTrigger = true; //トリガーオフ(ボール反射表現)

        //タグ変更
        characterList[downUserId].gameObject.tag = "Down";
        Debug.Log(characterList[downUserId].name + ":ダウン");

        StartCoroutine(PiyoPiyo(downUserId));   //ダウンコルーチン
    }

    //ダウン表示処理
    IEnumerator PiyoPiyo(Guid id)
    {
        yield return new WaitForSeconds(0.7f);//１秒待つ

        //レンダラー取得
        MeshRenderer rend = characterList[id].gameObject.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>();
        rend.enabled = true;

        SEManager.Instance.Play(
              audioPath: SEPath.PIYOPIYO,        //再生したいオーディオのパス
              volumeRate: 1,                //音量の倍率
              delay: 0,                     //再生されるまでの遅延時間
              pitch: 1,                     //ピッチ
              isLoop: false,                 //ループ再生するか
              callback: null                //再生終了後の処理
        );
    }

    //ダウン復帰処理
    public async void DownBackUser(Guid downUserId)
    {
        CapsuleCollider hitBox;      //当たり判定
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //コライダー取得
        hitBox.isTrigger = false; //トリガーオン(ボール反射表現)

        //レンダラー取得
        MeshRenderer rend = characterList[downUserId].gameObject.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>();
        rend.enabled = false;

        //自機だった場合
        if (roomModel.ConnectionId == downUserId) characterList[downUserId].gameObject.tag = "Player";    //Pleyerタグに
        else characterList[downUserId].gameObject.tag = "Enemy";
    }

    //ユーザー準備待機処理
    public async void Stand()
    {
        Text text = matchText.GetComponent<Text>();
        matchText.GetComponent<TextManager>().enabled = false;

        text.text = "レディ?";
        game_State = GAME_STATE.READY;
        leaveButton.SetActive(false);

        //コルーチンスタート
        autoReady = StartCoroutine(AutoReady());
        timeOut = StartCoroutine(TimeOutError());

    }
    //タイムアウトエラー
    IEnumerator TimeOutError()
    {
        yield return new WaitForSeconds(5.0f);//5秒待つ
        errorUI.SetActive(true);
    }
    //自動準備
    IEnumerator AutoReady()
    {
        yield return new WaitForSeconds(3.0f);//5秒待つ
        Debug.Log("自動レディ");

        //自身のキャラクターが生成されていたら
        if(characterList.ContainsKey(roomModel.ConnectionId)) Ready();
    }

    //準備関数
    public async void Ready()
    {
        //準備ボタン非表示
        ReadyButton.SetActive(false);

        bool isReady = true;
        await roomModel.ReadyAsync(roomModel.ConnectionId, isReady);
        Debug.Log("準備完了");
    }

    //ユーザー準備完了通知処理
    public async void ReadyUser(Guid id, bool isReady)
    {
        SEManager.Instance.Play(
            audioPath: SEPath.READY,        //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );

        //受け取ったプレイヤーのUI更新
        GameObject Ready = standUIList[id].transform.GetChild(4).gameObject;
        Ready.SetActive(true);

    }
    //エラー関数
    public void Error()
    {
        errorUI.SetActive(true);
    }
    
    //ゲーム開始関数
    public async void GameStart()
    {
        //コルーチンを止める
        StopCoroutine(timeOut);

        game_State = GAME_STATE.READYED;
        leaveButton.SetActive(false);

        Text text = matchText.GetComponent<Text>();    
        matchText.GetComponent<TextManager>().enabled = false;

        text.text = "試合決定！";

        StartCoroutine(StartCount());
    }

    //ゲーム開始(カメラ・UI遷移)関数
    IEnumerator StartCount()
    {
        yield return new WaitForSeconds(2f);//１秒待つ

        joinUI.SetActive(false);

        BGMManager.Instance.Stop(BGMPath.MENU);

        BGMManager.Instance.Play(
            audioPath: BGMPath.BUTTLE, //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                //再生されるまでの遅延時間
            pitch: 1,                //ピッチ
            isLoop: true,             //ループ再生するか
            allowsDuplicate: true             //他のBGMと重複して再生させるか
        );
        SEManager.Instance.Play(
            audioPath: SEPath.COUNT_DOWN, //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0.0f,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );


        gameUI.SetActive(true);
        standByUI.SetActive(false);
        game_State = GAME_STATE.START;

        isStart = true;       
    }

    //ユーザー死亡処理
    public void DeadUser(DeadData deadData,int deadNum)
    {
        //最後のプレイヤー以外は再生
        if (!deadData.IsLast)
        {
            //ヒット(歓声)
            SEManager.Instance.Play(
            audioPath: SEPath.GUEST_HIT,  //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
            );
            //ダウン
            SEManager.Instance.Play(
                audioPath: SEPath.DOWN,        //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 1,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );
            //デッド
            SEManager.Instance.Play(
                  audioPath: SEPath.DEAD,      //再生したいオーディオのパス
                  volumeRate: 1,                //音量の倍率
                  delay: 0.0f,                     //再生されるまでの遅延時間
                  pitch: 1,                     //ピッチ
                  isLoop: false,                 //ループ再生するか
                  callback: null                //再生終了後の処理
            );
        }

        //自機がやられた通知だった場合
        if (deadData.ConnectionId == roomModel.ConnectionId) isDead = true;
        
        //死亡ユーザーのタグ変更
        characterList[deadData.ConnectionId].tag = "Dead";

        this.deadNum = deadNum;//死亡人数更新

        //残機リスト
        GameObject lifeList = scoreUIList[deadData.ConnectionId].transform.GetChild(5).gameObject;
        Destroy(lifeList);  //死亡者の残機リスト削除

        //死亡者のリザルトUI生成
        GameObject resultUI = Instantiate(resultUIPrefab, Vector3.zero, Quaternion.identity, resultUIPos.gameObject.transform);
        //リザルトUIのImageコンポーネント取得
        Image UIColor = resultUI.GetComponent<Image>();

        resultUIList[deadData.ConnectionId] = resultUI;        //フィールドで保持

        //プレイヤーNo取得(UI)
        Image number = resultUI.transform.GetChild(4).gameObject.GetComponent<Image>();
        //リザルト詳細情報取得
        GameObject detailList = resultUI.transform.GetChild(5).gameObject;
        //プレイヤー名取得
        Text name = resultUI.transform.GetChild(3).gameObject.GetComponent<Text>();
        name.text = deadData.Name;

        //ポイント
        Text pointText = detailList.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Text>();
        pointText.text = deadData.Point.ToString();

        //生存時間
        Text timeText = detailList.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>();
        int minutes = 0;  //分
        int seccond = 0;  //秒
       
        //最後のプレイヤーだった場合
        if(deadData.IsLast) timeText.text = "--:--";       
        else
        {
            //一分以上は繰り返し
            while (deadData.Time >= 60)
            {
                deadData.Time -= 60;
                minutes++;
            }
            seccond = deadData.Time;

            //テキスト左0詰め
            var minutesText = minutes.ToString("D2");
            var seccondText = seccond.ToString("D2");

            timeText.text = minutesText + ":" + seccondText;
        }
        
        //投げた回数
        Text throwText = detailList.transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Text>();
        throwText.text = deadData.ThrowNum.ToString();

        //当てた回数
        Text hitText = detailList.transform.GetChild(3).transform.GetChild(1).gameObject.GetComponent<Text>();
        hitText.text = deadData.HitNum.ToString();

        //キャッチ回数
        Text catchText = detailList.transform.GetChild(4).transform.GetChild(1).gameObject.GetComponent<Text>();
        catchText.text = deadData.CatchNum.ToString();

        //入室順に変更
        switch (deadData.JoinOrder)
        {
            case 1:
                number.sprite = player1;
                resultUI.name = "player1";
                UIColor.color = RedColor;
                break;
            case 2:
                number.sprite = player2;
                resultUI.name = "player2";
                UIColor.color = BlueColor;


                break;
            case 3:
                number.sprite = player3;
                resultUI.name = "player3";
                UIColor.color = GreenColor;

                break;
            case 4:
                number.sprite = player4;
                resultUI.name = "player4";
                UIColor.color = YellowColor;

                break;
            default:
                Debug.Log("不到達点");
                break;
        }
    }

    //ゲーム終了処理
    public void FinishGameUser()
    {
        game_State = GAME_STATE.FINISH;

        //コルーチン開始
        StartCoroutine(FinishGame());
    }

    //ゲーム終了関数
    IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1f);//１秒待つ

        //リザルト表示
        resultObj.SetActive(true);
    }
   
    //ホーム遷移関数
    public async void OnClickHome()
    {
        //同期通信解除
        CancelInvoke();
        //退出処理
        await roomModel.LeaveAsync();
        //MagicOnion切断処理
        //await roomModel.DisConnectAsync();

        //シーン再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
