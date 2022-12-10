using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityNoise : MonoBehaviour
{
    public float intensity = 1.0f;
    public float noiseSize = 0.5f;
    public float Speed = 1.0f;

    Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    void Update()
    {
        light.intensity = intensity + ( Mathf.PerlinNoise(Time.time * Speed, 2.0f) - 0.5f) * noiseSize;
    }
}

