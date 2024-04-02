using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetTiles : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵 객체 할당.
    public Sprite targetSprite; // 찾고자 하는 스프라이트

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    int sizeX, sizeY;

    TileDataManager [,] tileArray;


    private void Awake()
    {   
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        checkTileSprite();
    }

    // TileDataManager에 스프라이트 값 저장.
    public void checkTileSprite()
    {
        //tileArray = new TileDataManager[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                // 타일 위치 설정
                Vector3Int tilePosition = new Vector3Int(i , j);
            
                TileBase tile = tilemap.GetTile(tilePosition);
                
                if(tile != null)
                {
                    // 가져온 타일의 스프라이트를 확인한다.
                    Sprite tileSprite = (tile as Tile).sprite;

                    if(tileSprite == targetSprite) 
                    {
                        Vector2Int pos = new Vector2Int(i , j);
                        Sprite spr = tileSprite;
                       //TileDataManager.instance.tileArray[i,j] = new TileDataManager(pos,tileSprite);
                       
                    }
                }
            }
        }
    }



}
