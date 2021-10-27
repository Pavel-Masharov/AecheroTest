using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected float health;
    public bool isDeath { get; protected set; }
    void Start()
    {
        isDeath = false;
    }
    public void TakingDamage(float damage) 
    {
        health -= damage;

        if(PlayerController.levelPlayer>2)
        {
            PlayerController.SetHealthVampir(damage);
        }
        
    }
    protected void Death(float experience, ParticleSystem particleDeath)
    {
        if( health <= 0)
        {
            particleDeath.Play();
            isDeath = true;
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
          
            PlayerController.SetExp(experience);

            Destroy(gameObject,3f);

          
        }

        
    }
}
