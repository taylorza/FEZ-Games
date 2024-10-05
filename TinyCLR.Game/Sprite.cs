// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;

namespace TinyCLR.Game
{
    /// <summary>
    /// Sprite GameObject manages 1 or more Animation sequences. 
    /// </summary>
    public class Sprite : GameObject
    {
        private AnimationSequence[] _animationSequences;
        private int _currentAnimation;

        public Sprite Clone()
        {
            AnimationSequence[] animations = new AnimationSequence[_animationSequences.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i] = _animationSequences[i].Clone();
            }

            return new Sprite(animations);
        }

        /// <summary>
        /// Event fired when the sprite transitions to a new frame.
        /// </summary>
        public event SpriteAnimationEventHandler NewFrame;

        protected void SetAnimationSequences(bool running, AnimationSequence[] animationSequences)
        {
            if (animationSequences == null) throw new ArgumentNullException("animationSequences");

            if (_animationSequences != null)
            {
                foreach (var animationSequence in animationSequences)
                {
                    animationSequence.NewFrame -= AnimationSequence_NewFrame;
                }
            }

            _animationSequences = animationSequences;

            foreach (var animationSequence in animationSequences)
            {
                if (running) animationSequence.ResetAndStart();
                animationSequence.NewFrame += AnimationSequence_NewFrame;
            }
        }

        /// <summary>
        /// Gets or Sets the currently active animation.
        /// </summary>
        public int CurrentAnimation
        {
            get { return _currentAnimation; }
            set { _currentAnimation = value; }
        }

        /// <summary>
        /// Gets or Sets the currently active frame in the animation.
        /// </summary>
        public int CurrentFrame
        {
            get { return _animationSequences[_currentAnimation].CurrentFrame; }
            set { _animationSequences[_currentAnimation].CurrentFrame = value; }
        }

        /// <summary>
        /// Gets the state of the current animation sequence.
        /// </summary>
        public bool IsAnimationRunning { get { return _animationSequences[CurrentAnimation].IsRunning; } }

        /// <summary>
        /// Creates a static sprite from a bitmap.
        /// </summary>
        /// <param name="texture">Texture of the sprite.</param>
        public Sprite(Bitmap texture) :
          this(false, new AnimationSequence(texture, 0, new Rect[] { new Rect(0, 0, texture.Width, texture.Height) }))
        {
        }

        /// <summary>
        /// Creates a static sprite from a region of a bitmap.
        /// </summary>
        /// <param name="texture">Texture of the sprite.</param>
        /// <param name="region">Rectangular region of the texture to use as the sprite.</param>
        public Sprite(Bitmap texture, Rect region) :
          this(false, new AnimationSequence(texture, 0, new Rect[] { region }))
        {
        }

        /// <summary>
        /// Create a Sprite instance with the set of animation sequences. 
        /// </summary>
        /// <param name="animationSequences">Array of animation sequences associated with the sprite.</param>
        public Sprite(params AnimationSequence[] animationSequences) : this(true, animationSequences)
        {
        }

        /// <summary>
        /// Create a Sprite instance with the set of animation sequences. 
        /// </summary>
        /// <param name="startRunning">Specifies if the animations are currently running.</param>
        /// <param name="animationSequences">Array of animation sequences associated with the sprite.</param>
        public Sprite(bool startRunning, params AnimationSequence[] animationSequences)
        {
            if (animationSequences == null) throw new ArgumentNullException("animationSequences");
            SetAnimationSequences(startRunning, animationSequences);

            NewFrame += (a, b, c) => { };
        }

        /// <summary>
        /// Start running the animation.
        /// </summary>
        public void StartAnimation()
        {
            _animationSequences[CurrentAnimation].Start();
        }

        /// <summary>
        /// Reset the animation and start running it.
        /// </summary>
        public void ResetAndStartAnimation()
        {
            _animationSequences[CurrentAnimation].ResetAndStart();
        }

        /// <summary>
        /// Stop the animation at the current frame.
        /// </summary>
        public void StopAnimation()
        {
            _animationSequences[CurrentAnimation].Stop();
        }

        /// <summary>
        /// Stop the current animation and reset.
        /// </summary>
        public void StopAndResetAnimation()
        {
            _animationSequences[CurrentAnimation].StopAndReset();
        }

        /// <summary>
        /// Single step the animation.
        /// </summary>
        public void StepAnimation()
        {
            _animationSequences[CurrentAnimation].Step();
        }

        /// <summary>
        /// Reset the animation to the first frame.
        /// </summary>
        public void ResetAnimation()
        {
            _animationSequences[CurrentAnimation].Reset();
        }

        /// <summary>
        /// Update the state of the sprite.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time since the last update.</param>
        public override void Update(float elapsedTime)
        {
            if (_animationSequences[CurrentAnimation].FrameCount > 1)
            {
                _animationSequences[CurrentAnimation].Update(elapsedTime);
            }
            base.Update(elapsedTime);
        }

        /// <summary>
        /// Render the sprite to the surface.
        /// </summary>
        /// <param name="surface">Surface bitmap to which the sprite is rendered.</param>
        public override void Draw()
        {
            _animationSequences[CurrentAnimation].Draw(Surface, WorldX, WorldY);
            base.Draw();
        }

        /// <summary>
        /// Retrieve the animation sequence at the specified index.
        /// </summary>
        /// <param name="index">Index to the animation sequence.</param>
        /// <returns></returns>
        public AnimationSequence this[int index]
        {
            get { return _animationSequences[index]; }
        }

        protected virtual void OnNewFrame(int prevFrame, int newFrame)
        {
            NewFrame(this, prevFrame, newFrame);
        }

        void AnimationSequence_NewFrame(int prevFrame, int newFrame)
        {
            OnNewFrame(prevFrame, newFrame);
        }
    }

    public delegate void SpriteAnimationEventHandler(Sprite sender, int prevFrame, int newFrame);
}
