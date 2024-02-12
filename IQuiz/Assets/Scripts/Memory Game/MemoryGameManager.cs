using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private GameObject levelfinishedPanel;
        [SerializeField] private TMP_Text levelFinishedText;

        [SerializeField] private TMP_Text countDownText;
        [Space]

        private GameObject instantiatedBodyPart;
        [SerializeField] GameObject wrongGO;
        [SerializeField] GameObject correctGO;
        [SerializeField] private List<GameObject> instantiatedGO = new List<GameObject>();
        private List<int> prevRandomIndex = new List<int>();
        private string bodyPartName;
        private string question;
        private string answer;
        private string buttonName;
        [SerializeField] private bool isAnswered;

        bool hasStarted;

        private Coroutine questionCoroutine;

        #endregion

        #region Public Variables

        [Header("Sounds")]
        public AudioSource instantiatingSoundEffect;
        public AudioSource timerSounds;

        [Header("Time")]
        public float memorizingGameTimer;
        public float memorizingTimeLimit;
        public float answeringTimer;
        public float countDownTimer;

        [Space]
        public GameObject sandClockPrefab;
        GameObject sandClockGO;
        public Transform sandClockParent;
        public MemoryUIManager memoryUIManager;

        [Space]
        [Header("Levels and Player")]
        public int currentLevel;
        public int currentQuestion;
        public int playerScore;
        public int questionLimit;
        public int levelLimit;
        bool gameIsDone;

        #endregion



        // Start is called before the first frame update
        #region Unity CallBack Methods
        void Start()
        {
            StartCoroutine(Question());

        }

        // Update is called once per frame
        void Update()
        {
            SetTextUI(question, questionText);
            SetTextUI($"Score: {playerScore}", scoreText);
            SetTextUI($"Level: {currentLevel}", levelText);

            UpdateTimer();
            //SWITCH CASE THAT HANDLES THE BEHAVIOR OF GAME THROUGH CURRENT LEVEL VARIABLE
            switch (currentLevel)
            {
                case 1:
                    memorizingTimeLimit = 8;
                    break;
                case 2:
                    memorizingTimeLimit = 6;
                    break;
                case 3:
                    memorizingTimeLimit = 4;
                    break;
                case 4:
                    answeringTimer = 6;
                    memorizingTimeLimit = 4;
                    break;
                case 5:
                    answeringTimer = 4;
                    memorizingTimeLimit = 4;
                    break;
                default:
                    LevelFinished();
                    break;
            }

            if (memoryUIManager.IsSettingsOpen() && correctGO != null || memoryUIManager.IsSettingsOpen() && wrongGO != null)
            {
                Destroy(correctGO);
                Destroy(wrongGO);
            }
        }

        IEnumerator Question()
        {
            ClearTexts();
            if (currentLevel <= levelLimit)
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
            isAnswered = false;
            Debug.Log("question set");
            SpawnSandClock();

            if (!timerSounds.isPlaying)
            {
                timerSounds.Play();
            }

            yield return new WaitForSeconds(memorizingTimeLimit);
            //MEMORIZING TIME LIMIT IS OVER
            //TIME FOR PLAYER TO ANSWER
            if (timerSounds.isPlaying)
            {
                timerSounds.Stop();
            }

            Destroy(sandClockGO);
            prevRandomIndex.Clear();
            SwitchImage();

            //IF LEVEL 4 AND UP THEN PUT ANSWERING TIMER
            if (currentLevel >= 4)
            {
                sandClockGO = Instantiate(sandClockPrefab, sandClockParent);
                sandClockGO.GetComponent<TimerSand>().roundDuration = answeringTimer;
                yield return new WaitForSeconds(answeringTimer);
                if (!isAnswered)
                {
                    Destroy(sandClockGO);
                    StartCoroutine(AnswerChecker());
                    yield break;
                }
                Destroy(sandClockGO);
                yield break;

            }
        }

        IEnumerator AnswerChecker()
        {
            if (Checker())
            {
                correctGO = Instantiate(correctPrefab, canvasTransform);
                yield return new WaitForSeconds(3f);
                Destroy(correctGO);
                playerScore++;
                questionCoroutine = StartCoroutine(Question());
            }
            else
            {
                wrongGO = Instantiate(wrongPrefab, canvasTransform);
                yield return new WaitForSeconds(3f);
                Destroy(wrongGO);
                DestroyImage();
                questionCoroutine = StartCoroutine(Question());
            }
        }
        #endregion

        #region Spawn and Destroy Image method
        public void InstantiateButton() //SPAWN PICTURES 
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
        void LevelFinished()
        {
            levelfinishedPanel.SetActive(true);
            Time.timeScale = 0f;
            levelFinishedText.text = $"CONGRATULATIONS! YOU GOT A {playerScore} SCORE!";
            gameIsDone = true;
            TryAgainButton();

        }

        public void TryAgain()
        {
            levelfinishedPanel.SetActive(false);
            Time.timeScale = 1f;
            currentLevel = 1;
            currentQuestion = 1;
            playerScore = 0;
            gameIsDone = true;
        }

        public void SwitchImage()
        {
            foreach (var item in instantiatedGO)
            {
                Image itemImage = item.GetComponent<Image>();
                itemImage.sprite = backCardSprite;
                item.GetComponent<Button>().interactable = true;
            }
            Debug.Log("All image switched!");
        }

        public void TryAgainButton()
        {
            currentLevel = 1;
            currentQuestion = 0;
            playerScore = 0;
            StopAllCoroutines();
            prevRandomIndex.Clear();
            DestroyImage();
            if (sandClockGO != null)
            {
                Destroy(sandClockGO);
            }
            if (timerSounds.isPlaying)
            {
                timerSounds.Stop();
            }
            ClearTexts();
            // Restart the coroutine by calling it again
            questionCoroutine = StartCoroutine(Question());
        }

        #endregion

        #region Button Methods
        public void OnButtonClick(string _buttonName)
        {
            buttonName = _buttonName;
            Debug.Log(_buttonName);

            if (Checker())
            {
                isAnswered = true; ;
                DestroyImage();
                StartCoroutine(AnswerChecker());
                ClearTexts();
                if (sandClockGO != null)
                {
                    Destroy(sandClockGO);

                }
            }
            else
            {
                isAnswered = true;
                DestroyImage();
                StartCoroutine(AnswerChecker());
                ClearTexts();
                if (sandClockGO != null)
                {
                    Destroy(sandClockGO);
                }
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
            sandClockGO.GetComponent<TimerSand>().roundDuration = memorizingTimeLimit;
            Debug.Log("duration time is set");
        }
        void UpdateTimer()
        {
            if (memorizingGameTimer >= 0)
            {
                memorizingGameTimer -= Time.deltaTime;
            }
            else
            {
                ResetTimer();
            }
        }

        void ResetTimer()
        {
            memorizingGameTimer = memorizingTimeLimit;
        }
        #endregion
    }
}
