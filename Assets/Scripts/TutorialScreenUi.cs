using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GGJ23
{
    public class TutorialScreenUi : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        
        void Start()
        {
            _backButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}