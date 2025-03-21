using System.Collections.Generic;
using System.Drawing;

namespace supLauncher.Core.Models
{
    /// <summary>
    /// メニューページの設定情報を管理するクラス
    /// 元のCMenuPageクラスの設定データ部分を分離
    /// </summary>
    public class MenuPage
    {
        /// <summary>
        /// メニューフォームの表示位置を定義する列挙型
        /// </summary>
        public enum MenuDispPosition
        {
            /// <summary>ルートメニューと同じ位置</summary>
            RootMenu,
            
            /// <summary>現在のメニューの位置</summary>
            CurrentMenu,
            
            /// <summary>画面中央</summary>
            ScreenCenter
        }

        /// <summary>メニューページのタイトル</summary>
        public string Title { get; set; } = "";

        /// <summary>メニューの行数</summary>
        public int Rows { get; set; } = 10;

        /// <summary>メニューの列数</summary>
        public int Columns { get; set; } = 2;

        /// <summary>ウィンドウの幅</summary>
        public int Width { get; set; } = 559;

        /// <summary>ウィンドウの高さ</summary>
        public int Height { get; set; } = 406;

        /// <summary>現在のX座標</summary>
        public int CurrentX { get; set; } = 0;

        /// <summary>現在のY座標</summary>
        public int CurrentY { get; set; } = 0;

        /// <summary>フォント名</summary>
        public string FontName { get; set; } = "ＭＳ Ｐゴシック";

        /// <summary>フォントサイズ</summary>
        public float FontSize { get; set; } = 12;

        /// <summary>太字フラグ</summary>
        public bool FontBold { get; set; } = true;

        /// <summary>斜体フラグ</summary>
        public bool FontItalic { get; set; } = false;

        /// <summary>下線フラグ</summary>
        public bool FontUnderline { get; set; } = false;

        /// <summary>表示位置</summary>
        public MenuDispPosition DisplayPosition { get; set; } = MenuDispPosition.RootMenu;

        /// <summary>背景画像ファイルパス</summary>
        public string BackgroundImageFile { get; set; } = "";

        /// <summary>背景画像をタイル表示するかどうか</summary>
        public bool BackgroundImageTile { get; set; } = false;

        /// <summary>メニューバーを表示するかどうか</summary>
        public bool MenuVisible { get; set; } = true;

        /// <summary>ステータスバーを表示するかどうか</summary>
        public bool StatusBarVisible { get; set; } = true;

        /// <summary>編集ロックがかかっているかどうか</summary>
        public bool LockOn { get; set; } = false;

        /// <summary>ロックパスワード</summary>
        public string LockPassword { get; set; } = "";

        /// <summary>背景色</summary>
        public Color BackColor { get; set; } = SystemColors.ControlDark;

        /// <summary>ボタンの色</summary>
        public Color ButtonColor { get; set; } = SystemColors.Control;

        /// <summary>テキストの色</summary>
        public Color TextColor { get; set; } = SystemColors.ControlText;

        /// <summary>ハイライト時のテキスト色</summary>
        public Color HighlightTextColor { get; set; } = SystemColors.Highlight;

        /// <summary>キャンセルボタンのインデックス</summary>
        public int CancelButtonIndex { get; set; } = -1;

        /// <summary>現在選択中のボタンインデックス</summary>
        public int CurrentButtonIndex { get; set; } = 0;

        /// <summary>メニュー項目のコレクション</summary>
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        /// <summary>メニューファイル名</summary>
        public string FileName { get; set; } = "";

        /// <summary>内容が変更されたかどうか</summary>
        public bool IsChanged { get; set; } = false;
    }
}
