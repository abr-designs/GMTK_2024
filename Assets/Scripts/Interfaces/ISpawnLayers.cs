using System.Collections;
using Interactables;
using Levels;
using UnityEngine;

namespace Interfaces
{
    public interface ISpawnLayers
    {
        public Transform GeneratedTransform { get; }
        public Vector3 SpawnLocation { set; }
        Coroutine SpawnLayer(int layerIndex, LayerData layerData, LevelDataContainer currentLevel, ControlPanelContainer controlPanelContainer);
    }
}