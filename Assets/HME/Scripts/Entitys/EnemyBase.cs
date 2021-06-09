using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

public class EnemyBase : EntityBase
{
    private void OnValidate()
    {
        if (RefoundTime <= 0) { RefoundTime = 0.01f; }
    }

    Vector2 LastTargetPos;
    float DistanceToLastTargetPos;

    [Header("AI")]
    [SerializeField]
    bool DoorUnstandable = false;
    [SerializeField]
    float LookingDistance = 10;

    /// <summary>
    /// Look around to see if I see objects
    /// </summary>
    /// <param name="Tag">Trigger tag</param>
    /// <param name="Objects">Objects what I'm looking for</param>
    /// <returns>Was one of the objects found?</returns>
    protected bool LookAround(string Tag = "Player", params GameObject[] Objects)
    {
        //In this point, I should look at all objects
        foreach (GameObject RayToObj in Objects)
        {
            //If distance between object and me more then LookingDistance, i will not do this method
            float DistanceToObj = Vector3.Distance(transform.position, RayToObj.transform.position);
            if (DistanceToObj > LookingDistance) { continue; }

            //Calculating the direction where I will looking
            Vector3 Dir = RayToObj.transform.position - transform.position;
            float Angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg - transform.eulerAngles.z;
            Vector3 DirVec = transform.TransformDirection(Quaternion.Euler(0, 0, Angle)
                * Vector3.right * DistanceToObj);

            //Mask for layers what I should not raycast
            int Mask = ~LayerMask.GetMask("Ignore Raycast", "HalfWall");
            //Now I prepare my laser to RAAAAYCAST
            RaycastHit2D Hit =
                Physics2D.Raycast(transform.position, DirVec, DistanceToObj, Mask);
            Debug.DrawRay(transform.position, DirVec);
            //Some object filters
            if (Hit.transform == null) { continue; }
            else if (Hit.transform.tag == Tag)
            {
                //Voila. We found a new target.
                LastTargetPos = Hit.transform.position;
                DistanceToLastTargetPos = Hit.distance;
                return true;
            }
        }

        //Target didn't exist or target was wrong
        return false;
    }

    [SerializeField]
    Seeker Seeker = default;
    [SerializeField]
    float WaypointDistance = 0.25f;

