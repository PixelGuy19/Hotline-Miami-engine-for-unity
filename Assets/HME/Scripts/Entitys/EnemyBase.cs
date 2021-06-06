using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

public class EnemyBase : EntityBase
{
    private void OnValidate()
    {
        if (RefoundTime <= 0) { RefoundTime += 0.01f; }
    }

    Vector2 LastTargetPos;
    float DistanceToLastTargetPos;

    [Header("AI")]
    [SerializeField]
    bool DoorUnstandable;
    [SerializeField]
    float LookingDistance = 7;

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
    protected void MoveTo(Vector2 Position, bool CutPath = false, Action OnReached = null) //Rework this method to "confetca" condition
    {
        if (!gameObject.activeSelf) { return; }
        if (Seeker.IsDone())
        {
            Seeker.StartPath(MyBody.position, Position, (Path Path) =>
            {
                if (MovingCoroutine != null) { ForcedStop(); }
                MovingCoroutine = StartCoroutine(AIMove(Path));
            });
        }

        IEnumerator AIMove(Path CurrentPath)
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
    protected void ShootTo(Vector2 Position)
    {
        if (GunInHands == null) { return; }
        LookAt(Position);
        Debug.Log($"Distance {DistanceToLastTargetPos}/{LookingDistance}({DistanceToLastTargetPos / LookingDistance * 100}%)" +
            $"\n Tolerance angle = {(5 - 5 * DistanceToLastTargetPos / LookingDistance)}" +
            $"\n Angle to rot = {Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, Angle))}");
        //(5 - 5 * DistanceToLastTargetPos / LookingDistance)
        if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, Angle)) < GunInHands.Spread / 2) { Shoot(); }
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
    protected void Patrol()
    {
        if(PatrolMethod == PatrolMode.StandStill) { return; }
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
                RaycastHit2D ForwardCheck = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 1, 
                    Mask);
                if (ForwardCheck.transform != null)
                {
                    transform.eulerAngles -= new Vector3(0, 0, 90f);
                }
                yield return new WaitForFixedUpdate();
                if (IsStoped() && !Locked) { Patrol(); }
            }
        }        
    }

    [SerializeField]
    float RefoundTime = 2;
    [SerializeField]
    bool ReturnAfter = true;
    [SerializeField]
    float RecheckTime = 0.5f;
    [SerializeField]
    Way PatrolWay = default;
    IEnumerator Srategy() //И так, осталось допилить остальную функциональность
    {
        bool NewTargetPosFounded = false;

        StartCoroutine(Looking());
        while (true)
        {
            if (NewTargetPosFounded)
            {
                yield return new WaitForSeconds(RecheckTime);

                StartCoroutine(Research());
                if (NewTargetPosFounded)
                {
                    if (GunInHands != null) 
                    {
                        if (DistanceToLastTargetPos < LookingDistance)
                        {
                            ShootTo(LastTargetPos);
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
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (ReturnAfter)
            {
                transform.rotation = Quaternion.identity;
                MoveTo(PatrolWay.Waypoints[0], default, Patrol);
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
    public void Reboot()
    {
        PickUpGun(DefaultGun);
        DefaultGun = null;
        StartCoroutine(Srategy());
        Patrol();
    }

    [SerializeField]
    bool StartFromFirstWaypoint = true;
    protected override void Start()
    {
        if (StartFromFirstWaypoint) { transform.position = PatrolWay.Waypoints[0]; }
        Reboot();
        base.Start();
    }
    private void OnEnable()
    {
        Reboot();
        if (GunInHands != null) { MyAnim.Play(GunInHands.InHands.Moving); }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, LookingDistance);
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