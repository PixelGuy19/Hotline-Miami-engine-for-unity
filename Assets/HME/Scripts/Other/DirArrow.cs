using UnityEngine;

public class DirArrow : MonoBehaviour
{
    [SerializeField]
    ArrowDirection Direction = ArrowDirection.Up;
    [SerializeField]
    float Offset = 1, AnimTime = 1;
    float Timer;
    Vector2 StartArrowPos;
    private void Awake()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, (float)Direction);
        StartArrowPos = gameObject.transform.position;
    }
    private void Update()
    {
        Timer = Mathf.PingPong(Time.time, AnimTime);
        float Percent = Timer / AnimTime;
        Percent = Easing.Quadratic.InOut(Percent);

        if (Direction == ArrowDirection.Right || Direction == ArrowDirection.Left)
        {
            gameObject.transform.position = StartArrowPos + 
                new Vector2(Offset * Percent * -Mathf.Sign((float)Direction), 0);
        }
        else
        {
            gameObject.transform.position = StartArrowPos +
                new Vector2(0, Offset * Percent * -Mathf.Sign((float)Direction));
        }
    }

    enum ArrowDirection
    {
        Up = 0, Right = -90, Down = -180, Left = 90
    }
}