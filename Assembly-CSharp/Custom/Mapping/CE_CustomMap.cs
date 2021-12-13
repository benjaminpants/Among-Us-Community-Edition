using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InnerNet;

public class CE_CustomMap
{
    private static bool MapTestingActive = false;
    private static void ClearMapCollision(ShipStatus map)
    {
        Collider2D[] colids = map.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colids)
        {
            UnityEngine.Object.Destroy(col.gameObject);
        }
    }

    public static void SpawnSprite(int x, int y, bool Solid)
    {
        Texture2D texture;

        if (Solid)
        {
            string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "TileTest2.png");
            texture = CE_TextureNSpriteExtensions.LoadPNG(path);
            texture.filterMode = FilterMode.Point;
        }
        else
        {
            string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "TileTest.png");
            texture = CE_TextureNSpriteExtensions.LoadPNG(path);
            texture.filterMode = FilterMode.Point;
        }


        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));
        GameObject go = new GameObject("Test");
        go.layer = LayerMask.NameToLayer("Ship");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        var position = renderer.transform.position;
        position.x = 0.5f * x;
        position.y = 0.5f * y;
        position.z = (position.y / 1000f) + 0.5f;
        renderer.transform.position = position;
        renderer.sprite = sprite;

        if (Solid)
        {
            BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
            boxCollider.transform.position = renderer.transform.position;
            boxCollider.size = new Vector3(0.5f, 0.5f);
        }
    }

    public static void MapTest(ShipStatus map)
    {
        if (!MapTestingActive) return;
        ClearMapCollision(map);
        // return mapInfos[2];
        for (int x = -25; x < 25; x++)
        {
            for (int y = -25; y < 25; y++)
            {
                bool isSolid = (x == -25 || y == -25 || y == 24 || x == 24);
                SpawnSprite(x, y, isSolid);
            }
        }
    }
}
