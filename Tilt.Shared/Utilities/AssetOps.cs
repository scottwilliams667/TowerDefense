using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Utilities
{
    public static class AssetOps
    {
        private static Serializer mSerializer;

        static AssetOps()
        {
            mSerializer = new Serializer();
        }

        public static T LoadAsset<T>(string relativePath)
        {
            ContentManager content = ServiceLocator.GetService<ContentManager>();
            T asset = content.Load<T>(String.Format(@"{0}\{1}", Version, relativePath));

            return asset;
        }

        public static T LoadSharedAsset<T>(string relativePath)
        {
            ContentManager content = ServiceLocator.GetService<ContentManager>();
            T asset = content.Load<T>(String.Format(@"_shared/{0}", relativePath));

            return asset;

        }

        public static Serializer Serializer
        {
            get { return mSerializer; }
        }


        private static string mVersion = "X";

        public static string Version
        {
            get { return mVersion; }
        }

        public static void SetVersion(string model)
        {
            if (model.Contains("iPhone 5"))
                mVersion = "5";
            else if (model == "iPhone 6 Plus")
                mVersion = "P";
            else if (model == "iPhone 6")
                mVersion = "R";
            else if (model == "iPhone 6S Plus")
                mVersion = "P";
            else if (model == "iPhone 6S")
                mVersion = "R";
            else if (model == "iPhone 7 Plus")
                mVersion = "P";
            else if (model == "iPhone 7")
                mVersion = "R";
            else if (model == "iPhone SE")
                mVersion = "5";
            else if (model == "iPhone 8")
                mVersion = "R";
            else if (model == "iPhone 8 Plus")
                mVersion = "P";
            else if (model == "iPhone X")
                mVersion = "X";
            else if (model == "iPhone XS")
                mVersion = "X";
            else if (model == "iPhone XR")
                mVersion = "XR";
            else if (model == "iPhone X Max")
                mVersion = "XMax";
            else
            {
                throw new Exception("Cannot find iPhone Version");
            }
        }
    }
}
