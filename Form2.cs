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

        private string[] woerter = { "Rot", "Blau", "Gelb", "Schwarz", "Weiß" };
        private Color[] farben = { Color.Red, Color.Blue, Color.Yellow, Color.Black, Color.White };

        private Random random = new Random();
        private bool warningShown = false;

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
            this.Size = new Size(650, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnZurueck = new Button
            {
                Text = "← Zurück zum Hauptmenü",
                Location = new Point(20, 20),
                Size = new Size(180, 30),
                BackColor = Color.LightBlue
            };
            btnZurueck.Click += (s, e) => this.Close();
            this.Controls.Add(btnZurueck);

            lblWort = new Label
            {
                Text = "Rot",
                Location = new Point(20, 60),
                Size = new Size(550, 50),
                Font = new Font("Arial", 24, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblWort);

            gamePanel = new Panel
            {
                Location = new Point(20, 120),
                Size = new Size(550, 520),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(gamePanel);

            gameBoard = new StroopNEinzelspieler(gamePanel);
            gameBoard.PictureBoxClicked += GameBoard_PictureBoxClicked;

            Label lblAnzahl = new Label
            {
                Text = "Anzahl PictureBoxes:",
                Location = new Point(220, 660),
                Size = new Size(130, 25)
            };
            this.Controls.Add(lblAnzahl);

            nudAnzahlPictureBoxes = new NumericUpDown
            {
                Location = new Point(350, 660),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 100, // höher setzen!
                Value = 9
            };

            nudAnzahlPictureBoxes.ValueChanged += NudAnzahlPictureBoxes_ValueChanged;
            this.Controls.Add(nudAnzahlPictureBoxes);

            btnGridErstellen = new Button
            {
                Text = "Grid erstellen",
                Location = new Point(440, 658),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen
            };
            btnGridErstellen.Click += BtnGridErstellen_Click;
            this.Controls.Add(btnGridErstellen);

            gameBoard.AnzahlPictureBoxes = 9;
            AendereWort();
        }

        // 🔴 HIER kommt deine gewünschte MessageBox
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
                        "Bro chill mal",
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
        }

        private void AendereWort()
        {
            int wortIndex = random.Next(woerter.Length);
            lblWort.Text = woerter[wortIndex];

            int farbenIndex = random.Next(farben.Length);
            lblWort.ForeColor = farben[farbenIndex];

            lblWort.BackColor = lblWort.ForeColor == Color.White
                ? Color.DarkGray
                : Color.LightYellow;
        }

        private void AenderePictureBoxFarbe(PictureBox pictureBox)
        {
            int farbenIndex = random.Next(farben.Length);
            pictureBox.BackColor = farben[farbenIndex];
        }

        private void GameBoard_PictureBoxClicked(object sender, PictureBoxClickEventArgs e)
        {
            AendereWort();
            AenderePictureBoxFarbe(e.ClickedPictureBox);
        }
    }
}
