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
using static GameDirector;
using UnityEngine.InputSystem.XR;
using MessagePack.Resolvers;


public class GameDirector : MonoBehaviour
{
    //プレイヤープレハブ
    [SerializeField] GameObject characterPrefab;
    //ルームモデル
    [SerializeField] RoomModel roomModel;


    //ルームデータ(表示用)
    [SerializeField] Text roomNameText;
    [SerializeField] Text userName;

    private UserModel userModel;
    //private UserModel userModel;

    //入室データ(入力用)
    [SerializeField] Text roomName;
    [SerializeField] Text userId;

    //プレイヤー生成親オブジェクト
    [SerializeField] private GameObject spawnObj;
    //プレイヤー生成位置
    [SerializeField] private Transform[] spawnPosList;

    SpriteRenderer charaNum;

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

    [SerializeField] GameObject controller;
    public bool isStart;
    public float time;              //生存時間

    private Guid[] joinedId = new Guid[0];
    int joinNum = 0;

    //ゲームUI
    [SerializeField] GameObject gameUI;

    public Sprite player1;
    public Sprite player2;
    public Sprite player3;
    public Sprite player4;

    public Sprite you;

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

    GameObject ballObj;

    [SerializeField] GameObject ballPrefab;

    //マスタークライアント判定
    private bool isJoinFirst;

    //オブジェクト移動遷移補完時間
    private float dotweenTime = 0.1f;
    //サーバー通信時間
    private float commuTime = 0.02f;

    private int animNum;

    public Guid enemyId;     //敵ID保存用
    public int enemyPoint;
    public int point;
    public int hitNum;

    PlayerManager playerManager;

    //自滅防止用変数
    public Guid getUserId;
    public int deadNum;
    public bool isDead;

    public int JoinNum;
    //ゲーム状態
    public enum GAME_STATE
    {
        STOP = 0,             //停止中
        PREPARATION = 1,      //準備中
        READY = 2,            //準備完了中
        COUNTDOWN = 3,        //開始カウント中
        START = 4,            //ゲーム中
    }

    GAME_STATE game_State = GAME_STATE.STOP;

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

    void Awake()
    {
        //フレームレート設定
        Application.targetFrameRate = 60; // 初期状態は-1になっている
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

       

        //ユーザーが入室した際にOnJoinedUserメゾットを実行するようにモデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;   //ユーザー入室

        roomModel.MatchedUser += this.MatchedUser;     //マッチング

        roomModel.LeavedUser += this.LeavedUser;       //ユーザー退出

        roomModel.MovedUser += this.MovedUser;         //ユーザー移動情報

        roomModel.MovedBall += this.MovedBall;         //ボール移動情報

        roomModel.ThrowedBall += this.ThrowedBall;     //ボール投げ

        roomModel.GetBall += this.GetBall;             //ボール取得

        roomModel.HitBall += this.HitBall;             //ボールヒット

        roomModel.MoveCursor += this.MovedCursor;

        roomModel.DownUser += this.DownUser;             //ボールヒット
        roomModel.DownBackUser += this.DownBackUser;             //ボールヒット

        roomModel.ReadyUser += this.ReadyUser;         //ユーザー準備完了

        //roomModel.StartGameCount += this.GameCount;    //ゲーム内カウント開始

        roomModel.StartGameUser += this.GameStart;     //ゲーム開始

        roomModel.UserDead += this.DeadUser;           //ユーザー死亡

        roomModel.FinishGameUser += this.FinishGameUser;   //ゲーム終了


        isPlayer = false;

        isBall = false;

        isJoinFirst = false;

        cursor.SetActive(true);

        deadNum = 0;
        isDead = false;
        resultObj.SetActive(false);

        //ユーザーモデルを取得
        userModel = GameObject.Find("UserModel").GetComponent<UserModel>();

        //ユーザーID表示
        if (userModel.userName != "")
        {
            Debug.Log(userModel.userName);
            userName.text = userModel.userName;
        }


        //待機
        await roomModel.ConnectAsync();

        

    }

