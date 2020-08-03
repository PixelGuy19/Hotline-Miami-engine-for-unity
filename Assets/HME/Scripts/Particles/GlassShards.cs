using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassShards : ParticleSystemController
{
    protected override void UpdateParticle(ref ParticleSystem.Particle Particle)
    {
        base.UpdateParticle(ref Particle);
        Particle.velocity *= Random.Range(0.8f, 1f);
        ParticleSystem.MainModule Main = PSystem.main;
        Main.startRotationMultiplier = Random.Range(0, 360);
    }
}