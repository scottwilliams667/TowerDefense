using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using XnaMediaPlayer =  Microsoft.Xna.Framework.Media;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Systems
{
    public static class AudioSystem
    {
        private static Dictionary<string, SoundEffect> mCachedSounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<ulong, SoundEffectInstance> mInstances = new Dictionary<ulong, SoundEffectInstance>();

        private static Dictionary<string, XnaMediaPlayer.Song> mCachedSongs = new Dictionary<string, XnaMediaPlayer.Song>();

        static AudioSystem()
        {
        }

        public static bool IsSFXMuted
        {
            get { return LevelManager.Settings.IsSFXMuted; }
        }

        public static bool IsMusicMuted
        {
            get { return LevelManager.Settings.IsMusicMuted; }
        }

        public static void Initialize()
        {
            EventSystem.SubScribe(EventType.SoundEffect, OnSoundEffect_);
            EventSystem.SubScribe(EventType.MuteSFX, OnMute_);
            EventSystem.SubScribe(EventType.MuteMusic, OnMute_);
            EventSystem.SubScribe(EventType.PauseChanged, OnPauseChanged_);
            EventSystem.SubScribe(EventType.MusicChanged, OnMusicChanged_);

        }

        private static void OnSoundEffect_(object sender, IGameEventArgs e)
        {
            if (e.EventType != EventType.SoundEffect)
                return;

            SoundEffectArgs eventArgs = e as SoundEffectArgs;

            ulong entityId = eventArgs.Id;
            bool play = eventArgs.Play;
            bool loop = eventArgs.IsLooping;
            bool pause = eventArgs.Pause;
            bool stop = eventArgs.Stop;
            bool resume = eventArgs.Resume;
            string soundEffectName = eventArgs.SoundEffect;
            float volume = eventArgs.Volume;

            SoundEffect soundEffect = null;
            SoundEffectInstance instance = null;

            //remove old instances that arent playing anymore
            foreach (var oldInstance in mInstances.Where(i => i.Value.State == SoundState.Stopped).ToList())
                mInstances.Remove(oldInstance.Key);

            //load the sound effect if we haven't yet
            if (!mCachedSounds.ContainsKey(soundEffectName))
            {
                soundEffect = AssetOps.LoadSharedAsset<SoundEffect>(soundEffectName);
                instance = soundEffect.CreateInstance();
                mCachedSounds.Add(soundEffectName, soundEffect);

                if(!mInstances.ContainsKey(entityId))
                    mInstances.Add(entityId, instance);
            }
            else
            {
                //load sound effect from cache 
                //and create a new instance of it
                mCachedSounds.TryGetValue(soundEffectName, out soundEffect);
                mInstances.TryGetValue(entityId, out instance);

                if (instance == null)
                {
                    instance = soundEffect.CreateInstance();
                    mInstances.Add(entityId, instance);
                }

            }

            instance.Volume = LevelManager.Settings.IsSFXMuted ? 0.0f : volume * SoundEffect.MasterVolume;
            

            if (loop)
                instance.IsLooped = loop;
            if (play)
                instance.Play();
            if (pause)
                instance.Pause();
            if(resume)
                instance.Resume();
            if (stop)
                instance.Stop();



        }

        //47 for destroy, 51 for money
        //9 or 17 for menu clicks

        private static void OnMusicChanged_(object sender, IGameEventArgs e)
        {
            if (e.EventType != EventType.MusicChanged)
                return;

            MusicChangedArgs args = e as MusicChangedArgs;

            string musicName = args.SongName;
            bool loop = args.IsLooping;
            bool play = args.Play;
            bool pause = args.Pause;
            bool stop = args.Stop;

            XnaMediaPlayer.Song song = null;

            mCachedSongs.TryGetValue(musicName, out song);

            if (song == null)
            {
                song = AssetOps.LoadSharedAsset<XnaMediaPlayer.Song>(musicName);
                mCachedSongs.Add(musicName, song);
            }
            
            if (loop)
                
                XnaMediaPlayer.MediaPlayer.IsRepeating = true;
            if(pause)
                XnaMediaPlayer.MediaPlayer.Pause();
            if (play)
            {
                XnaMediaPlayer.MediaPlayer.Stop();
                XnaMediaPlayer.MediaPlayer.Play(song);
            }
            if (stop)
                XnaMediaPlayer.MediaPlayer.Stop();

        }

        private static void OnMute_(object sender, IGameEventArgs e)
        {
            if (e.EventType == EventType.MuteMusic)
            {
                LevelManager.Settings.IsMusicMuted = !LevelManager.Settings.IsMusicMuted;
                XnaMediaPlayer.MediaPlayer.Volume = (LevelManager.Settings.IsMusicMuted) ? 0.0f : 1.0f;
            }
            if (e.EventType == EventType.MuteSFX)
                LevelManager.Settings.IsSFXMuted = !LevelManager.Settings.IsSFXMuted;

            AssetOps.Serializer.SerializeSettings();
        }

        private static void OnPauseChanged_(object sender, IGameEventArgs e)
        {
            //if (SystemsManager.Instance.IsPaused)
            //{
            //    IEnumerable<SoundEffectInstance> currentlyPlayingSounds = mInstances.Values.Where(s => s.State == SoundState.Playing);
            //    foreach (SoundEffectInstance instance in currentlyPlayingSounds)
            //    {
            //        instance.Pause();
            //    }
            //}
            //else
            //{
            //    IEnumerable<SoundEffectInstance> currentlyPauseSounds = mInstances.Values.Where(s => s.State == SoundState.Paused);
            //    foreach (SoundEffectInstance instance in currentlyPauseSounds)
            //    {
            //        instance.Resume();
            //        instance.Volume = (LevelManager.Settings.IsSFXMuted) ? 0.0f : 1.0f;
            //    }
            //}
        }
    }
}
