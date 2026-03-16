using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form3 : Form
    {
        private StroopNEinzelspieler gameBoard;
        private Button btnZurueck;
        private Panel gamePanel;
        private Label lblWort;
        private Label lblLeben;
        private Label lblMeinLeben; //!!!
        private Label lblGegnerLeben; //!!!
        private Label lblZeit;
        private Label lblLetzteAntwort;
        private Label lblSchwierigkeit;
        private Label lblRundenZaehler;
        Sockets sockets; //!!!

        int meinLeben = 15;
        int gegnerLeben = 15; //!!!

        private List<string> woerter = new List<string> { "Rot", "Blau", "Gelb", "Grün", "Orange", "Lila", "Pink", "Braun", "Cyan" };
        private List<Color> farben = new List<Color> {
            Color.Red,          // Rot
            Color.Blue,         // Blau
            Color.Yellow,       // Gelb
            Color.LimeGreen,    // Grün
            Color.Orange,       // Orange
            Color.Purple,       // Lila
            Color.HotPink,      // Pink
            Color.SaddleBrown,  // Braun
            Color.Cyan          // Cyan
        };

        private Random random = new Random();
        private int leben;
        private int startLeben;
        private int rundenZaehler = 0;

        private System.Windows.Forms.Timer spielTimer;
        private TimeSpan vergangeneZeit;
        private bool timerLaeuft = false;

        private string aktuelleSchwierigkeit;

        public Form3(int startLeben)
        {
            this.startLeben = startLeben;
            leben = startLeben;
            InitializeComponent();
            InitializeTimer();
            InitializeGameComponents();
        }

        private void InitializeTimer()
        {
            spielTimer = new System.Windows.Forms.Timer();
            spielTimer.Interval = 1000;
            spielTimer.Tick += SpielTimer_Tick;
            vergangeneZeit = TimeSpan.Zero;
        }

        private void SpielTimer_Tick(object sender, EventArgs e)
        {
            vergangeneZeit = vergangeneZeit.Add(TimeSpan.FromSeconds(1));
            if (lblZeit != null)
                lblZeit.Text = $"Zeit: {vergangeneZeit:mm\\:ss}";
        }

        private void StarteTimer()
        {
            if (spielTimer == null) InitializeTimer();
            if (!timerLaeuft)
            {
                vergangeneZeit = TimeSpan.Zero;
                if (lblZeit != null) lblZeit.Text = "Zeit: 00:00";
                spielTimer.Start();
                timerLaeuft = true;
            }
        }

        private void StoppeTimer()
        {
            if (spielTimer != null && timerLaeuft)
            {
                spielTimer.Stop();
                timerLaeuft = false;
            }
        }

        private void Form3_Load(object sender, EventArgs e) { }

        private void InitializeGameComponents()
        {
            this.Text = "Stroop Test - Klassisch (Leben)";
            this.Size = new Size(650, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnZurueck = new Button
            {
                Text = "← Zurück zum Hauptmenü",
                Location = new Point(20, 20),
                Size = new Size(180, 30),
                BackColor = Color.LightBlue
            };
            btnZurueck.Click += (s, e) =>
            {
                StoppeTimer();
                this.Close();
            };
            this.Controls.Add(btnZurueck);

            lblLeben = new Label
            {
                Text = $"Leben: {leben}",
                Location = new Point(500, 20),
                Size = new Size(120, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblLeben);

            lblMeinLeben = new Label //!!!
            {
                Text = $"Leben: {meinLeben}",
                Location = new Point(500, 60),
                Size = new Size(120, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblMeinLeben);

            lblGegnerLeben = new Label //!!!
            {
                Text = $"Leben: {gegnerLeben}",
                Location = new Point(500, 100),
                Size = new Size(120, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblGegnerLeben);

            lblZeit = new Label
            {
                Text = "Zeit: 00:00",
                Location = new Point(20, 55),
                Size = new Size(150, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblZeit);

            lblLetzteAntwort = new Label
            {
                Text = "",
                Location = new Point(180, 55),
                Size = new Size(400, 25),
                Font = new Font("Arial", 10, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblLetzteAntwort);

            lblSchwierigkeit = new Label
            {
                Text = "Schwierigkeit: Leicht",
                Location = new Point(20, 85),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(lblSchwierigkeit);

            lblRundenZaehler = new Label
            {
                Text = "Runde: 0",
                Location = new Point(250, 85),
                Size = new Size(100, 20),
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.Black
            };
            this.Controls.Add(lblRundenZaehler);

            lblWort = new Label
            {
                Text = "Rot",
                Location = new Point(20, 110),
                Size = new Size(550, 60),
                Font = new Font("Arial", 28, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblWort);

            gamePanel = new Panel
            {
                Location = new Point(20, 180),
                Size = new Size(550, 460),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(gamePanel);

            gameBoard = new StroopNEinzelspieler(gamePanel);
            gameBoard.AnzahlPictureBoxes = 9;
            gameBoard.CorrectAnswerSelected += GameBoard_CorrectAnswerSelected;
            gameBoard.WrongAnswerSelected += GameBoard_WrongAnswerSelected;

            Button btnReset = new Button
            {
                Text = "Neustart",
                Location = new Point(20, 650),
                Size = new Size(100, 30),
                BackColor = Color.LightYellow
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            this.AutoScroll = true;
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int desiredHeight = (int)(screen.Height * 0.9);
            this.Height = Math.Min(720, desiredHeight);
            this.Width = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            StarteNeueRunde();
            StarteTimer();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            leben = startLeben;
            rundenZaehler = 0;
            lblLeben.Text = $"Leben: {leben}";
            lblRundenZaehler.Text = "Runde: 0";
            StoppeTimer();
            vergangeneZeit = TimeSpan.Zero;
            lblZeit.Text = "Zeit: 00:00";
            StarteNeueRunde();
            StarteTimer();
        }

        private string FarbeZuName(Color farbe)
        {
            if (farbe == Color.Red) return "Rot";
            if (farbe == Color.Blue) return "Blau";
            if (farbe == Color.Yellow) return "Gelb";
            if (farbe == Color.LimeGreen) return "Grün";
            if (farbe == Color.Orange) return "Orange";
            if (farbe == Color.Purple) return "Lila";
            if (farbe == Color.HotPink) return "Pink";
            if (farbe == Color.SaddleBrown) return "Braun";
            if (farbe == Color.Cyan) return "Cyan";
            return "";
        }

        private bool EntsprichtWortFarbe(string wort, Color farbe)
        {
            if (wort == "Rot" && farbe == Color.Red) return true;
            if (wort == "Blau" && farbe == Color.Blue) return true;
            if (wort == "Gelb" && farbe == Color.Yellow) return true;
            if (wort == "Grün" && farbe == Color.LimeGreen) return true;
            if (wort == "Orange" && farbe == Color.Orange) return true;
            if (wort == "Lila" && farbe == Color.Purple) return true;
            if (wort == "Pink" && farbe == Color.HotPink) return true;
            if (wort == "Braun" && farbe == Color.SaddleBrown) return true;
            if (wort == "Cyan" && farbe == Color.Cyan) return true;
            return false;
        }

        private void StarteNeueRunde()
        {
            rundenZaehler++;
            lblRundenZaehler.Text = $"Runde: {rundenZaehler}";

            aktuelleSchwierigkeit = Schwierigkeit.ErmittleSchwierigkeit(rundenZaehler);
            lblSchwierigkeit.Text = $"Schwierigkeit: {aktuelleSchwierigkeit}";

            int maxVersuche = 200;
            int versuch = 0;
            bool gefunden = false;
            Color gesuchteFarbe = Color.Empty;
            List<Label> sichtbareLabels = null;

            while (!gefunden && versuch < maxVersuche)
            {
                gameBoard.GeneriereAlleLabelsNeu();
                gameBoard.SetzeSichtbareLabelsNachSchwierigkeit(aktuelleSchwierigkeit);

                sichtbareLabels = new List<Label>();
                foreach (var pb in gamePanel.Controls.OfType<PictureBox>())
                {
                    if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl && lbl.Visible)
                    {
                        sichtbareLabels.Add(lbl);
                    }
                }

                if (sichtbareLabels.Count == 0)
                {
                    foreach (var pb in gamePanel.Controls.OfType<PictureBox>())
                    {
                        if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                        {
                            lbl.Visible = true;
                            sichtbareLabels.Add(lbl);
                            break;
                        }
                    }
                }

                HashSet<Color> sichtbareFarbenSet = new HashSet<Color>();
                HashSet<string> sichtbareTexteSet = new HashSet<string>();
                foreach (Label lbl in sichtbareLabels)
                {
                    sichtbareFarbenSet.Add(lbl.ForeColor);
                    sichtbareTexteSet.Add(lbl.Text);
                }

                List<Color> moeglicheFarben = new List<Color>();
                foreach (Color farbe in sichtbareFarbenSet)
                {
                    string name = FarbeZuName(farbe);
                    if (sichtbareTexteSet.Contains(name))
                    {
                        moeglicheFarben.Add(farbe);
                    }
                }

                if (moeglicheFarben.Count > 0)
                {
                    gefunden = true;
                    gesuchteFarbe = moeglicheFarben[random.Next(moeglicheFarben.Count)];
                }
                versuch++;
            }

            if (!gefunden)
            {
                List<Color> alleSichtbarenFarben = sichtbareLabels.Select(lbl => lbl.ForeColor).Distinct().ToList();
                gesuchteFarbe = alleSichtbarenFarben[random.Next(alleSichtbarenFarben.Count)];
            }

            string oberesWort;
            do
            {
                oberesWort = woerter[random.Next(woerter.Count)];
            } while (EntsprichtWortFarbe(oberesWort, gesuchteFarbe));

            lblWort.Text = oberesWort;
            lblWort.ForeColor = gesuchteFarbe;

            lblWort.BackColor = (gesuchteFarbe == Color.White ||
                                gesuchteFarbe == Color.Yellow ||
                                gesuchteFarbe == Color.HotPink)
                                ? Color.DarkGray
                                : Color.LightYellow;

            gameBoard.SetzeGesuchtesWort(gesuchteFarbe);
        }

        private void GameBoard_CorrectAnswerSelected(object sender, AnswerEventArgs e)
        {
            lblLetzteAntwort.Text = $"✓ Richtig! Das Wort '{e.GeklicktesWort}' war gesucht";
            lblLetzteAntwort.ForeColor = Color.Green;

            StarteNeueRunde();
        }

        private void GameBoard_WrongAnswerSelected(object sender, AnswerEventArgs e)
        {
            leben--;
            lblLeben.Text = $"Leben: {leben}";
            lblLetzteAntwort.Text = $"✗ Falsch! Gesucht war '{e.GesuchtesWort}', du hast '{e.GeklicktesWort}' geklickt";
            lblLetzteAntwort.ForeColor = Color.Red;

            if (leben <= 0)
            {
                StoppeTimer();
                MessageBox.Show("Keine Leben mehr übrig! Du hast verloren.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    StarteNeueRunde();
                });
            });
        }
  
        public Form3(Sockets socketsRef) //!!! SOCKEEEETSSSS
        {
            InitializeComponent();

            sockets = socketsRef;

            sockets.OnLebenReceived += GegnerLebenEmpfangen;

            UpdateUI();
        }

      void UpdateUI()
        {
            lblMeinLeben.Text = "Mein Leben: " + meinLeben;
            lblGegnerLeben.Text = "Gegner Leben: " + gegnerLeben;
        }

        void GegnerLebenEmpfangen(int leben)
        {
            gegnerLeben = leben;

            Invoke((MethodInvoker)delegate
            {
                UpdateUI();
            });
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StoppeTimer();
            base.OnFormClosing(e);
        }
    }
}