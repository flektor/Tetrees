using UnityEngine;
using UnityEngine.UI;

namespace GGJ23.UI
{
    public class StartScreenUi : MonoBehaviour
    {
        public CanvasGroup startCanvasGroup;
        public CanvasGroup tutorialCanvasGroup;
        public CanvasGroup levelCanvasGroup;

        public Button startButton;
        public Button tutorialButton;

        private void Start()
        {
            HideGroup(tutorialCanvasGroup);
            HideGroup(levelCanvasGroup);
            ShowGroup(startCanvasGroup);

            startButton.onClick.AddListener(() =>
            {
                HideGroup(startCanvasGroup);
                ShowGroup(levelCanvasGroup);
            });

            tutorialButton.onClick.AddListener(() =>
            {
                HideGroup(startCanvasGroup);
                ShowGroup(tutorialCanvasGroup);
            });
        }

        private void ShowGroup(CanvasGroup group)
        {
            group.alpha = 1;
            group.blocksRaycasts = true;
        }

        private void HideGroup(CanvasGroup group)
        {
            group.alpha = 0;
            group.blocksRaycasts = false;
        }
    }
}