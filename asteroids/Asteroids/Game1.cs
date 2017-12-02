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
/* H�r �r min Asteroidsklon, som g�r ut p� att samla s� m�nga rupees som m�jligt.
 * Spelet avslutas ocks� n�r 100 rupees g�tt utanf�r sk�rmen eller om det finns 100
 * aktiva rupees.
 * Har �ven lagt till bakgrundsmusik med l�g volym, bara kommentera bort om det
 * blir f�r mycket!
 * /Yakup
*/
namespace Asteroids_by_Yakup_Yildiz
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Skapa en lista f�r rubinerna
        List<Rupee> rupeeList;

        // Skapa variabler f�r rubinerna
        Texture2D redrupee;
        Texture2D yellowrupee;
        Texture2D bluerupee;

        double spawnTimer = 2500;
        double spawnInterval = 2500;

        // Skapa dessa variabler som anv�nds f�r att kolla musens l�ge
        MouseState oldmouseState, mouseState;

        // Ladda in bakgrundsmusiken
        Song hyrule8Bit;
        bool songstart = false;

        // Skapa en rektangel som ska definera storleken f�r f�nstret
        Rectangle mainframe;

        // Skapa en ny "status" str�ng
        string statusText;
        
        // Variabler f�r max h�jd/bredd
        int height;
        int width;

        // Po�ng!
        int score;

        // Variabel f�r antalet ritade rupees
        int spawns;

        // Anv�nds f�r rupees �ver kanten!
        int outofBounds;

        int elapsedTime;

        // Ange antalet rupees vid start
        int rupeeStart = 5;

        Random rand;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            /* S�tt inte spelet i fullscreen.
            S�tt spelets yta till 1024x800 pixlar! */
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            // S�tt f�nstrets h�jd/bredd till f�nstrets maxpixlar
            height = Window.ClientBounds.Height;
            width = Window.ClientBounds.Width;

            // Ladda in bakgrundsmusiken
            hyrule8Bit = Content.Load<Song>("Hyrule8bit");
            // Repetera bakgrundsmusiken
            MediaPlayer.IsRepeating = true;
            // S�tt volym p� bakgrundsmusiken
            MediaPlayer.Volume = 0.2f;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Ladda in alla bilder som ska anv�ndas i spelet
            redrupee = Content.Load<Texture2D>("RedRupee");
            yellowrupee = Content.Load<Texture2D>("YellowRupee");
            bluerupee = Content.Load<Texture2D>("BlueRupee");
            // Ladda in bakgrunden till spelet
            backgroundGrass = Content.Load<Texture2D>("grass");

            rand = new Random();

            rupeeList = new List<Rupee>();

            // Skapa en "for-loop" som skapar x antal rupees p� sk�rmen (v�rdet anges i kodens b�rjan, �r satt till 5)
            for (int i = 0; i < rupeeStart; i++)
            {
                // Slumpa fram en X samt en Y- koordinat till rupeen
                Vector2 pos1 = new Vector2(rand.Next(160,width - 160), rand.Next(205, height - 205));
                // Slumpa fram en riktning till rupeen
                Vector2 dir = new Vector2(rand.Next(-2, 2), rand.Next(-2,2));

                // Om riktningen �r 0 p� n�gon av koordinaterna, slumpa fram ett nytt tal
                while (dir.X == 0 && dir.Y == 0)
                {
                    dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                }
                // S�tt en slumpad hastighet p� en rupee
                int speed = rand.Next(1, 2);
                
                Texture2D tempRupee = redrupee;
                Rupee rupee = new Rupee(pos1, dir, speed, tempRupee, 2);
                // L�gg till rupeen i listan
                rupeeList.Add(rupee);
                spawns = spawns + 1;
            }
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // St�ng av spelet om "Esc" tycks ned
            // Anv�nds ENDAST i debug syfte, kommenterar bort den d� jag �r klar!
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            oldmouseState = mouseState;
            mouseState = Mouse.GetState();
            

            // Spela upp bakgrundsmusiken om den inte spelas!
            if (!songstart)
            {
                MediaPlayer.Play(hyrule8Bit);
                songstart = true;
            }
            
            // Anv�nd denna funktion till att avg�ra om v�nster musknapp har
            // tryckts ned samt att den har sl�ppts
            // Kontrollera var 0.2 sekund (blir sv�rt att reagera i tid under detta v�rde)
            elapsedTime = elapsedTime + gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime > 200)
            {
                oldmouseState = mouseState;
                elapsedTime = 0;
            }

            // D� det ska slumpas fram nya riktningar h�r, s� skapas dessa variabler.
            Vector2 dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            Vector2 dir2 = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            // Om riktningen �r 0 p� n�gon av koordinaterna, slumpa fram ett nytt tal
            while (dir.X == 0 && dir.Y == 0 || dir2.X == 0 && dir2.Y == 0)
            {
                dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                dir2 = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            }

            // Skapa en funktion som g�r n�got f�r varje rupee
            foreach (Rupee R in rupeeList)
            {
                Rectangle boundingBox = new Rectangle((int)R.pos.X, (int)R.pos.Y, R.texture.Width, R.texture.Height);
                // Skapa en funktion som tar bort en rupee om den g�r �ver en av spelets kanter!
                // L�gg ocks� till ett v�rde i "outofBounds" f�r att visa att det �kt ut en rupee
                
                // Kolla om rupeen �ker utanf�r n�gon X koordinat
                if (R.pos.X >= width || R.pos.X <= 0 - redrupee.Width)
                {
                    rupeeList.Remove(R);
                    outofBounds = outofBounds + 1;
                    break;
                }
                // Kolla om rupeen �ker utanf�r n�gon Y koordinat
                else if (R.pos.Y >= height || R.pos.Y <= 0 - redrupee.Height)
                {
                    rupeeList.Remove(R);
                    outofBounds = outofBounds + 1;
                    break;
                }


                /* Kolla om v�nster musknapp �r nedtryckt.
                 * �r den det, kolla om knapptrycket �r p� en rupee.
                 * �r den det s� radera denna rupee fr�n rupeelistan. */

                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldmouseState.LeftButton == ButtonState.Released)
                {
                    if (boundingBox.Contains(mouseState.X, mouseState.Y))
                    {
                        rupeeList.Remove(R);
                        // �ka ritningshastighet
                        spawnInterval = spawnInterval * 0.99;
                        /* Splitta rupees! V�rden p� rupees: R�d = 2, Gul = 1, Bl� = 0
                         * Om en r�d blir tr�ffad, splittra till gul
                         * Om en gul blir tr�ffad, splittra till bl�
                         */
                        if (R.hasSplit == 2)
                        {
                            rupeeList.Add(new Rupee(R.pos, dir, R.speed, yellowrupee, R.hasSplit - 1));
                            rupeeList.Add(new Rupee(R.pos, dir2, R.speed, yellowrupee, R.hasSplit - 1));
                            // L�gg till 2 v�rden i spawns f�r att visa 2 nya rupees!
                            spawns = spawns + 2;
                            // En r�d rupee �r v�rd 20 rupees(eller i detta fall po�ng)
                            score = score + 20;
                        }
                        if (R.hasSplit == 1)
                        {
                            rupeeList.Add(new Rupee(R.pos, dir, R.speed, bluerupee, R.hasSplit = 0));
                            rupeeList.Add(new Rupee(R.pos, dir2, R.speed, bluerupee, R.hasSplit = 0));
                            // L�gg till 2 v�rden i spawns f�r att visa 2 nya rupees!
                            spawns = spawns + 2;
                            // En gul rupee �r v�rd 15 rupees(eller i detta fall po�ng)
                            score = score + 15;
                        }
                        // N�r det inte finns n�got mer att splittras till, l�gg till po�ng
                        if (R.hasSplit == 0)
                        {
                            // En bl� rupee �r v�rd 5 rupees(eller i detta fall po�ng)
                            score = score + 5;
                        }
                        
                        break;
                    }
                }
                
                R.Update();
            }

            // Skapa en "spawnTimer som ritar ut objekten efter en viss tid (v�rdena best�ms d�r spawnTimer samt
            // spawnInterval skapas)
            spawnTimer = spawnTimer - gameTime.ElapsedGameTime.TotalMilliseconds;

            // K�r denna kod under om spawntimer �r st�rre/lika med 0
            if (spawnTimer <= 0)
            {
                // Slumpa fram en X samt en Y- koordinat till rupeen
                Vector2 pos1 = new Vector2(rand.Next(160, width - 160), rand.Next(205, height - 205));
                // L�gg till en rupee
                rupeeList.Add(new Rupee(pos1, dir, rand.Next(1, 2), redrupee, 2));
                spawns = spawns + 1;
                spawnTimer = spawnTimer + spawnInterval;
            }
            


            
            //Rectangle rupeeBox = new Rectangle((int)rupee.pos.X + rupee.width, (int)rupee.pos.Y + rupee.height, rupee.texture.Width, rupee.texture.Height);
            
            // Visa �terst�ende rupees, po�ng samt rupees som g�tt �ver gr�nsen i ramtiteln
            statusText =  "RupeeCollector! | Rupees Collected: " + score + " | Remaining rupees: " + rupeeList.Count + " | Rupees Out: " + outofBounds + " | Rupees Appeared: " + spawns;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.IsMouseVisible = true;
            spriteBatch.Begin();
            // Skriv ut bakgrunden till spelet
            // och fyll ut f�nstrets bakgrund med gr�sbakgrunden
            spriteBatch.Draw(backgroundGrass, mainframe, Color.White);
            mainframe = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            
            // Rita alla rupees i rupeeList
            foreach (Rupee R in rupeeList)
            {
                R.Draw(spriteBatch);
            }

            
            // Skriv ut antal po�ng, antal �terst�ende rubiner samt rubiner som �kt �ver kanten.
            Window.Title = statusText;

            // St�ng spelet n�r antalet rupees �verstiger 100 eller om rupees som g�tt utanf�r �verstiger 100
            if (rupeeList.Count == 100 || outofBounds == 100)
            {
                this.Exit();
            }
            spriteBatch.End();



            base.Draw(gameTime);
        }

        public Texture2D backgroundGrass { get; set; }
    }
}
