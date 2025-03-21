using System;

namespace supLauncher.Core.Models
{
    /// <summary>
    /// メニュー項目を表すクラス
    /// 元のCMenuFileItemInfクラスの機能を継承
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// メニュー項目の属性（実行方法）
        /// </summary>
        public enum ItemAttribute
        {
            /// <summary>アプリケーションを実行</summary>
            ExecApplication,
            
            /// <summary>次のメニューを開く</summary>
            OpenNextMenu,
            
            /// <summary>前のメニューに戻る</summary>
            BackPrevMenu
        }

        /// <summary>
        /// メニュー項目実行後のアクション
        /// </summary>
        public enum ItemAfter
        {
            /// <summary>ランチャーをそのまま継続</summary>
            ContinueHiMenu,
            
            /// <summary>ランチャーを終了</summary>
            EndHiMenu,
            
            /// <summary>ランチャーを最小化</summary>
            MinimizedHiMenu
        }

        /// <summary>メニュー項目のタイトル</summary>
        public string Title { get; set; } = "";
        
        /// <summary>メニュー項目の説明</summary>
        public string Comment { get; set; } = "";
        
        /// <summary>実行するコマンド</summary>
        public string Command { get; set; } = "";
        
        /// <summary>実行方法</summary>
        public ItemAttribute Attribute { get; set; }
        
        /// <summary>実行後のアクション</summary>
        public ItemAfter After { get; set; }
        
        /// <summary>非表示フラグ</summary>
        public bool NoUse { get; set; } = false;

        /// <summary>
        /// フラグ文字列を取得または設定
        /// </summary>
        public string Flag
        {
            get
            {
                string[] strFlag = new string[] { "", "", "" };

                switch (this.Attribute)
                {
                    case ItemAttribute.ExecApplication:
                        strFlag[0] = "-";
                        break;
                    case ItemAttribute.OpenNextMenu:
                        strFlag[0] = "*";
                        break;
                    case ItemAttribute.BackPrevMenu:
                        strFlag[0] = "B";
                        break;
                }

                switch (this.After)
                {
                    case ItemAfter.ContinueHiMenu:
                        strFlag[1] = "-";
                        break;
                    case ItemAfter.EndHiMenu:
                        strFlag[1] = "*";
                        break;
                    case ItemAfter.MinimizedHiMenu:
                        strFlag[1] = "I";
                        break;
                }

                switch (this.NoUse)
                {
                    case true:
                        strFlag[2] = "*";
                        break;
                    case false:
                        strFlag[2] = "-";
                        break;
                }

                return string.Join("", strFlag);
            }
            set
            {
                string strFlag = value + "---";

                switch (strFlag[0])
                {
                    case '-':
                        this.Attribute = ItemAttribute.ExecApplication;
                        break;
                    case '*':
                        this.Attribute = ItemAttribute.OpenNextMenu;
                        break;
                    case 'B':
                        this.Attribute = ItemAttribute.BackPrevMenu;
                        break;
                }

                switch (strFlag[1])
                {
                    case '-':
                        this.After = ItemAfter.ContinueHiMenu;
                        break;
                    case '*':
                        this.After = ItemAfter.EndHiMenu;
                        break;
                    case 'I':
                        this.After = ItemAfter.MinimizedHiMenu;
                        break;
                }

                switch (strFlag[2])
                {
                    case '*':
                        this.NoUse = true;
                        break;
                    case '-':
                        this.NoUse = false;
                        break;
                }
            }
        }
    }
}
