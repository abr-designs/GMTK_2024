using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "new_shape_layer", menuName = "Shaper Layer")]
    public class PrintLayerShapeScriptableObject : ScriptableObject
    {
        public Transform prefab;
        public Mesh Mesh;
        public Sprite sideViewSprite;
        public Sprite isoViewSprite;
    }
}