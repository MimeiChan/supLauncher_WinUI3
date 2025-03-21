using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using supLauncher.Core.Models;
using supLauncher.WinUI.ViewModels;
using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT.Interop;

namespace supLauncher.WinUI.Views
{
    /// <summary>
    /// 環境設定ダイアログ
    /// </summary>
    public sealed partial class SettingsDialog : Page
    {
        /// <summary>
        /// ビューモデル
        /// </summary>
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        /// <summary>
        /// ダイアログ結果
        /// </summary>
        public MenuPage Result { get; private set; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public bool IsCancelled { get; private set; } = true;

        private ContentDialog _dialog;
        private IntPtr _windowHandle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// ダイアログを表示する
        /// </summary>
        /// <param name="menuPage">編集対象のメニューページ</param>
        /// <param name="windowHandle">ウィンドウハンドル</param>
        /// <param name="xamlRoot">XamlRoot</param>
        /// <returns>ダイアログの結果</returns>
        public async Task<ContentDialogResult> ShowAsync(MenuPage menuPage, IntPtr windowHandle, XamlRoot xamlRoot)
        {
            // ウィンドウハンドルを保存
            _windowHandle = windowHandle;

            // ビューモデルを設定
            ViewModel.SetMenuPage(menuPage);

            // ダイアログを作成
            _dialog = new ContentDialog
            {
                Title = "環境設定",
                Content = this,
                PrimaryButtonText = "OK",
                CloseButtonText = "キャンセル",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            // OKボタンクリック時の処理
            _dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

            // ダイアログを表示
            return await _dialog.ShowAsync();
        }

        /// <summary>
        /// OKボタンがクリックされた
        /// </summary>
        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // パスワード検証
            if (!ViewModel.ValidatePassword())
            {
                // パスワードが一致しない場合はダイアログを閉じない
                args.Cancel = true;
                return;
            }

            // 結果を設定
            Result = ViewModel.GetUpdatedMenuPage();
            IsCancelled = false;
        }

        /// <summary>
        /// 背景色選択ボタンがクリックされた
        /// </summary>
        private async void BackColorButton_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = ViewModel.BackColor;
            var newColor = await ShowColorPickerAsync("背景色の選択", currentColor);
            if (newColor.HasValue)
            {
                ViewModel.BackColor = newColor.Value;
                ViewModel.BackColorText = $"#{newColor.Value.R:X2}{newColor.Value.G:X2}{newColor.Value.B:X2}";
            }
        }

        /// <summary>
        /// ボタン色選択ボタンがクリックされた
        /// </summary>
        private async void ButtonColorButton_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = ViewModel.ButtonColor;
            var newColor = await ShowColorPickerAsync("ボタン色の選択", currentColor);
            if (newColor.HasValue)
            {
                ViewModel.ButtonColor = newColor.Value;
                ViewModel.ButtonColorText = $"#{newColor.Value.R:X2}{newColor.Value.G:X2}{newColor.Value.B:X2}";
            }
        }

        /// <summary>
        /// 文字色選択ボタンがクリックされた
        /// </summary>
        private async void TextColorButton_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = ViewModel.TextColor;
            var newColor = await ShowColorPickerAsync("文字色の選択", currentColor);
            if (newColor.HasValue)
            {
                ViewModel.TextColor = newColor.Value;
                ViewModel.TextColorText = $"#{newColor.Value.R:X2}{newColor.Value.G:X2}{newColor.Value.B:X2}";
            }
        }

        /// <summary>
        /// 選択文字色選択ボタンがクリックされた
        /// </summary>
        private async void HighlightTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            var currentColor = ViewModel.HighlightTextColor;
            var newColor = await ShowColorPickerAsync("選択時文字色の選択", currentColor);
            if (newColor.HasValue)
            {
                ViewModel.HighlightTextColor = newColor.Value;
                ViewModel.HighlightTextColorText = $"#{newColor.Value.R:X2}{newColor.Value.G:X2}{newColor.Value.B:X2}";
            }
        }

        /// <summary>
        /// 背景画像参照ボタンがクリックされた
        /// </summary>
        private async void BrowseBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            // ファイルを開くダイアログを表示
            var filePicker = new FileOpenPicker();
            
            // ピッカーを初期化
            InitializeWithWindow.Initialize(filePicker, _windowHandle);
            
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            var file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                // 選択された画像ファイルのパスを設定
                ViewModel.BackgroundImageFile = file.Path;
            }
        }

        /// <summary>
        /// 背景画像クリアボタンがクリックされた
        /// </summary>
        private void ClearBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BackgroundImageFile = "";
        }

        /// <summary>
        /// 色選択ダイアログを表示する
        /// </summary>
        /// <param name="title">ダイアログタイトル</param>
        /// <param name="initialColor">初期色</param>
        /// <returns>選択された色</returns>
        private async Task<Color?> ShowColorPickerAsync(string title, Color initialColor)
        {
            // カラーピッカーを作成
            ColorPicker colorPicker = new ColorPicker();
            
            // 初期色を設定
            Windows.UI.Color winUIColor = new Windows.UI.Color
            {
                R = initialColor.R,
                G = initialColor.G,
                B = initialColor.B,
                A = 255
            };
            colorPicker.Color = winUIColor;

            // ダイアログを作成
            ContentDialog colorDialog = new ContentDialog
            {
                Title = title,
                Content = colorPicker,
                PrimaryButtonText = "OK",
                CloseButtonText = "キャンセル",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            // ダイアログを表示
            var result = await colorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // 選択された色を System.Drawing.Color に変換して返す
                return Color.FromArgb(
                    colorPicker.Color.R,
                    colorPicker.Color.G,
                    colorPicker.Color.B);
            }

            return null;
        }
    }
}
