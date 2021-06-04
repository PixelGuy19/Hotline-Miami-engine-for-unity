using UnityEngine;

public class PlayerBase : EntityBase
{
    static PlayerBase Player;
    private void Awake()
    {
        Player = this;
    }

    public static PlayerBase GetPlayer()
    {
        return Player;
    }

    public static void LockControls(bool Is)
    {
        EntityBase Player = GetPlayer();
        if (Player != null) { GetPlayer().Locked = Is; }
    }
    [Header("Player")]
    [SerializeField]
    GameObject FarVis = default;
    protected override void Update()
    {
        base.Update();
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Locked) { Move(0, 0); return; }

        if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!HasGun())
            {
                PickUpGun();
            }
            else
            {
                DropGun(true);
                SoundPlayer.PlaySound("Throw");
            }
        }
        Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Vector3.Distance(MousePos, transform.position) > 0.1) { LookAt(MousePos); }
        Camera.main.transform.position = (Input.GetKey(KeyCode.LeftShift) ? FarVis.transform.position :
            transform.position) + new Vector3(0, 0, -10);
    }
}
