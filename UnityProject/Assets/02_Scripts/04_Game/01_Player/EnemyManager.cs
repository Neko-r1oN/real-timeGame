using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool _isRendering = false;

    private SpriteRenderer img = null;
    public bool GetIsRendering()
    {
        return _isRendering;
    }

    private void Start()
    {
        GameObject parentObject = this.gameObject;

        img = parentObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        

    }
    void Update()
    {

        if (img.isVisible)
        {
            _isRendering = false;
        }
        else
        {
            _isRendering = true;
        }
    }
}