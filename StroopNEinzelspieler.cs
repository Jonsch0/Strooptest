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

        // Wort- und Farbarrays 
        public string[] woerter = { "Rot", "Blau", "Gelb", "Grün", "Orange", "Lila", "Pink", "Braun", "Cyan" };
        public Color[] farben = {
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

        // Speichert die aktuell gesuchte Wortbedeutung (basierend auf der Farbe des oberen Wortes)
        private string gesuchtesWort;

        // Events für richtige/falsche Antworten
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
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = index,
                BackColor = Color.White // Weißer Hintergrund für die Kästchen
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

        /// <summary>
        /// Setzt die gesuchte Wortbedeutung basierend auf der Farbe des oberen Wortes
        /// </summary>
        public void SetzeGesuchtesWort(Color farbeDesOberenWortes)
        {
            // Übersetze die Farbe in das entsprechende Wort
            if (farbeDesOberenWortes == Color.Red)
                gesuchtesWort = "Rot";
            else if (farbeDesOberenWortes == Color.Blue)
                gesuchtesWort = "Blau";
            else if (farbeDesOberenWortes == Color.Yellow)
                gesuchtesWort = "Gelb";
            else if (farbeDesOberenWortes == Color.Green)
                gesuchtesWort = "Grün";
            else if (farbeDesOberenWortes == Color.Orange)
                gesuchtesWort = "Orange";
            else if (farbeDesOberenWortes == Color.Purple)
                gesuchtesWort = "Lila";
            else if (farbeDesOberenWortes == Color.HotPink)
                gesuchtesWort = "Pink";
            else if (farbeDesOberenWortes == Color.Brown)
                gesuchtesWort = "Braun";
            else if (farbeDesOberenWortes == Color.Cyan)
                gesuchtesWort = "Cyan";
        }

        /// <summary>
        /// Generiert ALLE Labels im Grid NEU
        /// </summary>
        public void GeneriereAlleLabelsNeu()
        {
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                PictureBox pb = pictureBoxes[i];
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    int zeile = i / spalten;
                    int spalte = i % spalten;

                    var (neuesWort, neueFarbe) = GeneriereWortFarbeFuerIndex(i, zeile, spalte);

                    lbl.Text = neuesWort;
                    lbl.ForeColor = neueFarbe;
                }
            }
        }

        /// <summary>
        /// Generiert ein Wort-Farbe Paar für einen bestimmten Index
        /// </summary>
        private (string wort, Color farbe) GeneriereWortFarbeFuerIndex(int index, int zeile, int spalte)
        {
            string ausgewaehltesWort = null;
            Color ausgewaehlteFarbe = Color.Empty;
            bool gueltigeKombinationGefunden = false;
            int maxVersuche = 100;
            int versuche = 0;

            var nachbarn = ErmittleNachbarn(index, zeile, spalte);

            var verwendeteWoerterInNachbarschaft = new HashSet<string>();
            var verwendeteFarbenInNachbarschaft = new HashSet<Color>();

            foreach (int nachbarIndex in nachbarn)
            {
                if (nachbarIndex >= 0 && nachbarIndex < pictureBoxes.Count)
                {
                    PictureBox nachbarPb = pictureBoxes[nachbarIndex];
                    if (nachbarPb.Controls.Count > 0 && nachbarPb.Controls[0] is Label nachbarLbl)
                    {
                        verwendeteWoerterInNachbarschaft.Add(nachbarLbl.Text);
                        verwendeteFarbenInNachbarschaft.Add(nachbarLbl.ForeColor);
                    }
                }
            }

            while (!gueltigeKombinationGefunden && versuche < maxVersuche)
            {
                // Zufälliges Wort auswählen
                ausgewaehltesWort = woerter[rand.Next(woerter.Length)];

                // Zufällige Farbe auswählen
                ausgewaehlteFarbe = farben[rand.Next(farben.Length)];

                // Prüfung 1: Wort darf nicht der Farbe entsprechen (Stroop-Effekt)
                bool wortEntsprichtFarbe = UeberpruefeObWortFarbeEntspricht(ausgewaehltesWort, ausgewaehlteFarbe);

                // Prüfung 2: Weder Wort noch Farbe dürfen in Nachbarfeldern vorkommen
                bool wortInNachbarschaft = verwendeteWoerterInNachbarschaft.Contains(ausgewaehltesWort);
                bool farbeInNachbarschaft = verwendeteFarbenInNachbarschaft.Contains(ausgewaehlteFarbe);

                if (!wortEntsprichtFarbe && !wortInNachbarschaft && !farbeInNachbarschaft)
                {
                    gueltigeKombinationGefunden = true;
                }

                versuche++;
            }

            // Falls keine perfekte Kombination gefunden wurde
            if (!gueltigeKombinationGefunden)
            {
                versuche = 0;
                while (!gueltigeKombinationGefunden && versuche < maxVersuche)
                {
                    ausgewaehltesWort = woerter[rand.Next(woerter.Length)];
                    ausgewaehlteFarbe = farben[rand.Next(farben.Length)];

                    if (!UeberpruefeObWortFarbeEntspricht(ausgewaehltesWort, ausgewaehlteFarbe))
                    {
                        gueltigeKombinationGefunden = true;
                    }
                    versuche++;
                }
            }

            // Absoluter Notfallplan
            if (!gueltigeKombinationGefunden)
            {
                return (woerter[0], farben[1]);
            }

            return (ausgewaehltesWort, ausgewaehlteFarbe);
        }

        private List<int> ErmittleNachbarn(int index, int zeile, int spalte)
        {
            var nachbarn = new List<int>();
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            // Oben
            if (zeile > 0)
            {
                int obenIndex = index - spalten;
                if (obenIndex >= 0 && obenIndex < anzahlPictureBoxes)
                    nachbarn.Add(obenIndex);
            }

            // Unten
            if (zeile < spalten - 1)
            {
                int untenIndex = index + spalten;
                if (untenIndex >= 0 && untenIndex < anzahlPictureBoxes)
                    nachbarn.Add(untenIndex);
            }

            // Links
            if (spalte > 0)
            {
                int linksIndex = index - 1;
                if (linksIndex >= 0 && linksIndex < anzahlPictureBoxes)
                    nachbarn.Add(linksIndex);
            }

            // Rechts
            if (spalte < spalten - 1)
            {
                int rechtsIndex = index + 1;
                if (rechtsIndex >= 0 && rechtsIndex < anzahlPictureBoxes)
                    nachbarn.Add(rechtsIndex);
            }

            return nachbarn;
        }

        private bool UeberpruefeObWortFarbeEntspricht(string wort, Color farbe)
        {
            // Mapping von Wort zu Farbe
            if (wort == "Rot" && farbe == Color.Red) return true;
            if (wort == "Blau" && farbe == Color.Blue) return true;
            if (wort == "Gelb" && farbe == Color.Yellow) return true;
            if (wort == "Grün" && farbe == Color.Green) return true;
            if (wort == "Orange" && farbe == Color.Orange) return true;
            if (wort == "Lila" && farbe == Color.Purple) return true;
            if (wort == "Pink" && farbe == Color.HotPink) return true;
            if (wort == "Braun" && farbe == Color.Brown) return true;
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
                    // Prüfe ob das angeklickte Wort dem gesuchten Wort entspricht
                    bool istRichtig = (lbl.Text == gesuchtesWort);

                    // Löse das entsprechende Event aus
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

        public int GetAnzahlPictureBoxes()
        {
            return pictureBoxes.Count;
        }

        public void SetzePictureBoxFarbe(int index, Color farbe)
        {
            if (index >= 0 && index < pictureBoxes.Count)
            {
                pictureBoxes[index].BackColor = farbe;
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