using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InnerNet;

public class CE_CustomMap
{
    public static bool MapTestingActive = false;


    public static NormalPlayerTask CreateTask(Type tasktype, SystemTypes systype, int maxstep)
    {
        GameObject task = UnityEngine.GameObject.Instantiate(new GameObject());
        NormalPlayerTask datatask = task.AddComponent(tasktype) as NormalPlayerTask;
        datatask.StartAt = systype;
        datatask.MaxStep = maxstep;
        return GameObject.Instantiate(task).GetComponent(tasktype) as NormalPlayerTask;
    }

    public static SystemConsole CreateTaskConsole(Type minigametype, Vector3 transf, string name, Sprite sprite, NormalPlayerTask task)
    {
        GameObject ins = GameObject.Instantiate(new GameObject());
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one;
        col2d.isTrigger = true;
        SystemConsole console = ins.AddComponent<SystemConsole>();
        SpriteRenderer img = ins.AddComponent<SpriteRenderer>();
        img.sprite = sprite;
        img.material.shader = Shader.Find("Sprites/Outline");
        console.Image = img;
        ins.transform.position = transf;
        Minigame uploaddat = CE_PrefabHelpers.FindPrefab(name, minigametype) as Minigame;
        console.MinigamePrefab = uploaddat;
        console.IsCustom = true;
        console.TaskOverride = task;
        return console;
    }
    private static void ClearMapCollision(ShipStatus map)
    {
        foreach (Transform col in map.transform)
        {
            if (true)
            {
                UnityEngine.Object.Destroy(col.gameObject);
            }
        }
    }

    public static GameObject SpawnSprite(int x, int y, bool Solid)
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
        go.layer = 9;
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        var position = renderer.transform.position;
        renderer.material = new Material(Shader.Find("Unlit/MaskShader"));
        position.x = 0.5f * x;
        position.y = 0.5f * y;
        position.z = (position.y / 1000f) + 0.5f;
        renderer.transform.position = position;
        renderer.sprite = sprite;

        if (Solid)
        {
            go.layer = LayerMask.NameToLayer("IlluminatedBlocking");
            position.z = 0.5f;
            BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
            boxCollider.transform.position = renderer.transform.position;
            boxCollider.size = new Vector3(0.5f, 0.5f);
        }
        return go;
    }

    public static void MapTest(ShipStatus map)
    {
        Texture2D texture;
        string path = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("Mapping"), "TileTest.png");
        texture = CE_TextureNSpriteExtensions.LoadPNG(path);
        texture.filterMode = FilterMode.Point;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.65f));
        if (!MapTestingActive) return;
        foreach (NormalPlayerTask mp in map.CommonTasks)
        {
            UnityEngine.GameObject.Destroy(mp.gameObject);
        }
        map.CommonTasks = new NormalPlayerTask[1];
        foreach (NormalPlayerTask mp in map.NormalTasks)
        {
            UnityEngine.GameObject.Destroy(mp.gameObject);
        }
        map.NormalTasks = new NormalPlayerTask[1];
        foreach (NormalPlayerTask mp in map.LongTasks)
        {
            UnityEngine.GameObject.Destroy(mp.gameObject);
        }
        map.LongTasks = new NormalPlayerTask[1];
        NormalPlayerTask uptask = CreateTask(typeof(NormalPlayerTask),SystemTypes.Weapons,69);
        CreateTaskConsole(typeof(WeaponsMinigame),new Vector3(3f,3f, (3f / 1000f) + 0.5f),"WeaponsMinigame",sprite,uptask);
        map.CommonTasks[0] = uptask;
        map.LongTasks[0] = CreateTask(typeof(UploadDataTask), SystemTypes.Cafeteria, 2);
        map.NormalTasks[0] = CreateTask(typeof(UploadDataTask), SystemTypes.Electrical, 2);
        ClearMapCollision(map);
        for (int x = -5; x < 5; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                bool isSolid = (x == -5 || y == -5 || y == 4 || x == 4);
                SpawnSprite(x, y, BoolRange.Next(0.1f));
            }
        }
    }
}
