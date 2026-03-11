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

        // Listen statt Arrays
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

        // Speichert die aktuell gesuchte Wortbedeutung
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

        public void GeneriereAlleLabelsNeu()
        {
            int spalten = (int)Math.Ceiling(Math.Sqrt(anzahlPictureBoxes));

            // Stelle sicher dass wir genau so viele Elemente haben wie PictureBoxes
            if (pictureBoxes.Count > woerter.Count)
            {
                // Falls mehr PictureBoxes als Wörter, fülle mit Wiederholungen auf
                GeneriereMitWiederholungen(spalten);
                return;
            }

            // Normale Generierung ohne Wiederholungen
            GeneriereOhneWiederholungen(spalten);
        }

        private void GeneriereOhneWiederholungen(int spalten)
        {
            // Erstelle gemischte Listen
            List<string> gemischteWoerter = woerter.OrderBy(x => rand.Next()).ToList();
            List<Color> gemischteFarben = farben.OrderBy(x => rand.Next()).ToList();

            // Dictionary für Zuordnungen
            Dictionary<int, (string wort, Color farbe)> zuordnungen = new Dictionary<int, (string, Color)>();

            // Versuche für jede Position eine passende Kombination zu finden
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                int zeile = i / spalten;
                int spalte = i % spalten;

                // Bestehende Nachbarn ermitteln
                var nachbarn = ErmittleNachbarn(i, zeile, spalte);
                var verboteneWoerter = new HashSet<string>();
                var verboteneFarben = new HashSet<Color>();

                foreach (int nachbarIndex in nachbarn)
                {
                    if (zuordnungen.ContainsKey(nachbarIndex))
                    {
                        var nachbar = zuordnungen[nachbarIndex];
                        verboteneWoerter.Add(nachbar.wort);
                        verboteneFarben.Add(nachbar.farbe);
                    }
                }

                // Verfügbare Optionen filtern
                var verfuegbareWoerter = gemischteWoerter
                    .Where(w => !verboteneWoerter.Contains(w))
                    .ToList();

                var verfuegbareFarben = gemischteFarben
                    .Where(f => !verboteneFarben.Contains(f))
                    .ToList();

                string gewaehltesWort = "";
                Color gewaehlteFarbe = Color.Empty;
                bool gefunden = false;

                // Versuche eine Kombination mit Stroop-Effekt zu finden
                foreach (var wort in verfuegbareWoerter)
                {
                    foreach (var farbe in verfuegbareFarben)
                    {
                        if (!UeberpruefeObWortFarbeEntspricht(wort, farbe))
                        {
                            gewaehltesWort = wort;
                            gewaehlteFarbe = farbe;
                            gefunden = true;
                            break;
                        }
                    }
                    if (gefunden) break;
                }

                // Wenn nichts gefunden, nimm die erste verfügbare Kombination
                if (!gefunden && verfuegbareWoerter.Count > 0 && verfuegbareFarben.Count > 0)
                {
                    gewaehltesWort = verfuegbareWoerter[0];
                    gewaehlteFarbe = verfuegbareFarben[0];
                }

                // Absoluter Notfallplan
                if (!gefunden)
                {
                    // Nimm irgendein Wort und eine andere Farbe
                    gewaehltesWort = gemischteWoerter[i % gemischteWoerter.Count];

                    // Finde eine Farbe die nicht die eigene ist
                    foreach (var farbe in gemischteFarben)
                    {
                        if (!UeberpruefeObWortFarbeEntspricht(gewaehltesWort, farbe))
                        {
                            gewaehlteFarbe = farbe;
                            gefunden = true;
                            break;
                        }
                    }

                    // Falls immer noch nichts, nimm die erste Farbe
                    if (!gefunden && gemischteFarben.Count > 0)
                    {
                        gewaehlteFarbe = gemischteFarben[0];
                    }
                }

                // Speichern und aus Listen entfernen
                zuordnungen[i] = (gewaehltesWort, gewaehlteFarbe);
                gemischteWoerter.Remove(gewaehltesWort);
                gemischteFarben.Remove(gewaehlteFarbe);
            }

            // Labels zuweisen
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                PictureBox pb = pictureBoxes[i];
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    lbl.Text = zuordnungen[i].wort;
                    lbl.ForeColor = zuordnungen[i].farbe;
                }
            }
        }

        private void GeneriereMitWiederholungen(int spalten)
        {
            // Bei mehr PictureBoxes als Wörtern, erlaube Wiederholungen
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                int zeile = i / spalten;
                int spalte = i % spalten;

                PictureBox pb = pictureBoxes[i];
                if (pb.Controls.Count > 0 && pb.Controls[0] is Label lbl)
                {
                    string wort = woerter[rand.Next(woerter.Count)];
                    Color farbe = farben[rand.Next(farben.Count)];

                    // Stelle sicher dass Wort und Farbe nicht übereinstimmen
                    int versuche = 0;
                    while (UeberpruefeObWortFarbeEntspricht(wort, farbe) && versuche < 50)
                    {
                        farbe = farben[rand.Next(farben.Count)];
                        versuche++;
                    }

                    lbl.Text = wort;
                    lbl.ForeColor = farbe;
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
                if (obenIndex >= 0 && obenIndex < anzahlPictureBoxes)
                    nachbarn.Add(obenIndex);
            }

            if (zeile < spalten - 1)
            {
                int untenIndex = index + spalten;
                if (untenIndex >= 0 && untenIndex < anzahlPictureBoxes)
                    nachbarn.Add(untenIndex);
            }

            if (spalte > 0)
            {
                int linksIndex = index - 1;
                if (linksIndex >= 0 && linksIndex < anzahlPictureBoxes)
                    nachbarn.Add(linksIndex);
            }

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