using System;
using System.Collections.Generic;
using UniLinq;
using System.Text;
using CommNet;
using UnityEngine;


namespace CommnetAntennaExtension

{
    class CBK
    {
        public int levelsTracking = 3;
        //public int[] upgradesVisualTracking;
        public double[] DSNRange;
        public bool founded = false;

        public CBK()
        {
            ConfigNode[] configs = GameDatabase.Instance.GetConfigNodes("CUSTOMBARNKIT");

            if (configs != null && configs.Length != 0)
            {
                founded = true;

                if (configs.Length > 1)
                {
                    Logging.Log("More than 1 CustomBarnKit node found. Loading the first one");
                }

                ConfigNode config = configs[0];

                ConfigNode node = new ConfigNode();

                if (config.TryGetNode("TRACKING", ref node))
                {
                    LoadValue(node, "levels", ref levelsTracking);
                    LoadValue(node, "DSNRange", ref DSNRange);

                }
            }
            else
            {
                Logging.Log("No config to load");
            }
        }



        private static void LoadValue(ConfigNode node, string key, ref int param)
        {
            if (node.HasValue(key))
            {
                string s = node.GetValue(key);
                int val;
                if (int.TryParse(s, out val))
                {
                    param = val != -1 ? val : int.MaxValue;
                }
                else
                {
                    Logging.Log("Fail to parse \"" + s + "\" into an int for key " + key);
                }
            }
        }
        private static void LoadValue(ConfigNode node, string key, ref double[] param)
        {
            if (node.HasValue(key))
            {
                string s = node.GetValue(key);
                string[] split = s.Split(',');
                double[] result = new double[split.Length];

                for (int i = 0; i < split.Length; i++)
                {
                    string v = split[i];
                    double val;
                    if (double.TryParse(v, out val))
                    {
                        result[i] = val >= 0 ? val : double.MaxValue;
                    }
                    else
                    {
                        Logging.Log("Fail to parse \"" + s + "\" into a double array for key " + key);
                        return;
                    }
                }
                param = result;
            }
        }
        private static void LoadValue(ConfigNode node, string key, ref int[] param)
        {
            if (node.HasValue(key))
            {
                string s = node.GetValue(key);
                string[] split = s.Split(',');
                int[] result = new int[split.Length];

                for (int i = 0; i < split.Length; i++)
                {
                    string v = split[i];
                    int val;
                    if (int.TryParse(v, out val))
                    {
                        result[i] = val >= 0 ? val : int.MaxValue;
                    }
                    else
                    {
                        Logging.Log("Fail to parse \"" + s + "\" into an int array for key " + key);
                        return;
                    }
                }
                param = result;
            }
           
        }
    }
}
