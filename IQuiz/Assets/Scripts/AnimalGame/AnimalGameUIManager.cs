using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace IQuiz
{
    public class AnimalGameUIManager : MonoBehaviour
    {
        [Header("Reference to other Scripts")]
        public AnimalGameManager animalGameManager; //creating reference to animalgamemanager script

        [Space]
        [Header("Player Attributes")]
        public TMP_Text scoreText; // reference to text mesh pro to display to screen

        [Space]
        [Header("Pause Panel")]
        public GameObject pausePanel;
        public bool isPaused;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateScore();
            
        }

        public void UpdateScore()
        {
            scoreText.text = $"SCORE: {animalGameManager.playerScore.ToString()}"; //changing the value of display to screen to the value of the player score
        }

        public void PauseGame(bool _isPaused)
        {
            isPaused = _isPaused;
            Time.timeScale = isPaused ? 0f : 1f; pausePanel.SetActive(isPaused);
        }
    }
}
