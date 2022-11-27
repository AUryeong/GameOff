using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollector : MonoBehaviour
{
    ParticleSystem particle;
    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        int triggerParticleCount = particle.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for(int i =0; i < triggerParticleCount; i++)
        {
            ParticleSystem.Particle triggerParticle = particles[i];
            triggerParticle.remainingLifetime = 0;
            particles[i] = triggerParticle;
            IngameUIManager.Instance.GetGaugeParticle();
        }

        particle.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }
}
