using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityNoise : MonoBehaviour
{
    public float intensity = 1.0f;
    public float noiseSize = 0.5f;
    public float Speed = 1.0f;

    private Light _light;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        _light.intensity = intensity + ( Mathf.PerlinNoise(Time.time * Speed, 2.0f) - 0.5f) * noiseSize;
    }
}

