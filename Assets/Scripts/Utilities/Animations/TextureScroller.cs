using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities.Animations
{
    public class TextureScroller : MonoBehaviour
    {
        //============================================================================================================//
        [Serializable]
        private class TextureScrollData
        {
            [SerializeField] private string name;
            [SerializeField]
            private Vector2 speed;

            [SerializeField]
            private Renderer targetRenderer;

            [SerializeField] private bool randomOffset;

            [SerializeField]
            private Color32 color = new Color32(255,255,255,255);
    
            private Material _material;
            private float _currentScrollX, _currentScrollY;

            public void Init()
            {
                _material = targetRenderer.material;
                _material.color = color;

                if (randomOffset == false)
                    return;

                _currentScrollX =  speed.x != 0f ? Random.Range(0f, 1f) : 0f;
                _currentScrollY =  speed.y != 0f ? Random.Range(0f, 1f) : 0f;

                Update();
            }

            public void Update()
            {
                _currentScrollX += speed.x * Time.deltaTime;
                _currentScrollY += speed.y * Time.deltaTime;
                
                _material.mainTextureOffset = new Vector2(_currentScrollX, _currentScrollY);
            }
        }
        //============================================================================================================//

        [SerializeField]
        private TextureScrollData[] targetScrollers;
        

        private void Start()
        {
            for (int i = 0; i < targetScrollers.Length; i++)
            {
                targetScrollers[i].Init();
            }
        }

        private void Update()
        {
            for (int i = 0; i < targetScrollers.Length; i++)
            {
                targetScrollers[i].Update();
            }
        }
    }
}
