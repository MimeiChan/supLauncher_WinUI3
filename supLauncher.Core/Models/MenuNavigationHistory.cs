using System.Collections.Generic;

namespace supLauncher.Core.Models
{
    /// <summary>
    /// メニューのナビゲーション履歴を管理するクラス
    /// 元のCMenuChainクラスの機能を継承
    /// </summary>
    public class MenuNavigationHistory
    {
        private readonly Stack<MenuHistoryItem> _historyStack = new Stack<MenuHistoryItem>();

        /// <summary>
        /// メニュー履歴の項目を表す内部クラス
        /// </summary>
        public class MenuHistoryItem
        {
            /// <summary>ファイル名</summary>
            public string FileName { get; set; } = "";
            
            /// <summary>選択ボタンのインデックス</summary>
            public int ButtonIndex { get; set; }
        }

        /// <summary>
        /// 履歴をクリアする
        /// </summary>
        public void Clear()
        {
            _historyStack.Clear();
        }

        /// <summary>
        /// 現在、ルートメニューかどうかを取得する
        /// </summary>
        /// <returns>ルートメニューの場合はtrue</returns>
        public bool IsRootMenu()
        {
            return _historyStack.Count == 0;
        }

        /// <summary>
        /// 履歴にメニュー情報を追加する
        /// </summary>
        /// <param name="fileName">メニューファイル名</param>
        /// <param name="buttonIndex">選択ボタンのインデックス</param>
        public void Push(string fileName, int buttonIndex)
        {
            _historyStack.Push(new MenuHistoryItem
            {
                FileName = fileName,
                ButtonIndex = buttonIndex
            });
        }

        /// <summary>
        /// 履歴から1つ前のメニュー情報を取得する
        /// </summary>
        /// <returns>前のメニュー情報。履歴がない場合はnull</returns>
        public MenuHistoryItem? Pop()
        {
            if (_historyStack.Count > 0)
            {
                return _historyStack.Pop();
            }
            return null;
        }

        /// <summary>
        /// 履歴から現在のメニュー情報を参照する（取り出さない）
        /// </summary>
        /// <returns>現在のメニュー情報。履歴がない場合はnull</returns>
        public MenuHistoryItem? Peek()
        {
            if (_historyStack.Count > 0)
            {
                return _historyStack.Peek();
            }
            return null;
        }

        /// <summary>
        /// 履歴の数を取得する
        /// </summary>
        public int Count => _historyStack.Count;
    }
}
