using UnityEngine;
using Windows.Kinect;
using System.Text;
using UnityEngine.UI;


namespace Assets.Scripts
{


    public class ShapesGestureManager : MonoBehaviour

    {
        private KinectPlayersManager _kinectPlayersManager;
        private GestureDetector _gestureDetector;
        private KinectSensor _sensor;

        public GameObject KinectPlayersManager;
        public Text Status;


        public void SetTrackingId(ulong id)
        {
            _gestureDetector.TrackingId = id;
            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
            _gestureDetector.IsPaused = false;
            _gestureDetector.OnGestureDetected += GestureDetectorOnGestureDetected;


        }

        private void GestureDetectorOnGestureDetected(object sender, GestureEventArgs e)
        {
            if (GameOver.IsGameover)
            {
                return;
            }
            var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;
           
            if (Status != null)
            {
                StringBuilder text = new StringBuilder(string.Format("Gesture:{0} Detected: {1}\n", e.GestureName, isDetected));
                text.Append(string.Format("Confidence: {0}\n", e.DetectionConfidence));
                Status.text = text.ToString();
            }
           

            //if it's detected with satisfied confidence level 
            //we add the shape into the game 
            var spawner = FindObjectOfType<Spawner>();
            var currentPreviewShape = spawner.GetPreviewShapeName();
            if (isDetected && currentPreviewShape == e.GestureName)
            {
                spawner.AddPreviewShapetoGame();
            }

        }

        
        // Use this for initialization
        void Start()
        {
            _kinectPlayersManager = KinectPlayersManager.GetComponent<KinectPlayersManager>();
            if (_kinectPlayersManager == null)
            {
                return;
            }
            _kinectPlayersManager.OnShapesPlayerTrackingIdChanges += _kinectPlayersManager_OnShapesPlayerTrackingIdChanges;
             _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {

                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                }
                var shapesdbPath = Application.dataPath + "/database/Shapes.gbd";
                _gestureDetector = new GestureDetector(_sensor, shapesdbPath);
            }
            if (Status != null)
            {
                Status.text = "";
            }
        }

        private void _kinectPlayersManager_OnShapesPlayerTrackingIdChanges(object sender, KinectPlayersEventArgs e)
        {
            var shapesPlayerTrackingId = e.TrackingId;

            if (shapesPlayerTrackingId == 0)
            {
                // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                _gestureDetector.TrackingId = 0;
                _gestureDetector.IsPaused = true;
                return;
            }

            if ( _gestureDetector.TrackingId != shapesPlayerTrackingId)
            {
                _gestureDetector.IsPaused = false;
                SetTrackingId(shapesPlayerTrackingId);
            }
        }

        void Update()
        {
            if (GameOver.IsGameover)
            {
                return;
            }

            if (_kinectPlayersManager == null)
            {
                return;
            }
            
        }

    }
}
