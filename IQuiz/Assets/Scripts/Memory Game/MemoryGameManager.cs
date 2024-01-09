using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace IQuiz
{
    public class MemoryGameManager : MonoBehaviour
    {
        #region Private Variable
        [Header("Prefab")]
        [SerializeField] private GameObject[] bodyPartImage;
        [SerializeField] private GameObject correctPrefab;
        [SerializeField] private GameObject wrongPrefab;
        [Space]
        [Header("UI Reference")]
        [SerializeField] private Sprite backCardSprite;
        [SerializeField] private Transform imageHolder;
        [SerializeField] private Transform canvasTransform;
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;
        [Space]

        private GameObject instantiatedBodyPart;
        [SerializeField] private List<GameObject> instantiatedGO = new List<GameObject>();
        private List<int> prevRandomIndex = new List<int>();
        private string bodyPartName;
        private string question;
        private string answer;
        private string buttonName;

        #endregion

        #region Public Variables

        [Header("Sounds")]
        public AudioSource instantiatingSoundEffect;
        public AudioSource timerSounds;

        [Header("Time")]
        public float gameTime;
        public float timeLimit;
        public GameObject sandClockPrefab;
        GameObject sandClockGO;
        public Transform sandClockParent;

        [Space]
        [Header("Levels and Player")]
        public int currentLevel;
        public int currentQuestion;
        public int playerScore;
        public int questionLimit;
        public int levelLimit;

        #endregion



        // Start is called before the first frame update
        #region Unity CallBack Methods
        void Start()
        {
            StartCoroutine(Question());
            timeLimit = 8f;
        }

        // Update is called once per frame
        void Update()
        {
            SetTextUI(question, questionText);
            SetTextUI($"Score: {playerScore}", scoreText);
            SetTextUI($"Level: {currentLevel}", levelText);
        }

        IEnumerator Question()
        {
            if (currentLevel < levelLimit)
            {
                if (currentQuestion < questionLimit)
                {
                    currentQuestion++;
                }
                else
                {
                    currentLevel++;
                    currentQuestion = 1;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (!instantiatingSoundEffect.isPlaying)
                {
                    instantiatingSoundEffect.Play();
                    yield return new WaitForSeconds(0.5f);
                }
                InstantiateButton();
                Debug.Log("instantiated " + instantiatedBodyPart.name);
                yield return new WaitForSeconds(1.5f);
            }

            Debug.Log("instantiated");
            int randomIndex = Random.Range(0, instantiatedGO.Count);
            bodyPartName = instantiatedGO[randomIndex].name;
            bodyPartName = bodyPartName.Replace("(Clone)", "");
            answer = bodyPartName;

            question = $"Where is the {bodyPartName} ?";

            Debug.Log("question set");
            SpawnSandClock();
            if (!timerSounds.isPlaying)
            {
                timerSounds.Play();
            }
            yield return new WaitForSeconds(timeLimit);
            if (timerSounds.isPlaying)
            {
                timerSounds.Stop();
            }
            Destroy(sandClockGO);
            prevRandomIndex.Clear();
            SwitchImage();
        }

        IEnumerator AnswerChecker()
        {
            if (Checker())
            {
                GameObject correctGO = Instantiate(correctPrefab, canvasTransform);
                yield return new WaitForSeconds(3f);
                Destroy(correctGO);
                playerScore++;
                StartCoroutine(Question());
            }
            else
            {
                GameObject wrongGO = Instantiate(wrongPrefab, canvasTransform);
                yield return new WaitForSeconds(3f);
                Destroy(wrongGO);
                StartCoroutine(Question());
            }
        }
        #endregion

        #region Spawn and Destroy Image method
        public void InstantiateButton()
        {
            int randomIndex = Random.Range(0, bodyPartImage.Length);

            while (prevRandomIndex.Contains(randomIndex))
            {
                randomIndex = Random.Range(0, bodyPartImage.Length);
            }

            instantiatedBodyPart = Instantiate(bodyPartImage[randomIndex], imageHolder);

            // Set a unique name for each instantiated button based on its body part
            instantiatedBodyPart.name = bodyPartImage[randomIndex].name; // Set the button name

            instantiatedGO.Add(instantiatedBodyPart);
            prevRandomIndex.Add(randomIndex);

            Debug.Log($"random index: {randomIndex}");

            Button currentButton = instantiatedBodyPart.GetComponent<Button>();
            currentButton.interactable = false;

            // Add listener to the button's OnClick event
            currentButton.onClick.AddListener(() => OnButtonClick(currentButton.name));
        }


        public void DestroyImage()
        {
            for (int i = 0; i < instantiatedGO.Count; i++)
            {
                Destroy(instantiatedGO[i]);
            }
            instantiatedGO.Clear();
        }

        #endregion

        #region UI Method

        public void SetTextUI(string valueToSet, TMP_Text textToDisplay)
        {
            textToDisplay.text = valueToSet;
        }

        public void ClearTexts()
        {
            answer = "";
            question = "";
            buttonName = "";
        }

        public void SwitchImage()
        {
            foreach (var item in instantiatedGO)
            {
                Image itemImage = item.GetComponent<Image>();
                itemImage.sprite = backCardSprite;
                item.GetComponent<Button>().interactable = true;
            }
        }


        #endregion

        #region Button Methods
        public void OnButtonClick(string _buttonName)
        {
            buttonName = _buttonName;
            Debug.Log(_buttonName);

            if (Checker())
            {
                Debug.Log("Correct");
                DestroyImage();
                StartCoroutine(AnswerChecker());
                ClearTexts();
            }
            else
            {
                Debug.Log("Wrong");
                DestroyImage();
                StartCoroutine(AnswerChecker());
                ClearTexts();
            }
        }

        public bool Checker()
        {
            if (buttonName == answer)
            {
                
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Time Method
        void SpawnSandClock()
        {
            sandClockGO = Instantiate(sandClockPrefab, sandClockParent);
            sandClockGO.GetComponent<TimerSand>().roundDuration = timeLimit;
            Debug.Log("duration time is set");
        }

        void UpdateTimer()
        {
            if (gameTime >= 0)
            {
                gameTime -= Time.deltaTime;
            }
            else
            {
                ResetTimer();
            }
        }

        void ResetTimer()
        {
            gameTime = timeLimit;
        }
        #endregion
    }
}
