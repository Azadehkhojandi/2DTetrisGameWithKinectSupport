using System;

namespace Assets.Scripts
{
    public class GestureEventArgs : EventArgs
    {
        public bool IsBodyTrackingIdValid { get; private set; }

        public bool IsGestureDetected { get; private set; }

        public float DetectionConfidence { get; private set; }
        public string GestureName { get; private set; }
        public ulong TrackingId { get; set; }

        public GestureEventArgs(string gestureName, ulong trackingId , bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence)
        {
            this.IsBodyTrackingIdValid = isBodyTrackingIdValid;
            this.IsGestureDetected = isGestureDetected;
            this.DetectionConfidence = detectionConfidence;
            this.GestureName = gestureName;
            this.TrackingId = trackingId;
        }

        
    }
}