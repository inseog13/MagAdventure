using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    enum Type { Velocity, MovePosition, AddForce, Slide }
    [SerializeField]
    Type logicType;
    [SerializeField]
    float speed;
    float inputValue;
    float inputPower;

    Rigidbody2D body;
    Animator anim;
    SpriteRenderer spriter;
    Rigidbody2D.SlideMovement slide;

    [SerializeField]
    GameObject breaker;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        slide = new Rigidbody2D.SlideMovement();
    }

    void FixedUpdate()
    {
        switch (logicType)
        {
            case Type.Velocity:
                body.linearVelocityX = inputValue * speed;
                break;
            case Type.AddForce:
                body.AddForceX(inputValue * speed);
                break;
            case Type.MovePosition:
                inputPower = Mathf.MoveTowards(inputPower, Physics2D.gravity.y, 0.5f);
                body.MovePosition(body.position
                    + Vector2.right * inputValue * speed * Time.deltaTime
                    + Vector2.up * inputPower * Time.deltaTime);
                break;
            case Type.Slide:
                inputPower = Mathf.MoveTowards(inputPower, 0, 0.5f);
                body.Slide(Vector2.right * inputValue * speed
                    + Vector2.up * inputPower, Time.deltaTime, slide);
                break;
        }
    }

    void LateUpdate()
    {
        anim.SetFloat("speed", Mathf.Abs(inputValue));

        if (inputValue != 0)
            spriter.flipX = inputValue < 0;
    }

    void OnMove(InputValue value)
    {
        inputValue = value.Get<Vector2>().x;
        breaker.SetActive(inputValue == 0);
    }

    void SetVertical(float power)
    {
        slide.surfaceAnchor = Vector2.zero;
        inputPower = power;
    }
}
