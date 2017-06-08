using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Spawner : MonoBehaviour
    {
        private GameObject _previewShape;
        private int _previewGroupIndex;
        private bool _isShapeTimerEnabled;
        private float _endTime;
        private float _timeLeft;
        private int _ShapeTimerinSeconds = 5;//timer to make shape

       
        public GameObject[] Groups; // Groups
        public Text TimerText;
        public Text PreviewText;


        void Start()
        {
            // Spawn initial Group
            SpawnPreviewShape();
        }

        public string GetPreviewShapeName()
        {
            return _previewShape.gameObject.name.Replace("(Clone)", "");
        }
        // Update is called once per frame
        void Update()
        {
            if (GameOver.IsGameover)
            {
                return;
            }

            // for testing - if you press n it adds next shape to game
            if (Input.GetKeyDown(KeyCode.N))
            {
                FindObjectOfType<Spawner>().AddPreviewShapetoGame();
            }

            if (_isShapeTimerEnabled)
            {
                _timeLeft = _endTime - Time.time;
                if (_timeLeft < 0)
                {
                    _timeLeft = 0;
                    _isShapeTimerEnabled = false;
                    //user failed to create the shape in given time
                    FindObjectOfType<ScoreManager>().AddnextShapeScore(false);
                    //show another shape in preview section
                    SpawnPreviewShape();

                }
                else
                {
                    TimerText.text = string.Format("00:{0}", ((int)_timeLeft));
                }
            }


        }
        public void SpawnPreviewShape()
        {

            // Random Index
            _previewGroupIndex = Random.Range(0, Groups.Length);

            //show the shape in the right hand side - preview 
            var spawnerPosition = new Vector3(15, 10, 0);

            if (_previewShape != null)
            {
                Destroy(_previewShape);
            }

            _previewShape = Instantiate(Groups[_previewGroupIndex],
                 spawnerPosition,
                 Quaternion.identity);

            //show timer and give user time to make the shape
            _isShapeTimerEnabled = true;
            _endTime = Time.time + _ShapeTimerinSeconds;
            TimerText.text = string.Format("00:{0}", _ShapeTimerinSeconds);
            PreviewText.text = _previewShape.gameObject.name;

        }

        public void AddPreviewShapetoGame()
        {
            if (_isShapeTimerEnabled)
            {
                //disable timer 
                _isShapeTimerEnabled = false;

                //user successfully made the shape
                FindObjectOfType<ScoreManager>().AddnextShapeScore(true);

                //
                if (_previewShape != null)
                {
                    Destroy(_previewShape);
                }

                // Spawn Group at current Position
                Instantiate(Groups[_previewGroupIndex],
                    transform.position,
                    Quaternion.identity);
            }

        }



    }
}