    void Update()
    {
        //ユーザーID更新(表示用)
        /*if (userModel.userName != "")
        {
            //Debug.Log(userModel.userName);
            userName.text = userModel.userName;
        }*/

        // if (!isPlayer) return;
        
        if(isStart)
        {

            //生存時間更新
            time += Time.deltaTime;

        }

        //Debug.Log(game_State);
        switch (game_State)
        {
            

            case GAME_STATE.READY:
                standByUI.SetActive(true);
                break;
            case GAME_STATE.START:
                standByUI.SetActive(false);
                break;
            case GAME_STATE.STOP:
                standByUI.SetActive(false);
                break;
        }
      
    }



    //入室処理
    public async void JoinRoom()
    {
        //isJoinFirst = false;

        /*if (!userId)
        {
            return;
        }*/

        Debug.Log("ルーム名:"+roomName.text);
        Debug.Log("ユーザーID;" + userModel.userId);

        cursor.SetActive(true);

        game_State = GAME_STATE.READY;
        if (!isMatch)
        {

            await roomModel.JoinAsync(roomName.text, userModel.userId);     //ルーム名とユーザーIDを渡して入室

            isMatch = true;//await roomModel.JoinAsync(roomName.text, userModel.userId);
        }

        //同期通信呼び出し、以降は commuTime ごとに実行
        InvokeRepeating(nameof(SendData), 0.0f, commuTime);

        Debug.Log("入室完了");
    }

    //入室処理
    public async void JoinLobby()
    {
        /*if (!userId)
        {
            return;
        }*/

       
        game_State = GAME_STATE.READY;

        menuCanvas.SetActive(true);
         cursor.SetActive(true);




         await roomModel.JoinLobbyAsync(userModel.userId);     //ルーム名とユーザーIDを渡して入室

        //同期通信呼び出し、以降は commuTime ごとに実行
        //InvokeRepeating(nameof(SendData), 0.0f, commuTime);
        game_State = GAME_STATE.READY;

        Debug.Log("マッチング中");
    }

    //マッチングが成立したときの処理
    private async void MatchedUser(string roomName)
    {
      
            await roomModel.LeaveAsync();


            Debug.Log("マッチ:" + roomName);



        if (!isMatch)
        {
            //受け取ったユーザーIDをルーム名に渡して入室
            await roomModel.JoinAsync(roomName, userModel.userId);
            
            //同期通信呼び出し、以降は commuTime ごとに実行
            InvokeRepeating(nameof(SendData), 0.0f, commuTime);
            isMatch = true;
        }
    }

    //ユーザーが入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        bool isJoined = false;

        if (joinedId != null)
        {
            Debug.Log("保存ID:"+joinedId);
            //重複チェック
            foreach (Guid id in joinedId)
            {
                Debug.Log(id + ":" + user.ConnectionId);
                if (id == user.ConnectionId) return;
            }
        }
        //マスターチェック
        roomModel.OnMasterCheck(user);

        //キャラクター生成
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //インスタンス生成

        //待機中UI生成
        GameObject standByCharaUI = Instantiate(standUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);

        //プレイヤースコアUI生成
        GameObject charaInfoUI = Instantiate(playerUIPrefab, Vector3.zero, Quaternion.identity, spawnPlayerUIObj.gameObject.transform);

        characterList[user.ConnectionId] = characterObject;        //フィールドで保持

