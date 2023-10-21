using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _Target;                 // reference to the transform to follow
    [SerializeField] private float _LerpSpeed;
    private void FixedUpdate()
    {
        FollowTarget();   
    }

    private void FollowTarget()
    {
        // Validate the target
        if (!_Target)
            return;

        Vector3 targetPosition = new Vector3
        {
            x = _Target.transform.position.x,
            y = _Target.transform.position.y,
            z = this.transform.position.z
        };

        Vector3 lerpedPosition = Vector3.Slerp(this.transform.position, targetPosition, _LerpSpeed * Time.deltaTime);
        this.transform.position = lerpedPosition;
    }
}