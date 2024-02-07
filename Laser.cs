﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterGame
{
    internal class Laser
    {
        // laser animation.  
        public Animation LaserAnimation;
        // the speed the laser travels  
        float laserMoveSpeed = 30f;
        // position of the laser  
        public Vector2 Position;
        // The damage the laser deals.  
        public int Damage = 10;
        // set the laser to active  
        public bool Active;
        // Laser beams range.  
        int Range;
        // the width of the laser image.  
        public int Width
        {
            get { return LaserAnimation.FrameWidth; }
        }
        // the height of the laser image.  
        public int Height
        {
            get { return LaserAnimation.FrameHeight; }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            LaserAnimation = animation;
            Position = position;
            Active = true;
        }
        public void Update(GameTime gameTime)
        {
            Position.X += laserMoveSpeed;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);  
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }
    }
}
