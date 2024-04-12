using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : MonoBehaviour
{
    public int nodeValue;   
    public int rotationStack;   
    public int stack = 0;
    public int spiritElement;
    public bool isSignal = false;
    public bool isWalk = false;
    public bool isFactory = false;
    public bool isLoot = false;
    public Sprite nodeSprite;
   
    // Tile 90, 180, 270µµ
    // (0.00000, 0.00000, 0.70711, 0.70711) , (0.00000, 0.00000, 1.00000, 0.00000), (0.00000, 0.00000, -0.70711, 0.70711)
    public Quaternion rotation;
    
    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public int x;
    public int y;
  
    public (int[,] , int[,]) Pair(int[,] array1, int[,]array2)
    {
        return (array1, array2);
    }
}
