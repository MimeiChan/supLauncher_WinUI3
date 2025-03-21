using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using supLauncher.Core.Models;
using supLauncher.WinUI.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace supLauncher.WinUI.Views
{
    /// <summary>
    /// ボタン編集ダイアログ
    /// </summary>
    public sealed partial class ButtonEditDialog : Page
    {
        /// <summary>
        /// ビューモデル
        /// </summary>
        public ButtonEditViewModel ViewModel { get; } = new ButtonEditViewModel();

        /// <summary>
        /// ダイアログ結果
        /// </summary>
        public MenuItem Result { get; private set; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public bool IsCancelled { get; private set; } = true;

        private ContentDialog _dialog;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ButtonEditDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        /// <param name="menuItem">編集対象のメニュー項目</param>
        /// <param name="isEscapeButton">ESCキーとして設定されているかどうか</param>
        /// <returns>ダイアログの結果</returns>
        public async Task<ContentDialogResult> ShowAsync(MenuItem menuItem, bool isEscapeButton, XamlRoot xamlRoot)
        {
            // ビューモデルを設定
            ViewModel.SetMenuItem(menuItem);
            ViewModel.IsEscapeButton = isEscapeButton;

            // ダイアログを作成
            _dialog = new ContentDialog
            {
                Title = "メニュー項目の編集",
                Content = this,
                PrimaryButtonText = "OK",
                CloseButtonText = "キャンセル",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            // ダイアログを表示
            return await _dialog.ShowAsync();
        }

        /// <summary>
        /// OKボタンがクリックされた
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // 結果を設定
            Result = ViewModel.GetUpdatedMenuItem();
            IsCancelled = false;

            // ダイアログを閉じる
            _dialog.Hide();
        }

        /// <summary>
        /// キャンセルボタンがクリックされた
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // キャンセルフラグを設定
            IsCancelled = true;

            // ダイアログを閉じる
            _dialog.Hide();
        }

        /// <summary>
        /// 参照ボタンがクリックされた
        /// </summary>
        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // ファイルを開くダイアログを表示
            var filePicker = new FileOpenPicker();
            
            // ピッカーを初期化
            var window = (Application.Current as App).MainWindow;
            if (window != null)
            {
                var hwnd = WindowNative.GetWindowHandle(window);
                InitializeWithWindow.Initialize(filePicker, hwnd);
            }
            
            filePicker.FileTypeFilter.Add("*");
            filePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            var file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                // 選択されたファイルのパスをコマンドとして設定
                ViewModel.Command = file.Path;
            }
        }
    }
}
