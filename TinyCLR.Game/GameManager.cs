// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;
using System.Threading;
using TinyCLR.Game.Input;

namespace TinyCLR.Game
{
    /// <summary>
    /// GameManager provides the overall game and scene management functionality.
    /// Developers should derive a specialized class for 
    /// </summary>
    public class GameManager
    {
        private static readonly TimerContainer _timers = new TimerContainer();

        private readonly GameObjectContainer _scenes = new GameObjectContainer();

        private readonly Thread _gameLoopThread;
        private long _lastTicks;
        private int _targetFrameRate = 30;
        private float _updateInterval = 0;
        private float _updateTimeRemaining = 0;
        private float _maxUpdateTime = 0;
        private GameScene _currentScene;

        /// <summary>
        /// Drawing surface used to render the frame
        /// </summary>
        public Graphics Surface { get; private set; }

        /// <summary>
        /// Gets or Sets the target frame rate for the game.
        /// Only applicable if FixedFrameRate is true.
        /// </summary>
        public int TargetFrameRate
        {
            get { return _targetFrameRate; }
            set
            {
                _targetFrameRate = value;
                UpdateFrameCounters();
            }
        }

        /// <summary>
        /// Game loop runs Updates at a fixed frame rate as
        /// determined by the TargetFrameRate.
        /// </summary>
        public bool FixedFrameRate { get; set; }

        /// <summary>
        /// Manages the input devices that can be used to privide input
        /// to the game
        /// </summary>
        public InputManager InputManager { get; private set; }

        /// <summary>
        /// State of the Manager. If Enabled is false the Updates are nor executed
        /// but all drawing still takes place.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Provides global access to the GameManager instance
        /// </summary>
        public static GameManager Game { get; private set; }

        /// <summary>
        /// Construct an instance of the GameManager
        /// </summary>
        /// <param name="surface">Bitmap that the GameManager will render the scene to</param>
        public GameManager(Graphics surface)
        {
            Surface = surface ?? throw new ArgumentNullException("surface");
            Enabled = true;
            InputManager = new Input.InputManager();
            Game = this;
            UpdateFrameCounters();
            _gameLoopThread = new Thread(GameLoop);
        }

        /// <summary>
        /// Initialize the GameManager.
        /// This must be called to initialize and start the GameManager.
        /// </summary>    
        public void Initialize()
        {
            LoadContent();
            _lastTicks = DateTime.Now.Ticks;
            _gameLoopThread.Start();
        }

        /// <summary>
        /// Bring scene into view
        /// </summary>
        /// <param name="scene">Scene to show</param>
        public void ShowScene(GameScene scene)
        {
            if (_currentScene != null)
            {
                _currentScene.Enabled = false;
                _currentScene.Visible = false;
                _currentScene.Unload();
            }
            _currentScene = scene;
            _currentScene.LoadContent();
            _currentScene.Enabled = true;
            _currentScene.Visible = true;
        }

        /// <summary>
        /// Add a scene to the Game Manager.
        /// </summary>
        /// <param name="gameObject">GameObject to be added to the scene</param>
        protected void AddScene(GameScene scene)
        {
            scene.Enabled = false;
            scene.Visible = false;
            _scenes.Add(scene);
        }

        internal static void RegisterTimer(CountDownTimer timer)
        {
            _timers.Add(timer);
        }

        internal static void UnRegisterTimer(CountDownTimer timer)
        {
            _timers.Remove(timer);
        }

        /// <summary>
        /// Overriden in derived class to initialize and load game content.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Update the state of the GameObjects in the scene.
        /// If Enabled is false the GameOjects are not updated.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milliseconds since last update.</param>
        protected virtual void Update(float elapsedTime)
        {
            InputManager.Update(elapsedTime);
            if (Enabled)
            {
                _timers.Update(elapsedTime);
                int sceneCount = _scenes.Count;
                for (int i = 0; i < sceneCount; ++i)
                {
                    GameObject gameObject = _scenes[i];
                    if (gameObject.Enabled)
                    {
                        gameObject.Update(elapsedTime);
                    }
                }
            }
        }

        /// <summary>
        /// Render all the GameObjects in the scene to the Surface Bitmap
        /// </summary>    
        protected virtual void Draw()
        {
            int sceneCount = _scenes.Count;
            for (int i = 0; i < sceneCount; ++i)
            {
                GameScene scene = (GameScene)_scenes[i];
                if (scene.Visible)
                {
                    scene.Draw();
                }
            }
        }

        private void UpdateFrameCounters()
        {
            _updateInterval = 1.0f / _targetFrameRate;
            _maxUpdateTime = 3 * _updateInterval;
        }

        private void GameLoop()
        {
            while (true)
            {
                long now = DateTime.Now.Ticks;
                float elapsedTime = (float)(now - _lastTicks) / TimeSpan.TicksPerSecond;
                _lastTicks = now;

                if (FixedFrameRate)
                {
                    float updateTime = elapsedTime + _updateTimeRemaining;
                    if (updateTime > _maxUpdateTime)
                    {
                        updateTime = _maxUpdateTime;
                    }
                    while (updateTime >= _updateInterval)
                    {
                        updateTime -= _updateInterval;
                        Update(_updateInterval);
                    }
                    _updateTimeRemaining = updateTime;
                }
                else
                {
                    Update(elapsedTime);
                }

                Draw();

                Surface.Flush();
            }
        }
    }
}
