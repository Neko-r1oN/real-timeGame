////////////////////////////////////////////////////////////////////////////
///
///  ユーザーAPIスクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Shared.Interfaces.StreamingHubs;
using Shared.Model.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class UserModel : BaseModel
{
    public int userId { get; set; }       //登録ユーザーID
    public string userName { get; set; }   //登録ユーザー名

    public string authToken { get; set; }   //登録ユーザートークン

    private static UserModel instance;

    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObj = new GameObject("UserModel");
                //GameObject生成、NetworkManagerコンポーネントを追加
                instance = gameObj.AddComponent<UserModel>();
                //シーン移動時に削除しないようにする
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    //ユーザー移動通知
    public Action<MoveData> MovedUser { get; set; }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    /// <param name="name">ユーザー名</param>
    /// <returns>登録成功 or 失敗</returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);
        try
        {//登録成功
            Debug.Log("サーバー接続成功");
            User user = await client.RegistUserAsync(name);
            //ファイルにユーザーを保存
            this.userName = name;
            this.userId = user.Id;
            this.authToken = user.Token;
            SaveUserData();
            return true;
        }
        catch (Exception e)
        {//登録失敗
            Debug.Log("サーバー接続エラーか既に登録されてる名前です");
            Debug.Log(e);
            return false;
        }
    }

    //データ保存処理
    private void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.Name = this.userName;
        saveData.UserID = this.userId;
        saveData.AuthToken = this.authToken;
        string json = JsonConvert.SerializeObject(saveData);

        //ファイルにJsonを保存
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");   // Application.persistentDataPathは保存ファイルを置く場所

        //Json形式で書き込み
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    //ユーザー情報読み込み処理
    public bool LoadUserData()
    {
        //ローカルに存在しない場合
        if (!File.Exists(Application.persistentDataPath + "/saveData.json")) return false;
        
        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

        //ローカルファイルから各種値を取得
        this.userId = saveData.UserID;
        this.userName = saveData.Name;
        this.authToken = saveData.AuthToken;

        //読み込み判定
        return true;
    }

    /// <summary>
    /// ユーザー取得処理
    /// </summary>
    /// <param name="userId">取得したいユーザーID</param>
    /// <returns>登録成功 or 失敗</returns>
    public async UniTask<bool> GetUserInfoAsync(int userId)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {//登録成功
            Debug.Log("サーバー接続成功");
            User user = await client.GetUserInfoAsync(userId);

            Debug.Log("取得成功");
            //ファイルにユーザーを保存
            this.userName = user.Name;
            this.userId = user.Id;
            this.authToken = user.Token;

            //SaveUserData();

            return true;
        }
        catch (Exception e)
        {//登録失敗
            Debug.Log("取得失敗");
            Debug.Log("サーバー接続エラーか存在しないIDです");
            Debug.Log(e);
            return false;
        }
    }
}
