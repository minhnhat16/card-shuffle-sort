using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashVfx : MonoBehaviour
{
    ParticleSystem splash;
    ParticleSystem  shadow;
    ParticleSystem drops;
    public float playbackSpeed = 1.0f;
    public void SetColorVFX(Color color)
    {
        SetParticleSystemColor(splash, color);
        SetParticleSystemColor(shadow, color);
        SetParticleSystemColor(drops, color);
    }

    private void SetParticleSystemColor(ParticleSystem particleSystem, Color color)
    {
        var mainModule = particleSystem.main;
        mainModule.startColor = color;
    }
    public void PlayAndDeactivate()
    {
        SetPlaybackSpeed(splash, playbackSpeed);
        SetPlaybackSpeed(shadow, playbackSpeed);
        SetPlaybackSpeed(drops, playbackSpeed);

        // Start the coroutine to deactivate the GameObject after the ParticleSystems finish
        StartCoroutine(DeactivateAfterPlayback());
    }

    private void SetPlaybackSpeed(ParticleSystem particleSystem, float speed)
    {
        var mainModule = particleSystem.main;
        mainModule.simulationSpeed = speed;
    }

    private IEnumerator DeactivateAfterPlayback()
    {
        // Wait for all ParticleSystems to stop playing
        while (splash.isPlaying || shadow.isPlaying || drops.isPlaying)
        {
            yield return null;
        }

        // Deactivate the GameObject
        gameObject.SetActive(false);
    }
}
