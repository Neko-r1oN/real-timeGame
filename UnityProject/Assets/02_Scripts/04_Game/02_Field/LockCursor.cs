using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("x²‚Ì‰ñ“]Šp“x")]
    private float rotateX = 0;

    [SerializeField]
    [Tooltip("y²‚Ì‰ñ“]Šp“x")]
    private float rotateY = 0;

    [SerializeField]
    [Tooltip("z²‚Ì‰ñ“]Šp“x")]
    private float rotateZ = 0;


    public float time, changeSpeed;
    public bool enlarge;

    private void Start()
    {
        enlarge = true;
    }
    // Update is called once per frame
    void Update()
    {
        // X,Y,Z²‚É‘Î‚µ‚Ä‚»‚ê‚¼‚êAw’è‚µ‚½Šp“x‚¸‚Â‰ñ“]‚³‚¹‚Ä‚¢‚éB
        // deltaTime‚ğ‚©‚¯‚é‚±‚Æ‚ÅAƒtƒŒ[ƒ€‚²‚Æ‚Å‚Í‚È‚­A1•b‚²‚Æ‚É‰ñ“]‚·‚é‚æ‚¤‚É‚µ‚Ä‚¢‚éB
        gameObject.transform.Rotate(new Vector3(rotateX, rotateY, rotateZ) * Time.deltaTime);


        changeSpeed = Time.deltaTime * 0.1f;

        if (time < 0)
        {
            enlarge = true;
        }
        if (time > 0.7f)
        {
            enlarge = false;
        }

        if (enlarge == true)
        {
            time += Time.deltaTime;
            transform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
        else
        {
            time -= Time.deltaTime;
            transform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
    }

}
