namespace CommNetAntennasInfo
{
    class CBK
    {
        public int levelsTracking = 3;
        
        public bool founded = false;

        public CBK()
        {
            ConfigNode[] configs = GameDatabase.Instance.GetConfigNodes("CUSTOMBARNKIT");

            if (configs != null && configs.Length != 0)
            {
                founded = true;

                if (configs.Length > 1) Logging.Log("More than 1 CustomBarnKit node found. Loading the first one");

                ConfigNode config = configs[0];
                ConfigNode node = new ConfigNode();

                if (config.TryGetNode("TRACKING", ref node))
                {
                    LoadValue(node, "levels", ref levelsTracking);
                }
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
    }
}
