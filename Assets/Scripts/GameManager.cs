﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager gm;

    [Tooltip("If not set, the player will default to the gameObject tagged as Player.")]
    public GameObject player;

    public enum gameStates { Playing, Death, GameOver, BeatLevel };
    public gameStates gameState = gameStates.Playing;

    public int score = 0;
    public string level;
    public Text levelDisplay;

    [Tooltip("HP derived from the Player Health component")]
    public float hp = 0.0f, maxHp = 0.0f;
    public Text hpDisplay;
    public bool canBeatLevel = false;
    public int beatLevelScore = 0;

    public GameObject mainCanvas;
    public Text mainScoreDisplay;
    public GameObject gameOverCanvas;
    public Text gameOverScoreDisplay;

    [Tooltip("Only need to set if canBeatLevel is set to true.")]
    public GameObject beatLevelCanvas;

    public AudioSource backgroundMusic;
    public AudioClip gameOverSFX;

    [Tooltip("Only need to set if canBeatLevel is set to true.")]
    public AudioClip beatLevelSFX;

    private Health playerHealth;

    void Start()
    {
        if (gm == null)
            gm = gameObject.GetComponent<GameManager>();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        playerHealth = player.GetComponent<Health>();

        // setup score display
        Collect(0);

        // make other UI inactive
        gameOverCanvas.SetActive(false);
        if (canBeatLevel)
            beatLevelCanvas.SetActive(false);


        if (levelDisplay != null)
        {
            level = SceneManager.GetActiveScene().name;
            levelDisplay.text = level;
        }
        hp = maxHp = playerHealth.healthPoints;
        ReduceHP(0);
    }

    void Update()
    {
        switch (gameState)
        {
            case gameStates.Playing:
                if (playerHealth.isAlive == false)
                {
                    // update gameState
                    gameState = gameStates.Death;

                    // set the end game score
                    gameOverScoreDisplay.text = mainScoreDisplay.text;

                    // switch which GUI is showing		
                    mainCanvas.SetActive(false);
                    gameOverCanvas.SetActive(true);
                }
                else if (canBeatLevel && score >= beatLevelScore)
                {
                    // update gameState
                    gameState = gameStates.BeatLevel;

                    // hide the player so game doesn't continue playing
                    player.SetActive(false);

                    // switch which GUI is showing			
                    mainCanvas.SetActive(false);
                    beatLevelCanvas.SetActive(true);
                }
                break;
            case gameStates.Death:
                backgroundMusic.volume -= 0.01f;
                if (backgroundMusic.volume <= 0.0f)
                {
                    AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);

                    gameState = gameStates.GameOver;
                }
                break;
            case gameStates.BeatLevel:
                backgroundMusic.volume -= 0.01f;
                if (backgroundMusic.volume <= 0.0f)
                {
                    AudioSource.PlayClipAtPoint(beatLevelSFX, Camera.main.transform.position);

                    gameState = gameStates.GameOver;
                }
                break;
            case gameStates.GameOver:
                // nothing
                break;
        }

    }


    public void Collect(int amount)
    {
        score += amount;
        if (canBeatLevel)
        {
            mainScoreDisplay.text = score.ToString() + " of " + beatLevelScore.ToString();
        }
        else
        {
            mainScoreDisplay.text = score.ToString();
        }
    }

    public void ReduceHP(float amount)
    {
        hp -= amount;
        if (hpDisplay)
        {
            hpDisplay.text = string.Format("HP: {0} / {1}", hp, maxHp);
        }
    }
}
