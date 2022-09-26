using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class AudioComponent : Component
    {
        public AudioComponent(Entity owner) : base(owner)
        {
        }

        public override void Update()
        {
        }

        public virtual void Pause()
        {
        }

        public virtual void Play()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Resume()
        {
        }
    }

    public class SimpleAudioComponent : AudioComponent
    {
        private string mAudioSample;

        private bool mPlayed;

        public SimpleAudioComponent(string audioSample, Entity owner) : base(owner)
        {
            mAudioSample = audioSample;
        }

        public bool Played
        {
            get { return mPlayed; }
            set { mPlayed = value; }
        }

        public override void Play()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping =  false,
                Pause = false,
                Play = true,
                Stop = false,
                Resume = false,
                Id = Owner.Id
            });

            mPlayed = true;
        }

        public override void Pause()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping =  false,
                Pause = true,
                Play = false,
                Stop = false,
                Resume = false,
                Id = Owner.Id,
            });
        }
    }

    public class LoopingAudioComponent : AudioComponent
    {
        private string mAudioSample;

        private bool mIsPlaying;
        private bool mIsPaused;
        private bool mIsStopped;

        private float mVolume = 1.0f;

        public LoopingAudioComponent(string audioSample, Entity owner)
            : base(owner)
        {
            mAudioSample = audioSample;
        }

        public bool IsPlaying
        {
            get { return mIsPlaying;}
            set { mIsPlaying = value; }
        }

        public float Volume
        {
            get { return mVolume; }
            set { mVolume = value; }
        }

        public override void Play()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping = true,
                Play = true,
                Pause = false,
                Stop = false,
                Resume = false,
                Id = Owner.Id,
                Volume = mVolume
            });

            mIsPaused = false;
            mIsPlaying = true;
            mIsStopped = false;
        }

        public override void Pause()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping = true,
                Play = false,
                Pause = true,
                Stop = false,
                Resume = false,
                Id = Owner.Id
            });

            mIsPaused = true;
            mIsPlaying = false;
            mIsStopped = false;
        }

        public override void Resume()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping = true,
                Play = false,
                Pause = false,
                Stop = false,
                Resume = true,
                Id = Owner.Id,
                Volume = mVolume
            });

            mIsPaused = false;
            mIsPlaying = true;
            mIsStopped = false;
        }

        public override void Stop()
        {
            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                SoundEffect = mAudioSample,
                IsLooping = true,
                Play = false,
                Pause = false,
                Stop = true,
                Resume = false,
                Id = Owner.Id
            });

            mIsPaused = false;
            mIsPlaying = false;
            mIsStopped = true;
        }

        public override void UnRegister()
        {
            if(mIsPlaying)
                Stop();

            base.UnRegister();
        }
    }

    public class PlayUntilAudioComponent : TimerComponent
    {
        private bool mIsPlaying = false;
        private string mSoundEffect;
        public PlayUntilAudioComponent(float timeInSeconds, string soundEffect, Entity owner) : base(owner)
        {
            mTimeSet = timeInSeconds;
            mTimeLeft = timeInSeconds;
            mSoundEffect = soundEffect;
        }

        public void Play()
        {
            mIsPlaying = true;

            EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
            {
                Id = Owner.Id,
                IsLooping = false,
                Pause = false,
                Play = true,
                Resume =  false,
                SoundEffect = mSoundEffect,
                Stop = false
            });
        }

        public override void Update()
        {
            base.Update();

            //Timer is done and we are currently playing
            if (IsStopped && mIsPlaying)
            {
                mIsPlaying = false;
                EventSystem.EnqueueEvent(EventType.SoundEffect, Owner, new SoundEffectArgs()
                {
                    Id = Owner.Id,
                    IsLooping = false,
                    Pause = false,
                    Play = false,
                    Resume = false,
                    SoundEffect = mSoundEffect,
                    Stop = true
                });
            }
        }
    }
}



