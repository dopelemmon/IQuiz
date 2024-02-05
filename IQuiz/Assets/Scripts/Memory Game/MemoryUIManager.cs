using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IQuiz
{
    public class MemoryUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject SettingsUI;
        [SerializeField] private GameObject muted;
        [SerializeField] private GameObject unMuted;

        public AudioSource[] allSounds;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (IsSettingsOpen())
            {
                Time.timeScale = 0; // real game time to zero pausing the game
            }
            else
            {
                Time.timeScale = 1; // real game time to 1 default duration of game time and resuming the game
            }
        }

        public bool IsSettingsOpen()
        {
            return SettingsUI.activeInHierarchy ? true : false;
        }

        public void ToggleSettings()
        {
            if (!IsSettingsOpen())
            {
                SettingsUI.SetActive(true);
            }
            else
            {
                SettingsUI.SetActive(false);
            }
        }

        public void MuteSounds()
        {
            foreach (var sound in allSounds)
            {
                if (!sound.mute)
                {
                    sound.mute = true;
                    muted.SetActive(true);
                    unMuted.SetActive(false);
                }
                else
                {
                    sound.mute = false;
                    muted.SetActive(false);
                    unMuted.SetActive(true);
                }
            }
        }

        public void MainMenuButton()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
