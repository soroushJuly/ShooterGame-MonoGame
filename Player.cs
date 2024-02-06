using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShooterGame
{
    internal class Player
    {
        // Animation representing the player  
        public Texture2D PlayerTexture;
        // Position of the Player relative to the upper left side of the screen  
        public Vector2 Position;
        // State of the player  
        public bool Active;
        // Amount of hit points that player has  
        public int Health;
        // Get the width of the player ship  
        public int Width
        { get { return PlayerTexture.Width; } }
        // Get the height of the player ship  
        public int Height
        { get { return PlayerTexture.Height; } }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            Position = position;

            // Set the player to be active  
            Active = true;
            // Set the player health  
            Health = 100;
        }
        public void Update()
        {
        }
        // Probably the same as render()
        // batches the object to the graphic card
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition;
            drawPosition.X = Position.X - Width / 2;
            drawPosition.Y = Position.Y - Height / 2;
            spriteBatch.Draw(PlayerTexture, drawPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
