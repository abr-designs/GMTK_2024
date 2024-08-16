using System;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities.Debugging;
using Utilities.ReadOnly;

public class SpaceShip : MonoBehaviour
{
    public event Action OnPositionsChange;
    [SerializeField, Header("Ship Shape")]
    public LineRenderer lineRenderer;
    
    
    [SerializeField, ReadOnly]
    private Vector3 mouseWorldPosition;
    public Vector2 pos;

    [SerializeField]
    private Transform[] bits;

    //------------------------------------------------//

    [SerializeField, Min(5)]
    private int vertices;
    [SerializeField, Min(0.1f)]
    private float radius;
    //------------------------------------------------//

    [SerializeField, Header("DEBUGGING")]
    private float impactSize;
    [SerializeField, Min(0.1f)]
    private float impactDelta;
    
    //------------------------------------------------//

    private Camera _camera;

    private Vector3[] _positions;
    private float[] _radii;
    private int[] _collisionNonAlloc;
    
    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
        pos = Input.mousePosition;
        //Setup all positions
        //------------------------------------------------//
        _positions = new Vector3[vertices];
        _radii = new float[vertices];
        _collisionNonAlloc = new int[vertices];

        lineRenderer.GetPositions(_positions);

        for (int i = 0; i < vertices; i++)
        {
            _radii[i] = radius;
        }
        //------------------------------------------------//

    }

    // Update is called once per frame
    private void Update()
    {
        mouseWorldPosition = GetMouseWorldPosition();
        
        //Check for clicks
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var localPos = transform.InverseTransformPoint(mouseWorldPosition);
            
            //Add to Points
            var hitCount = GetImpactedPoints(localPos, impactSize, _collisionNonAlloc);
            if (hitCount <= 0)
                return;

            ApplyDeltaToPoints(hitCount, _collisionNonAlloc, impactDelta);
            UpdateLineRenderer();
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            var localPos = transform.InverseTransformPoint(mouseWorldPosition);
            
            //TODO Subtract
            var hitCount = GetImpactedPoints(localPos, impactSize, _collisionNonAlloc);
            if (hitCount <= 0)
                return;
            
            ApplyDeltaToPoints(hitCount, _collisionNonAlloc, -impactDelta);
            UpdateLineRenderer();
        }
        
    }

    private void CheckCollisions()
    {
        
    }

    private Vector3 GetMouseWorldPosition()
    {
        var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        return pos;
    }
    private int GetImpactedPoints(Vector3 position, float impactRadius, int[] collisions)
    {
        Assert.IsNotNull(collisions);
        Assert.IsTrue(collisions.Length > 0);
        
        var count = 0;
        for (int i = 0; i < _positions.Length; i++)
        {
            if (i >= collisions.Length)
                break;
            
            if(Utilities.Physics.CollisionChecks.Point2Circle(_positions[i], position, impactRadius) == false)
                continue;

            collisions[count++] = i;
        }

        return count;
    }
    private void ApplyDeltaToPoints(int affectedCount, int[] affectedIndices, float delta)
    {
        for (int i = 0; i < affectedCount; i++)
        {
            var positionIndex = affectedIndices[i];

            _radii[positionIndex] += delta;
        }
    }
    private void UpdateLineRenderer()
    {
        var increment = 360f / vertices;

        for (int i = 0; i < vertices; i++)
        {
            var angle = (i * increment) * Mathf.Deg2Rad;
            var r = _radii[i];
            
            _positions[i] = new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
        }
        
        lineRenderer.SetPositions(_positions);
        OnPositionsChange?.Invoke();
    }

    //============================================================================================================//

#if UNITY_EDITOR

    [ContextMenu("Generate")]
    private void CreateShip()
    {
        var points = GeneratePoints(vertices, radius);
        lineRenderer.positionCount = vertices;
        lineRenderer.SetPositions(points);
       
    }
    
    private Vector3[] GeneratePoints(int vertexCount, float r)
    {
        var increment = 360f / vertexCount;
        var outData = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            var angle = i * increment * Mathf.Deg2Rad;
            
            outData[i] = new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
        }

        return outData;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;
        
        Draw.Circle(mouseWorldPosition, impactSize, 6, Color.green);
    }


    //============================================================================================================//
#endif
}
