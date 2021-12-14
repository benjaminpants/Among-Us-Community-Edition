using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InnerNet;
using Newtonsoft.Json;

public class CEM_TaskData
{
    public Type minigametype;
    public Type tasktype = typeof(NormalPlayerTask);
    public string MinigameName = "Invalid";

    public CEM_TaskData(Type mgt, Type tt, string mn)
    {
        minigametype = mgt;
        tasktype = tt;
        MinigameName = mn;
    }
    public CEM_TaskData(Type mgt, string mn)
    {
        minigametype = mgt;
        MinigameName = mn;
    }
}

public static class CE_CustomMapManager
{
    public static List<CE_MapInfo> MapInfos = new List<CE_MapInfo>();

    public static void Initialize()
    {
        MapInfos.Add(new CE_MapInfo("Dumb",new string[13]{
            "Happy Place",
            "Sad Place",
            "Mad Place",
            "Bad Place",
            "Good Place",
            "Evil Place",
            "Holy Place",
            "Test Place",
            "Sus Place",
            "The Forest",
            "Breh",
            "Memey Place",
            "Altenerate Skeld"
        }));
    }

    public static CE_MapInfo GetCurrentMap()
    {
        return MapInfos[PlayerControl.GameOptions.MapId];
    }
}

 

public class CE_CustomMap
{
    private static bool MapTestingActive = true;
//    public static bool MapTestingActiveMapId = false;
    private static void ClearMapCollision(ShipStatus map)
    {
        Collider2D[] colids = map.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colids)
        {
            UnityEngine.Object.Destroy(col.gameObject);
        }
    }
    
    /*  public static Console CreateConsole(Vector3 transf, Sprite sprite, TaskTypes tt, IntRange range, SystemTypes room, int consoleid)
    {
        GameObject ins = new GameObject();
        BoxCollider2D col2d = ins.AddComponent<BoxCollider2D>();
        col2d.size = Vector2.one / 2f;
        col2d.isTrigger = true;
        ins.transform.position = transf;
        Console console = ins.AddComponent<Console>();
        SpriteRenderer img = ins.AddComponent<SpriteRenderer>();
        img.sprite = sprite;
        img.material.shader = Shader.Find("Sprites/Outline");
        console.Image = img;
        console.Room = room;
        TaskSet ts = new TaskSet();
        ts.taskStep = range;
        ts.taskType = tt;
        console.ValidTasks = new TaskSet[] {
            ts
        };
        console.TaskTypes = new TaskTypes[]
        {
            tt
        };
        console.ConsoleId = consoleid;
        return console;
  */  }

      CreateSystemConsole(typeof(TaskAdderGame), new Vector3(5f, 5f, 0.5f), "TaskAddMinigame", sprite);

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
        return MapId[2];
        return MapInfos[2];
      //  GameObject newgam = new GameObject();
       // newgam.transform.position = new Vector3(maptospawn.SpawnLocation.Values[0], maptospawn.SpawnLocation.Values[1], maptospawn.SpawnLocation.Values[2]);
        // map.SpawnCenter = newgam.transform;
        for (int x = -25; x < 25; x++)
        {
            for (int y = -25; y < 25; y++)
            {
                bool isSolid = (x == -25 || y == -26 || y == 24 || x == 24);
                SpawnSprite(x, y, isSolid);
            }
        }
    }
}
