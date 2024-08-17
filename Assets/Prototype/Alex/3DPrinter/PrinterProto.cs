using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrinterProto : MonoBehaviour
{
    [SerializeField]
    private Transform prefab;

    [SerializeField]
    private Vector3 maxScale;

    [SerializeField]
    private Vector3 scalePosition;
    [SerializeField]
    private Vector3 movePosition;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private Transform container;
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LevelCoroutine());
    }

    private IEnumerator LevelCoroutine()
    {
        slider.interactable = false;
        
        text.text = "Space to Start!";
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        
        while (true)
        {
            text.text = "Ready?!";
            
            slider.interactable = false;
            yield return new WaitForSeconds(1f);
        
            text.text = "GO!!";
            slider.interactable = true;
            yield return new WaitForSeconds(1f);
            
            for (float i = 3f; i >= 0f; i -= Time.deltaTime)
            {
                text.text = $"{i:0}s Left!";
            
                yield return null;
            }
            
            text.text = string.Empty;

            var temp = Instantiate(prefab, scalePosition, Quaternion.identity);
            temp.localScale = Vector3.zero;
            var sliderMult = slider.value;
            
            text.text = "PRINT!";
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(ScaleCoroutine(
                temp,
                new Vector3(maxScale.x * sliderMult, maxScale.z, maxScale.y * sliderMult), 
                0.25f));

            slider.value = 0f;
            yield return new WaitForSeconds(1f);

            temp.SetParent(container);
            yield return StartCoroutine(MoveToPositionCoroutine(container, 0.25f));
        }

    }

    private IEnumerator ScaleCoroutine(Transform target, Vector3 targetScale, float time)
    {
        var currentScale = target.localScale;
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;

            target.transform.localScale = Vector3.Lerp(currentScale, targetScale, curve.Evaluate(dt));
                    
            yield return null;
        }
    }

    private IEnumerator MoveToPositionCoroutine(Transform target, float time)
    {
        var start = container.position;
        var end = container.position + (movePosition - scalePosition);
        for (var t = 0f; t <= time; t += Time.deltaTime)
        {
            var dt = t / time;
            

            target.transform.position = Vector3.Lerp(start, end, curve.Evaluate(dt));
                    
            yield return null;
        }
    }

    //============================================================================================================//

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(scalePosition, movePosition);        
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(scalePosition, 0.25f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(movePosition, 0.25f);
    }
}
