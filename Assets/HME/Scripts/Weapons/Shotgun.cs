using UnityEngine;

public class Shotgun : GunBase
{
    [SerializeField]
    GameObject Bullet = default;
    [SerializeField]
    GameObject SpawnPlace = default;
    [SerializeField]
    Gradient BulletColor = default;
    [SerializeField]
    float BulletScatter = 10;
    protected override void Use()
    {
        Shoot(Random.Range(6, 12));
    }
    void Shoot(int Bullets)
    {
        for (int i = 0; i < Bullets; i++)
        {
            CreateBullet(Bullet, SpawnPlace, 
                Random.Range(-BulletScatter, BulletScatter), 
                BulletColor).BulletSpeed *= Random.Range(0.7f, 1f);
        }
        Owner?.SoundPlayer.PlaySound("Shotgun");
    }
}