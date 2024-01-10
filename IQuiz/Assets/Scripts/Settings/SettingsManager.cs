using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IQuiz
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] GameObject settingsPanel;
        [SerializeField] GameObject muted;
        [SerializeField] GameObject unMuted;
        [SerializeField] AudioSource backgroundSound;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ToggleSettings()
        {
            //IF SETTINGS UI IS OPEN THEN CLOSE THE SETTINGS PANEL
            if (settingsPanel.activeInHierarchy)
            {
                settingsPanel.SetActive(false);
            }
            //IF CLOSED THEN OPEN SETTINGS PANEL
            else
            {
                settingsPanel.SetActive(true);
            }
        }

        public void MuteUnmute()
        {
            if (backgroundSound.mute)
            {
                backgroundSound.mute = false;
                muted.SetActive(true);
                unMuted.SetActive(false);
            }
            else
            {
                backgroundSound.mute = true;
                muted.SetActive(false);
                unMuted.SetActive(true);
            }
        }
    }
}
