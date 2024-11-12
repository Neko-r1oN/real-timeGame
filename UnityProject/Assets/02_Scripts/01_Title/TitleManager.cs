using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class TitleManager : MonoBehaviour
{

    
    [SerializeField] UserModel userModel;
    [SerializeField] Text nameText;

    [SerializeField] GameObject NameField;
    [SerializeField] GameObject RegistTrue;
    [SerializeField] GameObject RegistFalse;


    
    // Start is called before the first frame update
    void Start()
    {
        RegistTrue.SetActive(false);
        RegistFalse.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickRegist()
    {
       
        userModel.RegistUserAsync(nameText.text);

        if(userModel)
        {
            RegistTrue.SetActive(true); 

        }
        
    }
}
