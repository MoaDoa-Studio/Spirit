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

    // 타일매니저 클래스의 인스턴스 접근
    Node[,] nodes;

    private void Start()
    {   
        nodes = TileDataManager.instance.nodes;

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        InstantiateTile();
        checkTileSprite();
    }

    public void InstantiateTile()
    {
        nodes = new Node[sizeX, sizeY];

        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0;  j < sizeY; j++)
            {
                nodes[i, j] = new Node();
                Debug.Log(nodes[i, j]);
            }
        }

    }
    // TileDataManager에 스프라이트 값 저장.
    public void checkTileSprite()
    {
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
                        //TileDataManager.instance.tileArray[i,j] = new TileDataManager(pos,tileSprite);
                        nodes[i,j].nodeSprite = tileSprite;
                        Debug.Log(nodes[i,j].nodeSprite);
                    }
                }
            }
        }
    }



}
