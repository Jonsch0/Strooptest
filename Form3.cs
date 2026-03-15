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
        private Label lblZeit;
        private Label lblLetzteAntwort;

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

        private System.Windows.Forms.Timer spielTimer;
        private TimeSpan vergangeneZeit;
        private bool timerLaeuft = false;

        private string aktuelleSchwierigkeit = "Schwer";

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
            this.Size = new Size(650, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Zurück-Button
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

            // Leben-Anzeige
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

            // Zeit-Anzeige
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

            // Letzte Antwort-Anzeige
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

            // Wort-Label oben
            lblWort = new Label
            {
                Text = "Rot",
                Location = new Point(20, 90),
                Size = new Size(550, 60),
                Font = new Font("Arial", 28, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblWort);

            // Game Panel
            gamePanel = new Panel
            {
                Location = new Point(20, 160),
                Size = new Size(550, 520),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(gamePanel);

            // GameBoard initialisieren (immer 9 PictureBoxes)
            gameBoard = new StroopNEinzelspieler(gamePanel);
            gameBoard.AnzahlPictureBoxes = 9;
            gameBoard.CorrectAnswerSelected += GameBoard_CorrectAnswerSelected;
            gameBoard.WrongAnswerSelected += GameBoard_WrongAnswerSelected;

            // Reset Button
            Button btnReset = new Button
            {
                Text = "Neustart",
                Location = new Point(20, 690),
                Size = new Size(100, 30),
                BackColor = Color.LightYellow
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            // ---- Schwierigkeitsauswahl ----
            GroupBox grpSchwierigkeit = new GroupBox
            {
                Text = "Schwierigkeit",
                Location = new Point(140, 680),
                Size = new Size(380, 60),
                BackColor = Color.Transparent
            };

            RadioButton rbLeicht = new RadioButton
            {
                Text = "Leicht (3 Labels)",
                Location = new Point(20, 25),
                Size = new Size(120, 25),
                Checked = false
            };
            rbLeicht.CheckedChanged += (s, e) =>
            {
                if (rbLeicht.Checked)
                {
                    aktuelleSchwierigkeit = "Leicht";
                    StarteNeueRunde();
                }
            };

            RadioButton rbMittel = new RadioButton
            {
                Text = "Mittel (6 Labels)",
                Location = new Point(150, 25),
                Size = new Size(120, 25),
                Checked = false
            };
            rbMittel.CheckedChanged += (s, e) =>
            {
                if (rbMittel.Checked)
                {
                    aktuelleSchwierigkeit = "Mittel";
                    StarteNeueRunde();
                }
            };

            RadioButton rbSchwer = new RadioButton
            {
                Text = "Schwer (9 Labels)",
                Location = new Point(280, 25),
                Size = new Size(120, 25),
                Checked = true
            };
            rbSchwer.CheckedChanged += (s, e) =>
            {
                if (rbSchwer.Checked)
                {
                    aktuelleSchwierigkeit = "Schwer";
                    StarteNeueRunde();
                }
            };

            grpSchwierigkeit.Controls.AddRange(new Control[] { rbLeicht, rbMittel, rbSchwer });
            this.Controls.Add(grpSchwierigkeit);

            // --- Automatische Anpassung an Bildschirm ---
            this.AutoScroll = true;
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int desiredHeight = (int)(screen.Height * 0.9);
            this.Height = Math.Min(750, desiredHeight);
            this.Width = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Erste Runde starten
            StarteNeueRunde();
            StarteTimer();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            leben = startLeben;
            lblLeben.Text = $"Leben: {leben}";
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StoppeTimer();
            base.OnFormClosing(e);
        }
    }
}