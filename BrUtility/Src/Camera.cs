using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlueRavenUtility
{
    /// <summary>
    /// The way the camera moves to the target position.
    /// Smooth: Smoothly ramps up speed, moving towards the target position until it's near enough, at which point it snaps to the target location.
    /// Fast: Same as smooth, but faster speed and no ramp up.  //NOTE BUGGY
    /// Snap: Simply snaps the camera to the target position.
    /// </summary>
    public enum CameraMoveMode
    {
        Smooth,
        Fast,
        Snap
    }

    public class Camera
    {
		public static Camera CreateInstance(Viewport viewport)
		{
			Camera cam = new Camera(viewport);
			cam.Initialize();
			return cam;
		}

		public static Camera CreateInstance(Rectangle bounds, Rectangle screenBounds)
		{
			Camera cam = new Camera(bounds, screenBounds);
			cam.Initialize();
			return cam;
		}

        internal static Camera _camera;
		[Obsolete("Create your own camera variable; singleton is deprecated.")]
        public static Camera camera { get { return _camera; } set { _camera = value; } }

		private Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } private set { bounds = value; } }

        private Rectangle screenBounds;
        public Rectangle ScreenBounds { get { return screenBounds; } private set { screenBounds = value; } }

        public Vector2 Scale { get { return new Vector2((float)Bounds.Width / (float)ScreenBounds.Width, (float)Bounds.Height / (float)ScreenBounds.Height); } }
        public float[] data;

        public CameraMoveMode defaultMoveMode;

        public Camera(Viewport viewport)
        {
			Bounds = new Rectangle(0, 0, viewport.Width, viewport.Height);
            ScreenBounds = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

		public Camera(Rectangle bounds, Rectangle screenBounds)
		{
			Bounds = new Rectangle(0, 0, bounds.Width, bounds.Height);
            ScreenBounds = new Rectangle(0, 0, screenBounds.Width, screenBounds.Height);
		}

		internal void Initialize()
		{
			Rotation = 0;
			Zoom = Vector2.One;
			Origin = new Vector2(Bounds.Width / 2f, Bounds.Height / 2f);
			Position = Vector2.Zero;

			defaultMoveMode = CameraMoveMode.Smooth;

			data = new float[4];
		}

        public CameraMoveMode moveMode;
        public CameraMoveMode tempMoveMode;
        public int moveModeDuration;

        private Vector2 _target;
        public Vector2 target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value != prevTarget)
                {
                    prevTarget = target;
                }
                _target = value;

                if (!forceNoClamp)
                {
                    _target.X = MathHelper.Clamp(_target.X, clamp.X, clamp.X + clamp.Width);
                    _target.Y = MathHelper.Clamp(_target.Y, clamp.Y, clamp.Y + clamp.Height);
                }
            }
        }

        private Vector2 prevTarget;
        public Vector2 velocity;

        public float oldCameraRotation;

        private Vector2 _Position;
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (value != prevPosition)
                {
                    prevPosition = _Position;
                    hasMoved = true;
                }
                _Position = value;

                if (!forceNoClamp)
                {
                    _Position.X = MathHelper.Clamp(_Position.X, clamp.X, clamp.X + clamp.Width);
                    _Position.Y = MathHelper.Clamp(_Position.Y, clamp.Y, clamp.Y + clamp.Height);
                }
            }
        }
        public Vector2 prevPosition { get; set; }
        public float Rotation { get; set; }
        public Vector2 Zoom;
        public Vector2 Origin { get; set; }

        public Rectangle clamp;
        public bool forceNoClamp;

        public bool moving { get { return Position != prevPosition; } set { } }

        public Vector2 center { get { return new Vector2(Bounds.Width / 2, Bounds.Height / 2); } set { } }
        public Vector2 worldCenter { get { return Origin + Position; } set { Position = value - Origin; } }

        public bool hasMoved;

        #region directions
        public Vector2 screenUp
        {
            get
            {
                Vector2 finalvec = new Vector2(0, -1);
                Vector2 f = Vector2.Transform(finalvec, Matrix.CreateRotationZ(-Rotation));
                f.Normalize();
                return f;
            }
            set { }
        }

        public Vector2 screenDown
        {
            get
            {
                Vector2 finalvec = new Vector2(0, 1);
                Vector2 f = Vector2.Transform(finalvec, Matrix.CreateRotationZ(-Rotation));
                f.Normalize();
                return f;
            }
            set { }
        }

        public Vector2 screenLeft
        {
            get
            {
                Vector2 finalvec = new Vector2(-1, 0);
                Vector2 f = Vector2.Transform(finalvec, Matrix.CreateRotationZ(-Rotation));
                f.Normalize();
                return f;
            }
            set { }
        }

        public Vector2 screenRight
        {
            get
            {
                Vector2 finalvec = new Vector2(1, 0);
                Vector2 f = Vector2.Transform(finalvec, Matrix.CreateRotationZ(-Rotation));
                f.Normalize();
                return f;
            }
            set { }
        }

        public Vector2 down
        {
            get
            {
                return new Vector2(0, 1);
            }
            set { }
        }
        public Vector2 right
        {
            get
            {
                return new Vector2(1, 0);
            }
            set { }
        }
        public Vector2 up
        {
            get
            {
                return new Vector2(0, -1);
            }
            set { }
        }
        public Vector2 left
        {
            get
            {
                return new Vector2(-1, 0);
            }
            set { }
        }
        #endregion

		public void SetBounds(Rectangle bounds, Rectangle screenBounds)
		{
			Bounds = new Rectangle(0, 0, bounds.Width, bounds.Height);
            ScreenBounds = new Rectangle(0, 0, screenBounds.Width, screenBounds.Height);

			Origin = new Vector2(Bounds.Width / 2f, Bounds.Height / 2f);
		}

		public void AddRot(float amt)
        {
            Rotation = MathHelper.ToRadians(BindTo360((MathHelper.ToDegrees(Rotation) + amt)));
        }

        private float BindTo360(float val)
        {
            return EngineMathHelper.Mod(val, 360);
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom.X, Zoom.Y, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        #region setters
        public bool fading { get; private set; }
        private bool colorToTransparent;
        private float fadeTimer, fadeTimerMax;
        private Color currentColor, fadeColor;
        private bool fadeStart = false;
        /// <summary>
        /// Fades from transparent to a color or vice versa.
        /// </summary>
        /// <param name="color">The color to fade to or from.</param>
        /// <param name="colorToTransparent">fade to color (true) or from color (false).</param>
        public void SetFade(Color color, bool colorToTransparent, int time)
        {
            fadeStart = false;
            fadeColor = color;
            this.colorToTransparent = colorToTransparent;
            fadeTimer = time;
            fadeTimerMax = time;

            fading = true;
        }

        private bool tinting;
        private int tintTimer;
        private Color tintColor;
        public void SetTint(Color color, int time)
        {
            this.tintColor = color;
            this.tintTimer = time;

            this.tinting = true;
        }

        private bool quaking;
        private float quakeRange, quakeDuration;
        public void SetQuake(float range, int duration)
        {
            quakeRange = range;
            quakeDuration = duration;

            quaking = true;
        }

        public void SetMoveMode(CameraMoveMode mode, int duration)
        {
            if (moveModeDuration < 0)  //not already on another temp move modes
                tempMoveMode = moveMode;
            moveMode = mode;
            moveModeDuration = duration;
        }
        #endregion

        public void Update()
        {
            hasMoved = false;
            
            if (moveModeDuration > 0)
            {
                moveModeDuration--;
            }
            else
            {
                moveMode = tempMoveMode;
            }

            if (fading)
            {
                if (fadeTimer > 0)
                {
                    fadeTimer--;

                    float step = fadeTimer / fadeTimerMax;
                    if (colorToTransparent)
                        currentColor = Color.Lerp(Color.Transparent, fadeColor, step);
                    else
                        currentColor = Color.Lerp(fadeColor, Color.Transparent, step);
                    fadeStart = true;
                }

                if (fadeTimer <= 0)
                {
                    fading = false;
                    fadeStart = false;
                }
            }

            if (tinting)
            {
                if (tintTimer > 0)
                    tintTimer--;

                if (tintTimer <= 0)
                    tinting = false;
            }

            if (moveMode == CameraMoveMode.Snap)
                worldCenter = target;
            else if (moveMode == CameraMoveMode.Fast)
            {
                Vector2 dist = target - worldCenter;

                if (dist != Vector2.Zero)
                    dist = Vector2.Normalize(dist);
                else dist = Vector2.Zero;

                worldCenter += dist * 4;
            }
            else if (moveMode == CameraMoveMode.Smooth)
                worldCenter = Vector2.Lerp(worldCenter, target, .30f);
        }

        public void PostUpdate(Random rand)
        {
            if (quaking)
            {
                if (quakeDuration > 0)
                {
                    quakeDuration--;

                    Position += new Vector2((float)rand.NextDouble(quakeRange, -quakeRange), (float)rand.NextDouble(quakeRange, -quakeRange));
                }
                else quaking = false;
            }

            if (clamp != Rectangle.Empty && !forceNoClamp)
            {
                Position = Vector2.Clamp(Position, Vector2.Zero, new Vector2(clamp.Width - Bounds.Width, clamp.Height - Bounds.Height));
            }

            prevPosition = Position;
            prevTarget = target;
        }

        public void Draw(SpriteBatch batch)
        {
            if (fading && fadeStart)
                batch.DrawRectangle(new Rectangle(0, 0, Bounds.Width, Bounds.Height), currentColor);

            if (tinting)
                batch.DrawRectangle(new Rectangle(0, 0, Bounds.Width, Bounds.Height), tintColor);
        }
    }
}
