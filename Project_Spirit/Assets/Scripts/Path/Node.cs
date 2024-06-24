using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public int nodeValue;   
    public int rotationStack;   
    public int stack = 0;
    public int spiritElement;
    public int rock_reserve;
    public int wood_reserve;
    public bool isSignal = false;
    public bool isWalk = false;
    public bool isBuild = false; 
    public bool isFactory = false;
    public bool isLoot = false;
    public Sprite nodeSprite;
    public Building building;
    public ResourceBuilding resourceBuilding;
   
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
    
    enum NodeType
    {
        None = 0,
        Building = 1,
        Cradle = 2,
        Road = 3,
        Resource = 4,
        Mark = 5,
        Rock = 6,
        Wood = 7,
        Elemental_Essence = 8
    }
    NodeType nodeType = NodeType.None;
    public void SetNodeType(int type)
    {
        nodeType = (NodeType)type;
       
    }

    public int GetNodeType()
    {
        return (int)nodeType;
    }

   
}
