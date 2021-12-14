﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InnerNet;
using Newtonsoft.Json;

public class CEM_TaskData
{
    // minigame snizzle!
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
        //  MapInfos.Add(new CE_MapInfo("Lion"); // adds a dummy map named Lion
        MapInfos.Add(new CE_MapInfo("Lion",new string[18]{
            "Happy Place",
            "Sad Place",
            "Mad Place",
            "Bad Place",
            "Good Place",
            "Evil Place",
            "Holy Place",
            "Sans Undertale",
            "Lovely Day Outside",
            "The Forest",
            "Dumb Place",
            "Memey Place",
            "Inverted Skeld",
            "ehT Dleks",
            "Toon land",
            "Pizzaria",
            "Airship",
            "Polus"
        }));
    }

    public static CE_MapInfo GetCurrentMap()
    {
        return MapId[2];
     //   return MapInfos[2];
        return MapInfos[PlayerControl.GameOptions.MapId];
    }
}

 

public class CE_CustomMap
{
    private static bool MapTestingActive = true;
    public bool CustomTasksEnabled = true;
        public bool MiniMapEnabled = false;
        public bool CustomVentsEnabled = true;
//    public static bool MapTestingActiveMapId = false;
    private static void ClearMapCollision(ShipStatus map)
    {
        Collider2D[] colids = map.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colids)
        {
            UnityEngine.Object.Destroy(col.gameObject);
        }
    }

         GetCurrentMap() // Gets what map your playing on. and then code below checks it.
         If CurrentMap() = MapId[2] then // checks what map id it is
         MapTestingActive = true //enables the bools
         If CurrentMap() = MapId[1] then
         MapTestingActive = false
         If CurrentMap() = MapId[0] then
         MapTestingActive = false

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
        GameObject go = new GameObject("TestObject1");
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
      //  return MapId[2];
      //  return MapInfos[2];
      //  GameObject newgam = new GameObject();
       // newgam.transform.position = new Vector3(maptospawn.SpawnLocation.Values[0], maptospawn.SpawnLocation.Values[1], maptospawn.SpawnLocation.Values[2]);
        // map.SpawnCenter = newgam.transform;
        for (int x = -24; x < 29; x++)
        {
            for (int y = -24; y < 29; y++)
            {
                bool isSolid = (x == -24 || y == -24 || y == 24 || x == 24);
                SpawnSprite(x, y, isSolid);
            }
        }
    }
}
