using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ParticleSystem particleTakeDamage; // FX Получение урона

    [SerializeField] private Joystick joystick; // джойстик
    private readonly float speed = 6f; // скорость движения

    private Animator anim; // аниматор
    static readonly int runStateHash = Animator.StringToHash("isRun");  // хеш состояния бега
    static readonly int shotStateHash = Animator.StringToHash("shot");  // хеш выстрела

    [SerializeField] private ArrowLighting arrowPrefab; // префаб стрелы
    [SerializeField] private Transform positionShot; // координаты вылета стрелы
    private ArrowLighting arrow; // поле для хранения активной стрелы

    public static float healthPlayer { get; private set;} //Здоровье игрока
    public static float maxHealthPlayer { get;} = 600f; //Максимальное здоровье

    public static float experiencePlayer { get; private set;} // текущий опыт игрока
    public static float needExpPlayer { get; private set;} = 100f; // необходимый опыт

    public static int levelPlayer { get; private set;} = 1; // Уровень игрока

    public static bool isDeath { get; private set;} // состояние игрока
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        isDeath = false;
        experiencePlayer = 0;
        needExpPlayer = 100;
        levelPlayer = 1;
        healthPlayer = maxHealthPlayer;
    }
    private void Update()
    {
        if(!isDeath)
        {
            MovePlayer();                   //Метод движения и поворота
            AnimationPlayer();              //Упрвление анимацией
            Shot(FindClosestEnemy());       //Выстрел в ближайшего противника
            HealthControl();                // Контроль здоровья
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TakeDamage"))
        {
            float damage = collision.gameObject.GetComponent<Stone>().damage;
            TakingDamage(damage);
        }
    }

    private void MovePlayer() //Метод движения и поворота
    {
        //Поворот в сторону джойстика
        Vector3 joysticDirection = joystick.Direction;
        joysticDirection.z = joysticDirection.y;
        Vector3 lookDirection = joysticDirection + transform.position;
        Vector3 direction = new Vector3(lookDirection.x, transform.position.y, lookDirection.z);
        transform.LookAt(direction);
        
        //Движение в сторону джойстика
        joysticDirection.y = 0f;
        transform.Translate(speed * Time.deltaTime * joysticDirection, Space.World);
    }
    private void AnimationPlayer() //Упрвление анимацией
    {
        if (GetIsRun())
        {
            anim.SetBool(runStateHash, true);
        }
        else
        {
            anim.SetBool(runStateHash, false);
        }
    }

    private bool GetIsRun() // Метод проверки состояния джойстика
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Enemy FindClosestEnemy() //Метод поиска ближайшего врага
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
                Vector3 dirGoEnemy = goEnemy.transform.position - position;
                float curDistance = dirGoEnemy.sqrMagnitude;
                if (curDistance < distance)
                {
                    closestEnemy = goEnemy;
                    distance = curDistance;
                }
            }
            else
                continue;   
        }
        return closestEnemy;
    }

    private void Shot(Enemy closestEnemy) // метод выстрела
    {
        if(!GetIsRun())
        {
            if(closestEnemy != null)
            {
                Vector3 target = new Vector3(closestEnemy.transform.position.x, transform.position.y, closestEnemy.transform.position.z);
                transform.LookAt(target);

                if (arrow == null)
                {
                    anim.SetTrigger(shotStateHash);
                    arrow = Instantiate(arrowPrefab, positionShot.transform.position, positionShot.transform.rotation);   
                }
            }    
        }
    }

    private void HealthControl() // Метод контроля здоровья
    {
        if (healthPlayer > maxHealthPlayer)
            healthPlayer = maxHealthPlayer;
    }

    public void TakingDamage(float damage) // метод получения урона
    {
        particleTakeDamage.Play();
        healthPlayer -= damage;
        if (healthPlayer <= 0)
        {
            isDeath = true;
        }
    }

    public static float SetExp(float exp) //Метод изменения опыта
    {
        experiencePlayer += exp;
        LevelUp();
        return experiencePlayer;
    }

    private static void LevelUp() // Метод повышения уровня
    {
        if (experiencePlayer >= needExpPlayer)
        {
            levelPlayer++;
            experiencePlayer = 0;
            needExpPlayer += Mathf.Floor(needExpPlayer / 2f);
        }
    }
    public static float SetHealthVampir(float damage) //Метод вампиризма
    {
        return healthPlayer += damage * 0.23f;
    }



    // Класс и методы для сохранения и загрузки данных
    [System.Serializable]
    class SaveData
    {
        public float saveNeedExp;
        public int saveLevel;
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.saveNeedExp = needExpPlayer;
        data.saveLevel = levelPlayer;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            needExpPlayer = data.saveNeedExp;
            levelPlayer = data.saveLevel;  
        }
    }

}
