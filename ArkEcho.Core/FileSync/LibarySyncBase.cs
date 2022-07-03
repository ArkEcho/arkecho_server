﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class LibarySyncBase
    {
        private Rest rest = null;
        private Logger logger = null;

        public LibarySyncBase(Rest rest, Logger logger)
        {
            this.rest = rest;
            this.logger = logger;
        }

        public async Task<bool> SyncMusicLibrary(MusicLibrary library, string musicFolder)
        {

            logger.LogImportant($"Checking Files");

            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();
            bool checkLib = await CheckLibraryWithLocalFolder(musicFolder, library, exist, missing);

            if (!checkLib)
            {
                // Log
                return false;
            }

            if (missing.Count > 0)
            {
                logger.LogImportant($"Loading {missing.Count} Files");
                bool success = await loadMissingFiles(missing, exist);
            }

            logger.LogImportant($"Cleaning Up");

            await CleanUpFolder(musicFolder, exist);

            logger.LogStatic($"Success!");

            return true;
        }

        private async Task<bool> loadMissingFiles(List<MusicFile> missing, List<MusicFile> exist)
        {
            try
            {
                foreach (MusicFile file in missing)
                {
                    logger.LogDebug($"Loading {file.FileName}");

                    bool success = await LoadFileFromServer(file);
                    if (!success)
                        logger.LogError($"Error loading {file.FileName} from Server!");

                    exist.Add(file);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                return false;
            }
            return true;
        }

        private async Task<bool> LoadFileFromServer(MusicFile file)
        {
            if (file == null)
                return false;

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                byte[] fileBytes = await rest.GetMusicFile(file.GUID);

                if (fileBytes.Length == 0)
                    return false;

                using (FileStream stream = new FileStream(file.FullPath, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None))
                {
                    await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                sw.Stop();

                if (File.Exists(file.FullPath))
                {
                    logger.LogImportant($"Success loading File in {sw.ElapsedMilliseconds}, {file.FullPath}");
                    return true;
                }
                else
                {
                    logger.LogError($"Error loading File, {file.FullPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading File {file.Title}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> CheckLibraryWithLocalFolder(string musicFolder, MusicLibrary library, List<MusicFile> exist, List<MusicFile> missing)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (MusicFile file in library.MusicFiles)
                    {
                        string folder = getMusicFileFolder(musicFolder, file, library);
                        if (string.IsNullOrEmpty(folder))
                        {
                            logger.LogError($"Error building Path for {file.FileName}");
                            break;
                        }

                        file.Folder = new Uri(folder);

                        if (!Directory.Exists(file.Folder.LocalPath) || !File.Exists(file.FullPath))
                        {
                            Directory.CreateDirectory(file.Folder.LocalPath);
                            missing.Add(file);
                        }
                        else
                            exist.Add(file);
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                }
            }
            );

            logger.LogImportant($"Checking Library with Local Folder took {sw.ElapsedMilliseconds}ms");

            return success;
        }


        private async Task CleanUpFolder(string folder, List<MusicFile> okFiles)
        {
            foreach (string subFolder in Directory.GetDirectories(folder))
                await CleanUpFolder(subFolder, okFiles); // Rekursion

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (okFiles.Find(x => x.FullPath.Equals(file, StringComparison.OrdinalIgnoreCase)) == null)
                            File.Delete(file);
                    }

                    if (Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length == 0)
                        Directory.Delete(folder);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                }
            }
            );
        }

        private string getMusicFileFolder(string musicFolder, MusicFile file, MusicLibrary library)
        {
            Album album = library.Album.Find(x => x.GUID == file.Album);
            AlbumArtist artist = library.AlbumArtists.Find(x => x.GUID == file.AlbumArtist);

            if (album == null || artist == null)
                return string.Empty;

            return $"{musicFolder}{Resources.FilePathDivider}{artist.Name}{Resources.FilePathDivider}{album.Name}";
        }
    }
}
