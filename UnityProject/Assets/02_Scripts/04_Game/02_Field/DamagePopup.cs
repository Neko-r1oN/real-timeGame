using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] GameObject targets;             //対象の敵
    [SerializeField] GameObject canvas;    //ダメージUI生成キャンバス
    [SerializeField] GameObject damageUI;    //ダメージUI

    public float deleteTime = 1.0f;

    void OnCollisionEnter(Collision other)
    {

        
        //クリア判定オブジェクト(デバッグ用)
        if (other.gameObject.tag == "Player")
        {
            PopDamage();
        }


    }
    //ダメージテキストUI生成関数
    public void PopDamage(/*GameObject target*/)
    {
        /*targets = target;*/

        //コピー生成
        var obj = new GameObject("Target");
        var ui = Instantiate(damageUI);

        //親変更
        obj.transform.SetParent(targets.transform);
        ui.transform.SetParent(canvas.transform);

        ui.SetActive(true);

        var circlePos = Random.insideUnitCircle * 0.6f;
        //UIをターゲットの画面上の位置に配置
        obj.transform.position = transform.position + Vector3.up * Random.Range(3.0f,4.0f) + new Vector3(circlePos.x,circlePos.y);
        ui.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main,obj.transform.position);

        //指定時間後に削除
        Destroy(obj, deleteTime);
        Destroy(ui, deleteTime);
    }
}
