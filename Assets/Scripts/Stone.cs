using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stone : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleStoneHit;
    public float damage { get; private set; } = 0f;

    private void Start()
    {
        damage = 0f;
        Invoke(nameof(GetDamage), 0.5f);
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            particleStoneHit.Play();
            Destroy(gameObject, 0.7f);
        }
    }

    private void GetDamage()
    {
        damage = 30f;
    }
 
}
