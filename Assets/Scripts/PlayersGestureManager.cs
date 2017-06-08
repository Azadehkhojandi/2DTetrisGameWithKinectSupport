using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Windows.Kinect;
using System.Text;
using UnityEngine.UI;


namespace Assets.Scripts
{
    public class PlayersGestureManager : MonoBehaviour

    {
       
        private KinectPlayersManager _kinectPlayersManager;
        private KinectSensor _sensor;
        private string _playersdbPath;
        private Dictionary<ulong, GestureDetector> _bodies = new Dictionary<ulong, GestureDetector>();

        public GameObject KinectPlayersManager;
        public Text Status;

        private void GestureDetectorOnGestureDetected(object sender, GestureEventArgs e)
        {
            if (GameOver.IsGameover)
            {
                return;
            }
            var isDetected = e.IsBodyTrackingIdValid && e.IsGestureDetected;

            if (Status != null)
            {
                var text = new StringBuilder(string.Format("Gesture:{0} Detected: {1}\n", e.GestureName, isDetected));
                text.Append(string.Format("Confidence: {0}\n", e.DetectionConfidence));
                Status.text = text.ToString();
            }


            //if (e.DetectionConfidence >= 0.1f)
            {
                //to do set players
                switch (e.GestureName)
                {
                    case "Player1Select":
                        {

                            _kinectPlayersManager.SetShapesPlayerTrackingId(e.TrackingId);
                            if (_kinectPlayersManager.GetControlsPlayerTrackingId() == e.TrackingId)
                            {
                                _kinectPlayersManager.SetControlsPlayerTrackingId(0);
                            }

                            break;
                        }
                    case "Player2Select":
                        {
                            _kinectPlayersManager.SetControlsPlayerTrackingId(e.TrackingId);
                            if (_kinectPlayersManager.GetShapesPlayerTrackingId() == e.TrackingId)
                            {
                                _kinectPlayersManager.SetShapesPlayerTrackingId(0);
                            }
                            break;
                        }
                }
            }

        }

    



    // Use this for initialization
    void Start()
    {
        _kinectPlayersManager = KinectPlayersManager.GetComponent<KinectPlayersManager>();
        _sensor = KinectSensor.GetDefault();
        _playersdbPath = Application.dataPath + "/database/Players.gbd";
        if (_sensor != null)
        {

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }
        if (Status != null)
        {
            Status.text = "";
        }

    }

    void Update()
    {
        if (GameOver.IsGameover)
        {
            StopDetecting();
            return;
        }

        if (_kinectPlayersManager == null)
        {
            StopDetecting();
            return;
        }

        //set the detecting shapes for all the bodies in front of kinect sensor
        var bodies = _kinectPlayersManager.GetData();
        if (bodies == null || bodies.Length <= 0 || bodies.All(x => x.TrackingId == 0))
        {
            return;
        }
        //todo refactor 
        if (_bodies.Any())
        {
            foreach (var localbody in _bodies.Keys.ToArray())
            {
               if (!bodies.Any(x => x.TrackingId == localbody))
                {
                    //means the body has left the game
                    RemoveGestureDetector(localbody);
                }
            }
        }
        foreach (var body in bodies)
        {
            if (!_bodies.ContainsKey(body.TrackingId))
            {
                _bodies.Add(body.TrackingId, AddGestureDetector(body.TrackingId));
            }

        }

    }

    public GestureDetector AddGestureDetector(ulong trackingId)
    {
        var gestureDetector = new GestureDetector(_sensor, _playersdbPath)
        {
            TrackingId = trackingId,
            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
            IsPaused = false
        };
        gestureDetector.OnGestureDetected += GestureDetectorOnGestureDetected;
        return gestureDetector;
    }
    public void RemoveGestureDetector(ulong trackingId)
    {
        if (_bodies.Keys.Contains(trackingId))
        {
            _bodies[trackingId].IsPaused = true;
            _bodies[trackingId].TrackingId = 0;
            _bodies.Remove(trackingId);
            }


    }

    public void StopDetecting()
    {
        if (_bodies == null)
        {
            return;
        }
        foreach (var localbody in _bodies)
        {
            localbody.Value.IsPaused = true;
            localbody.Value.TrackingId = 0;

        }
    }
}
}
