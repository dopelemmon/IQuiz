using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IQuiz
{
    public class FruitsGameManager : MonoBehaviour
    {
        // Public Variables
        public GameObject[] fruitsPrefab; // Array of fruit prefabs
        public GameObject buttonPrefab; // Reference to the button prefab
        public Transform fruitParent; // Parent transform for fruit instantiation
        public Transform buttonsParent; // Parent transform for buttons

        // Private Variables
        GameObject fruitsGameObj; // Reference to the spawned fruit object
        string fruitName; // Name of the fruit
        StringShuffler shuffler = new StringShuffler(); // Instance of the StringShuffler class to shuffle strings
        List<GameObject> buttonList = new List<GameObject>(); // List to store created buttons

        string userInput = ""; // User's input for spelling the fruit name

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            // For example purposes, assume fruitName is obtained from the instantiated fruit object
            fruitName = "Apple"; // Replace this with the actual fruit name from your game

            // Shuffle the fruit name to create a jumbled version for buttons
            string shuffledName = shuffler.ShuffleString(fruitName);
        }

        // Update is called once per frame
        void Update()
        {
            // Check for user input to spawn fruits and create buttons
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnQuestion();
            }
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
                button.GetComponentInChildren<TMP_Text>().text = letter.ToString();
                button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(letter));
                buttonList.Add(button);
            }
        }

        // Handle button click event
        void OnButtonClick(char clickedLetter)
        {
            userInput += clickedLetter; // Add clicked letter to the user's input
            Debug.Log("Current User Input: " + userInput);

            // Check if the user input matches the fruit name
            if (userInput.Equals(fruitName, StringComparison.OrdinalIgnoreCase)) // Case insensitive comparison
            {
                Debug.Log("Congratulations! You've spelled the fruit name.");
                // Perform actions for correct spelling
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
    }
}
