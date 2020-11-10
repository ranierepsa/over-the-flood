using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Constants
    const string RUNNING = "Running";

    // Config variables
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;

    // State variables
    bool isAlive = true;

    // Cached references
    Rigidbody2D myRigidbody;
    SpriteRenderer spriteRenderer;
    Animator myAnimator;
    Collider2D myCollider;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        FlipSprite();
    }

    private void Run()
    {
        float horizontalVelocity = CrossPlatformInputManager.GetAxis("Horizontal");
        myRigidbody.velocity = new Vector2(horizontalVelocity * runSpeed, myRigidbody.velocity.y);
        myAnimator.SetBool(RUNNING, horizontalVelocity != 0);
    }

    private void Jump()
    {
        if (!myCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);
        }
    }

    private void FlipSprite()
    {
        float horizontalSpeed = myRigidbody.velocity.x;
        if (horizontalSpeed == 0) return;
        spriteRenderer.flipX = horizontalSpeed < 0;
    }
}
