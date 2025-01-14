using UnityEngine;

public class AngleManager : MonoBehaviour
{
    public bool isLeft;     //画像の向き

    private SpriteRenderer img = null;

    GameObject cam;

  
    
    private void Start()
    {
        cam = GameObject.Find("MainCamera");


        isLeft = false;
    }
    private void FixedUpdate()
    {
        //カメラに対して平行に画像が表示される処理

        // 対象物と自分自身の座標からベクトルを算出
        Vector3 vector3 = cam.transform.position - this.transform.position;

        // もし上下方向の回転はしない(Baseオブジェクトが床から離れないようにする)ようにしたければ以下のようにする。
        vector3.y = 0f;

        //Quaternion(回転値)を取得

        Quaternion quaternion = Quaternion.LookRotation(vector3);
        // 算出した回転値をこのゲームオブジェクトのrotationに代入
        this.transform.rotation = quaternion;
    }

    void Update()
    {
        if (isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = false;

        }
        if (!isLeft)
        {
            this.gameObject.transform.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

    }
    public bool GetAngle()
    {
        return isLeft;
    }

    //エネミー用
    public void SetAngle(bool isLeft)
    {
        this.isLeft = isLeft;
    }
}