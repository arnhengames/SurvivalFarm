using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public static class PlayerInfo
{
    public static int widthLocation = 0;
    public static int lengthLocation = 0;
    public static int heightLocation = 0;

    public static string GetPositionString()
    {
        return ("W: " + widthLocation + ", L: " + lengthLocation + ", H: " + heightLocation);
    }
}

public class PlayerController : MonoBehaviour
{
    //Gameplay variables for editor
    [SerializeField] float playerSpeed = 3.0f;

    //Components
    Rigidbody2D rb;
    Animator anim;

    //Movement & Animation
    Vector2 movement;
    Vector3 flippedScale;
    Vector3 notFlippedScale;
    int direction = 0; //0 - forward, 1 - back, 2 - left, 3 - right

    //Grid Position Management
    public Transform gridTracker;
    Vector3 targetPos;

    //Melee combat
    [SerializeField] GameObject frontMeleeHitBox;
    [SerializeField] GameObject sideMeleeHitBox;
    [SerializeField] GameObject backMeleeHitBox;
    bool isAttacking = false;

    //Debug Info
    public TMP_Text debugText1;

    private void Start()
    {
        targetPos = LevelData.StepHeightAxis(transform.position, PlayerInfo.heightLocation);
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        notFlippedScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        flippedScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleAttacking();
        GridUpdate();
    }  

    void GridUpdate()
    {
        GridPosition gridPos = LevelData.TransformPosToGridPos(gridTracker.transform.position);

        PlayerInfo.widthLocation = gridPos.w;
        PlayerInfo.lengthLocation = gridPos.l;

        debugText1.text = gridPos.GetValueString();
    }

    void HandleMovement()
    {
        //MOVING
        targetPos += new Vector3(movement.x, movement.y, 0.0f) * playerSpeed * Time.deltaTime;
        transform.position = LevelData.StepHeightAxis(targetPos, PlayerInfo.heightLocation);

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
}
