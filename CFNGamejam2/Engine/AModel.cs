#region Using
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
#endregion

namespace Engine
{
    public class AModel : PositionedObject, IDrawComponent, ILoadContent
    {
        public Vector3 DefuseColor = new Vector3(1,1,1);
        Texture2D XNATexture;
        //Matrix[] ModelTransforms;
        Matrix BaseWorld;
        public Vector3 ModelScale = new Vector3(1);
        public Vector3 ModelScaleVelocity = Vector3.Zero;
        public Vector3 ModelScaleAcceleration = Vector3.Zero;
        bool m_AnimatedScale = true;

        public XnaModel XNAModel { get; private set; }
        public Matrix TheWoldMatrix { get => BaseWorld; }
        public BoundingSphere Sphere { get => XNAModel.Meshes[0].BoundingSphere; }
        public bool AnimatedScale { get => m_AnimatedScale; set => m_AnimatedScale = value; }
        public bool Visable { get => Enabled; set => Enabled = value; }

        public AModel (Game game) : base(game)
        {
        }

        public AModel(Game game, XnaModel model) : base(game)
        {
            SetModel(model);
        }

        public AModel(Game game, XnaModel model, Texture2D texture) : base(game)
        {
            SetModel(model, texture);
        }

        public override void Initialize()
        {
            Enabled = true;
            Services.AddDrawableComponent(this);
            Services.AddLoadable(this);

            Services.GraphicsDM.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Services.GraphicsDM.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            Services.GraphicsDM.GraphicsDevice.BlendState = BlendState.Opaque;
            Services.GraphicsDM.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            BaseWorld = Matrix.Identity;

            base.Initialize();
        }

        public override void BeginRun()
        {
            LoadContent();
            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            /* A rule of thumb is ISROT - Identity, Scale, Rotate, Orbit, Translate.
                This is the order to multiple your matrices in.
                So for the moon and earth example, to place the moon:
                Identity - this is just Matrix.Identity (an all 1's matrix).
                Scale - Scale the moon to it's proper size.
                Rotate - rotate the moon around it's own center
                Orbit - this is a two step Translate then Rotate process,
                        first Translate (move) the moon to it's position relative to the
                        earth (i.e. if the earth was at 0, 0, 0).  The rotate the moon around
                        this point to position it in orbit.
                Translate - move the moon to the final location, which will be the same
                        as the location of earth in this case since it's already setup to be in orbit.*/

            //Calculate the base transformation by combining
            //translation, rotation, and scaling
            //BaseWorld = Matrix.CreateScale(ModelScale)
            //    * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
            //    * Matrix.CreateTranslation(Position);

            //if (Child)
            //{
            //    BaseWorld *= Matrix.CreateFromYawPitchRoll(ParentPO.Rotation.Y + ParentPO.ParentRotation.Y,
            //        ParentPO.Rotation.X + ParentPO.ParentRotation.X,
            //        ParentPO.Rotation.Z + ParentPO.ParentRotation.Z)
            //        * Matrix.CreateTranslation(ParentPO.Position + ParentPO.ParentPosition);
            //}

            MatrixUpdate();

            if (m_AnimatedScale)
            {
                float eGT = (float)gameTime.ElapsedGameTime.TotalSeconds;

                ModelScaleVelocity += ModelScaleAcceleration * eGT;
                ModelScale += ModelScaleVelocity * eGT;
            }
        }

        public virtual void Draw()
        {
            if (Visable)
            {
                if (XNAModel == null)
                    return;

                foreach (ModelMesh mesh in XNAModel.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        BasicEffect effect = (BasicEffect)meshPart.Effect;

                        //effect.Texture = XNATexture ?? effect.Texture; //Replace texture if XNATexture is not null.
                        effect.EnableDefaultLighting();

                        if (DefuseColor != Vector3.Zero)
                            effect.DiffuseColor = DefuseColor;

                        //effect.PreferPerPixelLighting = true;
                        effect.World = BaseWorld;

                        //if (XNATexture != null)
                        //effect.Texture = XNATexture;// ?? effect.Texture; //Replace texture if XNATexture is not null.

                        Services.Camera.Draw(effect);
                    }

                    mesh.Draw();
                }
            }
        }