        //コンポーネント付与
        characterList[user.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();


        standUIList[user.ConnectionId] = standByCharaUI;        //フィールドで保持

        scoreUIList[user.ConnectionId] = charaInfoUI;        //フィールドで保持

        
        //入室保存
        Array.Resize(ref joinedId, joinedId.Length +1);
        joinedId[joinedId.Length - 1] = user.ConnectionId;
       // joinedId[joinNum] = user.ConnectionId;
        //joinNum++;

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

        //ライフ生成
        /*GameObject lifePos = charaInfoUI.transform.GetChild(5).gameObject.GetComponent<GameObject>();

        for(int i = 1;i <= playerManager.hp; i++)
        {//設定分UI生成
            lifePos = Instantiate(lifePrefab, Vector3.zero, Quaternion.identity, lifePos.gameObject.transform);
        }
        */
        /*//HP(デフォルト)
        Text playerPoint = charaInfoUI.transform.GetChild(4).gameObject.GetComponent<Text>();
        playerPoint.text = playerManager.hp.ToString();*/

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


        if (user.ConnectionId == roomModel.ConnectionId)
        {

            //自機のNoをYOUに張り替え
            //自機区別テキスト表示
            GameObject you = characterObject.transform.GetChild(1).transform.GetChild(0).gameObject;
            you.SetActive(true);

            JoinNum = user.JoinOrder;

            //Instantiate(youPrefab, spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, characterList[roomModel.ConnectionId].gameObject.transform); //インスタンス生成

            characterObject.name = "MyPlay";
            //自機用のスクリプト＆タグを追加
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            characterList[roomModel.ConnectionId].tag = "Player";

            //自機を証明
            isPlayer = true;

            //入室番号
            Debug.Log("入室番号:"+user.JoinOrder);

        }
        else
        {
            string enemy = "Enemy";
            characterObject.name = enemy;
            //自機以外用のスクリプト＆タグを追加

            characterObject.tag = "Enemy";

            
        }
       
        child.SetActive(true);

    }

    //切断処理
    public async void DisConnectRoom()
    {
        isMatch = false;
        //同期通信解除
        CancelInvoke();

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

        //cursor.SetActive(false);

      
        Debug.Log("退出完了");
    }

    //ユーザーが退出したときの処理
    private async void LeavedUser(Guid connnectionId)
    {
       
        //プレイヤーがいなかったら
        if (!characterList.ContainsKey(connnectionId))
       {
           return;
       }
       
        //退出したプレイヤーのオブジェクト削除
        Destroy(characterList[connnectionId]);

        //退出したプレイヤーUIのオブジェクト削除
        Destroy(standUIList[connnectionId]);

        //退出したプレイヤーUIのオブジェクト削除
        Destroy(scoreUIList[connnectionId]);

        //退出したプレイヤーをリストから削除
        characterList.Remove(connnectionId);

        //退出したプレイヤーUIをリストから削除
        standUIList.Remove(connnectionId);

        //退出したプレイヤーUIをリストから削除
        scoreUIList.Remove(connnectionId);

        Debug.Log("退出したユーザー番号:"+ connnectionId);

        //プレイヤー判定をリセット
        isPlayer = false;

        game_State = GAME_STATE.STOP;

        Debug.Log("退出ユーザーオブジェクト削除");

        isMatch = false;

        menuCanvas.SetActive(true);
    }

    private async void SendData()
    {
        /*if(roomModel.isMaster)
        {
            animNum = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerManager>().animState;
        
            Debug.Log(animNum.ToString());
        }*/
        
        //コンポーネント付与
        //characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().Init();

       
        //移動情報
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,      //接続ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //キャラ位置
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //キャラ回転
            Angle = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<AngleManager> ().GetAngle(),  //キャラクターの向き
            AnimId = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().GetAnimId(),      //アニメーションID
        };

        //Debug.Log(moveData.AnimId);
        //プレイヤー移動
        await roomModel.MoveAsync(moveData);

        //カーソル座標送信
        if(roomModel.isMaster) await roomModel.MoveCursorAsync(cursor.transform.position);

        //フィールド上のボール検索
        ballObj = GameObject.Find("Ball");

        if (!ballObj) return;

        //ボールが取れたら
        if (ballObj) isBall = true;

        //ボールが存在している&マスタークライアント
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

    //ユーザーが移動したときの処理
    private async void MovedUser(MoveData moveData)
    {
        //送られてきたプレイヤーが存在していなかったら
        if (!characterList.ContainsKey(moveData.ConnectionId))
        {
            return;
        }

        //コンポーネント付与
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().Init();

        //Dotweenで移動補完
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Debug.Log("移動同期通った");
        //Dotweenで回転補完

        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
        //Debug.Log("回転同期");
        //アニメーション更新
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().SetEnemyAnim(moveData.AnimId);
        //Debug.Log("アニメーション通った");
        //キャラクターの向き更新
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<AngleManager>().SetAngle(moveData.Angle);
    } 

