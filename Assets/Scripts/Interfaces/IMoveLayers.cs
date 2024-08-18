using UnityEngine;

namespace Interfaces
{
    public interface IMoveLayers
    {
        Coroutine MoveLayer(int layerIndex, float yScale, Transform targetTransform, Transform containerTransform);
    }
}