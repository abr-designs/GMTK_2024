using UnityEngine;

public class Scaler : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float scalePerSecond;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localScale += Vector3.one * (scalePerSecond * Time.deltaTime);
    }
}
