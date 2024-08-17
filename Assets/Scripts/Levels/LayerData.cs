using System;
using UnityEngine;

namespace Levels
{
    [Serializable]
    public class LayerData
    {
        public Material Material;
        public PrintLayerShapeScriptableObject printLayerShape;
        //This should contain the scene ref to what was intended
        //public Transform sceneRefTransform;
        public Vector2 localPosition;
        public float yRotation;
        public Vector2 localScale = Vector2.one;
    }
}