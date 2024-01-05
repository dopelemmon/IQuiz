using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace IQuiz
{
    public class SandClock : MonoBehaviour
    {
        //*****************UI VARIABLES*******************************
        [Header("UI Variables")]
        [SerializeField] private Image topFillImage;
        [SerializeField] private Image bottomFillImage;
        [SerializeField] private RectTransform sandPyramidRect;

        [Space]
        [Header("Time Variable")]
        public float durationTime;
        public int totalRounds;

        private float timer;

        //EVENTS
        public UnityAction<int> onRoundStart;
        public UnityAction<int> onRoundEnd;
        //ROUND VARIABLE
        [SerializeField] int currentRound = 0;
        //RECT TRANSFORM POSITION VARIABLE
        float defaultSandPyramidYPos;

        void Awake()
        {
            SetRoundText(totalRounds);
            defaultSandPyramidYPos = sandPyramidRect.anchoredPosition.y; //SETTING THE FLOAT SAND PYRAMID POS INTO THE RECT TRANSFORM Y POSITION
        }
        void Start()
        {
            Begin();
        }

        public void Begin()
        {
            currentRound++;
            onRoundStart?.Invoke(currentRound);


            //SCALING SANDPYRAMID
            sandPyramidRect.DOScaleY(1f, durationTime / 3f).From(0f);
            sandPyramidRect.DOScaleY(0f, durationTime / 1.5f).SetDelay(durationTime / 3f).SetEase(Ease.Linear);

            //PYRAMID ANIMATION
            sandPyramidRect.anchoredPosition = Vector2.up * defaultSandPyramidYPos;
            sandPyramidRect.DOAnchorPosY(0f, durationTime).SetEase(Ease.Linear);
            ResetClock();


            //TOP SAND PART FALLING OR DECREASING ANIMATION CALCULATION WITH THE DURATION TIME VARIABLE

            topFillImage
                .DOFillAmount(0, durationTime)
                .SetEase(Ease.Linear)
                .OnUpdate(OnTimeUpdate)
                .OnComplete(OnRoundTimeComplete);

        }



        void OnTimeUpdate()
        {
            bottomFillImage.fillAmount = 1f - topFillImage.fillAmount;
        }

        public void OnRoundTimeComplete()
        {
            onRoundEnd?.Invoke(currentRound);

            if (currentRound < totalRounds)
            {
                transform
                .DORotate(Vector3.forward * 180f, .8f, RotateMode.FastBeyond360)
                .From(Vector3.zero)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    SetRoundText(totalRounds - currentRound);
                    Begin();
                });
            }
            else
            {
                SetRoundText(0);
                transform.DOShakeScale(.8f, .3f, 10, 90f, true);
            }
        }



        public void ResetClock()
        {
            timer = durationTime;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            topFillImage.fillAmount = 1f;
            bottomFillImage.fillAmount = 0f;
            sandPyramidRect.localScale = Vector3.one; // Reset scale if needed
                                                      // Reset any ongoing tweens or animations
                                                      // DOTween.Kill(sandPyramidRect); // Kill ongoing tweens if necessary
        }

        public void Answered()
        {
            transform
                .DORotate(Vector3.forward * 180f, .8f, RotateMode.FastBeyond360)
                .From(Vector3.zero)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    SetRoundText(totalRounds - currentRound);
                    Begin();
                });
            ResetClock();
        }

        void SetRoundText(int value)
        {

        }
    }
}
