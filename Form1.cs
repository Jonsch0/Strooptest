using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form1 : Form
    {
        private NumericUpDown nudLeben;
        public Label lblStatus;
        public TextBox tbCode;
        Sockets sockets = new Sockets();
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

            Button btnBeitreten = new Button //!!!
            {
                Text = "Spiel beitreten",
                Location = new Point(500, 170),
                Size = new Size(180, 40),
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnBeitreten.Click += btnBeitreten_Click;
            this.Controls.Add(btnBeitreten);

            lblStatus = new Label
            {
                Text = "[Verbindungsstatus]",
                Location = new Point(500, 120),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblStatus); 

            Button btnHosten = new Button
            {
                Text = "Spiel hosten",
                Location = new Point(500, 70),
                Size = new Size(180, 40),
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnHosten.Click += btnHosten_Click;
            this.Controls.Add(btnHosten);

            tbCode = new TextBox
            {
                Text = "[Verbindungscode]",
                Location = new Point(500, 170),
                Size = new Size(120, 20),
                ForeColor = Color.Green,
            }; 

            this.Controls.Add(tbCode);

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

        public void btnHosten_Click(object sender, EventArgs e) //hostBtn, statusLbl, beitretenBtn
        {
            string code = sockets.StarteHost();

            lblStatus.Text = "Host Code: " + code;

            Thread thread = new Thread(() =>
            {
                sockets.WarteAufSpieler(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Spieler verbunden, Spiel startet";
                    });
                });
            });

            thread.Start();
        }

        private void btnBeitreten_Click(object sender, EventArgs e)
        {
            string code = tbCode2.Text;

            lblStatus.Text = "Verbinde...";

            Thread thread = new Thread(() =>
            {
                bool success = sockets.Beitreten(code);

                Invoke((MethodInvoker)delegate
                {
                    if (success)
                        lblStatus.Text = "Spiel startet";
                    else
                        lblStatus.Text = "Verbindung fehlgeschlagen";
                });
            });

            thread.Start();
        }
        private void Form1_Load(object sender, EventArgs e) { }

        private void Beendenbtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}