    //ユーザーが移動したときの処理
    private async void MovedBall(MoveData moveBallData)
    {
        //ボールが無かったら
        if (!ballObj)
        {
            return;
        }
       
        //Dotweenで移動補完
        ballObj.transform.DOMove(moveBallData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Dotweenで回転補完
        ballObj.transform.DORotate(moveBallData.Rotate, dotweenTime).SetEase(Ease.Linear);
    }
    //ユーザーが移動したときの処理
    private async void MovedCursor(Vector3 cursorPos)
    {
        Debug.Log("カーソル受け");
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

        //投げた座標に玉を生成
        Vector3 pos = characterList[throwData.ConnectionId].transform.position;
        GameObject newbullet = Instantiate(ballPrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity); //弾を生成


        //最後に投げた人にマスタークライアント権を譲渡する予定

        Debug.Log("ボール通知受けたよ");
    }

    //ボール取得処理
    private async void GetBall(Guid getUserId)
    {
        Debug.Log(getUserId);

        //タグ変更
        characterList[getUserId].gameObject.tag = "BallPlayer";
        //取得者IDを更新
        this.getUserId = getUserId;

        
        Debug.Log(this.getUserId);
       
        Debug.Log("取得者ID更新");
        bool isDelete = false;
        //フィールド上のボール検索
        while (isDelete)
        {
            ballObj = GameObject.Find("Ball");

            if (ballObj)
            {
                Destroy(ballObj.gameObject);    //ボール削除
                isDelete = true;
            }
        }

        ballObj = GameObject.Find("Ball");

        if (ballObj) Destroy(ballObj.gameObject);    //ボール削除

        //マスタークライアントリセット
        roomModel.isMaster = false;

        if(getUserId == roomModel.ConnectionId) roomModel.isMaster = true;
    }

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
        //残機削除
        Destroy(lifeList.transform.GetChild(0).gameObject);

        //ポイント反映
        Text playerPoint = scoreUIList[hitData.EnemyId].transform.GetChild(4).gameObject.GetComponent<Text>();

        enemyPoint = int.Parse(playerPoint.text);

        enemyPoint += hitData.Point;
        playerPoint.text = enemyPoint.ToString();

        if (roomModel.ConnectionId == hitData.EnemyId)
        {
            point += hitData.Point;
            hitNum++;
        }

        Debug.Log("獲得ポイント:" + hitData.Point);
        //当てたユーザーの得点加算

        Debug.Log("ヒット");
    }
        
    public async void DownUser(Guid downUserId)
    {
        CapsuleCollider hitBox;
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //コライダー取得
        hitBox.isTrigger = true; //トリガーオフ(ボール反射表現)

        characterList[downUserId].gameObject.tag = "Down";

        Debug.Log(characterList[downUserId].name + ":ダウン");

        StartCoroutine(PiyoPiyo(downUserId));
    }

    IEnumerator PiyoPiyo(Guid id)
    {
        yield return new WaitForSeconds(0.7f);//１秒待つ

        GameObject piyo = characterList[id].gameObject.transform.GetChild(2).gameObject;   //コライダー取得

        MeshRenderer rend = piyo.GetComponent<MeshRenderer>();
        rend.enabled = true;
        //piyo.SetActive(true);
    }
    //ダウン復帰処理
    public async void DownBackUser(Guid downUserId)
    {
        CapsuleCollider hitBox;
        hitBox = characterList[downUserId].gameObject.GetComponent<CapsuleCollider>();   //コライダー取得
        hitBox.isTrigger = false; //トリガーオン(ボール反射表現)

        //ピヨピヨ非表示
        GameObject piyo = characterList[downUserId].gameObject.transform.GetChild(2).gameObject;   //コライダー取得


        MeshRenderer rend = piyo.GetComponent<MeshRenderer>();
        rend.enabled = false;

        //piyo.SetActive(false);

        //自機だった場合
        if (roomModel.ConnectionId == downUserId) characterList[downUserId].gameObject.tag = "Player";    //Pleyerタグに

        else characterList[downUserId].gameObject.tag = "Enemy";

        Debug.Log(characterList[downUserId].name + ":ダウン復帰");
    }

