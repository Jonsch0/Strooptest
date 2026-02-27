using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Strooptest
{
    internal class StroopNEinzelspieler
    {
        private Panel gamePanel;
        private int anzahlPictureBoxes = 9;
        private List<PictureBox> pictureBoxes = new List<PictureBox>();
        private Random rand = new Random();


        public event EventHandler<PictureBoxClickEventArgs> PictureBoxClicked;

        public StroopNEinzelspieler(Panel panel)
        {
            gamePanel = panel;
        }

        public int AnzahlPictureBoxes
        {
            get { return anzahlPictureBoxes; }
            set
            {
                // Prüfung auf maximal 40 PictureBoxes
                if (value > 40)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        "Bro du brauchst nicht so viele Felder chill."
                    );
                }

                if (value >= 1)
                {
                    anzahlPictureBoxes = value;
                    ErstellePictureBoxRaster();
                }
            }
        }

        public void ErstellePictureBoxRaster()
        {
            LoeschePictureBoxes();
            gamePanel.AutoScroll = true;
            gamePanel.BackColor = Color.LightGray;

            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));
            int zeilen = (int)Math.Ceiling((double)anzahlPictureBoxes / spalten);

            int pictureBoxSize = Math.Min(
                (gamePanel.Width - 20) / spalten,
                (gamePanel.Height - 20) / zeilen
            );

            int startX = (gamePanel.Width - (pictureBoxSize * spalten)) / 2;
            int startY = (gamePanel.Height - (pictureBoxSize * zeilen)) / 2;

            int counter = 0;
            for (int zeile = 0; zeile < zeilen; zeile++)
            {
                for (int spalte = 0; spalte < spalten; spalte++)
                {
                    if (counter >= anzahlPictureBoxes)
                        break;

                    PictureBox pb = ErstellePictureBox(zeile, spalte, pictureBoxSize, startX, startY, counter);
                    gamePanel.Controls.Add(pb);
                    pictureBoxes.Add(pb);
                    counter++;
                }
            }
        }

        private PictureBox ErstellePictureBox(int zeile, int spalte, int groesse, int startX, int startY, int index)
        {
            PictureBox pb = new PictureBox
            {
                Location = new Point(startX + (spalte * groesse), startY + (zeile * groesse)),
                Size = new Size(groesse - 4, groesse - 4),
                BackColor = GetRandomColor(),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = index
            };

            pb.Click += PictureBox_Click;
            return pb;
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(
                rand.Next(256),
                rand.Next(256),
                rand.Next(256)
            );
        }


        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPb = sender as PictureBox;
            if (clickedPb != null && clickedPb.Tag is int index)
            {
                PictureBoxClicked?.Invoke(this, new PictureBoxClickEventArgs
                {
                    Index = index,
                    ClickedPictureBox = clickedPb
                });
            }
        }

        public void LoeschePictureBoxes()
        {
            foreach (PictureBox pb in pictureBoxes)
            {
                gamePanel.Controls.Remove(pb);
                pb.Dispose();
            }
            pictureBoxes.Clear();
        }

        public int GetAnzahlPictureBoxes()
        {
            return pictureBoxes.Count;
        }

        public void SetzePictureBoxBild(int index, Image bild)
        {
            if (index >= 0 && index < pictureBoxes.Count)
            {
                pictureBoxes[index].Image = bild;
            }
        }

        public void SetzePictureBoxFarbe(int index, Color farbe)
        {
            if (index >= 0 && index < pictureBoxes.Count)
            {
                pictureBoxes[index].BackColor = farbe;
            }
        }
    }

    public class PictureBoxClickEventArgs : EventArgs
    {
        public int Index { get; set; }
        public PictureBox ClickedPictureBox { get; set; }
    }
}