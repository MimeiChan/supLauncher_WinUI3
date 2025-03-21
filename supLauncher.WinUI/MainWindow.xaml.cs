using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Extensions.DependencyInjection;
using supLauncher.WinUI.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace supLauncher.WinUI
{
    /// <summary>
    /// メインウィンドウクラス
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// ビューモデル
        /// </summary>
        public MainWindowViewModel ViewModel { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            
            // ビューモデルを取得
            ViewModel = App.Current.ServiceProvider.GetRequiredService<MainWindowViewModel>();
            
            // ウィンドウタイトルを設定
            Title = "supLauncher";
            
            // ウィンドウハンドルを保存
            _windowHandle = WindowNative.GetWindowHandle(this);
        }

        private IntPtr _windowHandle;

        #region メニューイベントハンドラ

        /// <summary>
        /// 編集モードメニュー項目がクリックされた
        /// </summary>
        private void EditModeItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteModeItem.IsChecked = !EditModeItem.IsChecked;
        }

        /// <summary>
        /// 実行モードメニュー項目がクリックされた
        /// </summary>
        private void ExecuteModeItem_Click(object sender, RoutedEventArgs e)
        {
            EditModeItem.IsChecked = !ExecuteModeItem.IsChecked;
        }

        /// <summary>
        /// 新規作成メニュー項目がクリックされた
        /// </summary>
        private async void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "新規作成",
                Content = "新しいメニューファイルを作成しますか？",
                PrimaryButtonText = "はい",
                CloseButtonText = "いいえ",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // ファイル保存ダイアログを表示
                FileSavePicker savePicker = InitializeFileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("XML定義ファイル", new[] { ".xml" });
                savePicker.SuggestedFileName = "MenuFile";

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    // 新規作成コマンドを実行
                    ViewModel.NewCommand.Execute(file.Path);
                }
            }
        }

        /// <summary>
        /// 開くメニュー項目がクリックされた
        /// </summary>
        private async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // ファイルを開くダイアログを表示
            FileOpenPicker openPicker = InitializeFileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".xml");

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // 開くコマンドを実行
                ViewModel.OpenCommand.Execute(file.Path);
            }
        }

        /// <summary>
        /// 環境設定メニュー項目がクリックされた
        /// </summary>
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 環境設定ダイアログを表示
            ViewModel.SettingsCommand.Execute(null);
        }

        /// <summary>
        /// アバウトメニュー項目がクリックされた
        /// </summary>
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // アバウトダイアログを表示
            var dialog = new ContentDialog
            {
                Title = "supLauncherについて",
                Content = "supLauncher\nVersion 3.0.0.0\n\n© 2025 MimeiChan",
                CloseButtonText = "閉じる",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// 終了メニュー項目がクリックされた
        /// </summary>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // アプリケーションを終了
            this.Close();
        }

        /// <summary>
        /// 切り取りメニュー項目がクリックされた
        /// </summary>
        private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CutCommand.Execute(null);
        }

        /// <summary>
        /// コピーメニュー項目がクリックされた
        /// </summary>
        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CopyCommand.Execute(null);
        }

        /// <summary>
        /// 貼り付けメニュー項目がクリックされた
        /// </summary>
        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PasteCommand.Execute(null);
        }

        /// <summary>
        /// 削除メニュー項目がクリックされた
        /// </summary>
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteCommand.Execute(null);
        }

        /// <summary>
        /// 非表示メニュー項目がクリックされた
        /// </summary>
        private void HiddenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleHiddenCommand.Execute(null);
        }

        /// <summary>
        /// ESCキーメニュー項目がクリックされた
        /// </summary>
        private void EscapeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleEscapeCommand.Execute(null);
        }

        #endregion

        #region ボタンイベントハンドラ

        /// <summary>
        /// メニューボタンのセレクションが変更された
        /// </summary>
        private void MenuButtonsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.OnSelectionChanged();
        }

        /// <summary>
        /// メニューボタンがクリックされた
        /// </summary>
        private void MenuButtonsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MenuButtonViewModel button)
            {
                ViewModel.ButtonClickCommand.Execute(button.Index);
            }
        }

        /// <summary>
        /// ボタンが右クリックされた
        /// </summary>
        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // 右クリックメニューを表示する処理
            if (sender is FrameworkElement element && element.Tag is int index)
            {
                // 対象のボタンを選択
                MenuButtonsGrid.SelectedIndex = index;

                // 右クリックメニューを表示
                if (!ViewModel.IsEditMode)
                {
                    // 実行モードの場合は何もしない
                    return;
                }

                // ToDo: コンテキストメニューを表示
            }
        }

        #endregion

        #region ヘルパーメソッド

        /// <summary>
        /// FileOpenPickerを初期化する
        /// </summary>
        private FileOpenPicker InitializeFileOpenPicker()
        {
            var openPicker = new FileOpenPicker();
            InitializeWithWindow.Initialize(openPicker, _windowHandle);
            return openPicker;
        }

        /// <summary>
        /// FileSavePickerを初期化する
        /// </summary>
        private FileSavePicker InitializeFileSavePicker()
        {
            var savePicker = new FileSavePicker();
            InitializeWithWindow.Initialize(savePicker, _windowHandle);
            return savePicker;
        }

        #endregion
    }
}
