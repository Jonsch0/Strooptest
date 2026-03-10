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
        private Label lblLetzteAntwort;

        // Wort- und Farbarrays
        private string[] woerter = { "Rot", "Blau", "Gelb", "Grün", "Orange", "Lila", "Pink", "Braun", "Cyan" };
        private Color[] farben = {
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

        public Form2()
        {
            InitializeComponent();
            InitializeGameComponents();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Hier muss nichts rein
        }

        private void InitializeGameComponents()
        {
            this.Text = "Stroop Test - Spiel";
            this.Size = new Size(650, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Zurück-Button
            btnZurueck = new Button
            {
                Text = "← Zurück zum Hauptmenü",
                Location = new Point(20, 20),
                Size = new Size(180, 30),
                BackColor = Color.LightBlue
            };
            btnZurueck.Click += (s, e) => this.Close();
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

            // Letzte Antwort-Anzeige
            lblLetzteAntwort = new Label
            {
                Text = "",
                Location = new Point(20, 55),
                Size = new Size(600, 25),
                Font = new Font("Arial", 10, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblLetzteAntwort);

            // Wort-Label oben
            lblWort = new Label
            {
                Text = "Rot",
                Location = new Point(20, 85),
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
                Location = new Point(20, 155),
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
                Location = new Point(220, 695),
                Size = new Size(130, 25)
            };
            this.Controls.Add(lblAnzahl);

            // NumericUpDown für Anzahl
            nudAnzahlPictureBoxes = new NumericUpDown
            {
                Location = new Point(350, 695),
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
                Location = new Point(440, 693),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnGridErstellen.Click += BtnGridErstellen_Click;
            this.Controls.Add(btnGridErstellen);

            // Neues Spiel starten
            gameBoard.AnzahlPictureBoxes = 9;
            StarteNeueRunde();
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
            StarteNeueRunde();
        }

        private void StarteNeueRunde()
        {
            // Neues Wort oben generieren
            int wortIndex = random.Next(woerter.Length);
            int farbenIndex = random.Next(farben.Length);

            lblWort.Text = woerter[wortIndex];
            lblWort.ForeColor = farben[farbenIndex];

            // Hintergrundfarbe anpassen für bessere Lesbarkeit
            lblWort.BackColor = (farben[farbenIndex] == Color.White ||
                                farben[farbenIndex] == Color.Yellow ||
                                farben[farbenIndex] == Color.HotPink)
                                ? Color.DarkGray
                                : Color.LightYellow;

            // Dem GameBoard mitteilen, welches Wort gesucht wird (basierend auf der Farbe des oberen Wortes)
            gameBoard.SetzeGesuchtesWort(farben[farbenIndex]);

            // Grid mit neuen Wörtern füllen
            gameBoard.GeneriereAlleLabelsNeu();
        }

        private void GameBoard_CorrectAnswerSelected(object sender, AnswerEventArgs e)
        {
            // Punkt erhöhen
            punkte++;
            lblPunkte.Text = $"Punkte: {punkte}";

            // Letzte Antwort anzeigen
            lblLetzteAntwort.Text = $"✓ Richtig! Das Wort '{e.GeklicktesWort}' war gesucht (+1 Punkt)";
            lblLetzteAntwort.ForeColor = Color.Green;

            // Neue Runde starten
            StarteNeueRunde();
        }

        private void GameBoard_WrongAnswerSelected(object sender, AnswerEventArgs e)
        {
            // Letzte Antwort anzeigen
            lblLetzteAntwort.Text = $"✗ Falsch! Gesucht war '{e.GesuchtesWort}', aber du hast '{e.GeklicktesWort}' geklickt";
            lblLetzteAntwort.ForeColor = Color.Red;

            // Keine neue Runde, aber kurze Pause zur Visualisierung
            System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // Neue Runde starten
                    StarteNeueRunde();
                });
            });
        }
    }
}
