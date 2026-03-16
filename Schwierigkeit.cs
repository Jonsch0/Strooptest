namespace Strooptest
{
    internal static class Schwierigkeit
    {
        public const string Leicht = "Leicht";
        public const string Mittel = "Mittel";
        public const string Schwer = "Schwer";

        public static string ErmittleSchwierigkeit(int rundenZaehler)
        {
            if (rundenZaehler <= 10)
                return Leicht;
            else if (rundenZaehler <= 20)
                return Mittel;
            else
                return Schwer;
        }
    }
}