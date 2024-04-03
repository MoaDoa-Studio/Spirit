using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int nodeValue;   // 타일의 값
    public Sprite nodeSprite;   // 타일 스프라이트
    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public int x;
    public int y;
  
}
