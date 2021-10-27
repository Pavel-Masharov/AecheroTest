using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : ArrowLighting
{
    private int numberOfAffected;

    private readonly float speedFlyL = 30f;

    private Enemy newTarget;
    public float damageL { get; private set; }

    void Start()
    {
        damageL = 80;
        numberOfAffected = 4;
       
        newTarget = FindNewTarget();
    }

    
    void Update()
    {
        MoveToNewTarget(newTarget); 
    }

    private Enemy FindNewTarget()
    {
        Enemy[] allEnemy;
        allEnemy = FindObjectsOfType<Enemy>();
        Enemy closestEnemy = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Enemy goEnemy in allEnemy)
        {
            if (!goEnemy.GetComponent<Enemy>().isDeath)
            {
                if (!goEnemy.GetComponent<IsElectro>())
                {
                    Vector3 dirGoEnemy = goEnemy.transform.position - position;
                    float curDistance = dirGoEnemy.sqrMagnitude;

                    if (curDistance < distance)
                    {
                        distance = curDistance;
                        closestEnemy = goEnemy;
                    }
                }
                else
                    continue;
            }
            else
                continue;        
        }
        return closestEnemy;
    }

    private void MoveToNewTarget(Enemy closestEnemy)
    {
        if(closestEnemy != null && numberOfAffected > 0)
        {   
            Vector3 target = new Vector3(closestEnemy.transform.position.x, 1.5f, closestEnemy.transform.position.z);
            transform.LookAt(target);
            transform.Translate(speedFlyL * Time.deltaTime * Vector3.forward);   
        }
        else if(closestEnemy == null)
        {
            DeliteAllElectro();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {          
            other.gameObject.AddComponent<IsElectro>(); // Ставим метку      
            newTarget = FindNewTarget(); // Запускаем метод поиска новой цели
            numberOfAffected--; // Уменьшаем возможное количество пороженных целей
            CalculateThePercentage(damageL, 35); // Вычисляем Урон от заряда
            other.gameObject.GetComponent<Goblin>().TakingDamage(damageL); // Наносим урон от заряда
            CreateRingLighting(other);
        }     
    }

    
    private void DeliteAllElectro() // Метод удаления меток заряда
    {
        GameObject[] allEnemy;
        allEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in allEnemy)
        {
            Destroy(item.GetComponent<IsElectro>());
        }
    }

    private float CalculateThePercentage(float value, float percent) //Метод вычислени урона от заряда
    {
       float result = (percent / 100) * value;
       return damageL -= result;
    }
    
    private void CreateRingLighting(Collider other)
    {
        Vector3 posRing = new Vector3(other.gameObject.transform.position.x, 1.5f, other.gameObject.transform.position.z);
        var ring = Instantiate(ringLighting, posRing, other.gameObject.transform.rotation);
        Destroy(ring.gameObject, 0.5f);
    }
}
