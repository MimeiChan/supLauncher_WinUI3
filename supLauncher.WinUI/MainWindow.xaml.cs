using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives; // FlyoutShowOptions用
using Microsoft.Extensions.DependencyInjection;
using supLauncher.Core.Models;
using supLauncher.WinUI.ViewModels;
using supLauncher.WinUI.Views;
using System;
using System.Threading.Tasks;
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

        // UIコントロールの参照（代替策として直接保持）
        private ToggleMenuFlyoutItem _editModeItem;
        private ToggleMenuFlyoutItem _executeModeItem;
        private GridView _menuButtonsGrid;

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
            
            // ウィンドウ参照をAppに保存
            if (App.Current is App app)
            {
                app.MainWindow = this;
            }

            // UIコントロールへの参照を取得（XAMLが正しく初期化された後）
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // UIコントロールの参照を取得
            _editModeItem = Content.XamlRoot.Host.Content.FindName("EditModeItem") as ToggleMenuFlyoutItem;
            _executeModeItem = Content.XamlRoot.Host.Content.FindName("ExecuteModeItem") as ToggleMenuFlyoutItem;
            _menuButtonsGrid = Content.XamlRoot.Host.Content.FindName("MenuButtonsGrid") as GridView;
        }

        private IntPtr _windowHandle;

        #region メニューイベントハンドラ

        /// <summary>
        /// 編集モードメニュー項目がクリックされた
        /// </summary>
        private void EditModeItem_Click(object sender, RoutedEventArgs e)
        {
            // 送信元のコントロールを使用
            var editModeItem = sender as ToggleMenuFlyoutItem;
            if (_executeModeItem != null)
            {
                _executeModeItem.IsChecked = !editModeItem.IsChecked;
            }
        }

        /// <summary>
        /// 実行モードメニュー項目がクリックされた
        /// </summary>
        private void ExecuteModeItem_Click(object sender, RoutedEventArgs e)
        {
            // 送信元のコントロールを使用
            var executeModeItem = sender as ToggleMenuFlyoutItem;
            if (_editModeItem != null)
            {
                _editModeItem.IsChecked = !executeModeItem.IsChecked;
            }
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
        private async void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 環境設定ダイアログを表示
            var settingsDialog = new SettingsDialog();
            await ShowSettingsDialogAsync(settingsDialog);
        }

        /// <summary>
        /// 環境設定ダイアログを表示する
        /// </summary>
        private async Task ShowSettingsDialogAsync(SettingsDialog settingsDialog)
        {
            // ダイアログを表示
            var result = await settingsDialog.ShowAsync(
                ViewModel.CurrentMenu, 
                _windowHandle, 
                this.Content.XamlRoot);
            
            if (result == ContentDialogResult.Primary && !settingsDialog.IsCancelled)
            {
                // 設定が更新された場合
                ViewModel.UpdateMenuSettings(settingsDialog.Result);
            }
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
        private async void MenuButtonsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MenuButtonViewModel button)
            {
                if (ViewModel.IsEditMode)
                {
                    // 編集モードの場合はダイアログを表示
                    await ShowButtonEditDialogAsync(button.Index);
                }
                else
                {
                    // 実行モードの場合はボタンコマンドを実行
                    ViewModel.ButtonClickCommand.Execute(button.Index);
                }
            }
        }

        /// <summary>
        /// ボタン編集ダイアログを表示する
        /// </summary>
        private async Task ShowButtonEditDialogAsync(int buttonIndex)
        {
            var editDialog = new ButtonEditDialog();
            var menuItem = ViewModel.GetMenuItem(buttonIndex);
            bool isEscapeButton = ViewModel.IsEscapeButton(buttonIndex);
            
            var result = await editDialog.ShowAsync(menuItem, isEscapeButton, this.Content.XamlRoot);
            
            if (result == ContentDialogResult.Primary && !editDialog.IsCancelled)
            {
                // 編集が完了した場合
                ViewModel.UpdateMenuItem(buttonIndex, editDialog.Result, editDialog.ViewModel.IsEscapeButton);
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
                if (_menuButtonsGrid != null)
                {
                    _menuButtonsGrid.SelectedIndex = index;
                }

                // 右クリックメニューを表示
                if (!ViewModel.IsEditMode)
                {
                    // 実行モードの場合は何もしない
                    return;
                }

                // 編集メニューを表示
                ViewModel.SelectedButtonIndex = index;
                
                // コンテキストメニューを表示
                var flyout = new MenuFlyout();
                
                var cutItem = new MenuFlyoutItem { Text = "切り取り" };
                cutItem.Click += (s, args) => ViewModel.CutCommand.Execute(null);
                flyout.Items.Add(cutItem);
                
                var copyItem = new MenuFlyoutItem { Text = "コピー" };
                copyItem.Click += (s, args) => ViewModel.CopyCommand.Execute(null);
                flyout.Items.Add(copyItem);
                
                var pasteItem = new MenuFlyoutItem { Text = "貼り付け" };
                pasteItem.Click += (s, args) => ViewModel.PasteCommand.Execute(null);
                pasteItem.IsEnabled = ViewModel.CanPaste;
                flyout.Items.Add(pasteItem);
                
                flyout.Items.Add(new MenuFlyoutSeparator());
                
                var deleteItem = new MenuFlyoutItem { Text = "削除" };
                deleteItem.Click += (s, args) => ViewModel.DeleteCommand.Execute(null);
                flyout.Items.Add(deleteItem);
                
                flyout.Items.Add(new MenuFlyoutSeparator());
                
                var toggleHiddenItem = new ToggleMenuFlyoutItem { Text = "非表示", IsChecked = ViewModel.IsCurrentItemHidden };
                toggleHiddenItem.Click += (s, args) => ViewModel.ToggleHiddenCommand.Execute(null);
                flyout.Items.Add(toggleHiddenItem);
                
                var toggleEscapeItem = new ToggleMenuFlyoutItem { Text = "ESCキー", IsChecked = ViewModel.IsCurrentItemEscapeButton };
                toggleEscapeItem.Click += (s, args) => ViewModel.ToggleEscapeCommand.Execute(null);
                flyout.Items.Add(toggleEscapeItem);
                
                // FlyoutShowOptionsを使用する際にPrimitives名前空間が必要
                flyout.ShowAt(element, new FlyoutShowOptions { Position = e.GetPosition(element) });
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
