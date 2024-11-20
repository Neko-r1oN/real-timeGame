using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    Dictionary<Guid,GameObject>characterList = new Dictionary<Guid, GameObject>();

    async void Start()
    {
        //ユーザーが入室した際にOnJoinedUserメゾットを実行するようにモデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;

        roomModel.LeavedUser += this.LeavedUser;

        //待機
        await roomModel.ConnectAsync();
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

        Debug.Log("退出完了");
    }

    //ユーザーが退出したときの処理
    private async void LeavedUser(Guid connnectionId)
    {
        //退出したプレイヤーのオブジェクト削除
        Destroy(characterList[connnectionId]);
        //退出したプレイヤーをリストから削除
        characterList.Remove(connnectionId);
    }
}
