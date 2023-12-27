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

        //NOTE: THE PREFABS ARE THE BUTTONS AND THE SOUNDS 
        //NOTE: INSTANTIATE MEANS SPAWNING GAME OBJECTS INTO GAME AREA
        public GameObject[] prefabChoices; //ARRAY OF PREFABS TO INSTANTIATE
        public Transform choicesParent; // THE PARENT OF INSTANTIATIGN PREFAB 

        private List<GameObject> instantiatedChoices = new List<GameObject>(); // LIST OF INSTANTIATED PREFAB 

        [Space]
        public AudioSource animalSoundQuestion; //THE SOUND TO BE PLAYED 
        int randomIndexSoundQuestion; //RANDOM SOUNDS IN INSTANTIATED BUTTONS

        [Header("Player Stats")]
        public int playerScore; // SCORE OF THE PLAYER

        // Start is called before the first frame update
        void Start()
        {
            //CALLING THESE 2 FUNCTIONS WHEN THE GAME STARTS
            InstantiateNewPrefabs();
            PlaySound(); // Play sound when the game starts
        }

        // Update is called once per frame
        void Update()
        {

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
            if (index == randomIndexSoundQuestion)
            {
                playerScore++; // IF ANSWER IS CORRECT THEN INCREASE SCORE

            }
            else
            {
                Debug.Log("Wrong button pressed."); // IF NOT THEN DO NOTHING
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
                buttonChoices.onClick.AddListener(() => DestroyInstantiatedPrefabs());
                buttonChoices.onClick.AddListener(() => InstantiateNewPrefabs());
                buttonChoices.onClick.AddListener(() => PlaySound());

                availableIndexes.RemoveAt(randomIndex);
            }
            GetAnimalSounds();
        }


    }
}
