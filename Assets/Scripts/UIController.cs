using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    // Шкала опыта
    [SerializeField] private Image experienceBar;
    [SerializeField] private Text experienceText;
    private float currentExp;
    private float maxExp;

    // Шкала здоровья
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    private float currentHealth;
    private float maxHealth;

    [SerializeField] private Text playerLevel; // Уровень игрока

    [SerializeField] private Text waveNumber; // Номер волны

    [SerializeField] private GameObject gameOver; // Окно проигрыша

    [SerializeField] private GameObject menu; // Окно проигрыша

    [SerializeField] private GameObject spawnManager; // Окно проигрыша


    private void Start()
    {
        spawnManager.SetActive(false);
        menu.SetActive(true);
        gameOver.SetActive(false);
        Time.timeScale = 0;
    }
    private void Update()
    {
        currentExp = PlayerController.experiencePlayer;
        maxExp = PlayerController.needExpPlayer;
        experienceBar.fillAmount = currentExp / maxExp;
        experienceText.text = currentExp + " / " + maxExp;

        currentHealth = PlayerController.healthPlayer;
        maxHealth = PlayerController.maxHealthPlayer;
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = currentHealth + " / " + maxHealth;

        playerLevel.text = "Level: " + PlayerController.levelPlayer;

        waveNumber.text = "Wave: " + SpawnManager.waveNumber;

        GameOver();
    }

    private void GameOver()
    {
        if (PlayerController.isDeath)
        {
            spawnManager.SetActive(false);
            gameOver.SetActive(true);
            Time.timeScale = 0;
        }

        else
            return;
    }

    public void StartGame()
    {
        menu.SetActive(false);
        gameOver.SetActive(false);
        Time.timeScale = 1;
        spawnManager.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        gameOver.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}
