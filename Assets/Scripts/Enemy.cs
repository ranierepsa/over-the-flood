using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.velocity = new Vector2(moveSpeed, 0f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetType() != typeof(CapsuleCollider2D))
            Flip();
    }

    private void Flip()
    {
        moveSpeed *= -1;
        transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1f);
    }
}