        public void MatrixUpdate()
        {
            BaseWorld = Matrix.CreateScale(ModelScale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(Position);

            if (Child) //TODO: Make recursive method. Only works two deep as is.
            {
                BaseWorld *= Matrix.CreateFromYawPitchRoll(ParentPO.Rotation.Y + ParentPO.ParentRotation.Y,
                    ParentPO.Rotation.X + ParentPO.ParentRotation.X,
                    ParentPO.Rotation.Z + ParentPO.ParentRotation.Z)
                    * Matrix.CreateTranslation(ParentPO.Position + ParentPO.ParentPosition);
            }
        }
        /// <summary>
        /// If position, rotation and velocity are used.
        /// </summary>
        /// <param name="position">Position to spawn at.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        /// <param name="velocity">Initial Velocity to spawn with.</param>
        public virtual void Spawn(Vector3 position, Vector3 rotation, Vector3 velocity)
        {
            Velocity = velocity;
            Spawn(position, rotation);
        }
        /// <summary>
        /// If only position and rotation are used.
        /// </summary>
        /// <param name="position">Position to spawn at.</param>
        /// <param name="rotation">Rotation to spawn at.</param>
        public virtual void Spawn(Vector3 position, Vector3 rotation)
        {
            Rotation = rotation;
            Spawn(position);
        }
        /// <summary>
        /// If only position is used.
        /// </summary>
        /// <param name="position">Position to spawn at.</param>
        public virtual void Spawn(Vector3 position)
        {
            Active = true;
            Hit = false;
            Position = position;
            MatrixUpdate();
        }
        /// <summary>
        /// Sphere collusion detection. Target sphere will be compared to this class's.
        /// Will return true of they intersect on the X and Y plane.
        /// The Z plane is ignored.
        /// </summary>
        /// <param name="target">Target Positioned Object.</param>
        /// <returns></returns>
        public bool SphereIntersect2DZ(AModel target)
        {
            float distanceX = target.Position.X - Position.X;
            float distanceY = target.Position.Y - Position.Y;
            float radius = ((Sphere.Radius * Scale) * 0.8f) +
                ((target.Sphere.Radius * target.Scale) * 0.8f);

            if ((distanceX * distanceX) + (distanceY * distanceY) < radius)
                return true;

            return false;
        }

        /// <summary>
        /// Sphere collusion detection. Target sphere will be compared to this class's.
        /// </summary>
        /// <param name="target">Target Positioned Object.</param>
        /// <returns></returns>
        public bool SphereIntersect(AModel target)
        {
            float radius = ((Sphere.Radius * Scale) * 0.8f) +
                ((target.Sphere.Radius * target.Scale) * 0.8f);

            if (Vector3.Distance(Position, target.Position) < radius)
                return true;

            return false;
        }

        public void SetModel(XnaModel model)
        {
            if (model != null)
            {
                XNAModel = model;

                //ModelTransforms = new Matrix[XNAModel.Bones.Count];
                //XNAModel.CopyAbsoluteBoneTransformsTo(ModelTransforms);
            }
        }

        public void SetModel(XnaModel model, Texture2D texture)
        {
            XNATexture = texture;

            SetModel(model);
        }

        public void LoadModel(string modelName)
        {
            if (modelName != null)
            {
                if (File.Exists("Content/Models/" + modelName + ".xnb"))
                    XNAModel = Game.Content.Load<XnaModel>("Models/" + modelName);
            }

            SetModel(XNAModel);
        }

        public void LoadModel(string modelName, string textureName)
        {
            if (textureName != null)
                XNATexture = Game.Content.Load<Texture2D>("Textures/" + textureName);

            LoadModel(modelName);
        }

        public XnaModel Load(string modelName)
        {
            return Game.Content.Load<XnaModel>("Models/" + modelName);
        }

        public SoundEffect LoadSoundEffect(string soundName)
        {
            return Game.Content.Load<SoundEffect>("Sounds/" + soundName);
        }

        public virtual void LoadContent()
        {
            //This method intentionally left blank.
        }

        public void Destroy()
        {
            //if (ModelTransforms != null)
            //    ModelTransforms.Initialize();

            BaseWorld = Matrix.Identity;
            XNAModel = null;
            XNATexture = null;

            Dispose();
        }
    }
}
