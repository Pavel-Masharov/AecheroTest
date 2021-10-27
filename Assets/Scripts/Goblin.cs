using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Goblin : Enemy
{
    [SerializeField] ParticleSystem particleDeath;

    [SerializeField] private Slider sliderHealth;
    private readonly float maxHealth = 250f;

    private GameObject player;

    [SerializeField] private GameObject stonePrefab;
    private GameObject stone;

    private Animator anim;
    static readonly int throwStateHash = Animator.StringToHash("throw");

    [SerializeField] private Transform positionShot;
    public float AngleInDegrees; 

    private readonly float exp = 20f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = maxHealth;    
    }

    void Update()
    {
        sliderHealth.value = ColculateHealth(); //Значение здоровья
        Throw(GetTrajectory()); // Бросаем камень в игрока 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ArrowLighting>())
        {
            float d = collision.gameObject.GetComponent<ArrowLighting>().damage;
            TakingDamage(d);
            Death(exp, particleDeath);

        }
        if (collision.gameObject.GetComponent<Stone>())
        {
            float d = collision.gameObject.GetComponent<Stone>().damage;
            TakingDamage(d);
            Death(exp, particleDeath);
        }
    }

    private void Throw(float force) // Бросаем камень
    {
        if(!isDeath)
        {
            transform.LookAt(player.transform.position);
            positionShot.localEulerAngles = new Vector3(-AngleInDegrees, 0f, 0f);

            if (stone == null)
            {
                anim.SetTrigger(throwStateHash);
                stone = Instantiate(stonePrefab, positionShot.transform.position, positionShot.transform.rotation);
                stone.GetComponent<Rigidbody>().velocity = positionShot.forward * force;
            }
        }       
    }

    private float GetTrajectory() //Получаем параболу
    {
        Vector3 fromTo = player.transform.position - transform.position; //получаем вектор выстрела
        Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z); // Получаем проекцию без учета Y

        float x = fromToXZ.magnitude; //Получаем расстояние до цели
        float y = fromTo.y;

        float AngleInRadians = AngleInDegrees * Mathf.PI / 180; //Вычисляем угол в радианах

        float gravity = 9.8f; //гравитация

        float v2 = (gravity * x * x) / (2 * (y - Mathf.Tan(AngleInRadians) * x) * Mathf.Pow(Mathf.Cos(AngleInRadians), 2)); //Вычисляем квадрат скорости

        float v = Mathf.Sqrt(Mathf.Abs(v2)); //Извлекаем корень

        return v;
    }

    private float ColculateHealth()
    {
        return health / maxHealth;
    }
}
