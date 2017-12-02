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
/* Här är min Asteroidsklon, som går ut på att samla så många rupees som möjligt.
 * Spelet avslutas också när 100 rupees gått utanför skärmen eller om det finns 100
 * aktiva rupees.
 * Har även lagt till bakgrundsmusik med låg volym, bara kommentera bort om det
 * blir för mycket!
 * /Yakup
*/
namespace Asteroids_by_Yakup_Yildiz
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Skapa en lista för rubinerna
        List<Rupee> rupeeList;

        // Skapa variabler för rubinerna
        Texture2D redrupee;
        Texture2D yellowrupee;
        Texture2D bluerupee;

        double spawnTimer = 2500;
        double spawnInterval = 2500;

        // Skapa dessa variabler som används för att kolla musens läge
        MouseState oldmouseState, mouseState;

        // Ladda in bakgrundsmusiken
        Song hyrule8Bit;
        bool songstart = false;

        // Skapa en rektangel som ska definera storleken för fönstret
        Rectangle mainframe;

        // Skapa en ny "status" sträng
        string statusText;
        
        // Variabler för max höjd/bredd
        int height;
        int width;

        // Poäng!
        int score;

        // Variabel för antalet ritade rupees
        int spawns;

        // Används för rupees över kanten!
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
            /* Sätt inte spelet i fullscreen.
            Sätt spelets yta till 1024x800 pixlar! */
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            // Sätt fönstrets höjd/bredd till fönstrets maxpixlar
            height = Window.ClientBounds.Height;
            width = Window.ClientBounds.Width;

            // Ladda in bakgrundsmusiken
            hyrule8Bit = Content.Load<Song>("Hyrule8bit");
            // Repetera bakgrundsmusiken
            MediaPlayer.IsRepeating = true;
            // Sätt volym på bakgrundsmusiken
            MediaPlayer.Volume = 0.2f;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Ladda in alla bilder som ska användas i spelet
            redrupee = Content.Load<Texture2D>("RedRupee");
            yellowrupee = Content.Load<Texture2D>("YellowRupee");
            bluerupee = Content.Load<Texture2D>("BlueRupee");
            // Ladda in bakgrunden till spelet
            backgroundGrass = Content.Load<Texture2D>("grass");

            rand = new Random();

            rupeeList = new List<Rupee>();

            // Skapa en "for-loop" som skapar x antal rupees på skärmen (värdet anges i kodens början, är satt till 5)
            for (int i = 0; i < rupeeStart; i++)
            {
                // Slumpa fram en X samt en Y- koordinat till rupeen
                Vector2 pos1 = new Vector2(rand.Next(160,width - 160), rand.Next(205, height - 205));
                // Slumpa fram en riktning till rupeen
                Vector2 dir = new Vector2(rand.Next(-2, 2), rand.Next(-2,2));

                // Om riktningen är 0 på någon av koordinaterna, slumpa fram ett nytt tal
                while (dir.X == 0 && dir.Y == 0)
                {
                    dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                }
                // Sätt en slumpad hastighet på en rupee
                int speed = rand.Next(1, 2);
                
                Texture2D tempRupee = redrupee;
                Rupee rupee = new Rupee(pos1, dir, speed, tempRupee, 2);
                // Lägg till rupeen i listan
                rupeeList.Add(rupee);
                spawns = spawns + 1;
            }
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Stäng av spelet om "Esc" tycks ned
            // Används ENDAST i debug syfte, kommenterar bort den då jag är klar!
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
            
            // Använd denna funktion till att avgöra om vänster musknapp har
            // tryckts ned samt att den har släppts
            // Kontrollera var 0.2 sekund (blir svårt att reagera i tid under detta värde)
            elapsedTime = elapsedTime + gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime > 200)
            {
                oldmouseState = mouseState;
                elapsedTime = 0;
            }

            // Då det ska slumpas fram nya riktningar här, så skapas dessa variabler.
            Vector2 dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            Vector2 dir2 = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            // Om riktningen är 0 på någon av koordinaterna, slumpa fram ett nytt tal
            while (dir.X == 0 && dir.Y == 0 || dir2.X == 0 && dir2.Y == 0)
            {
                dir = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
                dir2 = new Vector2(rand.Next(-2, 2), rand.Next(-2, 2));
            }

            // Skapa en funktion som gör något för varje rupee
            foreach (Rupee R in rupeeList)
            {
                Rectangle boundingBox = new Rectangle((int)R.pos.X, (int)R.pos.Y, R.texture.Width, R.texture.Height);
                // Skapa en funktion som tar bort en rupee om den går över en av spelets kanter!
                // Lägg också till ett värde i "outofBounds" för att visa att det åkt ut en rupee
                
                // Kolla om rupeen åker utanför någon X koordinat
                if (R.pos.X >= width || R.pos.X <= 0 - redrupee.Width)
                {
                    rupeeList.Remove(R);
                    outofBounds = outofBounds + 1;
                    break;
                }
                // Kolla om rupeen åker utanför någon Y koordinat
                else if (R.pos.Y >= height || R.pos.Y <= 0 - redrupee.Height)
                {
                    rupeeList.Remove(R);
                    outofBounds = outofBounds + 1;
                    break;
                }


                /* Kolla om vänster musknapp är nedtryckt.
                 * Är den det, kolla om knapptrycket är på en rupee.
                 * Är den det så radera denna rupee från rupeelistan. */

                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldmouseState.LeftButton == ButtonState.Released)
                {
                    if (boundingBox.Contains(mouseState.X, mouseState.Y))
                    {
                        rupeeList.Remove(R);
                        // Öka ritningshastighet
                        spawnInterval = spawnInterval * 0.99;
                        /* Splitta rupees! Värden på rupees: Röd = 2, Gul = 1, Blå = 0
                         * Om en röd blir träffad, splittra till gul
                         * Om en gul blir träffad, splittra till blå
                         */
                        if (R.hasSplit == 2)
                        {
                            rupeeList.Add(new Rupee(R.pos, dir, R.speed, yellowrupee, R.hasSplit - 1));
                            rupeeList.Add(new Rupee(R.pos, dir2, R.speed, yellowrupee, R.hasSplit - 1));
                            // Lägg till 2 värden i spawns för att visa 2 nya rupees!
                            spawns = spawns + 2;
                            // En röd rupee är värd 20 rupees(eller i detta fall poäng)
                            score = score + 20;
                        }
                        if (R.hasSplit == 1)
                        {
                            rupeeList.Add(new Rupee(R.pos, dir, R.speed, bluerupee, R.hasSplit = 0));
                            rupeeList.Add(new Rupee(R.pos, dir2, R.speed, bluerupee, R.hasSplit = 0));
                            // Lägg till 2 värden i spawns för att visa 2 nya rupees!
                            spawns = spawns + 2;
                            // En gul rupee är värd 15 rupees(eller i detta fall poäng)
                            score = score + 15;
                        }
                        // När det inte finns något mer att splittras till, lägg till poäng
                        if (R.hasSplit == 0)
                        {
                            // En blå rupee är värd 5 rupees(eller i detta fall poäng)
                            score = score + 5;
                        }
                        
                        break;
                    }
                }
                
                R.Update();
            }

            // Skapa en "spawnTimer som ritar ut objekten efter en viss tid (värdena bestäms där spawnTimer samt
            // spawnInterval skapas)
            spawnTimer = spawnTimer - gameTime.ElapsedGameTime.TotalMilliseconds;

            // Kör denna kod under om spawntimer är större/lika med 0
            if (spawnTimer <= 0)
            {
                // Slumpa fram en X samt en Y- koordinat till rupeen
                Vector2 pos1 = new Vector2(rand.Next(160, width - 160), rand.Next(205, height - 205));
                // Lägg till en rupee
                rupeeList.Add(new Rupee(pos1, dir, rand.Next(1, 2), redrupee, 2));
                spawns = spawns + 1;
                spawnTimer = spawnTimer + spawnInterval;
            }
            


            
            //Rectangle rupeeBox = new Rectangle((int)rupee.pos.X + rupee.width, (int)rupee.pos.Y + rupee.height, rupee.texture.Width, rupee.texture.Height);
            
            // Visa återstående rupees, poäng samt rupees som gått över gränsen i ramtiteln
            statusText =  "RupeeCollector! | Rupees Collected: " + score + " | Remaining rupees: " + rupeeList.Count + " | Rupees Out: " + outofBounds + " | Rupees Appeared: " + spawns;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.IsMouseVisible = true;
            spriteBatch.Begin();
            // Skriv ut bakgrunden till spelet
            // och fyll ut fönstrets bakgrund med gräsbakgrunden
            spriteBatch.Draw(backgroundGrass, mainframe, Color.White);
            mainframe = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            
            // Rita alla rupees i rupeeList
            foreach (Rupee R in rupeeList)
            {
                R.Draw(spriteBatch);
            }

            
            // Skriv ut antal poäng, antal återstående rubiner samt rubiner som åkt över kanten.
            Window.Title = statusText;

            // Stäng spelet när antalet rupees överstiger 100 eller om rupees som gått utanför överstiger 100
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
