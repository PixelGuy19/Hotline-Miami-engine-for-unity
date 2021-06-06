using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EntityBase : MonoBehaviour
{
    [SerializeField]
    WorldStateKeeper StateKeeper;
    
    //Base set
    [Header("Entity")]
    [SerializeField]
    public SoundPlayer SoundPlayer;
    [SerializeField]
    protected Animator MyAnim;
    public Animator LegsAnim;
    private void Reset()
    {
        MyAnim = gameObject.GetComponent<Animator>();
        MyBody = gameObject.GetComponent<Rigidbody2D>();
        MyBody.gravityScale = 0;
    }
    virtual protected void Start()
    {
        if(StateKeeper == null) { StateKeeper = GetComponentInParent<WorldStateKeeper>(); }
        if (DefaultGun != null) { PickUpGun(DefaultGun); }
    }

    //Base
    [SerializeField]
    protected Rigidbody2D MyBody;
    protected float Angle = 0f;
    public void LookAt(Vector3 Position)
    {
        Vector2 Direction = (Vector2)Position - (Vector2)transform.position;
        Angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
    }
    public void InstantLookAt(Vector3 Position, GameObject Object = null)
    {
        if (Object == null)
        {
            Object = gameObject;
        }

        Vector2 Direction = (Vector2)Position - (Vector2)transform.position;
        Object.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
    }

    public GameObject MyLegs;
    public float Speed = 10;
    protected bool Locked;
    protected void Move(float X = 0, float Y = 0)
    {
        if (Locked) { X = 0; Y = 0; }
        X *= Speed;
        Y *= Speed;

        MyBody.velocity = new Vector2(X, Y);
        if (Mathf.Abs(X) != 0 || Mathf.Abs(Y) != 0)
        {
            InstantLookAt(transform.position + new Vector3(X, Y), MyLegs);
            MyAnim.SetFloat("WalkingSpeed", 1);
        }
        else
        {
            MyAnim.SetFloat("WalkingSpeed", 0);
        }

        //if (Mathf.Abs(X) > Mathf.Abs(Y)) { MyAnim.SetFloat("WalkingSpeed", Mathf.Abs(X / Speed)); }
        //else { MyAnim.SetFloat("WalkingSpeed", Mathf.Abs(Y / Speed)); }

        MyLegs.SetActive(Mathf.Abs(X) + Mathf.Abs(Y) != 0);
        if (!MyLegs.activeSelf) { return; }

        if (Mathf.Abs(X) + Mathf.Abs(Y) == 0)
        {
            LegsAnim.Play("Legs", 0, 0);
        }
        LegsAnim.SetFloat("WalkingSpeed", 1); //MyAnim.GetFloat("WalkingSpeed")
    }
    protected void Move(Vector2 Dir)
    {
        Move(Dir.x, Dir.y);
    }

    //Guns
    [SerializeField]
    protected GunBase DefaultGun;
    GunBase GunToPickUp;
    [HideInInspector]
    public GunBase GunInHands { get; private set; }
    protected void PickUpGun()
    {
        if (GunToPickUp == null ||
        Vector2.Distance(GunToPickUp.State.Lies.transform.position, gameObject.transform.position) > 0.75f) { return; }
        PickUpGun(GunToPickUp);
        SoundPlayer.PlaySound("PickUpWeapon");
    }
    public void PickUpGun(GunBase Gun)
    {
        if (Locked || Gun == null) { return; }
        Gun.State.SetWorldState(WorldState.Stand);
        Gun.transform.parent = StateKeeper.Stand.transform;
        GunInHands = Gun;
        GunInHands.transform.position = Vector3.zero;
        MyAnim.Play(GunInHands.InHands.Moving);
        if (GunInHands.State.Lies != null)
        {
            GunInHands.State.Lies.transform.position = Vector3.zero;
            if (GunInHands.State.Lies.TryGetComponent<Rigidbody2D>(out Rigidbody2D Body))
            {
                Body.velocity = Vector2.zero;
            }
        }
        GunEquipped = GunInHands != DefaultGun;
        Gun.Owner = this;
    }
    [SerializeField]
    float ThrowForce = 100;
    protected void DropGun(bool Throw)
    {
        //Check for owner
        if (GunInHands == DefaultGun || Locked) { return; }
        GunInHands.Owner = null;

        //Set transform for in hands gun
        GunInHands.transform.parent = FloorManager.GetCurrentFloorObj().transform;
        GunInHands.transform.position = gameObject.transform.position;
        GunInHands.transform.rotation = Quaternion.identity;

        //Set transform for lies version of gun
        GunInHands.State.Lies.transform.rotation = Quaternion.Euler(0,0, gameObject.transform.eulerAngles.z - 90);
        GunInHands.State.SetWorldState(WorldState.Lies);

        Rigidbody2D GunBody = GunInHands.State.Lies.GetComponentInChildren<Rigidbody2D>();        
        if (Throw)
        {
            //Throw gun if it need
            GunBody.AddRelativeForce(new Vector2(0, ThrowForce), ForceMode2D.Impulse);
        }
        GunBody.AddTorque(ThrowForce * 5);
        
        GunInHands.DisablePickupTrigger(0.01f);
        PickUpGun(GunToPickUp == null ? DefaultGun : GunToPickUp);
    }

    bool GunEquipped = false;
    public bool HasGun()
    {
        return GunEquipped;
    }
    protected void Shoot()
    {
        if (GunInHands != null && !Locked)
        {
            GunInHands.State.Stand.transform.rotation = Quaternion.Euler(0, 0, gameObject.transform.eulerAngles.z - 90);
            GunInHands.transform.localPosition = Vector3.zero;
            if (GunInHands.Shoot())
            {
                MyAnim.Play(GunInHands.InHands.Using);
                if (GunInHands.InvertAfterUse) { transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1); }
            }
        }
    }
    public Sprite[] DieSprites;
    public void Die(bool Fully = true, GameObject Killer = null)
    {
        Move(0, 0);
        DropGun(false);
        Locked = true;

        if (Killer != null)
        {
            LookAt(Killer.transform.position);
            transform.eulerAngles += new Vector3(0, 0, 180);
        }

        if (!Fully)
        {
            StateKeeper.StartCoroutine(StandUp());
        }
        StateKeeper.SetWorldState(WorldState.Lies);

        IEnumerator StandUp()
        {
            yield return new WaitForSeconds(2);
            Locked = false;
            StateKeeper.SetWorldState(WorldState.Stand);
        }
    }

    [SerializeField]
    float RotationSpeed = 20f;
    private void FixedUpdate()
    {
        //Remove gun to make items rotation
        GunToPickUp = null;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, Angle), Time.deltaTime * RotationSpeed);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Gun")
        {
            GunToPickUp = collision.GetComponentInParent<GunBase>();
        }
    }
    protected virtual void Update()
    {
        if (MyLegs != null)
        {
            MyLegs.transform.position = transform.position;
        }
    }
    //Короче, латаем баги. Круговой скид и куча всего, блядь
}