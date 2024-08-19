using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Player_Move : MonoBehaviourPun, IPunObservable
{
    public float Speed;
    public Joystick joystick;
    public GameObject Item_pickup;
    public GameObject Item_putout;
    Rigidbody2D rigid;
    Animator anim;
    float h;
    float v;
    bool isHorizonMove;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector2 networkVelocity;
    private float lag;
    private bool isMoving;
    private float dirX;
    private float dirY;
    private Collider2D detectedItem = null;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rigid.gravityScale = 0;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (Item_pickup != null)
        {
            Item_pickup.SetActive(false);
            Item_pickup.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnItemPickupButtonClicked);
        }

        if (Item_putout != null)
        {
            Item_putout.SetActive(false);
        }
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            Camera playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 0, -10);
            playerCamera.orthographic = true;
            playerCamera.orthographicSize = 5;
        }

        StartCoroutine(CheckItemCollision());
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            h = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
            v = Input.GetAxisRaw("Vertical") + joystick.Vertical;
            isHorizonMove = Mathf.Abs(h) > Mathf.Abs(v);
            isMoving = !Mathf.Approximately(h, 0f) || !Mathf.Approximately(v, 0f);
            dirX = h;
            dirY = v;
            UpdateAnimation();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * Speed);
            rigid.velocity = networkVelocity;
            UpdateAnimation();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Vector2 moveVec = new Vector2(h, v).normalized;
            rigid.velocity = moveVec * Speed;
        }
    }

    void UpdateAnimation()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("DirX", dirX);
        anim.SetFloat("DirY", dirY);

        if (Mathf.Abs(dirX) + Mathf.Abs(dirY) == 0)
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
        else
        {
            anim.SetBool("Up", dirY > 0);
            anim.SetBool("Down", dirY < 0);
            anim.SetBool("Right", dirX > 0);
            anim.SetBool("Left", dirX < 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item") && photonView.IsMine)
        {
            detectedItem = collision;
            Item_pickup.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == detectedItem && photonView.IsMine)
        {
            detectedItem = null;
            Item_pickup.SetActive(false);
        }
    }

    void OnItemPickupButtonClicked()
    {
        if (photonView.IsMine && detectedItem != null)
        {
            photonView.RPC("PickupItem", RpcTarget.All, detectedItem.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void PickupItem(int itemID)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemID);
        if (itemPhotonView != null)
        {
            Debug.Log("Item picked up: " + itemPhotonView.gameObject.name);
            itemPhotonView.gameObject.SetActive(false);
        }
    }

    IEnumerator CheckItemCollision()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0);
            bool itemCollision = false;

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("item"))
                {
                    itemCollision = true;
                    break;
                }
            }

            if (Item_pickup != null)
            {
                Item_pickup.SetActive(itemCollision);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigid.velocity);
            stream.SendNext(isMoving);
            stream.SendNext(dirX);
            stream.SendNext(dirY);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector2)stream.ReceiveNext();
            isMoving = (bool)stream.ReceiveNext();
            dirX = (float)stream.ReceiveNext();
            dirY = (float)stream.ReceiveNext();
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (Vector3)networkVelocity * lag;
        }
    }
}
