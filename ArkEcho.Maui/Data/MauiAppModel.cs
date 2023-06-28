﻿using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
        public override Player Player { get; protected set; }

        public override LibrarySync Sync { get; }

        public override string MusicFolder { get { return getMusicSyncPath(); } }

        private IMauiHelper mauiHelper = null;

        public MauiAppModel(ILocalStorage localStorage, AppEnvironment environment, IMauiHelper mauiHelper)
            : base(environment, localStorage)
        {
            this.mauiHelper = mauiHelper;
            Sync = new LibrarySync(environment, rest, new RestLogger(environment, "LibrarySync", rest));
        }

        protected override async Task<bool> initializePlayer()
        {
            var player = new VLCPlayer(logger);
            Player = player;
            return player.InitPlayer();
        }

        public override async Task<bool> InitializeOnLogin()
        {
            await base.InitializeOnLogin();

            LibrarySync.LibraryCheckResult result = new LibrarySync.LibraryCheckResult();
            bool success = await Sync.CheckLibraryOnStart(getMusicSyncPath(), Library, result);

            if (!success)
            {
                SetStatus(IAppModel.Status.Connected);
                snackbarDialogService.CheckingLibraryFailed();
                return false;
            }

            if (result.FilesMissing)
                snackbarDialogService.MusicFilesMissing();

            mauiHelper.SetDragArea(false);
            return success;
        }

        public override Task LogoutUser()
        {
            mauiHelper.SetDragArea(true);
            return base.LogoutUser();
        }

        public override async Task<bool> StartSynchronizeMusic()
        {
            if (!await LoadLibraryFromServer())
                return false;

            return await Sync.StartSyncMusicLibrary(getMusicSyncPath(), Library);
        }

        private string getMusicSyncPath()
        {
            return mauiHelper.GetPlatformSpecificMusicFolder(AuthenticatedUser);
        }

        public override async Task<bool> ChangeMusicFolder()
        {
            string newFolder = await mauiHelper.PickFolder();

            if (string.IsNullOrEmpty(newFolder) || !Directory.Exists(newFolder))
                return false;

            UserSettings.MusicPath path = AuthenticatedUser.Settings.GetLocalUserSettings();
            if (path == null)
            {
                path = new UserSettings.MusicPath() { MachineName = System.Environment.MachineName, Path = new Uri(newFolder) };
                AuthenticatedUser.Settings.MusicPathList.Add(path);
            }
            else
                path.Path = new Uri(newFolder);

            return await rest.UpdateUser(AuthenticatedUser);
        }
    }
}
