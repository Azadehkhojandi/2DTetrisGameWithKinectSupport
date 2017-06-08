using System;

namespace Assets.Scripts
{
    public class KinectPlayersEventArgs : EventArgs
    {
        public KinectPlayersEventArgs(ulong trackingId)
        {
            TrackingId = trackingId;
        }
        public ulong TrackingId { get; set; }
    }
}