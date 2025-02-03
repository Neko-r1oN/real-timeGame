////////////////////////////////////////////////////////////////////////////
///
///  スタートテキストスクリプト
///  Author : 川口京佑  2025.01/28
///
////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{
    //インスペクターから設定するか、初期化時にGetComponentして、Imageへの参照を取得しておく。
    [SerializeField]
    Text text;

    [Header("1ループの長さ(秒単位)")]
    [SerializeField]
    [Range(0.1f, 5.0f)]
    float duration = 1.0f;


    [Header("ループ開始時の色")]
    [SerializeField]
    Color32 startColor = new Color32(255, 255, 255, 255);
    //ループ終了(折り返し)時の色を0～255までの整数で指定。
    [Header("ループ終了時の色")]
    [SerializeField]
    Color32 endColor = new Color32(255, 255, 255, 0);


    //インスペクターから設定した場合は、GetComponentする必要がなくなる為、Awakeを削除しても良い。
    void Awake()
    {
        if (text == null) text = GetComponent<Text>();
    }

    void Update()
    {
        text.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time / duration, 1.0f));
    }
}
