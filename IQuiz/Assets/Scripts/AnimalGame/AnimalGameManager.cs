using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace IQuiz
{
    public class AnimalGameManager : MonoBehaviour
    {
        //DECLARING VARIABLES 
        [Header("Component reference")]
        public AudioSource timerSounds;
        [Space]

        //NOTE: THE PREFABS ARE THE BUTTONS AND THE SOUNDS 
        //NOTE: INSTANTIATE MEANS SPAWNING GAME OBJECTS INTO GAME AREA
        //NOTE: THE HEADER AND SPACE ATTRIBUTES INSIDE BRACKETS ARE JUST TO ORGANIZE THE VARIABLE IN THE INSPECTOR PANEL
        [Header("Reference to other scripts")]
        public SandClock sandClock;
        [Space]

        [Header("Arrays")]
        public GameObject[] prefabChoices; //ARRAY OF PREFABS TO INSTANTIATE
        private List<GameObject> instantiatedChoices = new List<GameObject>(); // LIST OF INSTANTIATED PREFAB 
        [Space]

        [Header("Parent Transform")]

        public Transform choicesParent; // THE PARENT OF INSTANTIATIGN PREFAB 
        [Space]

        [Header("Question Variables")]
        public AudioSource animalSoundQuestion; //THE SOUND TO BE PLAYED 
        int randomIndexSoundQuestion; //RANDOM SOUNDS IN INSTANTIATED BUTTONS
        [Space]

        [Header("Player Stats")]
        public int playerScore; // SCORE OF THE PLAYER
        [Space]

        [Header("Timer")]
        public float gameTimer = 5f;
        public float timer = 10f;

        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            sandClock.onRoundEnd += OnRoundEnd;

            //CALLING THESE 2 FUNCTIONS WHEN THE GAME STARTS
            InstantiateNewPrefabs();
            PlaySound(); // Play sound when the game starts
        }

        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 5.0f)
            {
                if (!timerSounds.isPlaying)
                {
                    Debug.Log("Playing sound");
                    timerSounds.Play();
                }
            }

            if (timer < 0.2f)
            {
                if (timerSounds.isPlaying)
                {
                    timerSounds.Stop();
                }
            }
        }
        void OnRoundEnd(int round)
        {
            DestroyInstantiatedPrefabs();
            InstantiateNewPrefabs();
            timer = sandClock.durationTime;
            Debug.Log("EndRound");
        }

        public void GameTimer()
        {
            if (gameTimer > 0)
            {
                gameTimer -= 1f * Time.deltaTime;
            }
            else
            {
                Debug.Log("GAME OVER");
            }

        }

        //THIS FUNCTION GETS THE ANIMAL SOUNDS OF THE INSTANTIATED BUTTON
        public void GetAnimalSounds()
        {
            randomIndexSoundQuestion = Random.Range(0, 4);
            animalSoundQuestion = instantiatedChoices[randomIndexSoundQuestion].GetComponent<AudioSource>();
        }

        //CHECKER IF THE CLICKED BUTTON IS THE RIGHT ANSWER
        public void Checker(int index)
        {
            //****** DISABLES THE BUTTON ONCE THE BUTTON IS PRESSED ******
            foreach (var item in instantiatedChoices)
            {
                Button buttonComponent = item.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    Destroy(buttonComponent);
                }


                Image imageComponent = item.GetComponent<Image>();
                if (imageComponent != null)
                {
                    Color currentColor = imageComponent.color;
                    currentColor.a = 0.588f; // Set alpha value (approximately 0.588)
                    imageComponent.color = currentColor;
                }

            }
            //******************************************************

            //CHECKS IF THE BUTTON IS THE RIGHT ANSWER
            if (index == randomIndexSoundQuestion)
            {
                playerScore++;
                gameTimer = 10f;
            }
            else
            {
                Debug.Log("Wrong button pressed.");
                gameTimer = 10f;
            }
        }


        //PLAYS THE SOUND. THIS FUNCTION IS CALLED WHEN YOU PRESS THE MUSIC BUTTON OR YOU ANSWERED PREVIOUS QUESTION
        public void PlaySound()
        {
            animalSoundQuestion.Play();
        }

        //DESTROYS THE SPAWNED ANIMALS
        //THIS FUNCITON IS TO REPLACE THE QUESTIONS WITH THE NEW SET OF ANIMALS
        void DestroyInstantiatedPrefabs()
        {
            foreach (GameObject instantiatedPrefab in instantiatedChoices)
            {
                Destroy(instantiatedPrefab);
            }
            instantiatedChoices.Clear();
        }

        //THIS IS THE FUNCTION TO SPAWN ANIMAL BUTTON
        void InstantiateNewPrefabs()
        {
            List<int> availableIndexes = new List<int>(); // LIST OF AVAILABLE INDEXES
            //THIS LOOP IS TO AVOID SPAWNING THE SAME ANIMAL AT THE SAME QUESTION
            for (int i = 0; i < prefabChoices.Length; i++)
            {
                availableIndexes.Add(i);
            }
            // THIS WHOLE LOGIC IS TO SPAWN 4 ANIMALS ONLY FOR EACH QUESTION
            for (int i = 0; i < 4; i++)
            {
                if (availableIndexes.Count == 0)
                {
                    Debug.LogWarning("Not enough unique prefab choices available.");
                    break;
                }

                int randomIndex = Random.Range(0, availableIndexes.Count);
                int chosenIndex = availableIndexes[randomIndex];
                GameObject instantiatedPrefab = Instantiate(prefabChoices[chosenIndex], choicesParent);
                instantiatedChoices.Add(instantiatedPrefab);

                Button buttonChoices = instantiatedPrefab.GetComponent<Button>();

                // Use the loop index directly and ensure it's in the desired range
                int buttonIndex = i % 4; // This ensures the captured index stays in the 0 to 3 range

                // Adding function to onclick event with captured buttonIndex
                buttonChoices.onClick.AddListener(() => Checker(buttonIndex));

                availableIndexes.RemoveAt(randomIndex);
            }
            GetAnimalSounds();
            PlaySound();
        }
    }
}
