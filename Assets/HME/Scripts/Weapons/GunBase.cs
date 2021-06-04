using System.Collections;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    public EntityBase Owner;

    protected virtual void Use()
    {
        //Using code
    }

    //Amount of gun usings
    public int AmmoCount = 1;
    public bool InvertAfterUse;
    [SerializeField]
    bool InfAmmo = false;
    public bool Shoot()
    {
        if ((AmmoCount > 0 || InfAmmo) && Reloaded)
        {
            Use();

            Reloaded = false;
            StartCoroutine(Reloader());
            AmmoCount--;
            return true;
        }
        return false;
    }
    IEnumerator Reloader()
    {
        yield return new WaitForSeconds(ReloadTime);
        Reloaded = true;
    }
    protected BulletMove CreateBullet(GameObject Bullet, GameObject SpawnPos, float AdditionalAngle, Gradient BulletColor)
    {
        gameObject.transform.localRotation = Quaternion.identity;
        GameObject BulletObj = Instantiate(Bullet, SpawnPos.transform.position,
            Quaternion.Euler(0, 0, Owner.transform.eulerAngles.z + 90 + AdditionalAngle));
        BulletObj.GetComponent<SpriteRenderer>().color = BulletColor.Evaluate(Random.Range(0, 1f));
        return BulletObj.GetComponent<BulletMove>();
    }

    public WorldStateKeeper State;
    public GunAnimations InHands;
    [System.Obsolete]
    public void SetState(bool IsInWorld)
    {
        if (State.Stand != null)
        {
            State.Stand.SetActive(IsInWorld);
            State.Stand.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (InHands != null) { State.Lies.SetActive(!IsInWorld); }
    }

    [SerializeField]
    float ReloadTime = 1;
    bool Reloaded = true;

    [System.Serializable]
    public class GunAnimations
    {
        public string Moving;
        public string Using;
        public string Fatality;
    }
    [Tooltip("Distance between player and AI shooter which is minimal for a shot.")]
    public float AIDistance = 6f;
}
