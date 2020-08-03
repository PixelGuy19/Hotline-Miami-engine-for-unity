using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem PSystem;
    ParticleSystem.Particle[] Particles;
    protected virtual void UpdateParticle(ref ParticleSystem.Particle Particle)
    {
        //To override
    }
    private void LateUpdate()
    {
        InitializeIfNeeded();
        int NumParticlesAlive = PSystem.GetParticles(Particles);

        // Change only the particles that are alive
        for (int i = 0; i < NumParticlesAlive; i++)
        {
            UpdateParticle(ref Particles[i]);
        }

        // Apply the particle changes to the Particle System
        PSystem.SetParticles(Particles, NumParticlesAlive);

        void InitializeIfNeeded()
        {
            if (PSystem == null)
                PSystem = GetComponent<ParticleSystem>();

            if (Particles == null || Particles.Length < PSystem.main.maxParticles)
                Particles = new ParticleSystem.Particle[PSystem.main.maxParticles];
        }
    }
}
