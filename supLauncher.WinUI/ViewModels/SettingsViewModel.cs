using CommunityToolkit.Mvvm.ComponentModel;
using supLauncher.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace supLauncher.WinUI.ViewModels
{
    /// <summary>
    /// 環境設定ダイアログのビューモデル
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        private MenuPage _menuPage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsViewModel()
        {
            // 利用可能なフォントを取得
            AvailableFonts = new List<string>
            {
                "MS UI Gothic",
                "ＭＳ Ｐゴシック",
                "ＭＳ ゴシック",
                "Yu Gothic UI",
                "Yu Gothic",
                "Meiryo UI",
                "メイリオ",
                "Arial",
                "Times New Roman"
            };
        }

        /// <summary>
        /// メニューページをセット
        /// </summary>
        /// <param name="menuPage">設定対象のメニューページ</param>
        public void SetMenuPage(MenuPage menuPage)
        {
            _menuPage = menuPage;
            
            // 基本設定
            Title = menuPage.Title;
            Rows = menuPage.Rows;
            Columns = menuPage.Columns;
            Width = menuPage.Width;
            Height = menuPage.Height;
            MenuVisible = menuPage.MenuVisible;
            StatusBarVisible = menuPage.StatusBarVisible;
            
            // 表示位置
            switch (menuPage.DisplayPosition)
            {
                case MenuPage.MenuDispPosition.RootMenu:
                    IsRootMenuPosition = true;
                    break;
                case MenuPage.MenuDispPosition.CurrentMenu:
                    IsCurrentMenuPosition = true;
                    break;
                case MenuPage.MenuDispPosition.ScreenCenter:
                    IsScreenCenterPosition = true;
                    break;
            }
            
            // フォント設定
            FontName = menuPage.FontName;
            FontSize = menuPage.FontSize;
            FontBold = menuPage.FontBold;
            FontItalic = menuPage.FontItalic;
            FontUnderline = menuPage.FontUnderline;
            
            // 色設定
            BackColor = menuPage.BackColor;
            ButtonColor = menuPage.ButtonColor;
            TextColor = menuPage.TextColor;
            HighlightTextColor = menuPage.HighlightTextColor;
            
            // 背景画像
            BackgroundImageFile = menuPage.BackgroundImageFile;
            BackgroundImageTile = menuPage.BackgroundImageTile;
            
            // パスワード
            Password = menuPage.LockPassword;
            PasswordConfirm = menuPage.LockPassword;
            
            // プロパティ変更通知
            UpdateColorTexts();
        }

        /// <summary>
        /// 更新されたメニューページを取得
        /// </summary>
        /// <returns>更新されたメニューページ</returns>
        public MenuPage GetUpdatedMenuPage()
        {
            // 現在の設定をメニューページに反映
            _menuPage.Title = Title;
            _menuPage.Rows = Rows;
            _menuPage.Columns = Columns;
            _menuPage.Width = Width;
            _menuPage.Height = Height;
            _menuPage.MenuVisible = MenuVisible;
            _menuPage.StatusBarVisible = StatusBarVisible;
            
            // 表示位置
            if (IsRootMenuPosition)
                _menuPage.DisplayPosition = MenuPage.MenuDispPosition.RootMenu;
            else if (IsCurrentMenuPosition)
                _menuPage.DisplayPosition = MenuPage.MenuDispPosition.CurrentMenu;
            else if (IsScreenCenterPosition)
                _menuPage.DisplayPosition = MenuPage.MenuDispPosition.ScreenCenter;
            
            // フォント設定
            _menuPage.FontName = FontName;
            _menuPage.FontSize = FontSize;
            _menuPage.FontBold = FontBold;
            _menuPage.FontItalic = FontItalic;
            _menuPage.FontUnderline = FontUnderline;
            
            // 色設定
            _menuPage.BackColor = BackColor;
            _menuPage.ButtonColor = ButtonColor;
            _menuPage.TextColor = TextColor;
            _menuPage.HighlightTextColor = HighlightTextColor;
            
            // 背景画像
            _menuPage.BackgroundImageFile = BackgroundImageFile;
            _menuPage.BackgroundImageTile = BackgroundImageTile;
            
            // パスワード
            if (Password == PasswordConfirm)
            {
                _menuPage.LockPassword = Password;
                _menuPage.LockOn = !string.IsNullOrEmpty(Password);
            }
            
            return _menuPage;
        }

        /// <summary>
        /// 色テキストを更新する
        /// </summary>
        private void UpdateColorTexts()
        {
            BackColorText = $"#{BackColor.R:X2}{BackColor.G:X2}{BackColor.B:X2}";
            ButtonColorText = $"#{ButtonColor.R:X2}{ButtonColor.G:X2}{ButtonColor.B:X2}";
            TextColorText = $"#{TextColor.R:X2}{TextColor.G:X2}{TextColor.B:X2}";
            HighlightTextColorText = $"#{HighlightTextColor.R:X2}{HighlightTextColor.G:X2}{HighlightTextColor.B:X2}";
        }

        /// <summary>
        /// パスワードエラーチェック
        /// </summary>
        public bool ValidatePassword()
        {
            if (Password != PasswordConfirm)
            {
                PasswordError = "パスワードが一致しません";
                return false;
            }
            
            PasswordError = "";
            return true;
        }

        #region プロパティ

        /// <summary>
        /// 利用可能なフォント
        /// </summary>
        public List<string> AvailableFonts { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        [ObservableProperty]
        private string _title = "";

        /// <summary>
        /// 行数
        /// </summary>
        [ObservableProperty]
        private int _rows = 10;

        /// <summary>
        /// 列数
        /// </summary>
        [ObservableProperty]
        private int _columns = 2;

        /// <summary>
        /// 幅
        /// </summary>
        [ObservableProperty]
        private int _width = 559;

        /// <summary>
        /// 高さ
        /// </summary>
        [ObservableProperty]
        private int _height = 406;

        /// <summary>
        /// メニューバー表示
        /// </summary>
        [ObservableProperty]
        private bool _menuVisible = true;

        /// <summary>
        /// ステータスバー表示
        /// </summary>
        [ObservableProperty]
        private bool _statusBarVisible = true;

        /// <summary>
        /// ルートメニュー位置
        /// </summary>
        [ObservableProperty]
        private bool _isRootMenuPosition = true;

        /// <summary>
        /// 現在のメニュー位置
        /// </summary>
        [ObservableProperty]
        private bool _isCurrentMenuPosition = false;

        /// <summary>
        /// 画面中央位置
        /// </summary>
        [ObservableProperty]
        private bool _isScreenCenterPosition = false;

        /// <summary>
        /// フォント名
        /// </summary>
        [ObservableProperty]
        private string _fontName = "ＭＳ Ｐゴシック";

        /// <summary>
        /// フォントサイズ
        /// </summary>
        [ObservableProperty]
        private float _fontSize = 12;

        /// <summary>
        /// フォント太字
        /// </summary>
        [ObservableProperty]
        private bool _fontBold = true;

        /// <summary>
        /// フォント斜体
        /// </summary>
        [ObservableProperty]
        private bool _fontItalic = false;

        /// <summary>
        /// フォント下線
        /// </summary>
        [ObservableProperty]
        private bool _fontUnderline = false;

        /// <summary>
        /// 背景色
        /// </summary>
        [ObservableProperty]
        private Color _backColor = SystemColors.ControlDark;

        /// <summary>
        /// 背景色テキスト
        /// </summary>
        [ObservableProperty]
        private string _backColorText = "";

        /// <summary>
        /// ボタン色
        /// </summary>
        [ObservableProperty]
        private Color _buttonColor = SystemColors.Control;

        /// <summary>
        /// ボタン色テキスト
        /// </summary>
        [ObservableProperty]
        private string _buttonColorText = "";

        /// <summary>
        /// テキスト色
        /// </summary>
        [ObservableProperty]
        private Color _textColor = SystemColors.ControlText;

        /// <summary>
        /// テキスト色テキスト
        /// </summary>
        [ObservableProperty]
        private string _textColorText = "";

        /// <summary>
        /// ハイライトテキスト色
        /// </summary>
        [ObservableProperty]
        private Color _highlightTextColor = SystemColors.Highlight;

        /// <summary>
        /// ハイライトテキスト色テキスト
        /// </summary>
        [ObservableProperty]
        private string _highlightTextColorText = "";

        /// <summary>
        /// 背景画像ファイル
        /// </summary>
        [ObservableProperty]
        private string _backgroundImageFile = "";

        /// <summary>
        /// 背景画像タイル表示
        /// </summary>
        [ObservableProperty]
        private bool _backgroundImageTile = false;

        /// <summary>
        /// パスワード
        /// </summary>
        [ObservableProperty]
        private string _password = "";

        /// <summary>
        /// パスワード確認
        /// </summary>
        [ObservableProperty]
        private string _passwordConfirm = "";

        /// <summary>
        /// パスワードエラー
        /// </summary>
        [ObservableProperty]
        private string _passwordError = "";

        #endregion
    }
}
