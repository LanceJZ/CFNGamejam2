using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;

enum SoundMode
{
    Idle,
    Move
}

namespace CFNGamejam2.Entities
{
    public class Player : AModel
    {
        GameLogic RefGameLogic;
        List<TankShot> Shots;
        AModel HealthBar;
        AModel HealthBack;
        AModel Turret;
        AModel Gun;
        AModel[] Treads = new AModel[2];

        Timer PerShotTimer;
        Timer IdleSoundTimer;
        Timer MoveSoundTimer;

        SoundEffect ShootSound;
        SoundEffect IdleSound;
        SoundEffect MoveSound;
        SoundEffect ShotHitSound;

        KeyboardState LastKeyState;
        SoundMode EngineSound;

        int Speed = 50;
        int Health;
        bool WasBumped;

        public List<TankShot> ShotsRef { get => Shots; }
        public AModel GunRef { get => Gun; }

        public Player(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            Shots = new List<TankShot>();
            Turret = new AModel(game);
            Gun = new AModel(game);
            HealthBar = new AModel(game);
            HealthBack = new AModel(game);

            for (int i = 0; i < 2; i++)
            {
                Treads[i] = new AModel(game);
            }

            PerShotTimer = new Timer(game, 1);
            IdleSoundTimer = new Timer(game);
            MoveSoundTimer = new Timer(game);
            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            LoadModel("TankBody");
            Turret.LoadModel("TankTurret");
            Gun.LoadModel("TankGun");
            HealthBar.LoadModel("Core/Cube");
            HealthBack.LoadModel("Core/Cube");

            for (int i = 0; i < 2; i++)
            {
                Treads[i].LoadModel("TankTreads");
            }

            ShootSound = LoadSoundEffect("TankShot");
            IdleSound = LoadSoundEffect("TankIdle");
            MoveSound = LoadSoundEffect("TankMove");
            ShotHitSound = LoadSoundEffect("TankShotHit");

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Turret.Position.Y = 11.5f;
            Turret.AddAsChildOf(this, true, false);
            Gun.Position.X = 13.5f;
            Gun.Position.Y = -2.25f;
            Gun.AddAsChildOf(Turret, true, false);

            HealthBar.DefuseColor = new Vector3(0, 2, 0);
            HealthBar.ModelScale.X = 2;
            HealthBar.ModelScale.Z = 2;
            HealthBar.AddAsChildOf(this, true, false);
            HealthBack.Position.Y = 24;
            HealthBack.ModelScale.Y = 5;
            HealthBack.ModelScale.Z = 1;
            HealthBack.ModelScale.X = 1;
            HealthBack.DefuseColor = new Vector3(0.1f, 0.1f, 0.1f);
            HealthBack.AddAsChildOf(this, true, false);

            for (int i = 0; i < 2; i++)
            {
                Treads[i].Position.Y = -2.5f;
                Treads[i].Position.Z = 12 - (24 * i);
                Treads[i].AddAsChildOf(this, true, false);
            }

            IdleSoundTimer.Amount = (float)IdleSound.Duration.TotalSeconds;
            MoveSoundTimer.Amount = (float)MoveSound.Duration.TotalSeconds;
        }

