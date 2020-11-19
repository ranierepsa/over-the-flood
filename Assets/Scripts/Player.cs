using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Constants
    const string RUNNING = "Running";
    const string JUMP = "Jump";
    const string CLIMBING = "Climbing";
    const string DYING = "Dying";
    const string LAYER_ENEMY = "Enemy";
    const string LAYER_TRAPS = "Traps";
    const string LAYER_LADDER = "Ladder";
    const string LAYER_GROUND = "Ground";
    const string AXIS_VERTICAL = "Vertical";
    const string AXIS_HORIZONTAL = "Horizontal";

    // Config variables
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float verticalSpeed = 5f;
    [SerializeField] Vector2 dyingVelocity = new Vector2(0, 100);

    // State variables
    bool isAlive = true;
    bool isClimbing = false;
    float initialGravity;

    // Cached references
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        initialGravity = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (isAlive)
        {
            Run();
            Jump();
            Climb();
            FlipSprite();
        }
    }

    private void Run()
    {
        float horizontalVelocity = CrossPlatformInputManager.GetAxis(AXIS_HORIZONTAL);
        myRigidbody.velocity = new Vector2(horizontalVelocity * runSpeed, myRigidbody.velocity.y);
        SetAnimationParameter(RUNNING, horizontalVelocity != 0);
    }

    private void Jump()
    {
        if (!GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask(LAYER_GROUND))) { return; }
        if (CrossPlatformInputManager.GetButtonDown(JUMP))
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);
        }
    }

    private void Climb()
    {
        bool isTouchingLadder = IsTouchingLadder();
        if (!isTouchingLadder && isClimbing)
        {
            LeaveLadder();
        }
        else if (!isTouchingLadder)
        {
            return;
        }
        else if (IsMovingVertically())
        {
            StartClimbingAnimation();
        }
        else if (isClimbing)
        {
            StopClimbingAnimation();
        }
        else
        {
            ResetGravity();
        }
    }

    private void ResetGravity()
    {
        myRigidbody.gravityScale = initialGravity;
    }

    private void LeaveLadder()
    {
        SetAnimationParameter(CLIMBING, false);
        myAnimator.StopPlayback();
        isClimbing = false;
        myRigidbody.gravityScale = initialGravity;
    }

    private bool IsTouchingLadder()
    {
        return GetComponent<CapsuleCollider2D>().IsTouchingLayers(LayerMask.GetMask(LAYER_LADDER));
    }

    private bool IsMovingVertically()
    {
        return CrossPlatformInputManager.GetAxis(AXIS_VERTICAL) != 0;
    }

    private void StartClimbingAnimation()
    {
        float verticalVelocity = CrossPlatformInputManager.GetAxis(AXIS_VERTICAL);
        myRigidbody.gravityScale = 0;
        isClimbing = true;
        myAnimator.StopPlayback();
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, verticalVelocity * verticalSpeed);
        SetAnimationParameter(RUNNING, false);
        SetAnimationParameter(CLIMBING, verticalVelocity != 0);
    }

    private void StopClimbingAnimation()
    {
        myRigidbody.gravityScale = 0;
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        myAnimator.StartPlayback();
    }

    private void FlipSprite()
    {
        float horizontalSpeed = myRigidbody.velocity.x;
        if (horizontalSpeed == 0) return;
        GetComponentInChildren<SpriteRenderer>().flipX = horizontalSpeed < 0;
    }

    private void SetAnimationParameter(string parameter, bool value)
    {
        myAnimator.SetBool(parameter, value);
    }

    private bool IsTouchingLayer(string layer)
    {
        return myRigidbody.IsTouchingLayers(LayerMask.GetMask(layer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Die();
    }

    private void Die()
    {
        if (isAlive && (IsTouchingLayer(LAYER_ENEMY) || IsTouchingLayer(LAYER_TRAPS)))
        {
            myAnimator.SetTrigger(DYING);
            myRigidbody.velocity = dyingVelocity;
            GetComponent<CapsuleCollider2D>().sharedMaterial = null;
            Destroy(GetComponent<BoxCollider2D>());
            isAlive = false;
            StartCoroutine(ProcessDeathAfterSeconds(2f));
        }
    }

    private IEnumerator ProcessDeathAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
