using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Strooptest
{
    internal class StroopNEinzelspieler
    {
        private Panel gamePanel;
        private int anzahlPictureBoxes = 9;
        private List<PictureBox> pictureBoxes = new List<PictureBox>();
        private Random rand = new Random();

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

        private string gesuchtesWort;

        public event EventHandler<AnswerEventArgs> CorrectAnswerSelected;
        public event EventHandler<AnswerEventArgs> WrongAnswerSelected;

        public StroopNEinzelspieler(Panel panel)
        {
            gamePanel = panel;
        }

        public int AnzahlPictureBoxes
        {
            get { return anzahlPictureBoxes; }
            set
            {
                if (value > 40)
                    throw new ArgumentOutOfRangeException(nameof(value), "Bro du brauchst nicht so viele Felder chill.");
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
                    if (counter >= anzahlPictureBoxes) break;

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
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = index,
                BackColor = Color.White
            };

            Label lblWort = new Label
            {
                Text = "",
                ForeColor = Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            pb.Controls.Add(lblWort);
            pb.Click += PictureBox_Click;
            lblWort.Click += (s, e) => PictureBox_Click(pb, e);

            return pb;
        }

        public void SetzeGesuchtesWort(Color farbeDesOberenWortes)
        {
            if (farbeDesOberenWortes == Color.Red)
                gesuchtesWort = "Rot";
            else if (farbeDesOberenWortes == Color.Blue)
                gesuchtesWort = "Blau";
            else if (farbeDesOberenWortes == Color.Yellow)
                gesuchtesWort = "Gelb";
            else if (farbeDesOberenWortes == Color.LimeGreen)
                gesuchtesWort = "Grün";
            else if (farbeDesOberenWortes == Color.Orange)
                gesuchtesWort = "Orange";
            else if (farbeDesOberenWortes == Color.Purple)
                gesuchtesWort = "Lila";
            else if (farbeDesOberenWortes == Color.HotPink)
                gesuchtesWort = "Pink";
            else if (farbeDesOberenWortes == Color.SaddleBrown)
                gesuchtesWort = "Braun";
            else if (farbeDesOberenWortes == Color.Cyan)
                gesuchtesWort = "Cyan";
        }

        public void GeneriereAlleLabelsNeu()
        {
            if (anzahlPictureBoxes <= woerter.Count)
            {
                GeneriereEindeutig();
            }
            else
            {
                GeneriereMitNachbarschaftsregel();
            }
        }

        private void GeneriereEindeutig()
        {
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            List<string> gemischteWoerter = new List<string>();
            List<Color> gemischteFarben = new List<Color>();

            bool gueltig = false;

            while (!gueltig)
            {
                gemischteWoerter = woerter.OrderBy(x => rand.Next()).Take(anzahlPictureBoxes).ToList();
                gemischteFarben = farben.OrderBy(x => rand.Next()).Take(anzahlPictureBoxes).ToList();

                gueltig = true;

                for (int i = 0; i < anzahlPictureBoxes; i++)
                {
                    if (UeberpruefeObWortFarbeEntspricht(gemischteWoerter[i], gemischteFarben[i]))
                    {
                        gueltig = false;
                        break;
                    }
                }

                if (!gueltig) continue;

                for (int i = 0; i < anzahlPictureBoxes; i++)
                {
                    int zeile = i / spalten;
                    int spalte = i % spalten;

                    var nachbarn = ErmittleNachbarn(i, zeile, spalte);

                    foreach (int nachbar in nachbarn)
                    {
                        if (gemischteWoerter[i] == gemischteWoerter[nachbar] ||
                            gemischteFarben[i] == gemischteFarben[nachbar])
                        {
                            gueltig = false;
                            break;
                        }
                    }

                    if (!gueltig) break;
                }
            }

            for (int i = 0; i < anzahlPictureBoxes; i++)
            {
                PictureBox pb = pictureBoxes[i];

                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    lbl.Text = gemischteWoerter[i];
                    lbl.ForeColor = gemischteFarben[i];
                }
            }
        }

        private void GeneriereMitNachbarschaftsregel()
        {
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                PictureBox pb = pictureBoxes[i];
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    int zeile = i / spalten;
                    int spalte = i % spalten;
                    var nachbarn = ErmittleNachbarn(i, zeile, spalte);

                    HashSet<string> verboteneWoerter = new HashSet<string>();
                    HashSet<Color> verboteneFarben = new HashSet<Color>();
                    foreach (int nachbarIndex in nachbarn)
                    {
                        PictureBox nachbarPb = pictureBoxes[nachbarIndex];
                        if (nachbarPb.Controls.Count > 0 && nachbarPb.Controls[0] is Label nachbarLbl)
                        {
                            verboteneWoerter.Add(nachbarLbl.Text);
                            verboteneFarben.Add(nachbarLbl.ForeColor);
                        }
                    }

                    string neuesWort = "";
                    Color neueFarbe = Color.Empty;
                    bool gefunden = false;
                    int maxVersuche = 1000;
                    int versuche = 0;

                    while (!gefunden && versuche < maxVersuche)
                    {
                        neuesWort = woerter[rand.Next(woerter.Count)];
                        neueFarbe = farben[rand.Next(farben.Count)];

                        if (!verboteneWoerter.Contains(neuesWort) &&
                            !verboteneFarben.Contains(neueFarbe) &&
                            !UeberpruefeObWortFarbeEntspricht(neuesWort, neueFarbe))
                        {
                            gefunden = true;
                        }
                        versuche++;
                    }

                    if (!gefunden)
                    {
                        do
                        {
                            neuesWort = woerter[rand.Next(woerter.Count)];
                            neueFarbe = farben[rand.Next(farben.Count)];
                        } while (UeberpruefeObWortFarbeEntspricht(neuesWort, neueFarbe));
                    }

                    lbl.Text = neuesWort;
                    lbl.ForeColor = neueFarbe;
                }
            }
        }

        private List<int> ErmittleNachbarn(int index, int zeile, int spalte)
        {
            List<int> nachbarn = new List<int>();
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            if (zeile > 0)
            {
                int obenIndex = index - spalten;
                if (obenIndex >= 0 && obenIndex < anzahlPictureBoxes) nachbarn.Add(obenIndex);
            }
            if (zeile < spalten - 1)
            {
                int untenIndex = index + spalten;
                if (untenIndex >= 0 && untenIndex < anzahlPictureBoxes) nachbarn.Add(untenIndex);
            }
            if (spalte > 0)
            {
                int linksIndex = index - 1;
                if (linksIndex >= 0 && linksIndex < anzahlPictureBoxes) nachbarn.Add(linksIndex);
            }
            if (spalte < spalten - 1)
            {
                int rechtsIndex = index + 1;
                if (rechtsIndex >= 0 && rechtsIndex < anzahlPictureBoxes) nachbarn.Add(rechtsIndex);
            }
            return nachbarn;
        }

        private bool UeberpruefeObWortFarbeEntspricht(string wort, Color farbe)
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

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPb = sender as PictureBox;
            if (clickedPb != null && clickedPb.Tag is int index)
            {
                if (clickedPb.Controls.Count > 0 && clickedPb.Controls[0] is Label lbl)
                {
                    if (!lbl.Visible) return;

                    bool istRichtig = (lbl.Text == gesuchtesWort);

                    if (istRichtig)
                    {
                        CorrectAnswerSelected?.Invoke(this, new AnswerEventArgs
                        {
                            Index = index,
                            IstRichtig = true,
                            GeklicktesWort = lbl.Text,
                            GeklickteFarbe = lbl.ForeColor,
                            GesuchtesWort = gesuchtesWort
                        });
                    }
                    else
                    {
                        WrongAnswerSelected?.Invoke(this, new AnswerEventArgs
                        {
                            Index = index,
                            IstRichtig = false,
                            GeklicktesWort = lbl.Text,
                            GeklickteFarbe = lbl.ForeColor,
                            GesuchtesWort = gesuchtesWort
                        });
                    }
                }
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

        public int GetAnzahlPictureBoxes() => pictureBoxes.Count;

        public void SetzePictureBoxFarbe(int index, Color farbe)
        {
            if (index >= 0 && index < pictureBoxes.Count)
                pictureBoxes[index].BackColor = farbe;
        }

        public void SetzeSichtbareLabelsNachSchwierigkeit(string schwierigkeit)
        {
            if (pictureBoxes.Count != 9)
            {
                foreach (var pb in pictureBoxes)
                    if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                        lbl.Visible = true;
                return;
            }

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                PictureBox pb = pictureBoxes[i];
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    bool sichtbar = false;
                    switch (schwierigkeit)
                    {
                        case "Leicht":
                            sichtbar = (i == 3 || i == 4 || i == 5);
                            break;
                        case "Mittel":
                            sichtbar = (i <= 2 || i >= 6);
                            break;
                        case "Schwer":
                        default:
                            sichtbar = true;
                            break;
                    }
                    lbl.Visible = sichtbar;
                }
            }
        }
    }

    public class AnswerEventArgs : EventArgs
    {
        public int Index { get; set; }
        public bool IstRichtig { get; set; }
        public string GeklicktesWort { get; set; }
        public Color GeklickteFarbe { get; set; }
        public string GesuchtesWort { get; set; }
    }
}