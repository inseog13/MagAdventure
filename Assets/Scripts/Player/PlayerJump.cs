using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    enum Type {Velocity, Raycast, Overlap, Collider, isTorch}
    [SerializeField]
    Type logicType;
    [SerializeField]
    LayerMask groundLayer;
    
    public bool isGround;

    [SerializeField]
    float jumpPower;
    [SerializeField]
    float minVelocityY;
    [SerializeField]
    float maxVelocityY;

    Rigidbody2D body;
    Animator anim;
    Collider2D coll;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = FindFirstObjectByType<CompositeCollider2D>();
    }

    private void FixedUpdate()
    {
        switch (logicType)
        {
            case Type.Raycast:
                RaycastHit2D hit = Physics2D.CircleCast(body.position
                    + Vector2.down * 0.3f, 0.2f, Vector2.zero, 0f, groundLayer);
                if (hit.collider)
                {
                    isGround = true;
                }
                break;
            case Type.Overlap:
                Collider2D hitColl = Physics2D.OverlapCircle(body.position
                    + Vector2.down * 0.3f, 0.2f, groundLayer);
                if (hitColl)
                {
                    isGround = true;
                }
                break;
            case Type.isTorch:
                isGround = body.IsTouching(coll);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ground") || logicType != Type.Collider)
            return;

        if (body.linearVelocityY > minVelocityY)
            return;

        isGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ground") || logicType != Type.Collider)
            return;

        isGround = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //논그라운드 태그&&속력타입 필터;
        if (!collision.gameObject.CompareTag("Ground") && logicType != Type.Velocity)
            return;

        if (body.linearVelocityY > 1)
            return;

        isGround = collision.contacts[0].normal.y > 0;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //논그라운드 태그&&충돌타입 필터;
        if (!collision.gameObject.CompareTag("Ground") && logicType == Type.Collider)
            return;

        if (body.linearVelocityY < 1)
            return;

        isGround = false;
    }

    private void LateUpdate()
    {
        anim.SetBool("Ground", isGround);
    }

    void OnJump()
    {
        if (!isGround)
            return;

        body.AddForceY(jumpPower, ForceMode2D.Impulse);

        //캐쥬얼한 게임 한정 bec.리소스 많이먹음;
        //SendMessage("SetVertical", jumpPower);
    }
}
