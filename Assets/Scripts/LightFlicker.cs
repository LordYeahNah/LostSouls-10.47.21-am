using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light2D _Light;
    [SerializeField] private float _Standard = 0.3f;
    [SerializeField] private float _MinWaitTime;
    [SerializeField] private float _MaxWaitTime;

    private bool _LightIsActive = false;

    private void Start()
    {
        SetLight();
    }

    private IEnumerator FlickerLight(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SetLight();
    }

    public void SetLight()
    {
        if (_LightIsActive)
        {
            _Light.intensity = 0.0f;
            _LightIsActive = false;
        }
        else
        {
            _Light.intensity = _Standard;
            _LightIsActive = true;
        }

        StartCoroutine(FlickerLight(Random.Range(_MinWaitTime, _MaxWaitTime)));
    }
}
