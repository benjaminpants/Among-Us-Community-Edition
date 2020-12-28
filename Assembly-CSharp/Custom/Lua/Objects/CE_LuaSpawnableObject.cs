using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class CE_LuaSpawnableObject
{
    public byte ID { get; protected set; }
    public Vector3 Position;
    public Vector3 EulerAngles;
    public bool ClientSide;


}
