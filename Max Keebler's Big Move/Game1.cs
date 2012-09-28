using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Max_Keebler_s_Big_Move
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture, platformTexture;
        public int canvasHeight, canvasWidth;
        KeyboardState keyboard = Keyboard.GetState();
        Player player;
        List<Platform> Platforms;
        int horizontalCollisionDistance_platform, verticalCollisionDistance_platform;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            canvasHeight = graphics.GraphicsDevice.Viewport.Height;
            canvasWidth = graphics.GraphicsDevice.Viewport.Width;
            player = new Player(new Vector2( (canvasWidth-150), canvasHeight-60), this);
            Platforms = new List<Platform>();
            for (int i = 5; i < 8; i++)
            {
                Platform platform = new Platform((i * (canvasWidth / 3)), (canvasHeight - 20));
                Platforms.Add(platform);
            }
            horizontalCollisionDistance_platform = (player.getWidth() / 2) + (Platforms[0].getWidth() / 2);
            verticalCollisionDistance_platform = (player.getHeight() / 2) + (Platforms[0].getHeight() / 2);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerTexture = Content.Load<Texture2D>(@"Player");
            platformTexture = Content.Load<Texture2D>(@"Platform1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            keyboard = Keyboard.GetState();
            // update all collidables (platforms, enemies, bullets) and their positions first
            foreach(Platform myPlatform in Platforms)
            {
                myPlatform.Update();
                if (myPlatform.outOfBounds())
                {
                    myPlatform.position.X = canvasWidth;
                    myPlatform.platformRect.X = canvasWidth;
                }
                // Instead of having the platforms process player collision, we will have player process all collisions
                /*
                if (myPlatform.isCollidingLeft(player.playerRect) &&
                    myPlatform.isCollidingTop(player.playerRect))
                {
                    if (player.playerPos.Y == (canvasHeight - 60))
                    {
                        myPlatform.onCollisionLeft(player);
                    }
                    else
                    {
                        myPlatform.onCollisionTop(player);
                    }

                    
                    //if ((player.playerPos.Y+60) > (myPlatform.position.Y + 15))
                    //{
                    //    myPlatform.onCollisionLeft(player);
                    //}
                    //else
                    //{
                    //    myPlatform.onCollisionTop(player);
                    //}
                }
                else if (myPlatform.isCollidingTop(player.playerRect))
                {
                    myPlatform.onCollisionTop(player);
                }
                else if (myPlatform.isCollidingLeft(player.playerRect))
                {
                    myPlatform.onCollisionLeft(player);
                }  */
            } // this updates platforms
            // then update player
            player.Update(gameTime, keyboard);
            base.Update(gameTime);
        }

        // processes all player collisions resulting from horizontal movement
        public void updatePlayerHorizontalMovement(Rectangle futurePosition)
        {
            //Vector2 playerPos = player.playerPos;
            int playerCenterX, platformCenterX, collisionDistance, centerDistance;
            foreach (Platform nextPlatform in Platforms){
                if (nextPlatform.isColliding(futurePosition)){
                    
                    // find out the amount of pixels they are colliding by
                    playerCenterX = futurePosition.Center.X;
                    platformCenterX = nextPlatform.getCenter().X;
                    centerDistance = Math.Abs(playerCenterX - platformCenterX);
                    collisionDistance = (horizontalCollisionDistance_platform - centerDistance);
                    
                    // move the player by that distance (in the correct direction)
                    if (playerCenterX < platformCenterX) {
                        futurePosition.X -= collisionDistance;
                    } else {
                        futurePosition.X += collisionDistance;
                    }
                }
            }
            // all collisions have been accounted for, so update playerPos to reflect new position
            player.playerPos.X = futurePosition.X;
        }

        // processes all player collisions resulting from vertical movement
        public void updatePlayerVerticalMovement(Rectangle futurePosition)
        {
            //Vector2 playerPos = player.playerPos;
            int playerCenterY, platformCenterY, collisionDistance, centerDistance;
            foreach (Platform nextPlatform in Platforms)
            {
                if (nextPlatform.isColliding(futurePosition))
                {
                    // find out the amount of pixels they are colliding by
                    playerCenterY = futurePosition.Center.Y;
                    platformCenterY = nextPlatform.getCenter().Y;
                    centerDistance = Math.Abs(playerCenterY - platformCenterY);
                    collisionDistance = (verticalCollisionDistance_platform - centerDistance);

                    // move the player by that distance (in the correct direction)
                    if (playerCenterY < platformCenterY)
                    {
                        futurePosition.Y -= collisionDistance;
                        player.inAir = false; // player has landed on a platform
                    }
                    else
                    {
                        futurePosition.Y += collisionDistance;
                    }
                }
            }
            // all collisions have been accounted for, so update playerPos to reflect new position
            player.playerPos.Y = futurePosition.Y;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(playerTexture, player.playerPos, Color.White);
            foreach (Platform platform in Platforms)
            {
                spriteBatch.Draw(platformTexture, platform.position, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
