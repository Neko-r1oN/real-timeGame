using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;                   //DOTweenを使うときはこのusingを入れる


public class GameDirector : MonoBehaviour
{
    //プレイヤープレハブ
    [SerializeField] GameObject characterPrefab;
    //ルームモデル
    [SerializeField] RoomModel roomModel;

    [SerializeField] Text roomName;
    [SerializeField] Text userId;

    //プレイヤー生成親オブジェクト
    [SerializeField] private GameObject spawnObg;
    //プレイヤー生成位置
    [SerializeField] private Transform spawnPos;

    //プレイヤー存在判定
    private bool isPlayer;
    //Dotween遷移時間
    private float dotweenTime = 0.1f;

    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();

    async void Start()
    {
        //ユーザーが入室した際にOnJoinedUserメゾットを実行するようにモデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.LeavedUser += this.LeavedUser;

        roomModel.MovedUser += this.MovedUser;
        
        isPlayer = false;
        //待機
        await roomModel.ConnectAsync();

    }

    /// <summary>
    /// Unityの設定から１秒ごとの通信回数を指定
    /// </summary>
    private async void FixedUpdate()
    {
        if (!isPlayer) return;

        var moveData = new MoveData()
        {
            ConnectionId = roomModel.ConnectionId,
            Pos = characterList[roomModel.ConnectionId].transform.position,
            Rotate = characterList[roomModel.ConnectionId].transform.eulerAngles,

        };

        //移動
        await roomModel.MoveAsync(moveData);

    }



    //入室処理
    public async void JoinRoom()
    {
        Debug.Log("ルーム名:"+roomName.text);
        Debug.Log("ユーザーID;" + userId.text);

        await roomModel.JoinAsync(roomName.text, int.Parse(userId.text));     //ルーム名とユーザーIDを渡して入室

        Debug.Log("入室完了");
    }

    //ユーザーが入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity, spawnPos); //インスタンス生成

        //characterObject.transform.position = new Vector3(0,0,0);   //座標指定
        //characterObject.transform.parent = spawnObg.transform;
        characterList[user.ConnectionId] = characterObject;        //フィールドで保持

        if (user.ConnectionId == roomModel.ConnectionId)
        {
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();

        }
        isPlayer = true;


    }

    //切断処理
    public async void DisConnectRoom()
    {
        //退出処理
        await roomModel.LeaveAsync();

        //MagicOnion切断処理
        //await roomModel.DisConnectAsync();

        // プレイヤーオブジェクトの削除
        foreach (Transform player in spawnObg.transform)
        {
            Destroy(player.gameObject);
        }

        isPlayer = false;

        Debug.Log("退出完了");
    }

    //ユーザーが退出したときの処理
    private async void LeavedUser(Guid connnectionId)
    {
        //退出したプレイヤーのオブジェクト削除
        Destroy(characterList[connnectionId]);
        //退出したプレイヤーをリストから削除
        characterList.Remove(connnectionId);

        //プレイヤー判定をリセット
        isPlayer = false;

        Debug.Log("退出ユーザーオブジェクト削除");
    }

    //ユーザーが移動したときの処理
    private async void MovedUser(MoveData moveData)
    {
        //移動したプレイヤーの位置代入
        //characterList[moveData.ConnectionId].transform.position = moveData.Pos;

        //Dotweenで移動補完
        characterList[moveData.ConnectionId].transform.DOMove(moveData.Pos, dotweenTime).SetEase(Ease.Linear);

        //移動したプレイヤーの角度代入
        //characterList[moveData.ConnectionId].transform.eulerAngles = moveData.Rotate;

        //Dotweenで回転補完
        characterList[moveData.ConnectionId].transform.DORotate(moveData.Rotate, dotweenTime).SetEase(Ease.Linear);
    }

    //ゲームが開始された時の処理
    private async void StartedGame(bool isStart)
    {
        await roomModel.StartGameAsync(isStart);

        
    }

}
