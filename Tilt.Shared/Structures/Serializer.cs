using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PCLStorage;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Structures
{
    public class SaveFile
    {
        public SaveFile()
        {
            Minerals = new List<uint>();
        }

        public int LevelCompleted { get; set; }

        /// minerals are saved for every level completed.
        public List<uint> Minerals { get; set; }

        public int BaseHealth { get; set; }

        public int UnitsDestroyedOverCampaign { get; set; }

        public int ResourcesSpentOverCampaign { get; set; }

        public void Reset()
        {
            LevelCompleted = 0;

            Minerals = new List<uint>() { LevelManager.Levels[0].Minerals };

            BaseHealth = LevelManager.Levels[0].BaseHealth;

            UnitsDestroyedOverCampaign = 0;

            ResourcesSpentOverCampaign = 0;
        }
        
    }

    public class Settings
    {
        public bool IsSFXMuted { get; set; }

        public bool IsMusicMuted { get; set; }
    }

    public class Serializer
    {
        private const string kSaveFile = "Save.json";
        private const string kSettings = "Settings.json";
        private const string kEmptyFile = "{}";

        public async Task SerializeSettings()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile file = await rootFolder.CreateFileAsync(kSettings, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {

                using (var writer = new StreamWriter(stream))
                {
                    writer.BaseStream.Position = 0;

                    string text = JsonConvert.SerializeObject(LevelManager.Settings);
                    await writer.WriteAsync(text);

                    writer.Close();
                }
                stream.Close();
            }
        }

        public Settings DeserializeSettings()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            Task<IFile> fileTask = rootFolder.CreateFileAsync(kSettings, CreationCollisionOption.OpenIfExists);
            fileTask.Wait();
            IFile file = fileTask.Result;
            Task<Stream> streamTask = file.OpenAsync(PCLStorage.FileAccess.Read);
            streamTask.Wait();

            Settings settings;
            using (Stream stream = streamTask.Result)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    reader.BaseStream.Position = 0;

                    string text = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(text))
                    {
                        text = kEmptyFile;
                    }
                    settings = JsonConvert.DeserializeObject<Settings>(text);

                }
                stream.Close();
                streamTask.Dispose();
            }
            fileTask.Dispose();

            return settings;
        }

        public async Task SerializeSaveFile()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFile file = await rootFolder.CreateFileAsync(kSaveFile, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                using (var writer = new StreamWriter(stream))
                {
                    string text = JsonConvert.SerializeObject(LevelManager.SaveFile);
                    await writer.WriteAsync(text);
                }
            }
        }


        public SaveFile DeserializeSaveFile()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            Task<IFile> fileTask =  rootFolder.CreateFileAsync(kSaveFile, CreationCollisionOption.OpenIfExists);
            fileTask.Wait();
            IFile file = fileTask.Result;
            Task<Stream> streamTask = file.OpenAsync(PCLStorage.FileAccess.Read);
            streamTask.Wait();
            SaveFile saveFile;
            using (Stream stream = streamTask.Result)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string text = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(text))
                    {
                        text = kEmptyFile;
                    }
                    saveFile = JsonConvert.DeserializeObject<SaveFile>(text);

                }
            }
            return saveFile;
        }

        public List<Level> DeserializeLevelFile(string levelsFile)
        {
            var stream = GetFileStream(levelsFile, "json");
            if (stream == null)
                return null;
            List<Level> levels = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                StringBuilder builder = new StringBuilder(reader.ReadToEnd());
                levels = JsonConvert.DeserializeObject<List<Level>>(builder.ToString());
            }
            return levels;
        }

        public Dictionary<string, string> DeserializeStringsFile(string stringFile)
        {
            Dictionary< string, string> dictionary = new Dictionary<string, string>();

            var stream = GetFileStream(stringFile, "txt");
            if (stream == null)
                return dictionary;;

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string text = reader.ReadLine();
                    string[] kvp = text.Split('@');
                    dictionary.Add(kvp[0], kvp[1]);
                }
            }

            return dictionary;
        }

        public List<TileCoord> DeserializeMapDataFile(string mapDataFile)
        {
            List<TileCoord> tileCoords = new List<TileCoord>();

            var stream = GetFileStream(mapDataFile, "csv");
            if (stream == null)
                return tileCoords;

            string fileContents = string.Empty;

            int x = 0;
            int y = 0;
                

            using(StreamReader reader = new StreamReader(stream))
            {
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] split = line.Split(',');

                    for(int i =0 ; i < split.Length; i++)
                    {
                        string str = split[i];
                        if(str == "0")
                        {
                            TileCoord tileCoord = new TileCoord()
                            {
                                X = i,
                                Y = x

                            };

                            tileCoords.Add(tileCoord);
                        }

                    }

                    x++;


                }
            }

            return tileCoords;
        }

        public static Stream GetFileStream(string fileName, string extension)
        {
            var stream = Stream.Null;
            try
            {
                stream = TitleContainer.OpenStream(string.Format(@"Content/_shared/{0}.{1}", fileName, extension));
            }
            catch (Exception e)
            {
            }
            return stream;
        }
    }
}
