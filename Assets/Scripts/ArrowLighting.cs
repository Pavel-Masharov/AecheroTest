using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLighting : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleHit;
    public ParticleSystem ringLighting;
    [SerializeField] private GameObject lightning;

    public readonly float damage = 50f;

    Vector3 dir = new Vector3(0,-0.05f,1);
    private float speedFly = 25f;
    private void Update()
    {
        transform.Translate(speedFly * Time.deltaTime * dir);
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        particleHit.Play();
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(PlayerController.levelPlayer > 1)
            {
                Vector3 posRing = new Vector3(collision.gameObject.transform.position.x, 1.5f, collision.gameObject.transform.position.z);
                Instantiate(lightning, posRing, collision.gameObject.transform.rotation);
            }
        }
        speedFly = 0f;
        Destroy(gameObject, 0.3f);
    }
}
