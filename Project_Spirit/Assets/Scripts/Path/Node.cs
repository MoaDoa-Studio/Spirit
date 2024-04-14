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
    public bool isBuild = false;
    public bool isFactory = false;
    public bool isLoot = false;
    public Sprite nodeSprite;
    public Building building;
    
    // Tile 90, 180, 270µµ
    // (0.00000, 0.00000, 0.70711, 0.70711) , (0.00000, 0.00000, 1.00000, 0.00000), (0.00000, 0.00000, -0.70711, 0.70711)
    public Quaternion rotation;
    
    public Node(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
    public float x;
    public float y;
    
}
