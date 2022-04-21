using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
    
namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */


        /* Initialization */

        Guster1 guster;
        Kamen1 kamen1, kamen3, kamen4, kamen5, kamenPrivremeni, vrata_zatvorena, vrata_otvorena;
        Kamen2 kamen2;

        Bube1 buba1, buba2;
        public bool prvarazina;
        public bool drugarazina;
        public bool izmedurazina;
        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("L I Z L I");
            setBackgroundColor(Color.WhiteSmoke);
            setBackgroundPicture("backgrounds\\pozadina.png");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");



            //2. add sprites
            guster = new Guster1("sprites\\lizardD.png", 0, 165, 15);
            guster.SetSize(6);

            Game.AddSprite(guster);

            kamen1 = new Kamen1("sprites\\Slika1.png", 20, 330);
            kamen1.SetSize(65);
            Game.AddSprite(kamen1);
            guster.Y = kamen1.Y - kamen1.Heigth;


            kamen2 = new Kamen2("sprites\\Slika2.png", 430, 330, 10);
            kamen2.SetSize(65);
            Game.AddSprite(kamen2);


            kamen3 = new Kamen1("sprites\\Slika3.png", 720, 330);
            kamen3.SetSize(65);
            Game.AddSprite(kamen3);

            kamen4 = new Kamen1("sprites\\Slika4.png", 165, 115);
            kamen4.SetSize(60);
            Game.AddSprite(kamen4);

            kamen5 = new Kamen1("sprites\\Slika5.png", 395, 115);
            kamen5.SetSize(60);



            Game.AddSprite(kamen5);

            kamenPrivremeni = new Kamen1("sprites\\Slika2.png", kamen3.X - kamen2.Width / 2, kamen4.Y + kamen4.Heigth);
            kamenPrivremeni.SetSize(55);
            Game.AddSprite(kamenPrivremeni);
            kamenPrivremeni.SetVisible(false);

            buba1 = new Bube1("sprites\\bug1.png", 0, 0);
            buba1.SetSize(30);
            //bubu postavljamo na sredinu 3.kamena desno dole
            buba1.X = kamen3.X + buba1.Width / 2;
            buba1.Y = kamen3.Y - buba1.Heigth;
            Game.AddSprite(buba1);

            buba2 = new Bube1("sprites\\bug2.png", 0, 0);
            buba2.SetSize(25);
            // bubu postavljamo na gornji desni kamen
            buba2.X = kamen5.X + buba2.Width;
            buba2.Y = kamen5.Y - buba2.Heigth * 2;

            Game.AddSprite(buba2);


            vrata_zatvorena = new Kamen1("sprites\\Vratazatvorena.png", 0, 0);
            vrata_zatvorena.SetSize(60);
            vrata_zatvorena.X = kamen4.X;
            vrata_zatvorena.Y = kamen4.Y - vrata_zatvorena.Heigth;
            Game.AddSprite(vrata_zatvorena);
            vrata_zatvorena.SetVisible(false);



            vrata_otvorena = new Kamen1("sprites\\Vrataotvorena.png", 0, 0);
            vrata_otvorena.SetSize(60);
            vrata_otvorena.X = kamen4.X;
            vrata_otvorena.Y = kamen4.Y - vrata_otvorena.Heigth;
            Game.AddSprite(vrata_otvorena);
            vrata_otvorena.SetVisible(false);





            //3. scripts that start
            prvarazina = true;
            drugarazina = false;
            izmedurazina = false;
            Game.StartScript(KretanjeGuster);
            Game.StartScript(Kamen2Kretanje);
            Game.StartScript(Skok);
            Game.StartScript(GameOver);
            Game.StartScript(BubeJedi);

        }

        /* Scripts */
        Bitmap lizardlijevi = new Bitmap("sprites\\lizardL.png");
        Bitmap lizarddesni = new Bitmap("sprites\\lizardD.png");
        Bitmap lizardkutL = new Bitmap("sprites\\lizardKUTL.png");
        Bitmap lizardkutD = new Bitmap("sprites\\lizardKUTD.png");
        //kretanje gustera lijevo desno dok je na kamenju
        // unutar KretanjeGuster se nalazi i ogranicenje ukoliko skrene s kamenja da pada dole
        private int KretanjeGuster()
        {
            while (START) //ili neki drugi uvjet
            {
                // klasične kretnje lijevo desno
                if (sensing.KeyPressed("D"))
                {

                    guster.CurrentCostume = lizarddesni;
                    guster.X += guster.Brzina;

                    if (guster.X >= GameOptions.RightEdge - guster.Width)
                    {
                        guster.X = GameOptions.RightEdge - guster.Width - 10;
                    }




                }

                if (sensing.KeyPressed("A"))
                {
                    guster.CurrentCostume = lizardlijevi;
                    guster.X -= guster.Brzina;
                    if (guster.X <= GameOptions.LeftEdge)
                    {
                        guster.X = 0;
                    }




                }

                //ispod if je ako ne dodiriva neki kamen da automatski pada dolje
                // na putu prema dolje ukoliko naide na neki kamen prestaje padati
                if ((sensing.KeyPressed("D") || sensing.KeyPressed("A")))
                {
                    if (prvarazina)
                    {
                        //ukoliko ne dira nijedan kamen pada dolje. Ukoliko stane na neki kamen pad prestaje
                        if (!guster.TouchingSprite(kamen1) && !guster.TouchingSprite(kamen2) && !guster.TouchingSprite(kamen3) && guster.Y + guster.Heigth > kamen1.Y)
                        {
                            while (guster.Y - guster.Heigth < GameOptions.DownEdge)
                            {
                                guster.Y += guster.Brzina;
                                Wait(0.05);
                            }
                        }
                    }
                    // druga razina je najvise podesena zbog pada na donju razinu
                    if (drugarazina)
                    {
                        if (!guster.TouchingSprite(kamen5) && !guster.TouchingSprite(kamen4) && guster.Y + guster.Heigth > kamen5.Y)
                        {
                            while (guster.Y - guster.Heigth < GameOptions.DownEdge)
                            {
                                guster.Y += guster.Brzina;
                                //ovdje je uvjet ukoliko padne prema dole
                                if (guster.TouchingSprite(kamen1) || guster.TouchingSprite(kamen2) || guster.TouchingSprite(kamen3))
                                {
                                    guster.Y = kamen1.Y - guster.Heigth;
                                    break;
                                }
                                Wait(0.01);
                            }

                        }
                    }
                    // izmedu razina sluzi zbog uvjeta u prvoj razini da ukoliko guster takne gornji kamen odozdol da se odbije nazad prema dole
                    if (izmedurazina)
                    {
                        if (!guster.TouchingSprite(kamenPrivremeni) && guster.Y + guster.Heigth > kamenPrivremeni.Y)
                        {
                            while (guster.Y - guster.Heigth < GameOptions.DownEdge)
                            {
                                guster.Y += guster.Brzina;
                                Wait(0.05);
                                if (guster.TouchingSprite(kamen1) || guster.TouchingSprite(kamen2) || guster.TouchingSprite(kamen3))
                                {

                                    break;
                                }
                            }

                        }
                    }
                }


                Wait(0.01);
            }
            return 0;
        }





        private int Kamen2Kretanje()
        {
            // klasicna kretnja srednjeg donjeg kamena od 1. prema 3. i obrnuto
            kamen2.X += kamen2.Brzina;
            int br_koraka = 1;
            bool desno = true;
            while (START)
            {

                if (kamen2.X >= kamen3.X - kamen2.Width - 20)
                {
                    kamen2.X -= kamen2.Brzina;
                    if (guster.TouchingSprite(kamen2))
                    {
                        guster.X -= kamen2.Brzina;
                    }
                }
                if (br_koraka >= 0 && br_koraka < 20)

                { kamen2.X += kamen2.Brzina;
                    br_koraka++;


                }


                else if (br_koraka == 20)
                {
                    desno = false;
                    kamen2.X -= kamen2.Brzina;
                    if (kamen2.TouchingSprite(kamen1))
                    {
                        br_koraka = 0;
                        desno = true;

                    }

                }

                if (guster.TouchingSprite(kamen2))
                {
                    if (desno)
                    {
                        guster.X += kamen2.Brzina;
                    }
                    else
                    {
                        guster.X -= kamen2.Brzina;
                    }

                }


                Wait(0.05);
            }
            return 0;
        }


        private int Skok()
        {
            while (START)
            {
                bool pad = true;
                if (sensing.KeyPressed("W"))
                {
                    // zbog pada prema dole i udaranja u kamenje na drugoj razini
                    if (prvarazina)
                    {
                        int i;
                        // skok prema gore
                        for (i = 0; i < 20; i++)
                        {
                            guster.Y -= guster.Brzina / 3;
                            // ako je guster ispod kamena (prvi gornji s lijeve strane i udari u njega vraca ga nazad dole)
                            if (guster.Y <= kamen4.Y && (guster.X >= kamen4.X && guster.X <= kamen4.X + kamen4.Width) && !izmedurazina)
                            {
                                guster.Y = kamen4.Y + kamen4.Heigth + guster.Heigth;
                            }
                            // analogno kao za prvi kamen gore slijeva
                            if (guster.Y <= kamen5.Y && (guster.X >= kamen5.X && guster.X <= kamen5.X + kamen5.Width) && !izmedurazina)
                            {
                                guster.Y = kamen5.Y + kamen5.Heigth + guster.Heigth;
                            }
                            if (guster.Y <= kamenPrivremeni.Y && (guster.X >= kamenPrivremeni.X && guster.X <= kamenPrivremeni.X + kamenPrivremeni.Width) && !prvarazina)
                            {
                                guster.Y = kamenPrivremeni.Y + kamenPrivremeni.Heigth + guster.Heigth;
                            }

                            // lijevo desno u zraku
                            if (sensing.KeyPressed("D"))
                            {
                                guster.X += guster.Brzina / 4;
                            }
                            if (sensing.KeyPressed("A"))
                            {
                                guster.X -= guster.Brzina / 4;
                            }
                            Wait(0.01);
                        }
                        // ZA pad
                        while (pad)
                        {
                            // guster pada prema dole 
                            guster.Y += guster.Brzina / 4;
                            // lijevo desno kretnje u padu
                            if (sensing.KeyPressed("D"))
                            {
                                guster.X += guster.Brzina / 4;
                            }

                            if (sensing.KeyPressed("A"))
                            {
                                guster.X -= guster.Brzina / 4;
                            }
                            // ako guster dotakne prvi kamen pri padu pad se zaustavlja i guster ostaje na tom kamenu
                            // prva razina je u TRUE jer je ostao na prvoj razini, a druga razina ide u FALSE ukoliko padne s 2. razine na 1.
                            if (guster.TouchingSprite(kamen1) && guster.Y + 1 > kamen1.Y)
                            {
                                pad = false;
                                guster.Y = kamen1.Y - kamen1.Heigth;
                                prvarazina = true;
                                drugarazina = false;
                                break;
                            }
                            // analogno za ostalo kamenje
                            else if (guster.TouchingSprite(kamen2) && guster.Y + 1 > kamen2.Y)
                            {
                                pad = false;
                                guster.Y = kamen2.Y - kamen2.Heigth;
                                prvarazina = true;
                                drugarazina = false;
                                break;

                            }

                            else if (guster.TouchingSprite(kamen3) && guster.Y + 1 > kamen3.Y)
                            {
                                pad = false;
                                guster.Y = kamen3.Y - kamen3.Heigth;
                                prvarazina = true;
                                izmedurazina = false;
                                drugarazina = false;
                                break;

                            }
                            else if (guster.TouchingSprite(kamenPrivremeni) && guster.Y + 1 > kamenPrivremeni.Y)
                            {
                                pad = false;
                                guster.Y = kamenPrivremeni.Y - kamenPrivremeni.Heigth;
                                izmedurazina = true;
                                drugarazina = false;

                                break;
                            }
                            // ako guster stane na prvi gornji kamen privremeni kamen nestaje te ce se ponovo pojaviti samo s donjeg 3. kamena
                            else if (guster.TouchingSprite(kamen5) && guster.Y + 1 < kamen5.Y)
                            {
                                pad = false;
                                izmedurazina = true;
                                drugarazina = true;
                                guster.Y = kamen5.Y - kamen5.Heigth;
                                kamenPrivremeni.SetVisible(false);
                                break;


                            }
                            else if (guster.TouchingSprite(kamen4) && guster.Y - 1 < kamen4.Y)
                            {
                                pad = false;
                                izmedurazina = false;
                                guster.Y = kamen4.Y - kamen4.Heigth;
                                break;
                            }
                            // u slucaju da ne dodirne nijedan kamen guster pada skroz do dna
                            else
                            {
                                pad = true;
                            }
                            Wait(0.01);
                        }

                    }


                }
            }
            return 0;
        }


        private int BubeJedi()
        {

            // kad guster pojede prvu bubu, pojavljuje se mali kamen za odskok na drugu razinu

            while (START)
            {
                int brbuba = 0;
                if (guster.TouchingSprite(buba1))
                {

                    buba1.SetVisible(false);
                    brbuba += 1;
                    kamenPrivremeni.SetVisible(true);

                    vrata_zatvorena.SetVisible(true);


                }
                if (guster.TouchingSprite(buba2))
                {

                    buba2.SetVisible(false);
                    brbuba += 1;
                    vrata_zatvorena.SetVisible(false);
                    vrata_otvorena.SetVisible(true);


                }

                if (guster.TouchingSprite(vrata_otvorena))
                {
                    guster.SetVisible(false);
                    MessageBoxButtons buttons1 = MessageBoxButtons.OK;

                    DialogResult dr = MessageBox.Show("Čestitamo!\nZavršili ste prvi level.", "", buttons1);

                    // Korisniku se čestita na uspješnom kraju igrice i otvaraju mu se vrata za novi level
                    if (dr == DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }

            }
            return 0;
        }


        private int GameOver()
        {

            while (START)
            {
                // AKO guster bude ispod dna igra zavrsava i pokaziva se poruka od game over!
                if (guster.Y - guster.Heigth > GameOptions.DownEdge)
                {
                    Wait(0.05);
                    kamen1.SetVisible(false);
                    kamen2.SetVisible(false);
                    kamen3.SetVisible(false);
                    kamenPrivremeni.SetVisible(false);
                    kamen4.SetVisible(false);
                    kamen5.SetVisible(false);
                    vrata_otvorena.SetVisible(false);
                    vrata_zatvorena.SetVisible(false);
                    guster.SetVisible(false);
                    buba1.SetVisible(false);
                    buba2.SetVisible(false);
                    setBackgroundPicture("backgrounds\\fin.png");
                    setPictureLayout("center");
                    Wait(0.01);
                    MessageBoxButtons buttons = MessageBoxButtons.RetryCancel;

                    DialogResult dr = MessageBox.Show("Pokusaj ponovo?", "Zelis pokusati ponovo?", buttons);

                    // Korisniku se nudi opcija za ponovo pokretanje igre ispocetka te opcija za izlazak iz aplikacije
                    if (dr == DialogResult.Retry)
                    {
                        Application.Restart();
                    }


                    else
                    {
                        START = false;
                        Application.Exit();
                    }
                    

                    
                }
            }
            
            return 0;
        }

        /* ------------ GAME CODE END ------------ */


    }
}
