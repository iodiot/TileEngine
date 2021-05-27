using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class GameUpdaterScript : MonoBehaviour
{
    public Sprite TileSprite;
    public Sprite OtherTileSprite;
    public Sprite KnightSprite1;
    public Sprite KnightSprite2;

    [Range(0.1f, 0.8f)]
    public float ScaleFactor;

    [Range(1, 100)]
    public int Width;

    [Range(1, 100)]
    public int Height;

    private GameObject[,] tileMap;

    private GameObject knight;
    private int knightX, knightY;

    private GameObject parentGameObject;

    private float oldScaleFactor;
    private int oldWidth, oldHeight;

    private void BuildTileMap()
    {
        tileMap = new GameObject[oldWidth, oldHeight];

        parentGameObject = new GameObject("Sprites");

        for (var y = 0; y < oldHeight; ++y)
        {
            for (var x = 0; x < oldWidth; ++x)
            {
                var go = new GameObject("Sprite " + x.ToString() + ":" + y.ToString());
                go.transform.SetParent(parentGameObject.transform);
                go.transform.position = new Vector3(x, y, 0) * ScaleFactor - new Vector3(oldWidth, oldHeight, 0) * ScaleFactor * .5f;

                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = TileSprite;
                renderer.sortingOrder = -y;

                tileMap[x, y] = go;
            }
        }
    }

    private void DestroyTileMap()
    {
        if (tileMap != null)
        {
            for (var y = 0; y < oldHeight; ++y)
            {
                for (var x = 0; x < oldWidth; ++x)
                {
                    if (tileMap[x, y] != null)
                    {
                        DestroyImmediate(tileMap[x, y]);
                    }
                }
            }
        }

        tileMap = null;

        DestroyImmediate(parentGameObject);
    }

    private void UpdateKnight()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            knightY += 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            knightY -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            knightX -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            knightX += 1;
        }

        knightX = (int)Mathf.Clamp(knightX, 0, oldWidth - 1);
        knightY = (int)Mathf.Clamp(knightY, 0, oldHeight - 1);

        knight.transform.position = (new Vector3(knightX, knightY, 0) + new Vector3(0f, .5f, 0) - new Vector3(oldWidth, oldHeight, 0) * .5f) * ScaleFactor;

        var ticks = (int)(Time.realtimeSinceStartup * 1000);

        if (ticks % 20 == 0)
        {
            var r = knight.GetComponent<SpriteRenderer>();

            if (r.sprite == KnightSprite1)
            {
                r.sprite = KnightSprite2;
            }
            else if (r.sprite == KnightSprite2)
            {
                r.sprite = KnightSprite1;
            }
        }

        tileMap[knightX, knightY].GetComponent<SpriteRenderer>().sprite = OtherTileSprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        oldScaleFactor = ScaleFactor;
        oldWidth = Width;
        oldHeight = Height;

        BuildTileMap();

        knight = new GameObject("Knight");
        var renderer = knight.AddComponent<SpriteRenderer>();
        renderer.sprite = KnightSprite1;

        knightX = oldWidth / 2;
        knightY = oldHeight / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Width != oldWidth) || (Height != oldHeight) || (ScaleFactor != oldScaleFactor))
        {
            DestroyTileMap();

            oldScaleFactor = ScaleFactor;
            oldWidth = Width;
            oldHeight = Height;

            BuildTileMap();
        }

        UpdateKnight();
    }
}
