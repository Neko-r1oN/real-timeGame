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

    //待機中UI
    [SerializeField] GameObject standByUI;

    //ユーザー待機中UI生成親オブジェクト
    [SerializeField] private GameObject spawnUIObj;


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


   

    //ゲームUI
    [SerializeField] GameObject gameUI;

    public Sprite player1;
    public Sprite player2;
    public Sprite player3;
    public Sprite player4;

    public Sprite you;

    //リザルトUI
    [SerializeField] GameObject resultUI;

    //プレイヤー存在判定
    private bool isPlayer;
    //ボール存在判定
    private bool isBall;

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
    public int point;

    PlayerManager playerManager;

    //自打球防止用変数
    public Guid getUserId;

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

    
    //キャラクターリスト
    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();
    //待機UIリスト
    Dictionary<Guid, GameObject> standUIList = new Dictionary<Guid, GameObject>();
    //スコアUIリスト
    Dictionary<Guid, GameObject> scoreUIList = new Dictionary<Guid, GameObject>();

    void Awake()
    {
        //フレームレート設定
        Application.targetFrameRate = 60; // 初期状態は-1になっている
    }
    async void Start()
    {
       

        //ユーザーが入室した際にOnJoinedUserメゾットを実行するようにモデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;   //ユーザー入室

        roomModel.MatchedUser += this.MatchedUser;     //マッチング

        roomModel.LeavedUser += this.LeavedUser;       //ユーザー退出

        roomModel.MovedUser += this.MovedUser;         //ユーザー移動情報

        roomModel.MovedBall += this.MovedBall;         //ボール移動情報

        roomModel.ThrowedBall += this.ThrowedBall;     //ボール投げ

        roomModel.GetBall += this.GetBall;             //ボール取得

        roomModel.HitBall += this.HitBall;             //ボールヒット

        roomModel.ReadyUser += this.ReadyUser;         //ユーザー準備完了

        //roomModel.StartGameCount += this.GameCount;    //ゲーム内カウント開始

        roomModel.StartGameUser += this.GameStart;     //ゲーム開始

        roomModel.FinishGameUser += this.GameFinish;   //ゲーム終了


        isPlayer = false;

        isBall = false;

        isJoinFirst = false;

        cursor.SetActive(false);

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
        if (!isPlayer) return;
        
       /* if(game_State == GAME_STATE.PREPARATION)
        {
            standByUI.SetActive(true);
        }
        else
        {
            standByUI.SetActive(false);
        }*/

       
    }



    //入室処理
    public async void JoinRoom()
    {
        //isJoinFirst = false;

        if (!userId)
        {
            return;
        }

        Debug.Log("ルーム名:"+roomName.text);
        Debug.Log("ユーザーID;" + userModel.userId);

        cursor.SetActive(true);

        game_State = GAME_STATE.PREPARATION;

        await roomModel.JoinAsync(roomName.text, userModel.userId);     //ルーム名とユーザーIDを渡して入室
        //await roomModel.JoinAsync(roomName.text, userModel.userId);


        //同期通信呼び出し、以降は0.02fごとに実行
        InvokeRepeating(nameof(SendData), 0.0f, commuTime);

        Debug.Log("入室完了");
    }

    //入室処理
    public async void JoinLobby()
    {
        if (!userId)
        {
            return;
        }

        Debug.Log("ロビークリック");
        Debug.Log("ユーザーID;" + userId.text);

        cursor.SetActive(true);

        


        await roomModel.JoinLobbyAsync(userModel.userId);     //ルーム名とユーザーIDを渡して入室

        game_State = GAME_STATE.PREPARATION;

        Debug.Log("マッチング中");
    }

    //マッチングが成立したときの処理
    private async void MatchedUser(string roomName)
    {
        await roomModel.LeaveAsync();


        Debug.Log("入室するルーム名:"+roomName);

        

        //受け取ったユーザーIDをルーム名に渡して入室
        await roomModel.JoinAsync(roomName, userModel.userId);

       

        Debug.Log("マッチング入室完了");
    }

    //ユーザーが入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        //マスターチェック
        roomModel.OnMasterCheck(user);

        //キャラクター生成
        GameObject characterObject = Instantiate(characterPrefab,spawnPosList[user.JoinOrder - 1].transform.position, Quaternion.identity, spawnObj.gameObject.transform); //インスタンス生成

        //待機中UI生成
        GameObject standByCharaUI = Instantiate(standUIPrefab, Vector3.zero,Quaternion.identity,spawnUIObj.gameObject.transform);

        //プレイヤースコアUI生成
        GameObject charaInfoUI = Instantiate(playerUIPrefab, Vector3.zero, Quaternion.identity, spawnPlayerUIObj.gameObject.transform);

        characterList[user.ConnectionId] = characterObject;        //フィールドで保持
        
        standUIList[user.ConnectionId] = standByCharaUI;        //フィールドで保持

        scoreUIList[user.ConnectionId] = charaInfoUI;        //フィールドで保持



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


                break;
            case 2:
                number.sprite = player2;
                scoreUINumber.sprite = player2;
                charaNum.sprite = player2;
                characterObject.name = "player2";
                standByCharaUI.name = "UI2";

                break;
            case 3:
                number.sprite = player3;
                scoreUINumber.sprite = player3;
                charaNum.sprite = player3;
                characterObject.name = "player3";
                standByCharaUI.name = "UI3";
                break;
            case 4:
               
                number.sprite = player4;
                scoreUINumber.sprite = player4;
                charaNum.sprite = player4;
                characterObject.name = "player3";
                /*standByUI.SetActive(false);
                game_State = GAME_STATE.START;*/
                Debug.Log("4人目通貨");
                standByCharaUI.name = "UI4";

                break;
            default:
                Debug.Log("観戦者");
                break;
        }


        if (user.ConnectionId == roomModel.ConnectionId)
        {

            //自機のNoをYOUに張り替え
            charaNum.sprite = you;

            
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
            string enemy = "Enemy"/* + user.JoinOrder*/;
            characterObject.name = enemy;
            //自機以外用のスクリプト＆タグを追加
            characterObject.gameObject.AddComponent<EnemyManager>();
            characterObject.tag = "Enemy";

            
        }
       
        child.SetActive(true);

    }

    //切断処理
    public async void DisConnectRoom()
    {

        //同期通信解除
        CancelInvoke();

        //退出処理
        await roomModel.LeaveAsync();

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
       

        isPlayer = false;

        cursor.SetActive(false);

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

        //Destroy(ballObj);

        //退出したプレイヤーをリストから削除
        characterList.Remove(connnectionId);

        //退出したプレイヤーUIをリストから削除
        standUIList.Remove(connnectionId);

        Debug.Log("退出したユーザー番号:"+ connnectionId);

        //プレイヤー判定をリセット
        isPlayer = false;

        game_State = GAME_STATE.STOP;

        Debug.Log("退出ユーザーオブジェクト削除");
    }

    private async void SendData()
    {
        /*if(roomModel.isMaster)
        {
            animNum = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerManager>().animState;
        
            Debug.Log(animNum.ToString());
        }*/
        //移動情報
        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,      //接続ID
            Pos = characterList[roomModel.ConnectionId].transform.position,         //キャラ位置
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,   //キャラ回転
            AnimId = characterList[roomModel.ConnectionId].transform.GetChild(0).gameObject.GetComponent<PlayerAnimation>().GetAnimId(),      //アニメーションID
        };

        //Debug.Log(moveData.AnimId);
        //プレイヤー移動
        await roomModel.MoveAsync(moveData);



        /* var userState = new UserState()
        {
            isReady = false,                     //準備判定
            isGameCountFinish = false,           //カウントダウン終了判定
            isGameFinish = false,                //ゲーム終了判定
            Ranking = 0,                         //順位 
            Score = 0,                           //獲得スコア
           

        };

        //ユーザー状態 
        await roomModel.UpdateUserStateAsync(userState);*/


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
        //プレイヤーがいなかったら
        if (!characterList.ContainsKey(moveData.ConnectionId))
        {
            return;
        }

        //移動したプレイヤーの位置代入
        //characterList[moveData.ConnectionId].transform.position = moveData.Pos;

        //移動したプレイヤーの角度代入a
        //characterList[moveData.ConnectionId].transform.eulerAngles = moveData.Rotate;

        //Dotweenで移動補完
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);
        //Dotweenで回転補完
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
        //アニメーション更新
        characterList[moveData.ConnectionId].transform.GetChild(0).GetComponent<PlayerAnimation>().SetEnemyAnim(moveData.AnimId);

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

    //ボール発射処理
    private async void ThrowedBall(ThrowData throwData)
    {
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

        //取得者IDを更新
        this.getUserId = getUserId;

        
        Debug.Log(this.getUserId);
       
        Debug.Log("取得者ID更新");
        //フィールド上のボール検索
        ballObj = GameObject.Find("Ball");

        Destroy(ballObj.gameObject);    //ボール削除

        //マスタークライアントリセット
        roomModel.isMaster = false;
    }

    public async void HitBall(HitData hitData)
    {
        //残機リスト
        GameObject lifeList = scoreUIList[hitData.ConnectionId].transform.GetChild(5).gameObject ;
        //残機削除
        Destroy(lifeList.transform.GetChild(0).gameObject);

        //ポイント反映
        Text playerPoint = scoreUIList[hitData.EnemyId].transform.GetChild(4).gameObject.GetComponent<Text>();

        point = int.Parse(playerPoint.text);

        point += hitData.Point;
        playerPoint.text = point.ToString();

        Debug.Log("獲得ポイント:" + hitData.Point);
        //当てたユーザーの得点加算

        Debug.Log("ヒット");
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
        standByUI.SetActive(false);
        game_State = GAME_STATE.START;

        Debug.Log("ゲーム開始");
       
        gameUI.SetActive(true);
        //await roomModel.StartGameAsync();
       

    }
    

    public void GameFinish(Guid connectionId, string userName, bool isFinishAllUser)
    {
        Debug.Log("ゲーム終了");

        
        resultUI.SetActive(true);
    }

   
    public async void OnClickHome()
    {
        //退出
        await roomModel.LeaveAsync();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
