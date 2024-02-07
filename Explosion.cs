﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterGame
{
    internal class Explosion
    {
        Animation explosionAnimation;
        Vector2 Position;
        public bool Active;
        int timeToLive;
        public int Width { get { return explosionAnimation.FrameWidth; }
        }
        public int Height { get { return explosionAnimation.FrameWidth; }
        }
        public void Initialize(Animation animation, Vector2 position)
        {
            explosionAnimation = animation;
            Position = position;
            Active = true;
            timeToLive = 30;
        }
        public void Update(GameTime gameTime)
        {
            explosionAnimation.Update(gameTime);
            timeToLive -= 1;
            if (timeToLive <= 0)
            {
                this.Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            explosionAnimation.Draw(spriteBatch);
        }
    }
}
