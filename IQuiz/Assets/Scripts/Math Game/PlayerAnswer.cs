using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IQuiz
{
    public class PlayerAnswer : MonoBehaviour
    {
        public MathGameManager mathGameManager;
        public int buttonNumber;
        public int minButtonNum;
        public int maxButtonNum;

        public bool hasAnswered;
        // Start is called before the first frame update
        void Start()
        {
            
            minButtonNum = 0;
            maxButtonNum = 20;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void CheckAnswer()
        {
            if(this.buttonNumber == mathGameManager.answer && !hasAnswered)
            {
                mathGameManager.instantiatedAnswerIndicator = 0;

            }
            if(this.buttonNumber != mathGameManager.answer && !hasAnswered)
            {
                mathGameManager.instantiatedAnswerIndicator = 1;

            }
        }
    }
}
