using UnityEngine;

namespace Interfaces
{
    public interface ICreateWorldReplacers
    {
        void CreateWorldVersion(string worldTag, float scale, GameObject container);
    }
}