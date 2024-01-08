using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace IQuiz
{
    public class TimerSand : MonoBehaviour
    {
        public Image topFillImage;
        public Image bottomFillImage;

        public float roundDuration;
        private float fillRate;

        public bool isRunning = false;

        // Start is called before the first frame update
        void Start()
        {
            Begin();
            CalculateFillRate();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateFillAmount();
        }

        public void Begin()
        {
            ResetClock();
            isRunning = true;
        }

        public void ResetClock()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            topFillImage.fillAmount = 1;
            bottomFillImage.fillAmount = 0;
            isRunning = false;
        }

        void CalculateFillRate()
        {
            fillRate = 1.0f / roundDuration; // Calculate fill rate per second
        }

        void UpdateFillAmount()
        {
            if (topFillImage.fillAmount > 0)
            {
                topFillImage.fillAmount -= Time.deltaTime * fillRate;
                bottomFillImage.fillAmount += Time.deltaTime * fillRate;
            }
            else
            {
                // Timer has reached zero
                isRunning = false;
                // You can add actions for when the timer reaches zero here
            }
        }

        public void ResetTime()
        {
            StartCoroutine(ClockRotation());
        }

        IEnumerator ClockRotation()
        {
            
             transform
                .DORotate(Vector3.forward * 180f, .8f, RotateMode.FastBeyond360)
                .From(Vector3.zero)
                .SetEase(Ease.InOutBack);

            yield return new WaitForSeconds(1);
            ResetClock();
            Begin();
            
            
        }
    }
}
