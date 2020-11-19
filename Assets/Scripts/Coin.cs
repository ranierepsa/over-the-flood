using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int pointsForCoinPickup = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType() == typeof(CapsuleCollider2D))
        {
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
            AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
