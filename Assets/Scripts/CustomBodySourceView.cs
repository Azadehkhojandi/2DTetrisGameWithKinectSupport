using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

namespace Assets.Scripts
{
    public class CustomBodySourceView : MonoBehaviour
    {
        private Dictionary<ulong, GameObject> _bodies = new Dictionary<ulong, GameObject>();
        private KinectPlayersManager _kinectPlayersManager;
        private Dictionary<Kinect.JointType, Kinect.JointType> _boneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
        {
            { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
            { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
            { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
            { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

            { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
            { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
            { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
            { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

            { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
            { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
            { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
            { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
            { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
            { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

            { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
            { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
            { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
            { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
            { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
            { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

            { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
            { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
            { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
            { Kinect.JointType.Neck, Kinect.JointType.Head },
        };

        public Material BoneMaterial;
        public GameObject KinectPlayersManager;
        

        void Update()
        {
            if (KinectPlayersManager == null)
            {
                return;
            }

            _kinectPlayersManager = KinectPlayersManager.GetComponent<KinectPlayersManager>();
            if (_kinectPlayersManager == null)
            {
                return;
            }


            var data = _kinectPlayersManager.GetPlayers();
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

            List<ulong> knownIds = new List<ulong>(_bodies.Keys);

            // First delete untracked bodies
            foreach (ulong trackingId in knownIds)
            {
                if (!trackedIds.Contains(trackingId))
                {
                    Destroy(_bodies[trackingId]);
                    _bodies.Remove(trackingId);
                }
            }

            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    if (!_bodies.ContainsKey(body.TrackingId))
                    {
                        _bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    }

                    RefreshBodyObject(body, _bodies[body.TrackingId]);
                }
            }
        }

        private GameObject CreateBodyObject(ulong id)
        {




            GameObject body = new GameObject(id.ToString());

            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                LineRenderer lr = jointObj.AddComponent<LineRenderer>();
                lr.SetVertexCount(2);
                lr.material = BoneMaterial;
                //lr.SetWidth(0.05f, 0.05f);
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                jointObj.name = jt.ToString();
                jointObj.transform.parent = body.transform;
            }

            return body;
        }

        private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
        {
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                Kinect.Joint sourceJoint = body.Joints[jt];
                Kinect.Joint? targetJoint = null;

                if (_boneMap.ContainsKey(jt))
                {
                    targetJoint = body.Joints[_boneMap[jt]];
                }

                Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
                jointObj.localPosition = GetVector3FromJoint(sourceJoint);

                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                if (targetJoint.HasValue)
                {
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));

                    //todo Adam - Az set different colors for each player 
                    //we have tarking id of shape and motion player 
                    //we only need to change Kinect.TrackingState.Tracked to different color 
                    //_KinectPlayersManager.GetShapesPlayerTrackingId();
                    //_KinectPlayersManager.GetMotionPlayerTrackingId();
                    if (bodyObject.gameObject.name == _kinectPlayersManager.GetShapesPlayerTrackingId().ToString())
                    {
                        lr.startColor = Color.green;
                        lr.endColor = Color.green;

                    }
                    else if (bodyObject.gameObject.name == _kinectPlayersManager.GetControlsPlayerTrackingId().ToString())
                    {
                        lr.startColor = Color.blue;
                        lr.endColor = Color.blue;
                    }
                    else
                    {
                        lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                    }
                   
                }
                else
                {
                    lr.enabled = false;
                }
            }
        }

        private static Color GetColorForState(Kinect.TrackingState state)
        {
            switch (state)
            {
                case Kinect.TrackingState.Tracked:
                    return Color.green;

                case Kinect.TrackingState.Inferred:
                    return Color.red;

                default:
                    return Color.black;
            }
        }

        private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
        {
            return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        }
    }
}
