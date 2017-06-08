using UnityEngine;
using Windows.Kinect;
using System.Text;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class ControlsGestureManager : MonoBehaviour

    {
        private GestureDetector _gestureDetector;
        private KinectPlayersManager _kinectPlayersManager;
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
        

            //move the shape 
            var group = FindObjectOfType<Group>();

            if (group.IsPreviewGroup())
            {
                return;
            }

            switch (e.GestureName)
            {
                case "Lean_Left":
                {
                    group.MoveLeft(true);
                    break;
                }
                case "Lean_Right":
                {
                    group.MoveRight(true);
                    break;
                }
                case "Up":
                {
                    group.Rotate(true);
                    break;
                }
                case "Down":
                {
                    group.MoveDownwards();
                    break;
                }
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
            _kinectPlayersManager.OnControlsPlayerTrackingIdChanges += _kinectPlayersManager_OnControlsPlayerTrackingIdChanges;

            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {

                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                }
                var shapesdbPath = Application.dataPath + "/database/Controls.gbd";
                _gestureDetector = new GestureDetector(_sensor, shapesdbPath);
            }
            if (Status != null)
            {
                Status.text = "";
            }


        }

        private void _kinectPlayersManager_OnControlsPlayerTrackingIdChanges(object sender, KinectPlayersEventArgs e)
        {
            var controlsPlayerTrackingId = e.TrackingId;

            if (controlsPlayerTrackingId == 0)
            {
                // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                _gestureDetector.TrackingId = 0;
                _gestureDetector.IsPaused = true;
                return;
            }
            if ( _gestureDetector.TrackingId != controlsPlayerTrackingId)
            {
                _gestureDetector.IsPaused = false;
                SetTrackingId(controlsPlayerTrackingId);
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
