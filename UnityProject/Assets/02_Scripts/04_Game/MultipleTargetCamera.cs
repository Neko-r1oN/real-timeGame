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

    //�擾�^�O�֘A
    private string ballTag = "Ball";              //�擾�^�O��(�{�[��)
    private string easyBallTag = "EasyBall";      //�擾�^�O��(�C�[�W�[�{�[��)
    private string playerTag = "Player";          //�擾�^�O��(�{�[��������)
    private string enemyTag = "Enemy";            //�擾�^�O��(�{�[��������)
    private string enemyBallTag = "BallPlayer";   //�擾�^�O��(�{�[��������)

    private GameObject[] searchObj;            //���W����p

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
        //�{�[���^�O�擾
        searchObj = ObjSerch(ballTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //�{�[�������҃^�O�擾
        searchObj = ObjSerch(easyBallTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //�v���C���[�^�O�擾
        searchObj = ObjSerch(playerTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //�G�^�O�擾
        searchObj = ObjSerch(enemyTag);

        if (searchObj != null)
        {
            foreach (GameObject player in searchObj)
            {
                targets.Add(player.gameObject.transform);
            }
        }
        //�{�[�������ғG�^�O�擾
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

        // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;

        
        // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
        GameObject[] objs = GameObject.FindGameObjectsWithTag(getTagName);

        // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă�������)
        if (objs.Length == 0)
        {
            return null;
        }
       

        //�z���Ԃ�
        return objs;
    }
}