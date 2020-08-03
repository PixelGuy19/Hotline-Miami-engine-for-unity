using UnityEngine;

public class WorldStateKeeper : MonoBehaviour
{
    public GameObject Stand, Lies; //Incapsulate this

    [SerializeField]
    WorldState CurrentState;

    public WorldState GetCurrentState()
    {
        return CurrentState;
    }
    public void SetWorldState(WorldState State)
    {
        if (Stand != null) { Stand.SetActive(State == WorldState.Stand); }
        if (Lies != null) { Lies.SetActive(State == WorldState.Lies); }
        CurrentState = State;
    }
}
public enum WorldState { Stand, Lies }