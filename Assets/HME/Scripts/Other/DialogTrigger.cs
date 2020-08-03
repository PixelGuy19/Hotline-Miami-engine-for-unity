using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public LocalizedMessage MyMessage;
    public bool Forced;
    public EntityBase Speaker;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((Input.GetButtonDown("Fire2") || Forced) && collision.tag == "Player")
        {
            DialogManager.LoadDialog(MyMessage);
            gameObject.SetActive(false);

            if(Speaker != null)
            {
                Speaker.LookAt(PlayerBase.GetPlayer().transform.position);
				PlayerBase.GetPlayer().LookAt(Speaker.transform.position);
            }
        }
    }
}