    Coroutine MovingCoroutine;
    protected void MoveTo(Vector2 Position, bool CutPath = false, Action OnReached = null)
    {
        if (!gameObject.activeSelf) { return; }
        if (Seeker.IsDone())
        {
            Seeker.StartPath(MyBody.position, Position, (Path Path) =>
            {
                if (MovingCoroutine != null) { ForcedStop(); }
                MovingCoroutine = StartCoroutine(PathMove(Path));
            });
        }

        IEnumerator PathMove(Path CurrentPath)
        {
            IsMoving = true;

            int Cutout = GunInHands == null ? 0 : (int)GunInHands.MinShootDistance;
            if (!CutPath) { Cutout = 0; }

            for (int i = 1; i < CurrentPath.vectorPath.Count - Cutout; i++)
            {
                Vector2 Dir = ((Vector2)CurrentPath.vectorPath[i] - MyBody.position).normalized;
                LookAt(CurrentPath.vectorPath[i]);
                Move(Dir);
                while (!(Vector2.Distance(CurrentPath.vectorPath[i], MyBody.position) < WaypointDistance))
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            Move(0, 0);
            IsMoving = false;
            OnReached?.Invoke();
        }
    }
    [SerializeField]
    float ShootSlowDown = 0.1f;
    protected IEnumerator ShootTo(Vector2 Position)
    {
        if (GunInHands == null) { yield break; }
        yield return new WaitForSeconds(ShootSlowDown);
        LookAt(Position);
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, Angle)) //Angle to rotate
            < GunInHands.Spread / 2) //Half of spread degrees
        {            
            Shoot();
        }
    }

    bool IsMoving;
    protected void ForcedStop()
    {
        if (!IsStoped())
        {
            StopCoroutine(MovingCoroutine);
            Move(0, 0);
            IsMoving = false;
        }
    }
    protected bool IsStoped()
    {
        return MovingCoroutine == null ? true : !IsMoving;
    }
    protected void ForgetTarget()
    {
        LastTargetPos = new Vector2();
    }

    public PatrolMode PatrolMethod;
    int CurrentWaypoint = 0;
    bool PatrolPaused = false;
    protected void Patrol()
    {
        PatrolPaused = false;

        if (PatrolMethod == PatrolMode.StandStill) { return; }
        else if (PatrolMethod == PatrolMode.Path)
        {
            CurrentWaypoint++;
            if (CurrentWaypoint == PatrolWay.Waypoints.Count)
            { CurrentWaypoint = 0; }
            MoveTo(PatrolWay.Waypoints[CurrentWaypoint], false, Patrol);
        }
        else
        {
            StartCoroutine(PatrolMove());
            IEnumerator PatrolMove()
            {
                Move(transform.right);
                int Mask =~ LayerMask.GetMask("Ignore Raycast", "Door");
                Collider2D Wall = Physics2D.OverlapCircle(transform.position + transform.right, 0.1f);
                if (Wall != null)
                {
                    Angle = transform.eulerAngles.z - 90;
                    Move(0, 0);
                    yield return WaitForLookAt();
                }         
                yield return new WaitForFixedUpdate();
                yield return new WaitWhile(() => PatrolPaused);
                Patrol();                
            }
        }
    }
    IEnumerator WaitForLookAt()
    {
        yield return new WaitWhile(() =>
        Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, Angle)) > 0);
    }

    [SerializeField]
    float RefoundTime = 2;
    [SerializeField]
    bool ReturnAfter = true;
    [SerializeField]
    float RecheckTime = 0.01f;
    [SerializeField]
    Way PatrolWay = default;
    IEnumerator Srategy() //И так, осталось допилить остальную функциональность //Stop patrol problem
    {
        bool NewTargetPosFounded = false;
        Vector2 ChaseStartPlace = transform.position;

        StartCoroutine(Looking());
        while (true)
        {
            if (NewTargetPosFounded)
            {
                yield return new WaitForSeconds(RecheckTime); //it's important

                StartCoroutine(Research());
                if (NewTargetPosFounded)
                {
                    PatrolPaused = true;
                    if (GunInHands != null) 
                    {
                        if (DistanceToLastTargetPos < LookingDistance)
                        {
                            StartCoroutine(ShootTo(LastTargetPos));
                        }
                        if (DistanceToLastTargetPos > GunInHands.MinShootDistance)
                        {
                            MoveTo(LastTargetPos);
                        }
                        else
                        {
                            ForcedStop();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    MoveTo(LastTargetPos);
                }
            }
            
            yield return new WaitForEndOfFrame();
        }

        IEnumerator Research()
        {
            for (float i = 0; i < RefoundTime; i += 0.1f)
            {
                if (NewTargetPosFounded)
                {
                    ChaseStartPlace = transform.position;
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }

            if (ReturnAfter)
            {
                MoveTo(ChaseStartPlace, default, Patrol);                
            }
            else { Patrol(); }
        }
        IEnumerator Looking()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                NewTargetPosFounded = LookAround("Player", PlayerBase.GetPlayer().gameObject);
            }
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PickUpGun(DefaultGun);
        DefaultGun = null;
        StartCoroutine(Srategy());
        if (GunInHands != null) { MyAnim.Play(GunInHands.InHands.Moving); }
        PatrolPaused = false;
        Patrol();
    }

    [SerializeField]
    bool StartFromFirstWaypoint = true;
    protected override void Start()
    {
        if (StartFromFirstWaypoint) { transform.position = PatrolWay.Waypoints[0]; }
        OnEnable();
        base.Start();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, LookingDistance);
        Gizmos.DrawWireSphere(transform.position + transform.right, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Door>(out Door NextDoor))
        {
            if (DoorUnstandable && NextDoor.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                Die(false, NextDoor.gameObject);
            }
        }
    }

    public enum PatrolMode { Path, AroundWall, StandStill }
}