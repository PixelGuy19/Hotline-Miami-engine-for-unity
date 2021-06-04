using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorEdge : MonoBehaviour
{
    [SerializeField]
    Tilemap MyTilemap = default;
    [SerializeField]
    EdgeCollider2D MyEdge = default;
    [SerializeField]
    int EdgeOffset = 1;
    private void Awake()
    {
        if (MyEdge == null || MyTilemap == null) { return; }

        MyEdge.points = new Vector2[]
        {
            (Vector2Int)MyTilemap.cellBounds.min +
            (Vector2)MyTilemap.tileAnchor + new Vector2(-EdgeOffset,-EdgeOffset),
            new Vector2(MyTilemap.cellBounds.min.x, MyTilemap.cellBounds.max.y)
            + (Vector2)MyTilemap.tileAnchor + new Vector2(-EdgeOffset,+EdgeOffset),
            (Vector2Int)MyTilemap.cellBounds.max +
            (Vector2)MyTilemap.tileAnchor + new Vector2(EdgeOffset,EdgeOffset),
            new Vector2(MyTilemap.cellBounds.max.x, MyTilemap.cellBounds.min.y)
            + (Vector2)MyTilemap.tileAnchor + new Vector2(EdgeOffset,-EdgeOffset),
            (Vector2Int)MyTilemap.cellBounds.min
            + (Vector2)MyTilemap.tileAnchor + new Vector2(-EdgeOffset,-EdgeOffset)
        };
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Momo");
            StartCoroutine(FadeOutCollision());
        }
        IEnumerator FadeOutCollision()
        {
            SpriteRenderer Rend = collision.GetComponent<SpriteRenderer>();
            if (Rend == null) { Rend = collision.GetComponentInParent<SpriteRenderer>(); }

            for (float i = Rend.color.a; i > -1; i -= Time.deltaTime * 10)
            {
                yield return null;
                Rend.color = new Color(Rend.color.r, Rend.color.g, Rend.color.b, i);
            }
            Destroy(collision);
        }
    }
}
