using System;
using System.Drawing;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form1 : Form
    {
        private NumericUpDown nudLeben;

        public Form1()
        {
            InitializeComponent();
            InitializeNavigationButtons();
        }

        private void InitializeNavigationButtons()
        {
            this.Text = "Stroop Test - Hauptmenü";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitel = new Label
            {
                Text = "Stroop Test",
                Location = new Point(120, 20),
                Size = new Size(200, 40),
                Font = new Font("Arial", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitel);

            // Button: Endlosmodus (Punkte) -> startet Form2
            Button btnEndlos = new Button
            {
                Text = "Endlosmodus (Punkte)",
                Location = new Point(100, 70),
                Size = new Size(180, 40),
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnEndlos.Click += BtnEndlos_Click;
            this.Controls.Add(btnEndlos);

            // Label für Leben-Einstellung (für Klassisch)
            Label lblLeben = new Label
            {
                Text = "Leben für Klassisch:",
                Location = new Point(100, 130),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblLeben);

            // NumericUpDown für Leben (5-20, Default 15)
            nudLeben = new NumericUpDown
            {
                Location = new Point(230, 128),
                Size = new Size(50, 20),
                Minimum = 5,
                Maximum = 20,
                Value = 15
            };
            this.Controls.Add(nudLeben);

            // Button: Klassisch (Leben) -> startet Form3
            Button btnKlassisch = new Button
            {
                Text = "Klassisch (Leben)",
                Location = new Point(100, 170),
                Size = new Size(180, 40),
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnKlassisch.Click += BtnKlassisch_Click;
            this.Controls.Add(btnKlassisch);

            // Beenden Button
            Beendenbtn.Text = "Beenden";
            Beendenbtn.Location = new Point(100, 240);
            Beendenbtn.Size = new Size(180, 40);
            Beendenbtn.Font = new Font("Arial", 12, FontStyle.Regular);
            Beendenbtn.BackColor = Color.LightCoral;
            Beendenbtn.FlatStyle = FlatStyle.Flat;
        }

        private void BtnEndlos_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();   // Endlosmodus (Punkte)
            form2.Show();
            this.Hide();
            form2.FormClosed += (s, args) => this.Show();
        }

        private void BtnKlassisch_Click(object sender, EventArgs e)
        {
            int leben = (int)nudLeben.Value;
            Form3 form3 = new Form3(leben); // Klassisch (Leben)
            form3.Show();
            this.Hide();
            form3.FormClosed += (s, args) => this.Show();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void Beendenbtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}