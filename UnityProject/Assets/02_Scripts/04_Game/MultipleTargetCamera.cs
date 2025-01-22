using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public Camera cam;
    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = 0.5f;

    public float minZoom = 5;
    public float maxZoom = 0.5f;
    public float zoomLimiter = 0.5f;

    private Vector3 velocity;

    //取得タグ関連
    private string ballTag = "Ball";              //取得タグ名(ボール)
    private string easyBallTag = "EasyBall";      //取得タグ名(イージーボール)
    private string playerTag = "Player";          //取得タグ名(ボール所持者)
    private string enemyTag = "Enemy";            //取得タグ名(ボール所持者)
    private string enemyBallTag = "BallPlayer";   //取得タグ名(ボール所持者)

    private GameObject[] searchObj;            //座標代入用

    private void Start()
    {
        
    }
    private void Reset()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        targets.Clear();
        GetObj();

        if (targets.Count == 0) return;

        Move();
        Zoom();
    }

    private void Zoom()
    {
        var newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    private void Move()
    {
        var centerPoint = GetCenterPoint();
        var newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1) return targets[0].position;
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    private void GetObj()
    {
        //ボールタグ取得
        searchObj = ObjSerch(ballTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //ボール所持者タグ取得
        searchObj = ObjSerch(easyBallTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //プレイヤータグ取得
        searchObj = ObjSerch(playerTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //敵タグ取得
        searchObj = ObjSerch(enemyTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //ボール所持者敵タグ取得
        searchObj = ObjSerch(enemyBallTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }

    }

    private GameObject[] ObjSerch(string getTagName)
    {

        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;

        
        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(getTagName);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
        if (objs.Length == 0)
        {
            return null;
        }
       

        //配列を返す
        return objs;
    }
}