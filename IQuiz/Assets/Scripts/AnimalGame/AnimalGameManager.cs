using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        public AudioSource[] answerSounds; // the audiosource sounds variable 
        public TMP_Text levelText; // LEVEL TEXT ELEMENT
        public Button soundButton;
        [SerializeField] private GameObject gameFinishedPanel;
        [SerializeField] private TMP_Text gameFinishedText;
        [Space]

        //NOTE: THE PREFABS ARE THE BUTTONS AND THE SOUNDS 
        //NOTE: INSTANTIATE MEANS SPAWNING GAME OBJECTS INTO GAME AREA
        //NOTE: THE HEADER AND SPACE ATTRIBUTES INSIDE BRACKETS ARE JUST TO ORGANIZE THE VARIABLE IN THE INSPECTOR PANEL
        [Header("Reference to other scripts")]
        public TimerSand sandClock;
        [Space]

        [Header("Arrays")]
        public GameObject[] prefabChoices; //ARRAY OF PREFABS TO INSTANTIATE
        public GameObject[] answerIndicatorPrefab;
        private List<GameObject> instantiatedChoices = new List<GameObject>(); // LIST OF INSTANTIATED PREFAB 
        private List<GameObject> instantiatedIndicators = new List<GameObject>();
        [Space]

        [Header("Parent Transform")]

        public Transform choicesParent; // THE PARENT OF INSTANTIATIGN PREFAB 
        public Transform canvasTransform;
        [Space]

        [Header("Question Variables")]
        public AudioSource animalSoundQuestion; //THE SOUND TO BE PLAYED 
        int randomIndexSoundQuestion; //RANDOM SOUNDS IN INSTANTIATED BUTTONS
        [Space]

        [Header("Player Stats")]
        public int playerScore; // SCORE OF THE PLAYER
        public int answerInt;
        bool isCorrect;
        [Space]

        [Header("Timer")]
        public float gameTimer = 5f;
        public float timer = 10f;
        public float timerLimit;
        public bool gameIsDone;

        [Header("Levels")]
        public int currentLevel;
        public int maxLevel;
        public int currentQuestion;
        public int maxQuestion;

        public bool isAnswered;


        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            //CALLING THESE 2 FUNCTIONS WHEN THE GAME STARTS
            InstantiateNewPrefabs();
            PlaySound(); // Play sound when the game starts
            currentLevel = 1;
            currentQuestion = 1;

        }

        // Update is called once per frame
        void Update()
        {
            //TIMER CODE BELOW
            if (timer >= 0)
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
                if(timer > 5f && timerSounds.isPlaying)
                {
                    timerSounds.Stop();
                }

                if (timer < 0.2f)
                {
                    if (timerSounds.isPlaying)
                    {
                        timerSounds.Stop();
                    }
                }
            }

            if(timer <= 0 && !isAnswered)
            {
                StartCoroutine(AnswerIndcatorAnimation());
                isAnswered = true;
            }

            //LEVEL MANAGER
            switch (currentLevel)
            {
                
                case 1://IF LEVEL 1 DO THIS: *SAME IN ALL LEVELS IN CASE VALUE*
                    timerLimit = 10;
                    sandClock.roundDuration = timerLimit;
                    break;
                case 2:
                    timerLimit = 8;
                    sandClock.roundDuration = timerLimit;
                    break;
                case 3:
                    timerLimit = 6;
                    sandClock.roundDuration = timerLimit;
                    break;
                case 4:
                    timerLimit = 6;
                    sandClock.roundDuration = timerLimit;
                    soundButton.interactable = false;
                    break;
                case 5:
                    timerLimit = 4;
                    sandClock.roundDuration = timerLimit;
                    soundButton.interactable = false;
                    break;
                default:
                    LevelFinished();
                    break;
            }

            levelText.text = $"LEVEL: {currentLevel.ToString()}";
        }

        void LevelFinished()
        {
            gameFinishedPanel.SetActive(true);
            Time.timeScale = 0f;
            gameFinishedText.text = $"CONGRATULATIONS! YOU GOT {playerScore} SCORE!";
            gameIsDone = true;
        }

        public void TryAgain()
        {
            gameFinishedPanel.SetActive(false);
            Time.timeScale = 1f; 
            currentLevel = 1;
            playerScore = 0; 
            currentQuestion = 0;
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
                answerInt = 0;
                gameTimer = 10f;
                isCorrect = true;
            }
            else
            {
                Debug.Log("Wrong button pressed.");
                answerInt = 1;
                gameTimer = 10f;
            }
            StartCoroutine(AnswerIndcatorAnimation());
            isAnswered = true;

        }

        IEnumerator AnswerIndcatorAnimation()
        {
            if (currentLevel <= maxLevel)
            {
                if (currentQuestion < maxQuestion)
                {
                    currentQuestion++;
                }
                else
                {
                    currentLevel++;
                    currentQuestion = 1;
                }
            }
            if (timerSounds.isPlaying)
            {
                timerSounds.Stop();
            }
            
            answerSounds[answerInt].Play();
            yield return new WaitForSeconds(0.5f);

            GameObject instantiatedAnswerIndicator = Instantiate(answerIndicatorPrefab[answerInt], canvasTransform);
            instantiatedIndicators.Add(instantiatedAnswerIndicator);
            DestroyInstantiatedPrefabs();
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(2f); // Use WaitForSecondsRealtime for accurate time measurement

            // Clear indicators and perform necessary cleanup
            instantiatedIndicators.Remove(instantiatedAnswerIndicator); // Remove from list before destroying
            Destroy(instantiatedAnswerIndicator);
            InstantiateNewPrefabs();
            if (isCorrect)
            {
                playerScore++;
                isCorrect = false;
            }
            answerInt = 1;
            // Set the timescale back to 1 after all the operations
            Time.timeScale = 1f;
            sandClock.ResetTime();
            timer = timerLimit;
            isAnswered = false;
            Debug.Log("EndRound");
            
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
