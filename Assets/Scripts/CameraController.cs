using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _Target;                 // reference to the transform to follow
    [SerializeField] private float _LerpSpeed;                      // Speed the character will lerp ay

    [Header("Camera Bounds")] 
    [SerializeField] private float _MinY;
    [SerializeField] private float _MaxY;
    [SerializeField] private float _MinX;
    [SerializeField] private float _MaxX;

    private void Start()
    {
    }
    
    
    private void FixedUpdate()
    {
        FollowTarget();   
    }

    private void FollowTarget()
    {
        // Validate the target
        if (!_Target)
            return;

        // Get the position we want to move to
        Vector3 targetPosition = new Vector3
        {
            x = _Target.transform.position.x,
            y = _Target.transform.position.y,
            z = this.transform.position.z
        };

        // Create the lerp
        Vector3 lerpedPosition = Vector3.Slerp(this.transform.position, targetPosition, _LerpSpeed * Time.deltaTime);
        // Clamp within the camera bounds
        Vector3 clampedPosition = new Vector3
        {
            x = Mathf.Clamp(lerpedPosition.x, _MinX, _MaxX),
            y = Mathf.Clamp(lerpedPosition.y, _MinY, _MaxY),
            z = lerpedPosition.z
        };
        this.transform.position = clampedPosition;                  // Clamp the position
    }

    public void SetTarget(Transform target)
    {
        _Target = target;
    }
}