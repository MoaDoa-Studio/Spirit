using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : MonoBehaviour
{
    public int nodeValue;   // 타일의 값
    public bool isSignal = false;
    public bool isWalk = false;
    public int rotationStack;   // 플레이어가 해당 노드를 몇번 회전 시켰는지? 
    public int stack = 0;
    public Sprite nodeSprite;   // 타일 스프라이트
    // Tile 90, 180, 270도
    // (0.00000, 0.00000, 0.70711, 0.70711) , (0.00000, 0.00000, 1.00000, 0.00000), (0.00000, 0.00000, -0.70711, 0.70711)
    public Quaternion rotation;
    
    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public int x;
    public int y;
  
}
