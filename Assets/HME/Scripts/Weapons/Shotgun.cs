using UnityEngine;

public class Shotgun : GunBase
{
    [Header("Shotgun")]
    [Space(10)]
    [SerializeField]
    GameObject Bullet = default;
    [SerializeField]
    GameObject SpawnPlace = default;
    [SerializeField]
    Gradient BulletColor = default;
    protected override void Use()
    {
        Shoot(Random.Range(6, 12));
    }
    void Shoot(int Bullets)
    {
        for (int i = 0; i < Bullets; i++)
        {
            CreateBullet(Bullet, SpawnPlace, 
                Random.Range(-Spread, Spread), 
                BulletColor).BulletSpeed *= Random.Range(0.7f, 1f);
        }
        Owner?.SoundPlayer.PlaySound("Shotgun");
    }
}