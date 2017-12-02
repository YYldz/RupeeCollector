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

namespace Asteroids_by_Yakup_Yildiz
{
    class Rupee
    {
        // Definera position, rikting, textur, hastighet, höjd(skärm), och bredd(width)
        public Vector2 pos;
        public Vector2 direction;
        public Texture2D texture;
        public int speed;
        // Skapa ett heltal som är 2 när en rupee är röd, och 1 när den är gul samt 0 när den är blå
        public int hasSplit = 2;
        public int height;
        public int width;

        public bool isVisible = true;

        public Rupee(Vector2 pos, Vector2 direction, int speed, Texture2D texture, int hasSplit)
        {
            this.pos = pos;
            this.direction = direction;
            this.texture = texture;
            this.speed = speed;
            this.hasSplit = hasSplit;
            height = texture.Height;
            width = texture.Width;

        }

        // Lägg till en update funktion just för detta objekt
        // Lägg till kod som gör att en rupee rör sig
        public void Update()
        {
            pos.X = pos.X + speed * direction.X;
            pos.Y = pos.Y + speed * direction.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, pos, new Rectangle(0,0,texture.Width,texture.Height), Color.White);
        }
    }
}
