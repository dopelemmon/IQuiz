using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;

namespace IQuiz
{
    public class FruitsGameManager : MonoBehaviour
    {
        // Public Variables
        [Header("UI Elements")]
        public GameObject[] fruitsPrefab; // Array of fruit prefabs
        public GameObject buttonPrefab; // Reference to the button prefab
        public Transform fruitParent; // Parent transform for fruit instantiation
        public Transform buttonsParent; // Parent transform for buttons
        public TMP_Text nameText;
        public TMP_Text playerScoreText;
        public TMP_Text levelText;
        public Button undoButton;
        [Space]

        [Header("Other Script Reference")]
        public TimerSand sandClock;
        [Space]

        [Header("Audio")]
        public AudioSource timerSounds;
        [Space]

        [Header("Time")]
        public float gameTimer;
         float gametimerLimit = 15;

        [Header("Player Stats")]
        public int playerScore;

        // Private Variables
        GameObject fruitsGameObj; // Reference to the spawned fruit object
        string fruitName; // Name of the fruit
        StringShuffler shuffler = new StringShuffler(); // Instance of the StringShuffler class to shuffle strings
        List<GameObject> buttonList = new List<GameObject>(); // List to store created buttons

        [SerializeField] private Button lastButtonClicked; // Reference to the last clicked button

        char clickedLetter;

        bool isRoundEnd;
        [SerializeField]
        int prevRandomIndex = 0;
        string userInput = ""; // User's input for spelling the fruit name

        [Space]
        [Header("Levels")]
        public int currentLevel;
        public int currentQuestion;
        public int maxLevel;
        public int maxQuestion;

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {

            // For example purposes, assume fruitName is obtained from the instantiated fruit object
            fruitName = "Apple"; // Replace this with the actual fruit name from your game

            // Shuffle the fruit name to create a jumbled version for buttons
            string shuffledName = shuffler.ShuffleString(fruitName);

            SpawnQuestion();
            playerScore = 0;
            playerScoreText.text = $"Score: {playerScore}";
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTime();
            levelText.text = $"LEVEL: {currentLevel}";
        }

        #endregion

        #region Button and Fruits Methods

        // Create buttons with shuffled letters
        void CreateButtons(string shuffledName)
        {
            foreach (char letter in shuffledName)
            {
                // Instantiate buttons and set their properties
                GameObject button = Instantiate(buttonPrefab, buttonsParent);
                buttonList.Add(button);
                button.GetComponentInChildren<TMP_Text>().text = letter.ToString();
                button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(letter, lastButtonClicked));

            }
        }

        // Handle button click event
        void OnButtonClick(char _clickedLetter, Button _clickedButton)
        {
            clickedLetter = _clickedLetter;
            userInput += _clickedLetter; // Add clicked letter to the user's input
            Debug.Log("Current User Input: " + userInput);
            nameText.text = userInput;

            // Check if the user input matches the fruit name
            if (userInput.Equals(fruitName, StringComparison.OrdinalIgnoreCase)) // Case insensitive comparison
            {
                Debug.Log("Congratulations! You've spelled the fruit name.");
                // Perform actions for correct spelling
                playerScore++;

                //trial of sandclock
                StartCoroutine(EndQuestion());

            }

        }

        // Method to handle the "Undo" button click
        public void UndoButtonClick()
        {
            if (userInput.Length > 0)
            {
                userInput = "";
                nameText.text = userInput;

                foreach (var buttonGO in buttonList)
                {
                    Button button = buttonGO.GetComponent<Button>();
                    button.interactable = true;
                }

            }
        }

        // Spawn fruits and set up the game
        public void SpawnFruits(int fruitsIndex)
        {
            // Instantiate the selected fruit prefab
            fruitsGameObj = Instantiate(fruitsPrefab[fruitsIndex], fruitParent);
            fruitName = fruitsGameObj.name; // Get the name of the spawned fruit
            Debug.Log($"Fruit name: {fruitName}");

            // Remove "(Clone)" suffix from the fruit name if present
            fruitName = fruitName.Replace("(Clone)", "");

            // Shuffle the characters of the fruitName string using StringShuffler
            string shuffledName = shuffler.ShuffleString(fruitName);
            Debug.Log($"Shuffled Fruit name: {shuffledName}");
        }

