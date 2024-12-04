using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class LockOnSystem : MonoBehaviour
{
   
    private string tagName = "Enemy";        // �C���X�y�N�^�[�ŕύX�\

    private GameObject searchNearObj;         // �ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)

  

    void Start()
    {
        searchNearObj = GameObject.Find("MyPlay");
        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearObj = Serch();
    }

    void Update()
    {

       

             // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        searchNearObj = Serch();

        if (searchNearObj != null)
        {

            Debug.Log(searchNearObj);
            

            // �v�����Ԃ����������āA�Č���
        }
        
    }

    /// <summary>
    /// �w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    /// </summary>
    /// <returns></returns>
    private GameObject Serch()
    {

        // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;

        // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
        GameObject searchTargetObj = null;

        // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă�������)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objs����P����obj�ϐ��Ɏ��o��
        foreach (GameObject obj in objs)
        {

            // obj�Ɏ��o�����Q�[���I�u�W�F�N�g�ƁA���̃Q�[���I�u�W�F�N�g�Ƃ̋������v�Z���Ď擾
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistance���X�V
                nearDistance = distance;
                this.gameObject.transform.position = searchNearObj.gameObject.transform.position;
                // searchTargetObj���X�V
                searchTargetObj = obj;
            }
        }

        //�ł��߂������I�u�W�F�N�g��Ԃ�
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
                    shortestDistance = distance; // �ŒZ�����̍X�V
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
