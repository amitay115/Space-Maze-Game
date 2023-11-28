using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameController : MonoBehaviour
{
    public static GameController manager;
    public static float GameSpeed;
    public int mysteryboxcount = 0;
    public int multiple = 2;
    public int Coin;
    public float ScoreByTime;
    //public Text CoinText;
    public TextMeshProUGUI scoreText;
    public bool InGame, GameEnd;
    public KeyCode ReviveKey = KeyCode.E;
    Character _char;
    public GameObject[] items;
    float starttime;
    public int lastcoin;

    public int diamond = 0;
    public TextMeshProUGUI DiamondText;
    public GameObject gameOverPanel;
    public GameObject TabPanel;

    void Awake()
    {
        manager = this;
    }
    private void OnEnable()
    {
        manager = this;
    }
    private void Start()
    {
        // SoundManager.main.Music(true);
        diamond = 0;
        _char = Character.main;
        GameSpeed = _char.FwdSpeed;
        mysteryboxcount = 0;
        starttime = Time.time;
    }
    void Update()
    {
        // CoinText.text = Coin.ToString();
        scoreText.text = ((int)ScoreByTime).ToString("000000");

        if (Input.GetKey(ReviveKey))
        {
           // SoundManager.main.PauseSimpleClip();
            StartGame();
        }
        if (InGame && !GameEnd)
        {
            if (Time.timeScale != 0)
                ScoreByTime += 1 * GameSpeed * Time.deltaTime * multiple;
            if ((Time.time - starttime) > 30f && GameSpeed < 25)
            {
                GameSpeed += 0.1f * Time.deltaTime;
            }
        }
    }
    public void StartGame()
    {
       
        GameEnd = false;
        SoundManager.main.Music(true);
        gameOverPanel.SetActive(false);
        TabPanel.SetActive(false);
        Character.main.robot.PlayStartShout();
        _char.StartGame();
        InGame = true;
    }
    public void EndGame()
    {
        GameEnd = true;
        InGame = false;
        gameOverPanel.SetActive(true);
        TabPanel.SetActive(true);
        SoundManager.main.Music(false);
    }
    public void IncrementScore()
    {
        diamond++;
       // SoundManager.main.PlaySimpleClip(SoundManager.main.CoinEat);
        DiamondText.text = diamond.ToString();
    }
    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
