using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ23.UI
{
    public class LevelSelectScreen : MonoBehaviour
    {
        public List<Button> buttons = new List<Button>();

        private void Awake()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                var level = i + 1;
                buttons[i].onClick.AddListener(() => StartLevel(level));
            }
        }

        public static void StartLevel(int level)
        {
            SceneManager.LoadScene(level, LoadSceneMode.Single);
            SceneManager.LoadScene(IngameUi.IngameUiSceneName, LoadSceneMode.Additive);
        }
    }
}