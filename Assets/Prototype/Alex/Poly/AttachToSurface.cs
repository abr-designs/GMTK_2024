using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToSurface : MonoBehaviour
{
    [SerializeField]
    private SpaceShip spaceShipSurface;

    private int _indexToMonitor;
    private Vector3 dif;
    

    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        spaceShipSurface.OnPositionsChange += OnSurfacePositionsChange;

        var wsBounds = _spriteRenderer.bounds;
        var shipTransform = spaceShipSurface.transform;

        var localBounds = new Bounds
        {
            center = shipTransform.InverseTransformPoint(wsBounds.center),
            min = shipTransform.InverseTransformPoint(wsBounds.min),
            max = shipTransform.InverseTransformPoint(wsBounds.max),
        };

        var posCount = spaceShipSurface.lineRenderer.positionCount;
        var positions = new Vector3[posCount];
        spaceShipSurface.lineRenderer.GetPositions(positions);

        var closestIndex = -1;
        var dist = 9999f;
        
        for (int i = 0; i < posCount; i++)
        {
            if(Utilities.Physics.CollisionChecks.Point2Rect(positions[i], localBounds) == false)
                continue;

            var temp = (positions[i] - localBounds.center).sqrMagnitude;
            if(temp >= dist)
                continue;

            dif = (positions[i] - localBounds.center);
            dist = temp;
            closestIndex = i;
        }

        if (closestIndex < 0)
            throw new Exception();

        _indexToMonitor = closestIndex;
        
        OnSurfacePositionsChange();
    }
    
    private void OnSurfacePositionsChange()
    {
        var pointPosition = spaceShipSurface.lineRenderer.GetPosition(_indexToMonitor);
        var outDir = (pointPosition - spaceShipSurface.transform.position).normalized;

        transform.up = outDir;
        transform.position = pointPosition + dif;
    }

}
