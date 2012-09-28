using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Max_Keebler_s_Big_Move
{
    class Player
    {
        public bool inAir;
        const float JumpSpeed = 1.0f;
        public int playerWidth = 30;
        public int playerHeight = 60;
        public Vector2 playerPos;
        float x, y;
        int verticalVelocity;
        int horizontalVelocity;
        Game1 game;
        public Rectangle playerRect, futurePosition;

        public Player(Vector2 position, Game1 game)
        {
            this.game = game;
            playerPos = position;
            x = playerPos.X;
            y = playerPos.Y;
            inAir = false;
            verticalVelocity = 20;
            horizontalVelocity = 0;
            playerRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, playerWidth, playerHeight);
            futurePosition = new Rectangle((int)playerPos.X, (int)playerPos.Y, playerWidth, playerHeight);
        }

        // 1) update player velocity (ie, with any jumping)
        // 2) update player horizontal movement
        // 3) update player vertical movement
        public void Update(GameTime gametime, KeyboardState keyboard)
        {
            // Update velocity
            if(keyboard.IsKeyDown(Keys.Space) && inAir == false) 
            {
                jump();
            }
            if (verticalVelocity < 20)
            {
                verticalVelocity++;
            }

            //update horizontal movement
            // algorithm:
            //      1) create hypothetical rectangle based on horizontal velocity
            //      2) check this "future rectangle" against all collidables
            //      3) If there is a collision:
            //      4)      process that collision accordingly. ie for a platform the player hits against it
            futurePosition.X = (int)playerPos.X + horizontalVelocity;
            futurePosition.Y = (int)playerPos.Y;
            game.updatePlayerHorizontalMovement(futurePosition);

            //update vertical movement
            futurePosition.X = (int)playerPos.X;
            futurePosition.Y = (int)playerPos.Y + verticalVelocity;
            game.updatePlayerVerticalMovement(futurePosition);

            // adjust for out of bounds
            if (playerPos.Y >= game.canvasHeight-playerHeight)
            {
                playerPos.Y = game.canvasHeight - playerHeight;
                inAir = false;
            }

            // update player rectangle
            playerRect.X = (int)playerPos.X;
            playerRect.Y = (int)playerPos.Y;
        }

        private void jump()
        {
            inAir = true;
            verticalVelocity = -20;
        }

        public int getWidth()
        {
            return playerWidth;
        }

        public int getHeight()
        {
            return playerHeight;
        }

    }
}
