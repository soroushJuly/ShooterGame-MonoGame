using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShooterGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Represents the player  
        Player player;
        // Keyboard states used to determine key presses   
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Gamepad states used to determine button presses   
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        //Mouse states used to track Mouse button press   
        MouseState currentMouseState;
        MouseState previousMouseState;
        // A movement speed for the layer  
        float playerMoveSpeed;

        // Image used to display the static background   
        Texture2D mainBackground;
        // Parallaxing Layers   
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // Enemies  
        Texture2D enemyTexture;
        List<Enemy> enemies;
        //The rate at which the enemies appear  
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        // A random number generator  
        Random random;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize the player class
            player = new Player();
            // Set a constant player move speed
            playerMoveSpeed = 8.0f;

            // Initialize the enemies list
            enemies = new List<Enemy>();
            // Set the time keepers to zero  
            previousSpawnTime = TimeSpan.Zero;
            // Used to determine how fast enemy respawns  
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            // Initialize our random number generator  
            random = new Random();


            //Background  
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load the player resources   
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
            GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition);

            // Load the enemy animation  
            enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");

            // Load the parallaxing background   
            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width, 
                GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width, 
                GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Graphics/mainbackground");
        }

        protected override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Save the previous state of the keyboard, game pad, and mouse so we can determine single key/button presses  
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard, gamepad and mouse and store it  
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            //Update the player   
            UpdatePlayer(gameTime);

            // Update the enemies  
            UpdateEnemies(gameTime);

            // Update the collisions   
            UpdateCollision();

            // Update the parallaxing background    
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            base.Update(gameTime);
        }
        // Getting and reacting to the inputs for the player
        private void UpdatePlayer(GameTime gameTime)
        {
            // Get Mouse State then Capture the Button type and Respond Button Press
            // TODO: fix mouse input on reaching the mouse position 
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 posDelta = mousePosition - player.Position;
                posDelta.Normalize();
                posDelta = posDelta * playerMoveSpeed;
                player.Position = player.Position + posDelta;
            }

            // Get Thumbstick Controls   
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            // Use the Keyboard / Dpad   
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            { player.Position.X -= playerMoveSpeed; }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            { player.Position.X += playerMoveSpeed; }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            { player.Position.Y -= playerMoveSpeed; }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            { player.Position.Y += playerMoveSpeed; }

            // Make sure that the player does not go out of bounds   
            player.Position.X = MathHelper.Clamp(player.Position.X, player.Width / 2,
            GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, player.Height / 2,
            GraphicsDevice.Viewport.Height - player.Height / 2);
        }
        private void AddEnemy()
        {
            // Create the animation object  
            Animation enemyAnimation = new Animation();
            // Initialize the animation with the correct animation information  
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            // Randomly generate the position of the enemy  
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100,
            GraphicsDevice.Viewport.Height - 100));
            // Create an enemy  
            Enemy enemy = new Enemy();
            // Initialize the enemy  
            enemy.Initialize(enemyAnimation, position);
            // Add the enemy to the active enemies list 
            enemies.Add(enemy);
        }
        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect function to   
            // determine if two objects are overlapping   
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            // Only create the rectangle once for the player  
            playerRectangle = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            // Do the collision between the player and the enemies
            for (int i = 0; i < enemies.Count; i++)  
            {
                enemyRectangle = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, 
                    enemies[i].Width, 
                    enemies[i].Height);
                // Determine if the two objects collided with each other  
                if (playerRectangle.Intersects(enemyRectangle))
                {
                    player.Health -= enemies[i].Damage;
                    // Since the enemy collided with the player destroy it  
                    enemies[i].Health = 0;
                    // If the player health is less than zero we died  
                    if (player.Health <= 0) player.Active = false;
                }
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds  
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                // Add an Enemy  
                AddEnemy();
            }
            // Update the Enemies  
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Start drawing  
            _spriteBatch.Begin();

            //Draw the Main Background Texture  
            _spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
            // Draw the moving background  
            bgLayer1.Draw(_spriteBatch);
            bgLayer2.Draw(_spriteBatch);

            // Draw the Player  
            player.Draw(_spriteBatch);

            // Draw the Enemies   
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
            }

            // Stop drawing  
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
