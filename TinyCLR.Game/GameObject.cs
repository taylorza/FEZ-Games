// Copyright (c) 2012 Chris Taylor

using System.Drawing;

namespace TinyCLR.Game
{
    /// <summary>
    /// Abstract base class for all objects that can be added to a GameManager scene.
    /// </summary>
    public abstract class GameObject
    {
        private GameObjectContainer _childObjects;
        private GameObject _owner;
        private float _x;
        private float _y;

        private FuncIntDelegate GetWorldX;
        private FuncIntDelegate GetWorldY;
        private ActionFloatDelegate UpdateHandler;
        private ActionDelegate DrawHandler;

        /// <summary>
        /// Default constructor for GameObject.
        /// </summary>
        public GameObject()
        {
            Enabled = true;
            Visible = true;
            GetWorldX = GetWorldXNoOwner;
            GetWorldY = GetWorldYNoOwner;
            UpdateHandler = UpdateNoChildObjects;
            DrawHandler = DrawNoChildObjects;
        }

        protected Graphics Surface { get { return GameManager.Game.Surface; } }

        /// <summary>
        /// Set the owner of the GameObject.
        /// </summary>
        public GameObject Owner
        {
            get { return _owner; }
            set
            {
                if (value != _owner)
                {
                    SetOwner(value);
                }
            }
        }

        /// <summary>
        /// Controls if the object is enabled or not. 
        /// If not enabled the object is not updated, but is still rendered to the scene
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Controls if the GameObject is Visible or not.
        /// If not visible the GameObject is still updated if Enabled.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// X Location of the GameObject relative to it's owner.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y Location of the GameObject relative to it's owner.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// X Location of the GameObject in the game world.
        /// </summary>
        protected int WorldX
        {
            get { return GetWorldX(); }
        }

        /// <summary>
        /// Y Location of the GameObject in the game world.
        /// </summary>
        protected int WorldY
        {
            get { return GetWorldY(); }
        }

        protected float ChildOffsetX;
        protected float ChildOffsetY;

        /// <summary>
        /// Move the game object relative to the current location.
        /// </summary>
        /// <param name="dx">Units to move in the X direction</param>
        /// <param name="dy">Units to move in the Y direction</param>
        public void Move(float dx, float dy)
        {
            _x += dx;
            _y += dy;

            X = (int)_x;
            Y = (int)_y;
        }

        /// <summary>
        /// Move the game object to the specified absolute location.
        /// </summary>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        public void MoveTo(float x, float y)
        {
            _x = x;
            _y = y;

            X = (int)_x;
            Y = (int)_y;
        }

        /// <summary>
        /// Overriden in derived class to initialize and load game content.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Update the state of the GameObject.
        /// If Enabled is false Update is not called.
        /// Must be overriden in derived class.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milliseconds since last update.</param>
        public virtual void Update(float elapsedTime)
        {
            UpdateHandler(elapsedTime);
        }

        /// <summary>
        /// Render the GameObject to the surface Bitmap
        /// </summary>
        /// <param name="surface">Bitmap on which the GameObject will be rendered</param>
        public virtual void Draw()
        {
            DrawHandler();
        }

        private void UpdateNoChildObjects(float elapsedTime)
        {
        }

        private void UpdateWithChildObjects(float elapsedTime)
        {
            var childObjects = _owner._childObjects;
            int childCount = childObjects.Count;
            for (int i = 0; i < childCount; i++)
            {
                GameObject o = childObjects[i];
                if (o.Enabled)
                {
                    o.Update(elapsedTime);
                }
            }
        }

        private void DrawNoChildObjects()
        {
        }

        private void DrawWithChildObjects()
        {
            var childObjects = _owner._childObjects;
            int childCount = childObjects.Count;
            for (int i = 0; i < childCount; i++)
            {
                GameObject o = childObjects[i];
                if (o.Visible)
                {
                    o.Draw();
                }
            }
        }

        private int GetWorldXFromOwner()
        {
            return _owner.WorldX + (int)_owner.ChildOffsetX + X;
        }

        private int GetWorldYFromOwner()
        {
            return _owner.WorldY + (int)_owner.ChildOffsetY + Y;
        }

        private int GetWorldXNoOwner()
        {
            return X;
        }

        private int GetWorldYNoOwner()
        {
            return Y;
        }

        private void SetOwner(GameObject obj)
        {
            if (_owner != null)
            {
                _owner._childObjects.Remove(this);
                if (_owner._childObjects.Count == 0)
                {
                    _owner.UpdateHandler = UpdateNoChildObjects;
                    _owner.DrawHandler = DrawNoChildObjects;
                }
            }
            _owner = obj;

            if (obj == null)
            {
                GetWorldX = GetWorldXNoOwner;
                GetWorldY = GetWorldYNoOwner;
            }
            else
            {
                if (_owner._childObjects == null) _owner._childObjects = new GameObjectContainer();
                _owner._childObjects.Add(this);
                GetWorldX = GetWorldXFromOwner;
                GetWorldY = GetWorldYFromOwner;
                if (_owner._childObjects.Count == 1)
                {
                    _owner.UpdateHandler = UpdateWithChildObjects;
                    _owner.DrawHandler = DrawWithChildObjects;
                }
            }
        }

        private delegate int FuncIntDelegate();
        private delegate void ActionFloatDelegate(float f1);
        private delegate void ActionDelegate();
    }
}
