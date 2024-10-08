﻿using System;
using System.Linq;
using Interactables.Enums;
using Levels.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Levels
{
    public class LevelDataContainer : MonoBehaviour
    {
        public int LayerCount => layers.Length;

        //============================================================================================================//
        
        public string levelName;
        //THis is the tag used to place the user generate 
        public string worldPlaceTag;

        public CONTROL_PANEL_TYPE controlPanelType;
        //What types of controls should be available for this Level/Puzzle
        public CONTROLS usableControls;

        [FormerlySerializedAs("levelDescription")] 
        [TextArea]
        public string levelScript;
        [TextArea]
        public string levelCompleteScript;
        
        [Min(0), Space(10f)]
        public int levelTime;

        [Min(1f)]
        public float yScale;
        [Min(1f)]
        public float maxScale;


        [SerializeField, Min(0f)]
        private float width;
        [SerializeField, Min(0f)]
        private float depth;

        public Vector3 MinPosition => new Vector3(-width / 2f, 0f, -depth / 2f);
        public Vector3 MaxPosition => new Vector3(width / 2f, 0f, depth / 2f);
        

        public LayerData[] layers;

        [Min(0.01f)]
        public float outputScale = 1f;


        [SerializeField, Min(0f)]
        // Player will lose a star rating for each amount of strikes earned
        private float strikesPerStar = 4f;

        //============================================================================================================//
        
        //Used to compare expected score from the player
        public int EvaluateScore(in Transform[] playerAttempt)
        {
            if (playerAttempt.Length != layers.Length)
                throw new Exception();

            var strikes = 0f;
            for (int i = 0; i < LayerCount; i++)
            {
                var layerData = layers[i];
                
                var localPosition = new Vector3(layerData.localPosition.x, yScale * i, layerData.localPosition.y);
                var localRotation = new Vector3(0f, layerData.yRotation, 0f);
                var localScale = new Vector3(layerData.localScale.x, yScale, layerData.localScale.y);
                
                var posDif = (playerAttempt[i].localPosition - localPosition).magnitude;
                var rotDif = (playerAttempt[i].localEulerAngles - localRotation).magnitude;
                var scaleDif = (playerAttempt[i].localScale - localScale).magnitude;

                Debug.Log($"posDif {posDif} rotDif {rotDif} scaleDif {scaleDif}");

                strikes += posDif + rotDif + scaleDif;
            }

            Debug.Log($"Strikes for level {strikes}");

            int stars = Mathf.Max(5 - Mathf.FloorToInt(strikes / strikesPerStar), 1);

            Debug.Log($"Level stars {stars}");

            return stars;
        }

        //Unity Functions
        //============================================================================================================//
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            var guidePosition = transform.position;
            for (int i = 0; i < LayerCount; i++)
            {
                var layerData = layers[i];

                if (layerData == null || layerData.printLayerShape == null)
                    continue;

                var pos = guidePosition + new Vector3(layerData.localPosition.x, yScale * i, layerData.localPosition.y);
                var scale = new Vector3(layerData.localScale.x, yScale, layerData.localScale.y);
                var color = layerData.Material == null ? Color.magenta : layerData.Material.color;
                Gizmos.color = color;
                Gizmos.DrawMesh(layerData.printLayerShape.Mesh,
                    pos,
                    Quaternion.Euler(0f, layerData.yRotation, 0f),
                    scale);
                Gizmos.color =  new Color(color.r - 0.15f,color.g - 0.15f,color.b - 0.15f);
                Gizmos.DrawWireMesh(layerData.printLayerShape.Mesh,
                    pos,
                    Quaternion.Euler(0f, layerData.yRotation, 0f),
                    scale);
            }

            var halfWidth = width / 2f;
            var halfDepth = depth / 2f;

            var points = new[]
            {
                new Vector3(-halfWidth, 0f, -halfDepth), //min
                new Vector3(-halfWidth, 0f, halfDepth), //TL
                new Vector3(halfWidth, 0f, halfDepth), //max
                new Vector3(halfWidth, 0f, -halfDepth) //BR
            };

            Gizmos.color = Color.yellow;
            Gizmos.DrawLineStrip(points, true);

        }
        //============================================================================================================//
#endif
    }

}