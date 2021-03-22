using System;
using UnityEngine;
namespace MySRP
{
    public class Light
    {
        [Range(0, 10)]
        public float intensity;
        public Color color;
    }
}