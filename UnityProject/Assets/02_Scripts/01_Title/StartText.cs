using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{

    //インスペクターから設定するか、初期化時にGetComponentして、Imageへの参照を取得しておく。
    [SerializeField]
    Text img;

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
        if (img == null)
            img = GetComponent<Text>();
    }

    void Update()
    {
        //Color.Lerpに開始の色、終了の色、0～1までのfloatを渡すと中間の色が返される。
        //Mathf.PingPongに経過時間を渡すと、0～1までの値が返される。
        img.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time / duration, 1.0f));
    }
}
