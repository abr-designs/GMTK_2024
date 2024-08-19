using UnityEngine;

namespace Utilities.Animations
{
    public class PingPongAnimator : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private float speed;

        [SerializeField]
        private float current;
    
        [SerializeField]
        private float startRotation, endRotation;
        [SerializeField]
        private Vector3 startScale, endScale;

        [SerializeField]
        private AnimationCurve curve;
        // Start is called before the first frame update// Update is called once per frame
        private void Update()
        {
            current += Time.deltaTime * speed;
            var t = curve.Evaluate(Mathf.PingPong(current, 1f));

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startRotation, endRotation, t));
        }
    }
}
