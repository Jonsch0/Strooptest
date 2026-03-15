using System;
using System.Drawing;
using System.Windows.Forms;

namespace Strooptest
{
    public partial class Form1 : Form
    {
        Sockets sockets = new Sockets();
        public Form1()
        {
            InitializeComponent();
            InitializeNavigationButtons();
        }

        private void InitializeNavigationButtons()
        {
            this.Text = "Stroop Test - Hauptmenü";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitel = new Label
            {
                Text = "Stroop Test",
                Location = new Point(120, 30),
                Size = new Size(200, 40),
                Font = new Font("Arial", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitel);

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

            Beendenbtn.Text = "Beenden";
            Beendenbtn.Location = new Point(100, 160);
            Beendenbtn.Size = new Size(180, 40);
            Beendenbtn.Font = new Font("Arial", 12, FontStyle.Regular);
            Beendenbtn.BackColor = Color.LightCoral;
            Beendenbtn.FlatStyle = FlatStyle.Flat;
        }

        private void BtnZuForm2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(sockets); //!!!
            form2.Show();
            this.Hide();
            form2.FormClosed += (s, args) => this.Show();
        }

        private void hostBtn_Click(object sender, EventArgs e) //hostBtn, statusLbl, beitretenBtn
        {
            string code = sockets.StarteHost();

            statusLbl.Text = "Host Code: " + code;

            Thread thread = new Thread(() =>
            {
                sockets.WarteAufSpieler(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        statusLbl.Text = "Spieler verbunden, Spiel startet";
                    });
                });
            });

            thread.Start();
        }

        private void beitretenBtn_Click(object sender, EventArgs e)
        {
            string code = codeBox.Text;

            statusLbl.Text = "Verbinde...";

            Thread thread = new Thread(() =>
            {
                bool success = sockets.Beitreten(code);

                Invoke((MethodInvoker)delegate
                {
                    if (success)
                        statusLbl.Text = "Spiel startet";
                    else
                        statusLbl.Text = "Verbindung fehlgeschlagen";
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