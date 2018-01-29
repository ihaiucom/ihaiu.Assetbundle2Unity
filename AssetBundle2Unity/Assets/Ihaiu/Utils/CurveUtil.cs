using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurveUtil
{

    public static int GetForecastParticleCount(this ParticleSystem particleSystem)
    {
        float rateOverTime = particleSystem.GetRateOverTimeMax();
        return Mathf.CeilToInt(Mathf.Min(particleSystem.main.startLifetime.GetMaxVal() * rateOverTime, particleSystem.main.maxParticles));
    }


    public static int GetForecastTrianglesCount(this ParticleSystem particleSystem)
    {
        int count = particleSystem.GetForecastParticleCount();
        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        if(renderer.renderMode == ParticleSystemRenderMode.Mesh && renderer.mesh != null)
        {
            return count * renderer.mesh.triangles.Length / 3;
        }
        else
        {
            return count *  2;
        }
    }


    public static ParticleSystemRenderMode GetRenderMode(this ParticleSystem particleSystem)
    {
        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        return renderer.renderMode;
    }

    public static string GetMeshName(this ParticleSystem particleSystem)
    {
        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        if (renderer.renderMode == ParticleSystemRenderMode.Mesh)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(renderer.mesh);
#endif
            return renderer.mesh.name;
        }
        else
        {
            return renderer.renderMode.ToString();
        }
    }


    public static float GetRateOverTimeMax(this ParticleSystem particleSystem)
    {
        float rateOverTime = particleSystem.emission.rateOverTime.GetMaxVal();
        return Mathf.Max(rateOverTime, particleSystem.GetBurstMax());
    }

    public static int GetBurstMax(this ParticleSystem particleSystem)
    {
        int val = 0;
        ParticleSystem.Burst[]  bursts = new ParticleSystem.Burst[particleSystem.emission.burstCount];
        particleSystem.emission.GetBursts(bursts);
        foreach (ParticleSystem.Burst burst in bursts)
        {
            val = Mathf.Max(val, burst.maxCount);
            val = Mathf.Max(val, burst.minCount);
            if(burst.cycleCount == 0 && burst.repeatInterval > 0)
            {
                val = Mathf.Max(val, Mathf.CeilToInt(1f / burst.repeatInterval) * val);
            }
        }
        return val;
    }

    public static float GetMaxVal(this ParticleSystem.MinMaxCurve minMaxCurve)
    {
        switch (minMaxCurve.mode)
        {
            case ParticleSystemCurveMode.TwoConstants:
                return Mathf.Max(minMaxCurve.constantMax, minMaxCurve.constantMin);
            case ParticleSystemCurveMode.Curve:
                return minMaxCurve.curve.GetMaxVal();
            case ParticleSystemCurveMode.TwoCurves:
                return Mathf.Max( minMaxCurve.curveMax.GetMaxVal(), minMaxCurve.curveMax.GetMaxVal());
            case ParticleSystemCurveMode.Constant:
            default:
                return minMaxCurve.constant;
        }
    }


    public static float GetMaxVal(this AnimationCurve animationCurve)
    {
        float maxVal = int.MinValue;
        foreach (Keyframe keyframe in animationCurve.keys)
        {
            if (maxVal < keyframe.value)
            {
                maxVal = keyframe.value;
            }
        }

        return maxVal;
    }


    public static float GetMinVal(this AnimationCurve animationCurve)
    {
        float minVal = int.MaxValue;
        foreach (Keyframe keyframe in animationCurve.keys)
        {
            if (minVal > keyframe.value)
            {
                minVal = keyframe.value;
            }
        }

        return minVal;
    }
}