    //ユーザー情報更新処理
    public　async void UpdateUserState(Guid connectionId,UserState userState)
    {
        //プレイヤーがいなかったら
        if (!characterList.ContainsKey(roomModel.ConnectionId))
        {
            return;   
        }


        //await roomModel.UpdateStateUser(connectionId,userState);
    }
    //ユーザー準備完了処理
    public async void ReadyUser(bool isReady)
    {
        isReady = true;
        await roomModel.ReadyAsync(isReady);
        Debug.Log("準備完了");
        game_State = GAME_STATE.READY;
    }

    

    public void  GameCount()
    {
        
        game_State = GAME_STATE.START;
        gameUI.SetActive(true);
       
        Debug.Log("カウントダウン開始");

        Invoke("GameStart", 4.0f);
    }

    public async void GameStart()
    {
        Text text = matchText.GetComponent<Text>();    
        matchText.GetComponent<TextManager>().enabled = false;

        text.text = "マッチング成立";

        StartCoroutine(StartCount());
    }

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



        standByUI.SetActive(false);
        game_State = GAME_STATE.START;

        Debug.Log("ゲーム開始");

        gameUI.SetActive(true);

        isStart = true;
        Debug.Log("toutatu");
        //リザルト表示
       
    }


    public void DeadUser(DeadData deadData,int deadNum)
    {

        //最後のプレイヤー以外は再生
        if (!deadData.IsLast)
        {

            SEManager.Instance.Play(
            audioPath: SEPath.GUEST_HIT,  //再生したいオーディオのパス
            volumeRate: 1,                //音量の倍率
            delay: 0,                     //再生されるまでの遅延時間
            pitch: 1,                     //ピッチ
            isLoop: false,                 //ループ再生するか
            callback: null                //再生終了後の処理
        );


            SEManager.Instance.Play(
                audioPath: SEPath.DOWN,        //再生したいオーディオのパス
                volumeRate: 1,                //音量の倍率
                delay: 1,                     //再生されるまでの遅延時間
                pitch: 1,                     //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                //再生終了後の処理
            );
        }

        if (deadData.ConnectionId == roomModel.ConnectionId)
        {
            //死亡判定
            isDead = true;
        }

        //死亡ユーザーのタグ変更
        characterList[deadData.ConnectionId].tag = "Dead";

        //死亡人数更新
        this.deadNum = deadNum;

        //残機リスト
        GameObject lifeList = scoreUIList[deadData.ConnectionId].transform.GetChild(5).gameObject;
        //死亡者の残機リスト削除
        Destroy(lifeList);


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

        //詳細情報
        
        //ポイント
        Text pointText = detailList.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Text>();
        pointText.text = deadData.Point.ToString();

        //生存時間
        Text timeText = detailList.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>();
        int minutes = 0;  //分
        int seccond = 0;  //秒

        //最後か
        Debug.Log(deadData.IsLast);
       
        //最後のプレイヤーだった場合
        if(deadData.IsLast)
        {

            timeText.text = "--:--";
        }
        else
        {
            

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


        //プレイヤーナンバー画像差し替え
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

    public void FinishGameUser()
    {
        Debug.Log("ゲーム終了通知");

        // Coroutine（コルーチン）を開始
        StartCoroutine(FinishGame());
    }
    IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(1f);//１秒待つ

        Debug.Log("toutatu");
        //リザルト表示
        resultObj.SetActive(true);
    }
   
    public async void OnClickHome()
    {
        //同期通信解除
        CancelInvoke();

        //退出処理
        await roomModel.LeaveAsync();

        //MagicOnion切断処理
        await roomModel.DisConnectAsync();

        //シーン再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
