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
        [Space]

        [Header("Sound")]
        public AudioSource timerSound;
        [Space]

        [Header("Script Reference")]
        public PlayerAnswer[] playerAnswer; // An array of PlayerAnswer script references
        public SandClock sandClock;
        [Space]

        [Header("Time")]
        public float timer;
        public float timerLimit;
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

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            // When the game starts, generate the first question and answer choices.
            Addition(); // Generate numbers for the question
            InitializeAnswer(); // Set up the answer choices
            sandClock.onRoundEnd += OnRoundEnd;

            instantiatedAnswerIndicator = 1;

            levelText.text = $"LEVEL {currentLevel.ToString()}";
            questionText.text = $"QUESTION {currentQuestion.ToString()}";

        }

        // Update is called once per frame
        void Update()
        {
            UpdateTime();


        }

        IEnumerator NextQuestion()
        {
            Time.timeScale = 0f;
            GameObject instantiatedPrefab = Instantiate(answerIndicatorPrefab[instantiatedAnswerIndicator], canvasTransform);
            yield return new WaitForSecondsRealtime(3f);
            Destroy(instantiatedPrefab);
            timer = 10f;
            foreach (var item in playerAnswer)
            {
                item.hasAnswered = false;
            }
            instantiatedAnswerIndicator = 1;
            if (currentQuestion < questionEachLevel)
            {
                currentQuestion++;
                Debug.Log("added currentquestion");
            }
            else
            {
                currentQuestion = 1;
                currentLevel++;
            }
            levelText.text = $"LEVEL {currentLevel.ToString()}";
            questionText.text = $"QUESTION {currentQuestion.ToString()}";
            LevelManager();
            Debug.Log($"Question {currentQuestion} at Level {currentLevel}");
            Time.timeScale = 1f;
        }

        #endregion

        #region Custom Methods

        public void LevelManager()
        {
            switch (currentLevel)
            {
                case 1:
                    StartAddition();
                    break;
                case 2:
                    StartSubtraction();
                    break;
                case 3:
                    StartMixed();
                    break;
                case 4:
                    StartMixed();
                    timerLimit = 8f;
                    timer = timerLimit;
                    sandClock.durationTime = timerLimit;
                    break;
                case 5:
                    StartMixed();
                    timerLimit = 6f;
                    timer = timerLimit;
                    sandClock.durationTime = timerLimit;
                    break;
                default:
                    Debug.Log("YOU HAVE REACHED THE MAXIMUM LEVEL ");
                    break;
            }
        }
        public void UpdateTime()
        {
            timer -= Time.deltaTime;

            if (timer <= 5f && !timerSound.isPlaying)
            {
                timerSound.Play();
                //Debug.Log("Playing timer sound");
            }

            if (timer <= 2f && timerSound.isPlaying)
            {
                timerSound.Stop();
                //Debug.Log("Stopping timer sound");
            }
        }

        void OnRoundEnd(int round)
        {
            StartCoroutine(NextQuestion());
        }



        public void SetHasAnswered()
        {
            foreach (var item in playerAnswer)
            {
                item.hasAnswered = true;
            }
        }

        public void InitializeAnswer()
        {
            int correctAnswerIndex = Random.Range(0, answerChoices.Length); // Randomly select the index for the correct answer

            List<int> numbersAvailable = new List<int>(); // Create a list to store available numbers

            // Fill the list with numbers from 1 to 19 (as buttons should have unique numbers from 1 to 19)
            for (int i = 1; i <= 19; i++)
            {
                numbersAvailable.Add(i);
            }

            // Remove the correct answer from the available numbers
            numbersAvailable.Remove(answer);

            // Loop through each button to assign values
            for (int i = 0; i < answerChoices.Length; i++)
            {
                if (i == correctAnswerIndex) // If this is the button for the correct answer:
                {
                    answerChoices[i].text = answer.ToString(); // Assign the correct answer to this button
                    playerAnswer[i].buttonNumber = answer; // Update the button's number with the correct answer
                }
                else // For other buttons (not the one with the correct answer):
                {
                    // Select a random number from the available numbers and assign it to the button
                    int randomIndex = Random.Range(0, numbersAvailable.Count);
                    int randomValue = numbersAvailable[randomIndex];
                    answerChoices[i].text = randomValue.ToString(); // Assign a random number to this button
                    playerAnswer[i].buttonNumber = randomValue; // Update the button's number
                    numbersAvailable.RemoveAt(randomIndex); // Remove the used number from available numbers


                }

            }
        }

        public int Addition()
        {
            operatorText.text = "+";
            // Generate two random numbers for addition.
            num1 = Random.Range(1, 10);
            num2 = Random.Range(1, 10);

            // Update the UI text elements to display the generated numbers.
            num1Text.text = num1.ToString();
            num2Text.text = num2.ToString();

            // Calculate the correct answer for the addition.
            answer = num1 + num2;
            return answer; // Return the calculated answer
        }

        public int Subtraction()
        {
            operatorText.text = "-";
            // Generate two random numbers for subtraction.
            num1 = Random.Range(1, 10);
            num2 = Random.Range(1, 10);

            // Ensure the first number is greater than the second for subtraction.
            while (num1 <= num2)
            {
                num1 = Random.Range(1, 10);
                num2 = Random.Range(1, 10);
            }

            num1Text.text = num1.ToString();
            num2Text.text = num2.ToString();
            // Calculate the correct answer for the subtraction.
            answer = num1 - num2;
            Debug.Log("subtraction");
            return answer; // Return the calculated answer

        }

        public void StartAddition()
        {


            for (int i = 1; i < questionEachLevel; i++)
            {
                Addition();
                InitializeAnswer();
            }

        }

        public void StartSubtraction()
        {

            for (int i = 1; i < questionEachLevel; i++)
            {
                Subtraction();
                InitializeAnswer();
            }
        }

        public void StartMixed()
        {

            for (int i = 1; i < questionEachLevel; i++)
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
