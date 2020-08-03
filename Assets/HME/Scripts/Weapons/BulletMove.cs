using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletMove : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D BulletRb;
    public float BulletSpeed;
    private void FixedUpdate()
    {
        BulletRb.velocity = transform.up * BulletSpeed;
    }
    [SerializeField]
    GameObject BulletHitObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityBase Enemy = collision.gameObject.GetComponent<EntityBase>();
        if (Enemy != null)
        {
            Enemy.Die(true, gameObject);
            return;
        }

        if (collision.tag != "BulletInvisible")
        {
            Instantiate(BulletHitObject, collision.ClosestPoint(transform.position),
                Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 270));
            Destroy(gameObject);
        }
    }
}
