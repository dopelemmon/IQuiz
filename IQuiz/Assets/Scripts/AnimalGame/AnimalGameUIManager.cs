using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace IQuiz
{
    public class AnimalGameUIManager : MonoBehaviour
    {
        public AnimalGameManager animalGameManager; //creating reference to animalgamemanager script
        public TMP_Text scoreText; // reference to text mesh pro to display to screen
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            scoreText.text = $"SCORE: {animalGameManager.playerScore.ToString()}"; //changing the value of display to screen to the value of the player score
        }
    }
}
