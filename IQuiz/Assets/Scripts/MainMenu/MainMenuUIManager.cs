using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IQuiz
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField] GameObject chooseGameModePanel;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ToggleChooseGame()
        {
            if (chooseGameModePanel.activeInHierarchy)
            {
                chooseGameModePanel.SetActive(false);
            }
            else
            {
                chooseGameModePanel.SetActive(true);
            }
        }

        #region CHOOSE GAME MODE BUTTON FUNCTION
        public void ChooseAnimals()
        {
            SceneManager.LoadScene("Animals");
        }
        public void ChooseMemory()
        {
            SceneManager.LoadScene("Memory");
        }
        public void ChooseMath()
        {
            SceneManager.LoadScene("Math");
        }
        public void ChooseFruits()
        {
            SceneManager.LoadScene("Fruits");
        }
        #endregion

        #region OTHER UI BUTTONS
        public void QuitApplication()
        {
            Application.Quit();
        }
        #endregion
    }
}
