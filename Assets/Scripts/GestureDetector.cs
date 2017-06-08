﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using UnityEngine;


//http://peted.azurewebsites.net/kinect-4-windows-v2-custom-gestures-in-unity/
//https://github.com/carmines/workshop/blob/master/Unity/VGBSample.unitypackage

namespace Assets.Scripts
{
    /// <summary>
    /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
    /// and calls the OnGestureDetected event handler when a gesture is detected.
    /// </summary>
    public class GestureDetector : IDisposable
    {
        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource _vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader _vgbFrameReader = null;

        public event EventHandler<GestureEventArgs> OnGestureDetected;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureDatabasePath"></param>
        public GestureDetector(KinectSensor kinectSensor, string gestureDatabasePath)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this._vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);
            this._vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            this._vgbFrameReader = this._vgbFrameSource.OpenReader();
            if (this._vgbFrameReader != null)
            {
                this._vgbFrameReader.IsPaused = true;
                this._vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }


            // load the 'Seated' gesture from the gesture database
            //var databasePath = Application.dataPath + this._gestureDatabase;
            using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(gestureDatabasePath))
            {
                // we could load all available gestures in the database with a call to vgbFrameSource.AddGestures(database.AvailableGestures), 
                // but for this program, we only want to track one discrete gesture from the database, so we'll load it by name
                foreach (Gesture gesture in database.AvailableGestures)
                {
                    if (gesture!=null && !string.IsNullOrEmpty(gesture.Name))
                    {
                        this._vgbFrameSource.AddGesture(gesture);
                    }
                }
            }


        }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this._vgbFrameSource.TrackingId;
            }

            set
            {
                if (this._vgbFrameSource.TrackingId != value)
                {
                    this._vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this._vgbFrameReader.IsPaused;
            }

            set
            {
                if (this._vgbFrameReader.IsPaused != value)
                {
                    this._vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._vgbFrameReader != null)
                {
                    this._vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this._vgbFrameReader.Dispose();
                    this._vgbFrameReader = null;
                }

                if (this._vgbFrameSource != null)
                {
                    this._vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this._vgbFrameSource.Dispose();
                    this._vgbFrameSource = null;
                }
            }
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    var discreteResults = frame.DiscreteGestureResults;

                    if (discreteResults != null)
                    {

                        var discreteGestureResults = new Dictionary<string, DiscreteGestureResult> ();
                        foreach (Gesture gesture in this._vgbFrameSource.Gestures)
                        {
                        
                            //if (gesture.Name.Equals(this.seatedGestureName) && gesture.GestureType == GestureType.Discrete)
                            //{
                            DiscreteGestureResult result = null;
                            discreteResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                discreteGestureResults.Add(gesture.Name,result);
                               
                            }
                            //}
                        }

                        if (this.OnGestureDetected != null && discreteGestureResults.Any())
                        {
                            var bestPossibleresult = discreteGestureResults.Where(x=>x.Value.Detected).OrderByDescending(x=>x.Value.Confidence).FirstOrDefault();
                            if (bestPossibleresult.Value != null)
                            {
                                this.OnGestureDetected(this, new GestureEventArgs(bestPossibleresult.Key,TrackingId,true, bestPossibleresult.Value.Detected, bestPossibleresult.Value.Confidence));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            if (this.OnGestureDetected != null)
            {
                this.OnGestureDetected(this, new GestureEventArgs("",0,false, false, 0.0f));
            }
        }
    }
}