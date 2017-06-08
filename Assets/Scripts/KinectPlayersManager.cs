using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Kinect = Windows.Kinect;


namespace Assets.Scripts
{
    public class KinectPlayersManager : MonoBehaviour

    {

        private CustomBodySourceManager _bodyManager;
        private ulong _shapesPlayerTrackingId;
        private ulong _controlsPlayerTrackingId;

        public GameObject BodySourceManager;
        public event EventHandler<KinectPlayersEventArgs> OnShapesPlayerTrackingIdChanges;
        public event EventHandler<KinectPlayersEventArgs> OnControlsPlayerTrackingIdChanges;

        public KinectPlayersManager(ulong controlsPlayerTrackingId)
        {
            _controlsPlayerTrackingId = controlsPlayerTrackingId;
        }

        //retun all detected bodies
        public Kinect.Body[] GetData()
        {
            if (_bodyManager == null)
            {
                return null;
            }
            return _bodyManager.GetData();
        }

        //todo refactor later
        public Kinect.Body[] GetPlayers()
        {
            if (_bodyManager != null)
            {
                var data = _bodyManager.GetData();
                if (data == null)
                {
                    return null;
                }

                var players = new List<Kinect.Body>();
                foreach (var b in data)
                {
                    if (b.TrackingId == _shapesPlayerTrackingId)
                    {
                        players.Add(b);
                    }
                    if (b.TrackingId == _controlsPlayerTrackingId)
                    {
                        players.Add(b);
                    }
                }
                if (players.Any())
                {
                    return players.ToArray();
                }
            }
            
            return null;
        }

        public void SetShapesPlayerTrackingId(ulong trackingId)
        {
            if (_shapesPlayerTrackingId!= trackingId)
            {
                _shapesPlayerTrackingId = trackingId;
                if (OnShapesPlayerTrackingIdChanges != null)
                {
                    OnShapesPlayerTrackingIdChanges(this,new KinectPlayersEventArgs(trackingId));
                }
            }
             
        }

        public void SetControlsPlayerTrackingId(ulong trackingId)
        {
            if (_controlsPlayerTrackingId != trackingId)
            {
                _controlsPlayerTrackingId = trackingId;
                if (OnControlsPlayerTrackingIdChanges != null)
                {
                    OnControlsPlayerTrackingIdChanges(this, new KinectPlayersEventArgs(trackingId));
                }
            }
        }

        public ulong GetShapesPlayerTrackingId()
        {
            return _shapesPlayerTrackingId;
        }

        public ulong GetControlsPlayerTrackingId()
        {
            return _controlsPlayerTrackingId;
        }

        void Update()
        {
            if (BodySourceManager == null)
            {
                return;
            }

            _bodyManager = BodySourceManager.GetComponent<CustomBodySourceManager>();
            if (_bodyManager == null)
            {
                return;
            }

            Kinect.Body[] data = _bodyManager.GetData();
            if (data == null)
            {
                return;
            }

            List<ulong> trackedIds = new List<ulong>();
            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    trackedIds.Add(body.TrackingId);
                }
            }

            if (!trackedIds.Contains(_shapesPlayerTrackingId))
            {
                _shapesPlayerTrackingId = 0;
            }

            if (!trackedIds.Contains(_controlsPlayerTrackingId))
            {
                _controlsPlayerTrackingId = 0;
            }


            //if (trackedIds.Count >= 2)
            //{
            //    if (_shapesPlayerTrackingId == 0)
            //    {
            //        _shapesPlayerTrackingId = trackedIds[0];

            //    }

            //    if (_controlsPlayerTrackingId == 0)
            //    {
            //        _controlsPlayerTrackingId = trackedIds[1];
            //    }
            //} 
            //else if (trackedIds.Count == 1)
            //{
                
            //        //picks the first player to play shapes
            //        if (_shapesPlayerTrackingId == 0)
            //        {
            //            _shapesPlayerTrackingId = trackedIds[0];
            //        }
            //        _controlsPlayerTrackingId = 0;
                
               
            //}
           
        }
    }

    public class KinectPlayersEventArgs : EventArgs
    {
        public KinectPlayersEventArgs(ulong trackingId)
        {
            TrackingId = trackingId;
        }
        public ulong TrackingId { get; set; }
    }
}
