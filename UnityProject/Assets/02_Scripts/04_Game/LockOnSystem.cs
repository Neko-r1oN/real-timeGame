using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class LockOnSystem : MonoBehaviour
{
   
    private string tagName = "Enemy";        // インスペクターで変更可能

    private GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)

  

    void Start()
    {
        searchNearObj = GameObject.Find("MyPlay");
        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = Serch();
    }

    void Update()
    {

       

             // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        searchNearObj = Serch();

        if (searchNearObj != null)
        {

            Debug.Log(searchNearObj);
            

            // 計測時間を初期化して、再検索
        }
        
    }

    /// <summary>
    /// 指定されたタグの中で最も近いものを取得
    /// </summary>
    /// <returns></returns>
    private GameObject Serch()
    {

        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;

        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;

        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objsから１つずつobj変数に取り出す
        foreach (GameObject obj in objs)
        {

            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistanceを更新
                nearDistance = distance;
                this.gameObject.transform.position = searchNearObj.gameObject.transform.position;
                // searchTargetObjを更新
                searchTargetObj = obj;
            }
        }

        //最も近かったオブジェクトを返す
        return searchTargetObj;
    }

}



    /*

    private float shortestDistance;
    private GameObject nearestEnemy;
    private Image image;
    private RectTransform rectTransform;

    private GameObject player;

    void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        player = GameObject.Find("Player");
        image.enabled = false;
    }
    void Update()
    {
        GetNearEnemy();
        LockonTarget();
    }

    void GetNearEnemy()
    {
        shortestDistance = 100f;
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemys != null)
        {
            foreach (GameObject enemy in enemys)
            {
                if (!enemy.GetComponent<EnemyManager>().GetIsRendering())
                {
                    return;
                }

                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance; // 最短距離の更新
                    nearestEnemy = enemy;
                }
            }
        }

    }

    void LockonTarget()
    {
        if (nearestEnemy != null)
        {
            image.enabled = true;
            Vector2 targetPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, nearestEnemy.transform.position);
            rectTransform.position = targetPoint;
        }
    }
    
}*/
