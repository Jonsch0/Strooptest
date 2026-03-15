using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form2 : Form
    {
        private StroopNEinzelspieler gameBoard;
        private NumericUpDown nudAnzahlPictureBoxes;
        private Button btnGridErstellen;
        private Button btnZurueck;
        private Panel gamePanel;
        private Label lblWort;
        private Label lblPunkte;
        private Label lblZeit;
        private Label lblLetzteAntwort;
      
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
        private bool warningShown = false;
        private int punkte = 0;

        private System.Windows.Forms.Timer spielTimer;
        private TimeSpan vergangeneZeit;
        private bool timerLaeuft = false;

        private string aktuelleSchwierigkeit = "Schwer";

        public Form2()
        {
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

        private void Form2_Load(object sender, EventArgs e) { }

        private void InitializeGameComponents()
        {
            this.Text = "Stroop Test - Spiel";
            this.Size = new Size(650, 850);
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

            // Punkte-Anzeige
            lblPunkte = new Label
            {
                Text = "Punkte: 0",
                Location = new Point(500, 20),
                Size = new Size(120, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblPunkte);

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

            // GameBoard initialisieren
            gameBoard = new StroopNEinzelspieler(gamePanel);
            gameBoard.CorrectAnswerSelected += GameBoard_CorrectAnswerSelected;
            gameBoard.WrongAnswerSelected += GameBoard_WrongAnswerSelected;

            // Anzahl PictureBoxes Label
            Label lblAnzahl = new Label
            {
                Text = "Anzahl PictureBoxes:",
                Location = new Point(220, 700),
                Size = new Size(130, 25)
            };
            this.Controls.Add(lblAnzahl);

            // NumericUpDown für Anzahl
            nudAnzahlPictureBoxes = new NumericUpDown
            {
                Location = new Point(350, 700),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 100,
                Value = 9
            };
            nudAnzahlPictureBoxes.ValueChanged += NudAnzahlPictureBoxes_ValueChanged;
            this.Controls.Add(nudAnzahlPictureBoxes);

            // Grid Erstellen Button
            btnGridErstellen = new Button
            {
                Text = "Grid erstellen",
                Location = new Point(440, 698),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnGridErstellen.Click += BtnGridErstellen_Click;
            this.Controls.Add(btnGridErstellen);

            // Reset Button
            Button btnReset = new Button
            {
                Text = "Neustart",
                Location = new Point(20, 698),
                Size = new Size(100, 30),
                BackColor = Color.LightYellow
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            // ---- Schwierigkeitsauswahl ----
            GroupBox grpSchwierigkeit = new GroupBox
            {
                Text = "Schwierigkeit",
                Location = new Point(20, 740),
                Size = new Size(550, 60),
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
                    nudAnzahlPictureBoxes.Value = 9;
                    nudAnzahlPictureBoxes.Enabled = false;
                    if (gameBoard.GetAnzahlPictureBoxes() > 0)
                        BtnGridErstellen_Click(null, null);
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
                    nudAnzahlPictureBoxes.Value = 9;
                    nudAnzahlPictureBoxes.Enabled = false;
                    if (gameBoard.GetAnzahlPictureBoxes() > 0)
                        BtnGridErstellen_Click(null, null);
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
                    nudAnzahlPictureBoxes.Value = 9;
                    nudAnzahlPictureBoxes.Enabled = false;
                    if (gameBoard.GetAnzahlPictureBoxes() > 0)
                        BtnGridErstellen_Click(null, null);
                }
            };

            grpSchwierigkeit.Controls.AddRange(new Control[] { rbLeicht, rbMittel, rbSchwer });
            this.Controls.Add(grpSchwierigkeit);

            // NumericUpDown standardmäßig deaktivieren (wegen Schwierigkeit "Schwer")
            nudAnzahlPictureBoxes.Enabled = false;

            // --- Automatische Anpassung an Bildschirm ---
            this.AutoScroll = true;
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int desiredHeight = (int)(screen.Height * 0.9);
            this.Height = Math.Min(850, desiredHeight);
            this.Width = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Neues Spiel starten
            gameBoard.AnzahlPictureBoxes = 9;
            StarteNeueRunde();
            StarteTimer();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            punkte = 0;
            lblPunkte.Text = "Punkte: 0";
            StoppeTimer();
            vergangeneZeit = TimeSpan.Zero;
            lblZeit.Text = "Zeit: 00:00";
            StarteNeueRunde();
            StarteTimer();
        }

        private void NudAnzahlPictureBoxes_ValueChanged(object sender, EventArgs e)
        {
            if (nudAnzahlPictureBoxes.Value > 40)
            {
                if (!warningShown)
                {
                    warningShown = true;
                    MessageBox.Show(
                        "Die Anzahl der PictureBoxes darf 40 nicht überschreiten!\n" +
                        "Bitte wählen Sie einen Wert zwischen 1 und 40.",
                        "Hinweis",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                nudAnzahlPictureBoxes.Value = 40;
                warningShown = false;
            }
        }

        private void BtnGridErstellen_Click(object sender, EventArgs e)
        {
            gameBoard.AnzahlPictureBoxes = (int)nudAnzahlPictureBoxes.Value;
            punkte = 0;
            lblPunkte.Text = "Punkte: 0";
            StoppeTimer();
            vergangeneZeit = TimeSpan.Zero;
            lblZeit.Text = "Zeit: 00:00";
            StarteNeueRunde();
            StarteTimer();
        }

        // Hilfsmethode: Farbe -> Name
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

        // Hilfsmethode: Prüft, ob Wort und Farbe übereinstimmen (für Stroop-Effekt)
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
            // 1. Grid generieren (alle Zellen erhalten Wort+Farbe)
            gameBoard.GeneriereAlleLabelsNeu();

            // 2. Sichtbarkeit nach Schwierigkeit setzen
            gameBoard.SetzeSichtbareLabelsNachSchwierigkeit(aktuelleSchwierigkeit);

            // 3. Alle sichtbaren Labels sammeln
            List<Label> sichtbareLabels = new List<Label>();
            foreach (var pb in gamePanel.Controls.OfType<PictureBox>())
            {
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl && lbl.Visible)
                {
                    sichtbareLabels.Add(lbl);
                }
            }

            // Fallback, falls keine sichtbaren Labels (sollte nicht passieren)
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

            // 4. Eindeutige sichtbare Schriftfarben ermitteln
            HashSet<Color> sichtbareFarbenSet = new HashSet<Color>();
            foreach (Label lbl in sichtbareLabels)
            {
                sichtbareFarbenSet.Add(lbl.ForeColor);
            }
            List<Color> sichtbareFarben = sichtbareFarbenSet.ToList();

            // 5. Zufällige Farbe aus den sichtbaren Schriftfarben wählen (gesuchte Farbe)
            Color gesuchteFarbe = sichtbareFarben[random.Next(sichtbareFarben.Count)];

            // 6. Den Namen der gesuchten Farbe ermitteln
            string gesuchterFarbname = FarbeZuName(gesuchteFarbe);

            // 7. Prüfen, ob dieser Name in einem sichtbaren Label als Text vorkommt
            bool nameVorhanden = sichtbareLabels.Any(lbl => lbl.Text == gesuchterFarbname);

            // 8. Wenn nicht vorhanden, ersetze ein zufälliges sichtbares Label
            if (!nameVorhanden && sichtbareLabels.Count > 0)
            {
                // Wähle ein zufälliges sichtbares Label
                Label zielLabel = sichtbareLabels[random.Next(sichtbareLabels.Count)];

                // Wähle eine zufällige Farbe, die nicht die gesuchte ist und nicht mit dem neuen Wort übereinstimmt
                Color neueFarbe;
                do
                {
                    neueFarbe = farben[random.Next(farben.Count)];
                } while (neueFarbe == gesuchteFarbe || EntsprichtWortFarbe(gesuchterFarbname, neueFarbe));

                zielLabel.Text = gesuchterFarbname;
                zielLabel.ForeColor = neueFarbe;
            }

            // 9. Oberes Wort zufällig wählen, aber nicht mit der gesuchten Farbe übereinstimmen
            string oberesWort;
            do
            {
                oberesWort = woerter[random.Next(woerter.Count)];
            } while (EntsprichtWortFarbe(oberesWort, gesuchteFarbe));

            lblWort.Text = oberesWort;
            lblWort.ForeColor = gesuchteFarbe;

            // Hintergrund anpassen (bei hellen Farben dunklerer Hintergrund)
            lblWort.BackColor = (gesuchteFarbe == Color.White ||
                                gesuchteFarbe == Color.Yellow ||
                                gesuchteFarbe == Color.HotPink)
                                ? Color.DarkGray
                                : Color.LightYellow;

            // 10. Dem GameBoard mitteilen, welche Farbe gesucht wird
            gameBoard.SetzeGesuchtesWort(gesuchteFarbe);
        }

        private void GameBoard_CorrectAnswerSelected(object sender, AnswerEventArgs e)
        {
            punkte++;
            lblPunkte.Text = $"Punkte: {punkte}";

            lblLetzteAntwort.Text = $"✓ Richtig! Das Wort '{e.GeklicktesWort}' war gesucht (+1 Punkt)";
            lblLetzteAntwort.ForeColor = Color.Green;

            StarteNeueRunde();
        }

        private void GameBoard_WrongAnswerSelected(object sender, AnswerEventArgs e)
        {
            lblLetzteAntwort.Text = $"✗ Falsch! Gesucht war '{e.GesuchtesWort}', aber du hast '{e.GeklicktesWort}' geklickt";
            lblLetzteAntwort.ForeColor = Color.Red;

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

        /*public Form2(Sockets socketsRef) //!!!
        {
            InitializeComponent();

            sockets = socketsRef;

            sockets.OnLebenReceived += GegnerLebenEmpfangen;

            UpdateUI();
        }*/

        void UpdateUI()
        {
            myLifeLabel.Text = "Mein Leben: " + meinLeben;
            enemyLifeLabel.Text = "Gegner Leben: " + gegnerLeben;
        }

        void GegnerLebenEmpfangen(int leben)
        {
            gegnerLeben = leben;

            Invoke((MethodInvoker)delegate
            {
                UpdateUI();
            });
        }
    }
}