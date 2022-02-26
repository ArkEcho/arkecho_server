﻿using ArkEcho.Player;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class ArkEchoJSPlayer : ArkEchoPlayer
    {
        private Logging.LoggingDelegate logDelegate = null;

        public IJSRuntime JS { get; private set; }

        public ArkEchoJSPlayer() : base()
        {
        }

        public bool InitPlayer(IJSRuntime JS, Logging.LoggingDelegate LogDelegate)
        {
            if (LogDelegate != null && JS != null)
            {
                logDelegate = LogDelegate;
                this.JS = JS;
                Initialized = true;

                var dotNetReference = DotNetObjectReference.Create(this);
                JS.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });

                return Initialized;
            }
            return false;
        }

        [JSInvokable]
        public void SetPositionJS(int Position)
        {
            base.Position = Position;
        }

        [JSInvokable]
        public void AudioEndedJS()
        {
            AudioEnd();
        }

        [JSInvokable]
        public void AudioPlayingJS(bool Playing)
        {
            this.Playing = Playing;
        }

        protected override bool logImpl(string Text, Logging.LogLevel Level)
        {
            if (logDelegate != null)
                return logDelegate.Invoke(Text, Level);
            return false;
        }

        protected override void loadImpl(bool StartOnLoad)
        {
            if (PlayingFile != null)
            {
                // TODO: Aus Config?
                string source = $"https://localhost:5001/api/Music/{PlayingFile.GUID}";
                JS.InvokeVoidAsync("Player.InitAudio", new object[] { source, PlayingFile.FileFormat, StartOnLoad, Volume, Mute });
            }
        }

        protected override void disposeImpl()
        {
            JS.InvokeVoidAsync("Player.DisposeAudio", new object[] { });
        }

        protected override void playImpl()
        {
            JS.InvokeVoidAsync("Player.PlayAudio", new object[] { });
        }

        protected override void pauseImpl()
        {
            JS.InvokeVoidAsync("Player.PauseAudio", new object[] { });
        }

        protected override void stopImpl()
        {
            JS.InvokeVoidAsync("Player.StopAudio", new object[] { });
        }

        protected override void setMuteImpl()
        {
            JS.InvokeVoidAsync("Player.SetAudioMute", new object[] { Mute });
        }

        protected override void setVolumeImpl()
        {
            JS.InvokeVoidAsync("Player.SetAudioVolume", new object[] { Volume });
        }

        protected override void setPositionImpl(int NewPosition)
        {
            JS.InvokeVoidAsync("Player.SetAudioPosition", new object[] { NewPosition });
        }
    }
}
