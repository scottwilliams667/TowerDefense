using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Utilities
{
    public static class StringOps
    {
        private static Dictionary<string, string> mStrings = new Dictionary<string, string>();

        static StringOps()
        {
            mStrings = AssetOps.Serializer.DeserializeStringsFile("Strings");
            mStrings.Add("tutorial_text_welcome", "WELCOME TO TRAINING COMMANDER. \n\n\nI'LL LET YOU GET SOME TRAINING IN BEFORE HELPING US OUT IN MOSCOW. \n\n\n\n\n\nTO BUILD OBJECTS:");
            mStrings.Add("tutorial_text_build", "PRESS BUILD");
            mStrings.Add("tutorial_text_select_object", "SELECT OBJECT &TAP MAP TO PLACE");
            mStrings.Add("tutorial_text_select_other", "SELECT OTHER OBJECTS");
            mStrings.Add("tutorial_text_build_all", "PRESS BUILD ALL");
            mStrings.Add("tutorial_text_play", "WHEN READY, PRESS PLAY");
        }

        public static string GetString(string key)
        {
            string value = string.Empty;

            mStrings.TryGetValue(key, out value);

            return value;
        }
    }
}
