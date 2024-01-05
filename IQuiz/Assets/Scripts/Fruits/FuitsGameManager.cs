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
        [Space]

        [Header("Other Script Reference")]
        public SandClock sandClock;
        [Space]

        [Header("Audio")]
        public AudioSource timerSounds;
        [Space]

        [Header("Time")]
        public float gameTimer;

        [Header("Player Stats")]
        public int playerScore;

        // Private Variables
        GameObject fruitsGameObj; // Reference to the spawned fruit object
        string fruitName; // Name of the fruit
        StringShuffler shuffler = new StringShuffler(); // Instance of the StringShuffler class to shuffle strings
        List<GameObject> buttonList = new List<GameObject>(); // List to store created buttons

        [SerializeField] private Button lastButtonClicked; // Reference to the last clicked button

        char clickedLetter;

        string userInput = ""; // User's input for spelling the fruit name

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            sandClock.onRoundEnd += OnRoundEnd;
            //sandClock.onRoundStart += OnRoundStart;
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
            // Randomly select a fruit prefab and spawn it
            int randomIndex = UnityEngine.Random.Range(0, fruitsPrefab.Length);
            SpawnFruits(randomIndex);

            // Shuffle the fruit name to create a jumbled version for buttons
            string shuffledName = shuffler.ShuffleString(fruitName);
            CreateButtons(shuffledName);
        }
        #endregion
        
        #region IENUMERATOR

        IEnumerator EndQuestion()
        {
            Debug.Log("Round End");
            yield return new WaitForSeconds(.1f);
            playerScoreText.text = $"Score: {playerScore}";
            SpawnQuestion();
            userInput = "";
            nameText.text = "";
            gameTimer = 15f;
            

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
            }

        }

        void OnRoundEnd(int round)
        {
            StartCoroutine(EndQuestion());
        }

        void OnRoundStart(int round)
        {
            SpawnQuestion();
            gameTimer = 15f;
        }
        #endregion


    }
}
