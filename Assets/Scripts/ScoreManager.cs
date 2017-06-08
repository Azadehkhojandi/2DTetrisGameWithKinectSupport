using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreManager : MonoBehaviour
    {
        private int _totalScore = 0;
        private int fullRowScore = 100;
        private int shapeScore = 10;

        public Text TotalScoreText;

        public  void AddFullrowScore()
        {
            _totalScore = _totalScore + fullRowScore;
            TotalScoreText.text = _totalScore.ToString();
            
        }
        public void AddnextShapeScore(bool success)
        {
            _totalScore = _totalScore + (shapeScore* (success ? 1 : -1));
            TotalScoreText.text = _totalScore.ToString();
        }

        public  int  GetTotalScore()
        {
           return  _totalScore;
        }

       
    }
}
   

