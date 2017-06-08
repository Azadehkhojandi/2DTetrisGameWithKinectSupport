using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameOver : MonoBehaviour
    {
        public static bool IsGameover=false;

    
        public GameObject GameOverBack;
        public GameObject ScoreCanvas;
        public Button ResetButton;
        public Text FinalScoreText;

        // Use this for initialization
        void Start()
        {
            ResetButton.onClick.AddListener(TaskOnClick);
            GameOverBack.SetActive(false);
            ScoreCanvas.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void PopUp()
        {
            IsGameover = true;
            GameOverBack.SetActive(true);
            ScoreCanvas.SetActive(false);
            var previewCanvas = GameObject.Find("PreviewCanvas");
            if (previewCanvas != null)
            {
                previewCanvas.SetActive(false);
            }
            var timerCanvas = GameObject.Find("TimerCanvas");
            if (timerCanvas != null)
            {
                timerCanvas.SetActive(false);
            }

            FinalScoreText.text = FindObjectOfType<ScoreManager>().GetTotalScore().ToString();
        }
        void TaskOnClick()
        {
            GameOver.IsGameover = false;
            SceneManager.LoadScene(transform.gameObject.scene.name);
        }
    }
}
