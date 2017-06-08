using Windows.Kinect;
using UnityEngine;

namespace Assets.Scripts
{
    public class CustomBodySourceManager : MonoBehaviour 
    {
        private KinectSensor _sensor;
        private BodyFrameReader _reader;
        private Body[] _data;
    
        public Body[] GetData()
        {
            return _data;
        }
    

        void Start () 
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _reader = _sensor.BodyFrameSource.OpenReader();
            
                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                }
            }   
        }
    
        void Update () 
        {
            if (_reader != null)
            {
                var frame = _reader.AcquireLatestFrame();
                if (frame != null)
                {
                    if (_data == null)
                    {
                        _data = new Body[_sensor.BodyFrameSource.BodyCount];
                    }
                
                    frame.GetAndRefreshBodyData(_data);
                
                    frame.Dispose();
                    frame = null;
                }
            }    
        }
    
        void OnApplicationQuit()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        
            if (_sensor != null)
            {
                if (_sensor.IsOpen)
                {
                    _sensor.Close();
                }
            
                _sensor = null;
            }
        }
    }
}
