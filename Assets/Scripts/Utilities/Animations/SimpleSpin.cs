using UnityEngine;

namespace Utilities.Animations
{
    public class SimpleSpin : MonoBehaviour
    {
        public bool reverse;
        [SerializeField]
        private Vector3 spin;

        // Update is called once per frame
        private void Update()
        {
            var currentRotation = transform.rotation;

            currentRotation *= Quaternion.Euler(spin * (Time.deltaTime * (reverse ? -1 : 1)));

            transform.rotation = currentRotation;
        }
    }
}
