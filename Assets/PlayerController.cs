using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 3.0f;

    [SerializeField] GameObject frontMeleeHitBox;
    [SerializeField] GameObject sideMeleeHitBox;
    [SerializeField] GameObject backMeleeHitBox;

    Rigidbody2D rb;
    Animator anim;

    Vector2 movement;
    Vector3 flippedScale;
    Vector3 notFlippedScale;
    int direction = 0; //0 - forward, 1 - back, 2 - left, 3 - right

    bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        notFlippedScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        flippedScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleAttacking();
    }

    void HandleMovement()
    {
        rb.velocity = movement * playerSpeed;

        if (!isAttacking)
        {
            if (movement != Vector2.zero)
            {
                if (Mathf.Abs(movement.x) > 0.5f)
                {
                    anim.Play("Base Layer.SideWalk");
                    //Flip the sprite if moving left or right
                    if (movement.x < 0f)
                    {
                        direction = 2;
                        transform.localScale = flippedScale;
                    }
                    else if (movement.x > 0f)
                    {
                        direction = 3;
                        transform.localScale = notFlippedScale;
                    }
                }
                else
                {
                    if (movement.y < 0f)
                    {
                        direction = 0;
                        anim.Play("Base Layer.FrontWalk");
                    }
                    else if (movement.y > 0f)
                    {
                        direction = 1;
                        anim.Play("Base Layer.BackWalk");
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 0:
                        anim.Play("Base Layer.FrontIdle");
                        break;
                    case 1:
                        anim.Play("Base Layer.BackIdle");
                        break;
                    case 2:
                        anim.Play("Base Layer.SideIdle");
                        break;
                    case 3:
                        anim.Play("Base Layer.SideIdle");
                        break;
                }
            }
        }
    }

    void HandleAttacking()
    {
        if (isAttacking)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Base Layer.FrontSlash") || stateInfo.IsName("Base Layer.BackSlash") || stateInfo.IsName("Base Layer.SideSlash"))
            {
                if (stateInfo.normalizedTime > 0.9f)
                {
                    frontMeleeHitBox.SetActive(false);
                    backMeleeHitBox.SetActive(false);
                    sideMeleeHitBox.SetActive(false);
                    isAttacking = false;
                }
            }
            else
            {
                switch (direction)
                {
                    case 0:
                        anim.Play("Base Layer.FrontSlash");
                        frontMeleeHitBox.SetActive(true);
                        break;
                    case 1:
                        anim.Play("Base Layer.BackSlash");
                        backMeleeHitBox.SetActive(true);
                        break;
                    case 2:
                        anim.Play("Base Layer.SideSlash");
                        sideMeleeHitBox.SetActive(true);
                        break;
                    case 3:
                        anim.Play("Base Layer.SideSlash");
                        sideMeleeHitBox.SetActive(true);
                        break;
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    public void Fire()
    {
        isAttacking = true;
    }
}
