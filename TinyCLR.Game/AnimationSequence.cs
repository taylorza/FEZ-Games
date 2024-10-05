// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;

namespace TinyCLR.Game
{
    /// <summary>
    /// AnimationSequence manages the animation of a sequence of
    /// images extracted from a sprite sheet.
    /// </summary>
    public class AnimationSequence
    {
        private readonly Bitmap _texture;
        private readonly Rect[] _frames;
        private int _currentFrame = 0;
        private int _frameInc = 1;
        private bool _autoReverse = true;
        private int _iterations = -1;
        private int _currentIteration = 0;
        private readonly float _animationPeriod = 1;
        private float _updatePeriod = 0;
        private readonly int _framesPerSecond = 0;
        private bool _running = false;

        /// <summary>
        /// Completed event is fired when the animation sequence completes.
        /// The event does not fire for perpetual animations
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// NewFrame event is fired when the animation transitions to a new
        /// frame. 
        /// This can be used to move sprites in step with the animation.
        /// </summary>
        public event AnimationEventHandler NewFrame;

        /// <summary>
        /// Returns true if the animation is running
        /// </summary>
        public bool IsRunning
        {
            get { return _running; }
        }

        /// <summary>
        /// Gets or Sets the currently active frame
        /// </summary>
        public int CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; }
        }

        /// <summary>
        /// Returns the number of frames in the animation
        /// </summary>
        public int FrameCount
        {
            get { return _frames.Length; }
        }

        public AnimationSequence(Bitmap texture, int framesPerSecond, params Rect[] frames) :
          this(texture, framesPerSecond, true, -1, frames)
        {
        }

        /// <summary>
        /// Creates an instance of an AnimationSequence
        /// </summary>
        /// <param name="texture">The Bitmap which contains the sprite sheet</param>
        /// <param name="framesPerSecond">Target frame rate for the animation</param>
        /// <param name="autoReverse">If true the animation sequence is run in reverse when the end is reached</param>
        /// <param name="iterations">The number of times to run the animation. -1 Runs the animation perpetually</param>
        /// <param name="frames">List of the rectangles used to extract each frame from the sprite sheet</param>
        public AnimationSequence(Bitmap texture, int framesPerSecond, bool autoReverse, int iterations, params Rect[] frames)
        {
            _texture = texture;
            _autoReverse = autoReverse;
            _iterations = iterations * (_autoReverse ? 2 : 1);
            _framesPerSecond = framesPerSecond;
            if (framesPerSecond > 0)
            {
                _animationPeriod = 1.0f / framesPerSecond;
            }
            _frames = frames;

            Completed += (s, e) => { };
            NewFrame += (a, b) => { };
        }

        /// <summary>
        /// Start running the animation
        /// </summary>
        public void Start()
        {
            _running = true;
        }

        /// <summary>
        /// Reset the animation and start running it.
        /// </summary>
        public void ResetAndStart()
        {
            Reset();
            Start();
        }

        /// <summary>
        /// Stop the animation at the current frame
        /// </summary>
        public void Stop()
        {
            _running = false;
        }

        /// <summary>
        /// Stop the current animation and reset
        /// </summary>
        public void StopAndReset()
        {
            _running = false;
            Reset();
        }

        /// <summary>
        /// Single step the animation
        /// </summary>
        public void Step()
        {
            int prevFrame = _currentFrame;
            bool complete = false;
            bool reset = false;

            _currentFrame += _frameInc;
            if (_currentFrame == -1 || _currentFrame == _frames.Length)
            {
                if (_autoReverse)
                {
                    _frameInc = -_frameInc;
                    _currentFrame += _frameInc;
                    _currentFrame += _frameInc;
                }
                else
                {
                    _currentFrame = 0;
                }

                _currentIteration++;

                if (_iterations == -1 || _currentIteration == _iterations)
                {
                    complete = true;
                    if (_currentIteration == _iterations) reset = true;
                }
            }
            OnNewFrame(prevFrame, _currentFrame);

            if (complete) OnCompleted(EventArgs.Empty);
            if (reset) StopAndReset();
        }

        /// <summary>
        /// Reset the animation to the first frame.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            _currentIteration = 0;
        }

        /// <summary>
        /// Update the animation
        /// </summary>
        /// <param name="elapsedTime">The time that has passed since the last update</param>
        public void Update(float elapsedTime)
        {
            if (_running)
            {
                _updatePeriod += elapsedTime;

                if (_frames.Length == 1) return;

                if (_updatePeriod >= _animationPeriod)
                {
                    _updatePeriod -= _animationPeriod;
                    Step();
                }
            }
        }

        /// <summary>
        /// Render the current animation frame to a Bitmap
        /// </summary>
        /// <param name="surface">Bitmap to render the frame onto</param>
        /// <param name="x">X location to render the frame</param>
        /// <param name="y">Y location to render the frame</param>
        public void Draw(Graphics surface, int x, int y)
        {
            Rect r = _frames[_currentFrame];
            surface.DrawImage(x, y, _texture, r.X, r.Y, r.Width, r.Height, 255);
        }

        /// <summary>
        /// Create a clone of the current AnimationSequence.
        /// </summary>
        /// <returns>Cloned Animation Sequence</returns>
        public AnimationSequence Clone()
        {
            var clone = new AnimationSequence(_texture, _framesPerSecond, _frames)
            {
                _autoReverse = _autoReverse,
                _iterations = _iterations
            };
            return clone;
        }

        /// <summary>
        /// Raise the Completed event if there are registerd handlers
        /// </summary>
        /// <param name="e"></param>
        protected void OnCompleted(EventArgs e)
        {
            Completed(this, e);
        }

        protected void OnNewFrame(int prevFrame, int newFrame)
        {
            NewFrame(prevFrame, newFrame);
        }
    }

    public delegate void AnimationEventHandler(int prevFrame, int newFrame);
}
