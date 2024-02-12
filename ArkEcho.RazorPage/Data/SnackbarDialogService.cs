﻿using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ArkEcho.RazorPage.Data
{
    public class SnackbarDialogService
    {
        private ISnackbar snackbarProvider;
        private NavigationManager navigation;
        private Snackbar musicFilesMissingSnackbar;

        public SnackbarDialogService(ISnackbar snackbar/*, NavigationManager navigation*/) // TODO
        {
            this.snackbarProvider = snackbar;
            //this.navigation = navigation;
        }

        public void CloseOnLogout()
        {
            if (musicFilesMissingSnackbar != null)
            {
                snackbarProvider.Remove(musicFilesMissingSnackbar);
                musicFilesMissingSnackbar = null;
            }
        }

        public void CheckingLibraryFailed()
        {
            snackbarProvider.Add("Checking Library Failed!", Severity.Info, config =>
            {
                config.SnackbarVariant = Variant.Outlined;
            });
        }

        public void MusicFilesMissing()
        {
            if (musicFilesMissingSnackbar != null)
                return;

            musicFilesMissingSnackbar = snackbarProvider.Add("Some Files are missing", Severity.Info, config =>
            {
                config.ShowCloseIcon = true;
                config.SnackbarVariant = Variant.Outlined;
                config.RequireInteraction = true;
                config.HideTransitionDuration = 250;

                config.Action = "Sync";
                config.ActionColor = Color.Tertiary;
                config.ActionVariant = Variant.Filled;
                config.Onclick = snackbar =>
                {
                    //navigation.NavigateTo("/Sync");
                    return Task.CompletedTask;
                };
            });
        }
    }
}
