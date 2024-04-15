using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[SerializeField]
public class SpriteTile : Tile
{
    public new Sprite sprite;
    public Quaternion rotation;
    public SpriteTile(Sprite sprite, Quaternion rotation)
    {
        this.sprite = sprite;
        this.rotation = rotation;
    }
}