        #endregion

        #region Questions Method
        public void SpawnQuestion()
        {
            if (fruitsGameObj != null)
            {
                // If fruit is spawned, destroy it and all created buttons
                Destroy(fruitsGameObj);

                for (int i = 0; i < buttonList.Count; i++)
                {
                    Destroy(buttonList[i]);
                }
                buttonList.Clear();
                userInput = "";
            }

            int instantiatedLimit = 0;
            int instantiatedLimitStart = 0;
            switch (currentLevel)
            {
                case 1:
                    instantiatedLimit = 5;
                    instantiatedLimitStart = 0;
                    Debug.Log($"Level: {currentLevel}");
                    break;
                case 2:
                    instantiatedLimit = 5;
                    instantiatedLimitStart = 0;
                    Debug.Log($"Level: {currentLevel}");
                    gametimerLimit = 10;
                    sandClock.roundDuration = 10;
                    break;
                case 3:
                    instantiatedLimit = 10;
                    instantiatedLimitStart = 0;
                    Debug.Log($"Level: {currentLevel}");
                    break;
                case 4:
                    instantiatedLimit = fruitsPrefab.Length;
                    instantiatedLimitStart = 12;
                    Debug.Log($"Level: {currentLevel}");
                    gametimerLimit = 8;
                    sandClock.roundDuration = 8;
                    break;
                case 5:
                    instantiatedLimit = fruitsPrefab.Length;
                    instantiatedLimitStart = 12;
                    undoButton.interactable = false;
                    Debug.Log($"Level: {currentLevel}");
                    break;
                default:
                    Debug.Log("Finished");
                    break;
            }
            // Randomly select a fruit prefab and spawn it
            int randomIndex = UnityEngine.Random.Range(instantiatedLimitStart, instantiatedLimit);
            
            
            while(randomIndex == prevRandomIndex)
            {
                randomIndex = UnityEngine.Random.Range(instantiatedLimitStart, instantiatedLimit);
                
            }
            SpawnFruits(randomIndex);
            Debug.Log($"Random Index: {randomIndex}");
            Debug.Log($"Prev Random Index: {prevRandomIndex}");
            prevRandomIndex = randomIndex;
            // Shuffle the fruit name to create a jumbled version for buttons
            string shuffledName = shuffler.ShuffleString(fruitName);
            CreateButtons(shuffledName);
        }
        #endregion

        #region IENUMERATOR

        IEnumerator EndQuestion()
        {
            if (timerSounds.isPlaying)
            {
                timerSounds.Stop();
                gameTimer = gametimerLimit;
            }
            isRoundEnd = false;
            Debug.Log("Round End");
            yield return new WaitForSeconds(.1f);
            playerScoreText.text = $"Score: {playerScore}";
            SpawnQuestion();
            userInput = "";
            nameText.text = "";
            gameTimer = gametimerLimit;
            sandClock.roundDuration = gametimerLimit;

            sandClock.ResetTime();
            if (currentLevel <= maxLevel)
            {
                if (currentQuestion < maxQuestion)
                {
                    currentQuestion++;
                }
                else
                {
                    currentQuestion = 1;
                    currentLevel++;
                }
            }


        }
        #endregion
        #region Timer Method
        public void UpdateTime()
        {
            if (gameTimer > 0)
            {
                gameTimer -= Time.deltaTime;

                if (gameTimer < 5f && !timerSounds.isPlaying)
                {
                    timerSounds.Play();
                }
                if (gameTimer < 2f && timerSounds.isPlaying)
                {
                    timerSounds.Stop();
                }
                isRoundEnd = true;
            }
            if (gameTimer <= 0 && isRoundEnd)
            {
                StartCoroutine(EndQuestion());
            }

        }
        #endregion


    }
}
