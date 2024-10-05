// Copyright (c) 2012 Chris Taylor

namespace TinyCLR.Game
{
    /// <summary>
    /// Manages a scene within the game.
    /// </summary>
    public class GameScene : GameObject
    {
        private readonly GameObjectContainer _gameObjects = new GameObjectContainer();

        /// <summary>
        /// Overriden in derived class to initialize and load game content.
        /// </summary>
        public override void LoadContent()
        {
        }

        /// <summary>
        /// Unload content held by the scene.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Add a GameObject to the scene.
        /// </summary>
        /// <param name="gameObject">GameObject to be added to the scene</param>
        protected void AddToScene(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
            gameObject.Owner = this;
        }

        /// <summary>
        /// Update the state of the GameObjects in the scene.
        /// If Enabled is false the GameOjects are not updated.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milliseconds since last update.</param>
        public override void Update(float elapsedTime)
        {
            if (Enabled)
            {
                int objCount = _gameObjects.Count;
                for (int i = 0; i < objCount; ++i)
                {
                    GameObject gameObject = _gameObjects[i];
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
        public override void Draw()
        {
            if (Visible)
            {
                int objCount = _gameObjects.Count;
                for (int i = 0; i < objCount; ++i)
                {
                    GameObject gameObject = _gameObjects[i];
                     gameObject.Draw();
                }
            }
        }

    }
}
