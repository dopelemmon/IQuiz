using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace IQuiz
{
    public class MathGameManager : MonoBehaviour
    {
        // This region includes declarations of variables and fields used in the class.
        #region variables
        [Header("UI Elements")]
        // These public variables hold references to the UI elements in the Unity Inspector.
        public TMP_Text num1Text; // Text for displaying number 1
        public TMP_Text num2Text; // Text for displaying number 2
        public TMP_Text operatorText;
        public TMP_Text[] answerChoices; // Array of TextMeshPro Text elements for answer choices
        public Button[] answerButton;
        public GameObject[] answerIndicatorPrefab;
        public Transform canvasTransform;
        public TMP_Text levelText;
        public TMP_Text questionText;
        public TMP_Text playerScoreText;
        public Sprite buttonSelectedSprite;
        public TMP_Text congratsText;
        public GameObject gameFinishedPanel;
        [Space]

        [Header("Sound")]
        public AudioSource timerSound;
        [Space]

        [Header("Script Reference")]
        public PlayerAnswer[] playerAnswer; // An array of PlayerAnswer script references
        public TimerSand sandClock;
        public Player playerStats;
        [Space]

        [Header("Time")]
        public float timer;
        public float timerLimit;

        bool gameIsDone;

        public bool isAnswered;
        [Space]

        [Header("Question Variable")]
        public int num1;
        public int num2;
        public int answer; // Variable to store the correct answer
        [Space]
        public int instantiatedAnswerIndicator;

        [Header("Level")]
        public int currentLevel;
        public int currentQuestion;
        public int questionEachLevel;


        #endregion

        // This region contains Unity's lifecycle methods (Start, Update, etc.).
        #region Unity Methods
        void Start()
        {
            // Initialization when the game starts.
            Addition(); // Generate numbers for the question
            InitializeAnswer(); // Set up the answer choices

            instantiatedAnswerIndicator = 1;

            levelText.text = $"LEVEL {currentLevel.ToString()}";
            questionText.text = $"QUESTION {currentQuestion.ToString()}";
            playerScoreText.text = $"SCORE: {playerStats.playerScore.ToString()}";
        }

        void Update()
        {
            UpdateTime(); // Update timer display and functionality
        }

        public void Answered()
        {
            StartCoroutine(InstantiateAnswer());

        }

        IEnumerator InstantiateAnswer()
        {
            GameObject instantiatedPrefab = Instantiate(answerIndicatorPrefab[instantiatedAnswerIndicator], canvasTransform);
            yield return new WaitForSecondsRealtime(2f);
            Destroy(instantiatedPrefab);
            isAnswered = false;
            StartCoroutine(NextQuestion());
        }

        // Coroutine for handling next question logic.
        IEnumerator NextQuestion()
        {
            sandClock.ResetTime();
            Debug.Log("next question initiated");
            timer = timerLimit;
            Debug.Log("timerlimit reset");
            if (timerSound.isPlaying)
            {
                timerSound.Stop();
            }
            // Check for the correct answer and update player score and timer.
            if (instantiatedAnswerIndicator == 0)
            {
                playerStats.UpdatePlayerScore();
                playerScoreText.text = $"SCORE: {playerStats.playerScore.ToString()}";
            }
            foreach (var button in answerButton)
            {
                button.image.sprite = buttonSelectedSprite;
                button.interactable = true;
            }
            instantiatedAnswerIndicator = 1;
            if (currentQuestion < questionEachLevel)
            {
                currentQuestion++;
            }
            else
            {
                currentQuestion = 1;
                currentLevel++;
            }
            levelText.text = $"LEVEL {currentLevel.ToString()}";
            questionText.text = $"QUESTION {currentQuestion.ToString()}";
            LevelManager();
            yield break;
        }

        #endregion

        // This region contains custom methods used in the game logic.
        #region Custom Methods

        // Method to manage game levels based on the current level.
        public void LevelManager()
        {
            switch (currentLevel)
            {
                // Different cases represent different levels and their corresponding operations.
                case 1:
                    StartAddition(); // Start addition-based questions
                    timerLimit = 10f;
                    break;
                case 2:
                    StartSubtraction(); // Start subtraction-based questions
                    timerLimit = 10f;
                    break;
                case 3:
                    StartMixed(); // Start mixed addition and subtraction questions
                    timerLimit = 10f;
                    break;
                case 4:
                    StartMixed(); // Mixed questions with adjusted timer and level settings
                    timerLimit = 8f;
                    timer = timerLimit;
                    sandClock.roundDuration = timerLimit;
                    break;
                case 5:
                    StartMixed(); // More challenging mixed questions with further adjusted settings
                    timerLimit = 6f;
                    timer = timerLimit;
                    sandClock.roundDuration = timerLimit;
                    break;
                default:
                    LevelFinished();

                    break;
            }
        }

        void LevelFinished()
        {
            gameFinishedPanel.SetActive(true);
            Time.timeScale = 0f;
            congratsText.text = $"CONGRATULATIONS! YOU GOT A SCORE OF {playerStats.playerScore}";
            gameIsDone = true;
        }

        public void TryAgain()
        {
            
            Time.timeScale = 1f;
            gameIsDone = false;
            currentLevel = 1;
            currentQuestion = 1;
            gameFinishedPanel.SetActive(false);
            levelText.text = $"LEVEL {currentLevel}";
            Addition();
            InitializeAnswer();
        }

        // Method to update timer functionality.
        public void UpdateTime()
        {

            if (timer >= 0 && !gameIsDone)
            {
                timer -= Time.deltaTime;
                // Play timer sound when the timer reaches a certain point.
                if (timer <= 5f && !timerSound.isPlaying)
                {
                    timerSound.Play();
                }

                // Stop timer sound when the timer is almost finished.
                if (timer <= 2f && timerSound.isPlaying)
                {
                    timerSound.Stop();
                }
            }
            if(timer <= 0 && !isAnswered)
            {
                timer = timerLimit;
                StartCoroutine(InstantiateAnswer());

            }


        }

        // Method to initialize answer choices for questions.
        public void InitializeAnswer()
        {
            // Randomly select the index for the correct answer.
            int correctAnswerIndex = Random.Range(0, answerChoices.Length);

            List<int> numbersAvailable = new List<int>(); // List to store available numbers

            // Fill the list with numbers from 1 to 19 (as buttons should have unique numbers from 1 to 19)
            for (int i = 1; i <= 19; i++)
            {
                numbersAvailable.Add(i);
            }

            // Remove the correct answer from the available numbers.
            numbersAvailable.Remove(answer);

            // Assign values to the answer buttons.
            for (int i = 0; i < answerChoices.Length; i++)
            {
                if (i == correctAnswerIndex)
                {
                    // Assign the correct answer to a randomly chosen button.
                    answerChoices[i].text = answer.ToString();
                    playerAnswer[i].buttonNumber = answer;
                }
                else
                {
                    // Assign random numbers to other buttons.
                    int randomIndex = Random.Range(0, numbersAvailable.Count);
                    int randomValue = numbersAvailable[randomIndex];
                    answerChoices[i].text = randomValue.ToString();
                    playerAnswer[i].buttonNumber = randomValue;
                    numbersAvailable.RemoveAt(randomIndex);
                }
            }
        }

        // Method to generate an addition question.
        public int Addition()
        {
            operatorText.text = "+";
            num1 = Random.Range(1, 10);
            num2 = Random.Range(1, 10);

            num1Text.text = num1.ToString();
            num2Text.text = num2.ToString();

            answer = num1 + num2;
            return answer;
        }

        // Method to generate a subtraction question.
        public int Subtraction()
        {
            operatorText.text = "-";
            num1 = Random.Range(1, 20);
            num2 = Random.Range(1, 20);

            // Ensure the first number is greater than the second for subtraction.
            while (num1 <= num2)
            {
                num1 = Random.Range(1, 20);
                num2 = Random.Range(1, 20);
            }

            num1Text.text = num1.ToString();
            num2Text.text = num2.ToString();

            answer = num1 - num2;
            return answer;
        }

        // Method to start addition-based questions.
        public void StartAddition()
        {
            // Generate addition questions for the current level.
            for (int i = 1; i <= questionEachLevel; i++)
            {
                Addition();
                InitializeAnswer();
            }
        }

        // Method to start subtraction-based questions.
        public void StartSubtraction()
        {
            // Generate subtraction questions for the current level.
            for (int i = 1; i <= questionEachLevel; i++)
            {
                Subtraction();
                InitializeAnswer();
            }
        }

        // Method to start mixed addition and subtraction questions.
        public void StartMixed()
        {
            // Generate mixed questions (addition or subtraction) for the current level.
            for (int i = 1; i <= questionEachLevel; i++)
            {
                if (Random.value < 0.5f)
                {
                    Addition();
                }
                else
                {
                    Subtraction();
                }
                InitializeAnswer();
            }
        }
        #endregion
    }
}

