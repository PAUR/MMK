﻿using System.Windows;
using System.Windows.Forms;
using MMK.ApplicationServiceModel;
using MMK.Notify.Model.Launchers;
using MMK.Notify.Observer.Remoting;
using MMK.Notify.Observer.Tasking.Common;
using MMK.Processing.AutoFolder;
using MMK.Wpf.Providers;

namespace MMK.Notify.Services
{
    public class GlobalShortcutService : Service
    {
        private readonly GlobalShortcutProviderCollection shortcutProviders;
        private readonly NotifyObserver observer;
        private readonly HashTagFolderCollection folderCollection;

        public GlobalShortcutService(NotifyObserver observer, HashTagFolderCollection folderCollection)
        {
            shortcutProviders = new GlobalShortcutProviderCollection();
            this.observer = observer;
            this.folderCollection = folderCollection;
        }

        protected override void OnInitialize()
        {
            var trayWindow = IoC.Get<TrayMenuService>().TrayMenuView;

            (shortcutProviders as IGlobalShortcutProvider).SetWindow(trayWindow);

            shortcutProviders.Add(new HotMarkLauncher(KeyModifyers.Ctrl, (int) Keys.T));
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.T, AddNormalizeHotKeyTasks);
            shortcutProviders.Add(KeyModifyers.Ctrl | KeyModifyers.Shift, (int) Keys.M, AddMoveFileToCollectionTasks);

            var swiftSearchLauncher = new SwiftSearchLauncher(trayWindow);

            swiftSearchLauncher.SetStartShortcut(KeyModifyers.Ctrl, Keys.Space);
            swiftSearchLauncher.SetStartFromClipboardShortcut(KeyModifyers.Ctrl | KeyModifyers.Shift, Keys.V);

            shortcutProviders.Add(swiftSearchLauncher);
        }


        private void AddNormalizeHotKeyTasks()
        {
            var filePaths = Explorer.GetForegroundSelectedItemsFileTree(".mp3");
            observer.Observe(NormalizeTrackNameTask.Many(filePaths));
        }

        private void AddMoveFileToCollectionTasks()
        {
            var filePaths = Explorer.GetForegroundSelectedItemsFileTree(".mp3");
            observer.Observe(MoveFileToMusicFolderTask.Many(filePaths, folderCollection));
        }

        public override void Start()
        {
            shortcutProviders.StartListen();
        }

        public override void Stop()
        {
            shortcutProviders.StopListen();
        }
    }
}