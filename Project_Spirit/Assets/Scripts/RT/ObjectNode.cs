using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectNode
{
    public GameObject obj; //건물 오브젝트
    public Tuple<Vector2Int, Vector2Int> range; //그 오브젝트가 사용하는 타일 범위<우측 상단, 좌측 하단>
    public bool isBreaking = false; //접촉 여부

    public ObjectNode(GameObject _obj, Tuple<Vector2Int, Vector2Int> _range)
    {
        if (_obj == null)
        {
            Debug.LogError("ObjectNode 생성자: _obj는 null입니다.");
        }

        if (_range == null)
        {
            Debug.LogError("ObjectNode 생성자: _range는 null입니다.");
        }

        obj = _obj;
        range = _range;
    }
}
