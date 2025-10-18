using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate GravityParameters GravityModifierDelegate(GravityParameters parameters);

public enum ModifierMode
{
    Instantaneous,
    Continuous
}

[Serializable]
public struct GravityParameters
{
    [SerializeField]
    public float gravityScale;

    [SerializeField]
    public Vector2 gravityDirection;
}



