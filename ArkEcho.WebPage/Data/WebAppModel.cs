﻿using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get { return jsPlayer; } }
        public override LibrarySync Sync { get; }

        public override string MusicFolder { get { return string.Empty; } }

        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, AppEnvironment environment)
            : base(environment, localStorage)
        {
            jsPlayer = new JSPlayer(jsRuntime, logger, environment.ServerAddress);
        }

        protected override async Task<bool> initializePlayer()
        {
            return jsPlayer.InitPlayer(rest.ApiToken.ToString());
        }

        public override async Task<bool> InitializeOnLogin() => await base.InitializeOnLogin();

        public override Task StartSynchronizeMusic() => throw new NotImplementedException();

        public override Task<bool> ChangeMusicFolder() => throw new NotImplementedException();
    }
}
