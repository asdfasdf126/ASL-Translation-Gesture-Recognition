using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System.Speech.Synthesis;

namespace SeniorProject
{
    class GestureDetector : IDisposable
    {
        #region private variables
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string gestureDatabase = @"Database\ASL.gbd";

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        private DebugUC debug;
        private MainWindow window;

        private SpeechSynthesizer synth;

        private static List<Sentence> sentences;

        private static volatile int index = 0;

        private static readonly string[] testSentence = { "Hello good morning", "Today", "How are you", "Good", "you", "Please help",
            "Who would like to try", "Goodbye" };
        #endregion

        #region construct/deconstruct

        public GestureDetector(KinectSensor kinectSensor, DebugUC debug, MainWindow window)
        {
            this.debug = debug;
            this.window = window;

            synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(VoiceGender.Female);

            if (sentences == null)
                sentences = new List<Sentence>();

            if (kinectSensor == null)
                throw new ArgumentNullException("kinectSensor");

            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if(this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            // load the gestures from the gesture database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.gestureDatabase))
                this.vgbFrameSource.AddGestures(database.AvailableGestures);

            if (debug != null)
                foreach (Gesture g in this.vgbFrameSource.Gestures)
                    debug.addGesture(g.Name);

            if(sentences.Count == 0)
                sentences.Add(new Sentence(testSentence[index], null, false));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                    this.vgbFrameSource.TrackingId = value;
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
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        #endregion

        #region events
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
                    IReadOnlyDictionary<Gesture, ContinuousGestureResult> continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                        // we only have one gesture in this source object, but you can get multiple gestures
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                            if(gesture.GestureType == GestureType.Continuous)
                            {
                                ContinuousGestureResult result = null;
                                continuousResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    if (result.Progress > .85)
                                    {
                                        Console.WriteLine("Chance of {0} gesture {1}.", gesture.Name, result.Progress);

                                        if (sentences.Count != 0)
                                        {
                                            if (!sentences[sentences.Count - 1].addWord(gesture.Name))
                                                sentences.Add(new Sentence(gesture.Name, synth));
                                        }
                                        else
                                            sentences.Add(new Sentence(gesture.Name, synth));
                                    }

                                    if (debug != null)
                                        debug.setPercent(gesture.Name, result.Progress);
                                }
                            }
                }

            window.TbWords = "";

            List<Sentence> remove = new List<Sentence>();
            sentences[0] = new Sentence(testSentence[index], null, false);

            foreach (Sentence sentence in sentences)
            {
                if(sentence.Destroy)
                {
                    remove.Add(sentence);
                    continue;
                }

                foreach(string word in sentence)
                    window.TbWords += word + " ";

                window.TbWords += "\n";
            }

            for (int x = 0; x < remove.Count; x++)
                sentences.RemoveAt(sentences.IndexOf(remove[x]));
        }

        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            Console.WriteLine("Tracking {0} lost!", e.TrackingId);
        }

        public static void incIndex()
        {
            index = (index+1) % testSentence.Length;
        }

        #endregion  
    }
}
