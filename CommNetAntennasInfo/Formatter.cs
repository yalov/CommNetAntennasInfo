using System;
using System.Globalization;
using KSP.Localization;

namespace CommnetAntennaExtension
{

    static class Formatter
    {
        private static readonly string m = Localizer.Format("#CAE_meter");
        private static readonly string digit_space = "\u2009\u2009\u200A";
        // " " - thin
        // " " - hair

        private static readonly string[] SI = {"",
            Localizer.Format("#CAE_kilo"),
            Localizer.Format("#CAE_mega"),
            Localizer.Format("#CAE_giga"),
            Localizer.Format("#CAE_tera")
        };

        private static readonly string[] SI_Spaced = {"",
            Localizer.Format("#CAE_kilo_spaced"),
            Localizer.Format("#CAE_mega_spaced"),
            Localizer.Format("#CAE_giga_spaced"),
            Localizer.Format("#CAE_tera_spaced")
        };

        
        public static string ValueShortAll(double value, int digits = 3, bool forced = true)
        {
            double v = Math.Abs(value);
            string result;

            int i;
            for (i = 0; v >= 1000 && i < SI.Length - 1; i++)
                v /= 1000;

            if (v < 10) result = Math.Round(Math.Sign(value) * v, Math.Max(0, digits - 1)).ToString(forced ? "F" + Math.Max(0, digits - 1) : "") + SI[i];
            else if (v < 100) result = Math.Round(Math.Sign(value) * v, Math.Max(0, digits - 2)).ToString(forced ? "F" + Math.Max(0, digits - 2) : "") + SI[i];
            else if (v < 1000) result = Math.Round(Math.Sign(value) * v, Math.Max(0, digits - 3)).ToString(forced ? "F" + Math.Max(0, digits - 3) : "") + SI[i];
            else result = value.ToString("0e0") + SI[0];

            return result;
        }

        public static string ValueShort(double value)
        {
            double v = Math.Abs(value);

            int i;
            for (i = 0; v >= 1000 && i < SI.Length - 1; i++)
                v /= 1000;

            if (v < 10)        return Math.Round(Math.Sign(value) * v, 2).ToString("F2") + SI[i];
            else if (v < 100)  return Math.Round(Math.Sign(value) * v, 1).ToString("F1") + SI[i];
            else if (v < 1000) return Math.Round(Math.Sign(value) * v, 0).ToString("F0") + SI[i];
            else return value.ToString("0e0") + SI[0];
        }

        public static string ValueExtraShort(double value)
        {
            double v = Math.Abs(value);

            int i;
            for (i = 0; v >= 1000 && i < SI.Length - 1; i++)
                v /= 1000;

            if (v < 10) return Math.Round(Math.Sign(value) * v, 1) + SI[i];
            else if (v < 1000) return Math.Round(Math.Sign(value) * v, 0) + SI[i];
            else return value.ToString("0e0") + SI[0];
        }

        public static string ValueExtraShortSpaced(double value)
        {
            double v = Math.Abs(value);

            int i;
            for (i = 0; v >= 1000 && i < SI.Length - 1; i++)
                v /= 1000;
            // 2t+1h
            if (v < 10)         return String.Format("{2}{0}{1}", Math.Round(Math.Sign(value) * v, 0), SI_Spaced[i], digit_space + digit_space);
            else if (v < 100)   return String.Format("{2}{0}{1}", Math.Round(Math.Sign(value) * v, 0), SI_Spaced[i], digit_space);
            else if (v < 1000)  return String.Format("{0}{1}",    Math.Round(Math.Sign(value) * v, 0), SI_Spaced[i]);
            else return value.ToString("0e0") + SI_Spaced[0];
        }

        public static string DistanceShort(double value) => ValueShort(value) + m;
        public static string DistanceExtraShort(double value) => ValueExtraShort(value) + m;

        public static string ToTitleCase(this string s) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
    }
}