        public override void Update(GameTime gameTime)
        {
            InPlay();
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void Bumped(Vector3 position)
        {
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += VelocityFromVectorsY(position, Position, 75);
            WasBumped = true;
        }

        public void HitDamage(int damage)
        {
            Health -= damage;
            HealthBar.ModelScale.Y = (Health / 2f);
            HealthBar.Position.Y = 19.5f + HealthBar.ModelScale.Y;

            if (Health <= 0)
            {
                GameOver();
            }
        }

        public void NewWave()
        {
            Gun.Rotation.Z = 0.1f;
            Turret.Rotation.Y = 0;
            Rotation.Y = MathHelper.PiOver2;
            Position.Z = RefGameLogic.RefGround.TheBorder - 30;
            Position.X = 0;
        }

        public void NewGame()
        {
            Active = true;
            Health = 10;
            HitDamage(0);
            NewWave();
        }

        public void GameOver()
        {
            RefGameLogic.RefUI.GameOver();
            Core.DefaultCamera.Position.Z = 600 + RefGameLogic.RefGround.TheBorder - 30;
            Core.DefaultCamera.Target = new Vector3(0, 0, RefGameLogic.RefGround.TheBorder - 30);
            RefGameLogic.GameOver();
            Active = false;
        }

        void InPlay()
        {
            if (!WasBumped)
                Input();

            KeepInBorders();

            if (Position.Z < -1178)
            {
                RefGameLogic.NewWave();
                NewWave();
                return;
            }

            if (Gun.Rotation.Z < 0)
                Gun.Rotation.Z = 0;

            if (Gun.Rotation.Z > 0.333f)
                Gun.Rotation.Z = 0.333f;

            Core.DefaultCamera.Position.Z = 600 + Position.Z;
            Core.DefaultCamera.Target = Position;

            WasBumped = false;
        }

        void KeepInBorders()
        {
            int edge = 150;

            if (Position.X > RefGameLogic.RefGround.TheBorder - edge)
            {
                Position.X = RefGameLogic.RefGround.TheBorder - edge;
            }

            if (Position.X < -RefGameLogic.RefGround.TheBorder + edge)
            {
                Position.X = -RefGameLogic.RefGround.TheBorder + edge;
            }

            if (Position.Z > RefGameLogic.RefGround.TheBorder - 20)
            {
                Position.Z = RefGameLogic.RefGround.TheBorder - 20;
            }

            if (Position.Z < -RefGameLogic.RefGround.TheBorder)
            {
                Position.Z = -RefGameLogic.RefGround.TheBorder;
            }
        }

        void Input()
        {
            KeyboardState theKeyboard = Keyboard.GetState();

            if (theKeyboard.IsKeyDown(Keys.W))
                MoveForward();
            else if (theKeyboard.IsKeyDown(Keys.S))
                MoveBackward();
            else
                StopMovement();

            if (theKeyboard.IsKeyDown(Keys.D))
                RotateCC();
            else if (theKeyboard.IsKeyDown(Keys.A))
                RotateCW();
            else
                StopRotation();

            switch (EngineSound)
            {
                case SoundMode.Idle:
                    PlayIdleSound();
                    break;
                case SoundMode.Move:
                    PlayMoveSound();
                    break;
            }

            if (theKeyboard.IsKeyDown(Keys.Left))
                RotateTurretCW();
            else if (theKeyboard.IsKeyDown(Keys.Right))
                RotateTurretCC();
            else
                StopTurretRotation();

            if (theKeyboard.IsKeyDown(Keys.Up))
                PointGunUp();
            else if (theKeyboard.IsKeyDown(Keys.Down))
                PointGunDown();
            else
                StopGunRotation();

            if (!LastKeyState.IsKeyDown(Keys.Space) && theKeyboard.IsKeyDown(Keys.Space))
            {
                if (PerShotTimer.Elapsed)
                {
                    PerShotTimer.Reset();
                    FireShot();
                }
            }

            LastKeyState = theKeyboard;
        }

        void FireShot()
        {
            ShootSound.Play();
            bool makeNew = true;
            int thisOne = 0;


            foreach (TankShot shot in Shots)
            {
                if (!shot.Active)
                {
                    makeNew = false;
                    break;
                }

                thisOne++;
            }

            if (makeNew)
            {
                thisOne = Shots.Count;
                Shots.Add(new TankShot(Game, ShotHitSound));
            }

            Vector3 vel = VelocityFromAngle(new Vector2(Turret.WorldRotation.Y,
                Gun.Rotation.Z), 250);
            Shots[thisOne].Spawn(Gun.TheWoldMatrix.Translation + (vel * 0.2f),
                Gun.Rotation, vel);
        }

        void PlayMoveSound()
        {
            if (MoveSoundTimer.Elapsed)
            {
                MoveSoundTimer.Reset();
                MoveSound.Play();
            }
        }

        void PlayIdleSound()
        {
            if (IdleSoundTimer.Elapsed)
            {
                IdleSoundTimer.Reset();
                IdleSound.Play();
            }
        }

        void MoveForward()
        {
            Velocity = VelocityFromAngleY(Rotation.Y, Speed * 1.5f);
            EngineSound = SoundMode.Move;
        }

        void MoveBackward()
        {
            Velocity = VelocityFromAngleY(Rotation.Y, -Speed);
            EngineSound = SoundMode.Move;
        }

        void RotateCW()
        {
            RotationVelocity.Y = 0.5f;
            EngineSound = SoundMode.Move;
        }

        void RotateCC()
        {
            RotationVelocity.Y = -0.5f;
            EngineSound = SoundMode.Move;
        }

        void StopRotation()
        {
            RotationVelocity = Vector3.Zero;
        }

        void StopMovement()
        {
            Velocity = Vector3.Zero;
            EngineSound = SoundMode.Idle;
        }

        void RotateTurretCW()
        {
            Turret.RotationVelocity.Y = 0.35f;
        }

        void RotateTurretCC()
        {
            Turret.RotationVelocity.Y = -0.35f;
        }

        void StopTurretRotation()
        {
            Turret.RotationVelocity.Y = 0;
        }

        void PointGunUp()
        {
            if (Gun.Rotation.Z < 0.3f)
                Gun.RotationVelocity.Z = 0.25f;
        }

        void PointGunDown()
        {
            if (Gun.Rotation.Z > 0)
                Gun.RotationVelocity.Z = -0.5f;
        }

        void StopGunRotation()
        {
            Gun.RotationVelocity.Z = 0;
        }
    }
}
