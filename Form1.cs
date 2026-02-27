using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeNavigationButtons();
        }

        private void InitializeNavigationButtons()
        {
            // Form1 Einstellungen
            this.Text = "Stroop Test - Hauptmenü";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Titel-Label
            Label lblTitel = new Label
            {
                Text = "Stroop Test",
                Location = new Point(120, 30),
                Size = new Size(200, 40),
                Font = new Font("Arial", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitel);

            // Button zu Form2
            Button btnZuForm2 = new Button
            {
                Text = "Zum Stroop Test",
                Location = new Point(100, 100),
                Size = new Size(180, 40),
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnZuForm2.Click += BtnZuForm2_Click;
            this.Controls.Add(btnZuForm2);

            // Beenden Button (bereits vorhanden, aber wir passen ihn an)
            Beendenbtn.Text = "Beenden";
            Beendenbtn.Location = new Point(100, 160);
            Beendenbtn.Size = new Size(180, 40);
            Beendenbtn.Font = new Font("Arial", 12, FontStyle.Regular);
            Beendenbtn.BackColor = Color.LightCoral;
            Beendenbtn.FlatStyle = FlatStyle.Flat;
        }

        private void BtnZuForm2_Click(object sender, EventArgs e)
        {
            // Form2 öffnen und Form1 verstecken
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();

            // Event-Handler für das Schließen von Form2
            form2.FormClosed += (s, args) =>
            {
                // Form1 wieder anzeigen, wenn Form2 geschlossen wird
                this.Show();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialisierung bei Form1-Load
        }

        private void Beendenbtn_Click(object sender, EventArgs e)
        {
            // Anwendung beenden
            Application.Exit();
        }
    }
}