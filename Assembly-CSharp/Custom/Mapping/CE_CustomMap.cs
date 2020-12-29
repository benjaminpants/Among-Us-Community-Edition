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

    public static void MapTest(ShipStatus map)
    {
        if (!MapTestingActive) return;
        ClearMapCollision(map);
    }
}
