using System.Collections;
using UnityEngine;

namespace UI
{
    public class MainMenuSpriteClimber : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] spriteRenderers;

        [SerializeField]
        private SpriteRenderer moverSpriteRenderer;
        private Transform _moverTransform;

        [SerializeField]
        private Vector2 startLocalPosition, endLocalPosition;

        [SerializeField]
        private float riseSpeed;
        [SerializeField]
        private float moveSpeed;
    
        [SerializeField, Header("To Spawn")]
        private SpriteRenderer spawnSpritePrefab;
        [SerializeField]
        private Vector2 minDimensions, maxDimensions;
        [SerializeField, Min(0f)]
        private float pauseTime;

    
        [SerializeField, Header("Light")]
        private SpriteRenderer lightRenderer;
        [SerializeField]
        private AnimationCurve flashCurve;
        [SerializeField]
        private float lightAnimationTime;
        [SerializeField]
        private float lightPauseTime;

        [SerializeField]
        private float startDelay;
        

        private bool playing;
        
    
        //============================================================================================================//

        private float maxy;
        // Start is called before the first frame update
        private void Start()
        {
            playing = true;
            _moverTransform = moverSpriteRenderer.transform;
            _moverTransform.localPosition = startLocalPosition;
        
            StartCoroutine(LightFlashCoroutine());
            StartCoroutine(MoveCoroutine());
            
            maxy = FindObjectOfType<Camera>().ScreenToWorldPoint(new Vector3(0f, Screen.height)).y;
        }

        
        // Update is called once per frame
        private void Update()
        {
            if (Time.time < startDelay)
                return;
            
            if (playing == false)
                return;

            if (_moverTransform.transform.position.y > maxy * 1.25f)
            {
                playing = false;
                StopAllCoroutines();
                return;
            }
            
            //TODO Add test for offscreen
            var currentPosition = transform.position;
            currentPosition.x += moveSpeed * Time.deltaTime;
            _moverTransform.localPosition += Vector3.up * (riseSpeed * Time.deltaTime);
            transform.position = currentPosition;
        }
    
        //============================================================================================================//

        private IEnumerator MoveCoroutine()
        {
            yield return new WaitForSeconds(startDelay);
            
            var waitForSeconds = new WaitForSeconds(pauseTime);
            while (true)
            {
                var newObject = Instantiate(spawnSpritePrefab, _moverTransform.position, Quaternion.identity, transform);
                newObject.gameObject.SetActive(true);
                newObject.size = new Vector2(
                    Random.Range(minDimensions.x, maxDimensions.x),
                    Random.Range(minDimensions.y, maxDimensions.y));

                yield return waitForSeconds;
            }
        }
        private IEnumerator LightFlashCoroutine()
        {
            yield return new WaitForSeconds(startDelay);
            
            var startingLightColor = lightRenderer.color;
            var clearLight = startingLightColor;
            clearLight.a = 0;

            var waitForSeconds = new WaitForSeconds(lightPauseTime);

            lightRenderer.color = clearLight;
            while (true)
            {
                for (var t = 0f; t < lightAnimationTime; t += Time.deltaTime)
                {
                    lightRenderer.color = Color.Lerp(clearLight, startingLightColor,flashCurve.Evaluate(t / lightAnimationTime));

                    yield return null;
                }

                yield return waitForSeconds;
            }
        }
    
    
        //============================================================================================================//

#if UNITY_EDITOR

        [ContextMenu("Get Sprite Renderers")]
        private void GetSpriteRenderersInChildren()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    
#endif
    }
}
