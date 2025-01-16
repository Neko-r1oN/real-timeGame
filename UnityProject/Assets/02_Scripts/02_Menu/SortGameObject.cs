using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SortGameObject : MonoBehaviour
{
    [ContextMenu("Sort game object")]
    private void Sort()
    {
        List<Transform> objList = new List<Transform>();

        // �q�K�w��GameObject�擾
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            objList.Add(transform.GetChild(i));
        }

        // �I�u�W�F�N�g�𖼑O�ŏ����\�[�g
        // ��������p�r�ɍ��킹�ĕύX���Ă�������
        objList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));

        // �\�[�g���ʏ���GameObject�̏����𔽉f
        foreach (var obj in objList)
        {
            obj.SetSiblingIndex(childCount - 1);
        }
    }


    private void Update()
    {
        List<Transform> objList = new List<Transform>();

        // �q�K�w��GameObject�擾
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            objList.Add(transform.GetChild(i));
        }

        // �I�u�W�F�N�g�𖼑O�ŏ����\�[�g
        // ��������p�r�ɍ��킹�ĕύX���Ă�������
        objList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));

        // �\�[�g���ʏ���GameObject�̏����𔽉f
        foreach (var obj in objList)
        {
            obj.SetSiblingIndex(childCount - 1);
        }
    }
}