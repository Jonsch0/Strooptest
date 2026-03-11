using System;
using System.Collections.Generic;
using System.Drawing;
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

        private List<string> woerter = new List<string> { "Rot", "Blau", "Gelb", "Grün", "Orange", "Lila", "Pink", "Braun", "Cyan" };
        private List<Color> farben = new List<Color> {
            Color.Red,      // Rot
            Color.Blue,     // Blau
            Color.Yellow,   // Gelb
            Color.Green,    // Grün
            Color.Orange,   // Orange
            Color.Purple,   // Lila
            Color.HotPink,  // Pink
            Color.Brown,    // Braun
            Color.Cyan      // Cyan
        };

        private Random random = new Random();
        private bool warningShown = false;
        private int punkte = 0;

        // Timer Variablen
        private System.Windows.Forms.Timer spielTimer;
        private TimeSpan vergangeneZeit;
        private bool timerLaeuft = false;

        public Form2()
        {
            InitializeComponent();
            InitializeTimer(); // WICHTIG: Erst Timer initialisieren!
            InitializeGameComponents();
        }

        private void InitializeTimer()
        {
            // Timer initialisieren
            spielTimer = new System.Windows.Forms.Timer();
            spielTimer.Interval = 1000; // 1 Sekunde
            spielTimer.Tick += SpielTimer_Tick;

            vergangeneZeit = TimeSpan.Zero;
        }

        private void SpielTimer_Tick(object sender, EventArgs e)
        {
            // Eine Sekunde hinzufügen
            vergangeneZeit = vergangeneZeit.Add(TimeSpan.FromSeconds(1));

            // Zeit im Format MM:SS anzeigen
            if (lblZeit != null)
            {
                lblZeit.Text = $"Zeit: {vergangeneZeit:mm\\:ss}";
            }
        }

        private void StarteTimer()
        {
            if (spielTimer == null)
            {
                InitializeTimer();
            }

            if (!timerLaeuft)
            {
                vergangeneZeit = TimeSpan.Zero;
                if (lblZeit != null)
                {
                    lblZeit.Text = "Zeit: 00:00";
                }
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
                BackColor = Color.LightYellow,
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

            // Neues Spiel starten
            gameBoard.AnzahlPictureBoxes = 9;
            StarteNeueRunde();
            StarteTimer(); // Timer nach der UI-Initialisierung starten
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            // Spiel zurücksetzen
            punkte = 0;
            lblPunkte.Text = "Punkte: 0";

            // Timer zurücksetzen
            StoppeTimer();
            vergangeneZeit = TimeSpan.Zero;
            lblZeit.Text = "Zeit: 00:00";

            // Neue Runde starten
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

            // Timer zurücksetzen
            StoppeTimer();
            vergangeneZeit = TimeSpan.Zero;
            lblZeit.Text = "Zeit: 00:00";

            StarteNeueRunde();
            StarteTimer();
        }

        private void StarteNeueRunde()
        {
            // Neues Wort oben generieren
            int wortIndex = random.Next(woerter.Count);
            int farbenIndex = random.Next(farben.Count);

            lblWort.Text = woerter[wortIndex];
            lblWort.ForeColor = farben[farbenIndex];

            // Hintergrundfarbe anpassen für bessere Lesbarkeit
            lblWort.BackColor = (farben[farbenIndex] == Color.White ||
                                farben[farbenIndex] == Color.Yellow ||
                                farben[farbenIndex] == Color.HotPink)
                                ? Color.DarkGray
                                : Color.LightYellow;

            // Dem GameBoard mitteilen, welches Wort gesucht wird
            gameBoard.SetzeGesuchtesWort(farben[farbenIndex]);

            // Grid mit neuen Wörtern füllen
            gameBoard.GeneriereAlleLabelsNeu();
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

            // Kurze Pause zur Visualisierung, aber Zeit läuft weiter
            System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    StarteNeueRunde();
                });
            });
        }

        // Form schließen - Timer stoppen
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StoppeTimer();
            base.OnFormClosing(e);
        }
    }
}
