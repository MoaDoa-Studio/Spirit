using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectNode
{
    public GameObject obj; //�ǹ� ������Ʈ
    public Tuple<Vector2Int, Vector2Int> range; //�� ������Ʈ�� ����ϴ� Ÿ�� ����<���� ���, ���� �ϴ�>
    public bool isBreaking = false; //���� ����

    public ObjectNode(GameObject _obj, Tuple<Vector2Int, Vector2Int> _range)
    {
        if (_obj == null)
        {
            Debug.LogError("ObjectNode ������: _obj�� null�Դϴ�.");
        }

        if (_range == null)
        {
            Debug.LogError("ObjectNode ������: _range�� null�Դϴ�.");
        }

        obj = _obj;
        range = _range;
    }
}
