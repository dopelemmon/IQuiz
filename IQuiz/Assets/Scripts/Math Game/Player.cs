using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IQuiz
{

    public class Player : MonoBehaviour
    {
        public int playerScore;
        // Start is called before the first frame update
        void Start()
        {
            playerScore = 0;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void UpdatePlayerScore()
        {
            playerScore++;
        }
    }
}
