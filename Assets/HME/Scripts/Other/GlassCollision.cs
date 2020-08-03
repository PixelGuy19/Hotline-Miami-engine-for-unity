using UnityEngine;

public class GlassCollision : MonoBehaviour
{
    [SerializeField]
    GameObject ShardsObj;
    [SerializeField]
    ParticleSystem ShardsSystem;
    [SerializeField]
    SpriteRenderer MySprite;
    [SerializeField]
    Sprite CrushedSpite;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsCrushed)
        {
            float Rot = Mathf.Abs((int)transform.rotation.eulerAngles.z);
            IsH = !(Rot == 90);
            Crash(collision.transform);
            ShardsSystem.Play();
            IsCrushed = true;
            MySprite.sprite = CrushedSpite;
        }
    }
    bool IsCrushed = false;
    public void Crash(Transform BulletTransform)
    {
        if (IsH)
        {
            SetSide(Mathf.Abs((BulletTransform.localRotation.eulerAngles.z)) < 270
                && Mathf.Abs((BulletTransform.localRotation.eulerAngles.z)) > 90);
        }
        else
        {
            SetSide(Mathf.Abs((BulletTransform.rotation.eulerAngles.z)) >= 180);
        }
        void SetSide(bool Forward)
        {
            ShardsObj.transform.localPosition = new Vector3(0, 0.25f, 0) * (Forward ? -1 : 1);
            ShardsObj.transform.localRotation = Quaternion.Euler(-90 * (Forward ? 1 : -1), 0, 0);
        }
    }
    bool IsH; //Is horizontal
}
