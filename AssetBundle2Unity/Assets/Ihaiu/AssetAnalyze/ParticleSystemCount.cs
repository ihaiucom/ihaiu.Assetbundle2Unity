using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemCount : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public int particleCount = 0;
    public int maxParticleCount = 0;

    public int forecastParticleCount = 0;
    public int forecastTrianglesCount = 0;
    
    private void Start()
    {
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        particleCount = particleSystem.particleCount;
        maxParticleCount = Mathf.Max(particleCount, maxParticleCount);
        forecastParticleCount = particleSystem.GetForecastParticleCount();
        forecastTrianglesCount = particleSystem.GetForecastTrianglesCount();
    }


}
