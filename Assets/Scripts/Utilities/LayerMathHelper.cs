using System;
using Interactables;
using Levels;
using Levels.Enums;
using UnityEngine;

namespace Utilities
{
    public static class LayerMathHelper
    {
        public static (Vector3 position, Vector3 rotation, Vector3 scale) GetAllTransformations(
            LayerData layerData,
            ControlPanelContainer controlPanel, 
            LevelDataContainer currentLevel)
        {
            var maxScale = currentLevel.maxScale;

            var outPosition = Vector3.zero;
            var outRotation = Vector3.zero;
            var outScale = new Vector3(0f, layerData.localScale.y, 0f);

            var levelMinPosition = currentLevel.MinPosition;
            var levelMaxPosition = currentLevel.MaxPosition;

            var controlValues = controlPanel.GetControlValues();

            //Go through each of the controls, then apply their values based on what they should be effecting
            for (int i = 0; i < controlValues.Length; i++)
            {
                var (control, value, value2) = controlValues[i];

                switch (control)
                {
                    case CONTROLS.SCALE:
                        var yScale = currentLevel.yScale;
                        outScale = new Vector3(maxScale * Mathf.Clamp(value, 0.1f, 1f),
                            yScale,
                            maxScale * Mathf.Clamp(value, 0.1f, 1f));
                        break;
                    case CONTROLS.X_SCALE:
                        outScale.x = maxScale * Mathf.Clamp(value, 0.1f, 1f);
                        break;
                    case CONTROLS.Z_SCALE:
                        outScale.z = maxScale * Mathf.Clamp(value, 0.1f, 1f);
                        break;
                    case CONTROLS.XZ_SCALE:
                        outScale.x = maxScale * Mathf.Clamp(value, 0.1f, 1f);
                        outScale.z = maxScale * Mathf.Clamp(value2, 0.1f, 1f);
                        break;
                    case CONTROLS.POS:
                        outPosition.x = Mathf.Lerp(levelMinPosition.x, levelMaxPosition.x, value);
                        outPosition.z = Mathf.Lerp(levelMinPosition.z, levelMaxPosition.z, value2);
                        break;
                    case CONTROLS.Y_ROT:
                        outRotation.y = value * 360f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }


            return (outPosition, outRotation, outScale);
        }
    }
}