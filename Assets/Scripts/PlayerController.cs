using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ParticleSystem particleTakeDamage; // FX ��������� �����

    [SerializeField] private Joystick joystick; // ��������
    private readonly float speed = 6f; // �������� ��������

    private Animator anim; // ��������
    static readonly int runStateHash = Animator.StringToHash("isRun");  // ��� ��������� ����
    static readonly int shotStateHash = Animator.StringToHash("shot");  // ��� ��������

    [SerializeField] private ArrowLighting arrowPrefab; // ������ ������
    [SerializeField] private Transform positionShot; // ���������� ������ ������
    private ArrowLighting arrow; // ���� ��� �������� �������� ������

    public static float healthPlayer { get; private set;} //�������� ������
    public static float maxHealthPlayer { get;} = 600f; //������������ ��������

    public static float experiencePlayer { get; private set;} // ������� ���� ������
    public static float needExpPlayer { get; private set;} = 100f; // ����������� ����

    public static int levelPlayer { get; private set;} = 1; // ������� ������

    public static bool isDeath { get; private set;} // ��������� ������
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
            MovePlayer();                   //����� �������� � ��������
            AnimationPlayer();              //��������� ���������
            Shot(FindClosestEnemy());       //������� � ���������� ����������
            HealthControl();                // �������� ��������
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

    private void MovePlayer() //����� �������� � ��������
    {
        //������� � ������� ���������
        Vector3 joysticDirection = joystick.Direction;
        joysticDirection.z = joysticDirection.y;
        Vector3 lookDirection = joysticDirection + transform.position;
        Vector3 direction = new Vector3(lookDirection.x, transform.position.y, lookDirection.z);
        transform.LookAt(direction);
        
        //�������� � ������� ���������
        joysticDirection.y = 0f;
        transform.Translate(speed * Time.deltaTime * joysticDirection, Space.World);
    }
    private void AnimationPlayer() //��������� ���������
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

    private bool GetIsRun() // ����� �������� ��������� ���������
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

    private Enemy FindClosestEnemy() //����� ������ ���������� �����
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

    private void Shot(Enemy closestEnemy) // ����� ��������
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

    private void HealthControl() // ����� �������� ��������
    {
        if (healthPlayer > maxHealthPlayer)
            healthPlayer = maxHealthPlayer;
    }

    public void TakingDamage(float damage) // ����� ��������� �����
    {
        particleTakeDamage.Play();
        healthPlayer -= damage;
        if (healthPlayer <= 0)
        {
            isDeath = true;
        }
    }

    public static float SetExp(float exp) //����� ��������� �����
    {
        experiencePlayer += exp;
        LevelUp();
        return experiencePlayer;
    }

    private static void LevelUp() // ����� ��������� ������
    {
        if (experiencePlayer >= needExpPlayer)
        {
            levelPlayer++;
            experiencePlayer = 0;
            needExpPlayer += Mathf.Floor(needExpPlayer / 2f);
        }
    }
    public static float SetHealthVampir(float damage) //����� ����������
    {
        return healthPlayer += damage * 0.23f;
    }



    // ����� � ������ ��� ���������� � �������� ������
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
