﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;

namespace Engine
{
    public class Plane : PositionedObject, IDrawComponent
    {
        VertexPositionTexture[] Verts = new VertexPositionTexture[6];
        VertexBuffer PlaneVertexBuffer;
        Texture2D XNATexture;
        Matrix BaseWorld;
        BasicEffect PlaneBasicEffect;

        public float Width;
        public float Height;

        public Plane(Game game) : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            PlaneBasicEffect = new BasicEffect(Engine.Core.GraphicsDM.GraphicsDevice);
            Engine.Core.AddDrawableComponent(this);
        }

        public override void BeginRun()
        {
            base.BeginRun();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BaseWorld = Matrix.Identity;

            BaseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z + -MathHelper.PiOver2)
                * Matrix.CreateTranslation(ParentPO.Position)
                * Matrix.CreateFromYawPitchRoll(ParentPO.Rotation.Y, ParentPO.Rotation.X, ParentPO.Rotation.Z)
                * Matrix.CreateTranslation(Position);

            // Set object and camera info
            PlaneBasicEffect.World = BaseWorld;
            PlaneBasicEffect.View = Engine.Core.DefaultCamera.View;
            PlaneBasicEffect.Projection = Engine.Core.DefaultCamera.Projection;
            Engine.Core.GraphicsDM.GraphicsDevice.SetVertexBuffer(PlaneVertexBuffer);
        }

        public void Draw()
        {
            // Begin effect and draw for each frame
            foreach (EffectPass pass in PlaneBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Engine.Core.GraphicsDM.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Verts, 0, 2);
            }
        }

        public void Create(Texture2D texture)
        {
            PlaneVertexBuffer = new VertexBuffer(Engine.Core.GraphicsDM.GraphicsDevice, typeof(VertexPositionTexture),
                Verts.Length, BufferUsage.None);

            ChangePlaneTexture(texture);
        }

        public void ChangePlaneTexture(Texture2D texture)
        {
            XNATexture = texture;
            PlaneBasicEffect.Texture = texture;
            PlaneBasicEffect.TextureEnabled = true;

            if (texture != null)
                ChangePlaneSize(texture.Width, texture.Height);
        }

        public void ChangePlaneSize(float Width, float Height)
        {
            SetupPlane(Width, Height);
        }

        public Texture2D Load(string textureName)
        {
            return Game.Content.Load<Texture2D>("Textures/" + textureName);
        }

        void SetupPlane(float width, float height)
        {
            // Setup plane
            Width = width;
            Height = height;

            Verts[0] = new VertexPositionTexture(new Vector3(-width / 3f, -height / 3f, 0), new Vector2(0, 0));
            Verts[1] = new VertexPositionTexture(new Vector3(-width / 3f, height / 3f, 0), new Vector2(0, 1));
            Verts[2] = new VertexPositionTexture(new Vector3(width / 3f, -height / 3f, 0), new Vector2(1, 0));
            Verts[3] = new VertexPositionTexture(new Vector3(-width / 3f, height / 3f, 0), new Vector2(0, 1));
            Verts[4] = new VertexPositionTexture(new Vector3(width / 3f, height / 3f, 0), new Vector2(1, 1));
            Verts[5] = new VertexPositionTexture(new Vector3(width / 3f, -height / 3f, 0), new Vector2(1, 0));

            PlaneVertexBuffer.SetData(Verts);
        }
    }
}
