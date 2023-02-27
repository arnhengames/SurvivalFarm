using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    CircleCollider2D myCollider;

    Vector2 movement;
    Vector3 flippedScale;
    Vector3 notFlippedScale;
    int direction = 0; //0 - forward, 1 - back, 2 - left, 3 - right

    bool isAttacking = false;
    bool isJumping = false;

    public int currentLayer = 1;

    Vector3 targetPos;

    private void Start()
    {
        targetPos = FindObjectOfType<LevelManager>().TransformPosToGridPos(transform.position, currentLayer);
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        myCollider = GetComponent<CircleCollider2D>(); 

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
        //MOVING
        targetPos += new Vector3(movement.x, movement.y, 0.0f) * playerSpeed * Time.deltaTime;
        transform.position = FindObjectOfType<LevelManager>().TransformPosToGridPos(targetPos, currentLayer);

        //ANIMATING
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

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 startPos = new Vector3(0f, 0.24f, 0f);

        startPos = LevelManager.StepLengthAxis(startPos, -50);

        for (int i = 0; i < 100; i++)
        {
            Gizmos.DrawLine(LevelManager.StepWidthAxis(startPos, 100), LevelManager.StepWidthAxis(startPos, -100));
            startPos = LevelManager.StepLengthAxis(startPos, 1);
        }

        Gizmos.color = Color.red;

        startPos = new Vector3(0f, 0.24f, 0f);

        startPos = LevelManager.StepWidthAxis(startPos, -50);

        for (int i = 0; i < 100; i++)
        {
            Gizmos.DrawLine(LevelManager.StepLengthAxis(startPos, 100), LevelManager.StepLengthAxis(startPos, -100));
            startPos = LevelManager.StepWidthAxis(startPos, 1);
        }
    }*/
}
