using CommunityToolkit.Mvvm.ComponentModel;
using supLauncher.Core.Models;

namespace supLauncher.WinUI.ViewModels
{
    /// <summary>
    /// ボタン編集ダイアログのビューモデル
    /// </summary>
    public partial class ButtonEditViewModel : ObservableObject
    {
        private MenuItem _menuItem;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ButtonEditViewModel()
        {
            _menuItem = new MenuItem();
        }

        /// <summary>
        /// メニュー項目をセット
        /// </summary>
        /// <param name="menuItem">編集対象のメニュー項目</param>
        public void SetMenuItem(MenuItem menuItem)
        {
            _menuItem = menuItem;
            
            // プロパティを更新
            Title = menuItem.Title;
            Comment = menuItem.Comment;
            Command = menuItem.Command;
            
            // 実行方法
            IsExecApplication = (menuItem.Attribute == MenuItem.ItemAttribute.ExecApplication);
            IsOpenNextMenu = (menuItem.Attribute == MenuItem.ItemAttribute.OpenNextMenu);
            IsBackPrevMenu = (menuItem.Attribute == MenuItem.ItemAttribute.BackPrevMenu);
            
            // 実行後
            IsContinueHiMenu = (menuItem.After == MenuItem.ItemAfter.ContinueHiMenu);
            IsEndHiMenu = (menuItem.After == MenuItem.ItemAfter.EndHiMenu);
            IsMinimizedHiMenu = (menuItem.After == MenuItem.ItemAfter.MinimizedHiMenu);
            
            // その他の設定
            IsHidden = menuItem.NoUse;
            
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Comment));
            OnPropertyChanged(nameof(Command));
            OnPropertyChanged(nameof(IsExecApplication));
            OnPropertyChanged(nameof(IsOpenNextMenu));
            OnPropertyChanged(nameof(IsBackPrevMenu));
            OnPropertyChanged(nameof(IsContinueHiMenu));
            OnPropertyChanged(nameof(IsEndHiMenu));
            OnPropertyChanged(nameof(IsMinimizedHiMenu));
            OnPropertyChanged(nameof(IsHidden));
            OnPropertyChanged(nameof(IsEscapeButton));
        }

        /// <summary>
        /// 更新されたメニュー項目を取得
        /// </summary>
        /// <returns>更新されたメニュー項目</returns>
        public MenuItem GetUpdatedMenuItem()
        {
            // 現在の設定をメニュー項目に反映
            _menuItem.Title = Title;
            _menuItem.Comment = Comment;
            _menuItem.Command = Command;
            
            // 実行方法
            if (IsExecApplication)
                _menuItem.Attribute = MenuItem.ItemAttribute.ExecApplication;
            else if (IsOpenNextMenu)
                _menuItem.Attribute = MenuItem.ItemAttribute.OpenNextMenu;
            else if (IsBackPrevMenu)
                _menuItem.Attribute = MenuItem.ItemAttribute.BackPrevMenu;
            
            // 実行後
            if (IsContinueHiMenu)
                _menuItem.After = MenuItem.ItemAfter.ContinueHiMenu;
            else if (IsEndHiMenu)
                _menuItem.After = MenuItem.ItemAfter.EndHiMenu;
            else if (IsMinimizedHiMenu)
                _menuItem.After = MenuItem.ItemAfter.MinimizedHiMenu;
            
            // その他の設定
            _menuItem.NoUse = IsHidden;
            
            return _menuItem;
        }

        #region プロパティ

        /// <summary>
        /// タイトル
        /// </summary>
        [ObservableProperty]
        private string _title = "";

        /// <summary>
        /// コメント
        /// </summary>
        [ObservableProperty]
        private string _comment = "";

        /// <summary>
        /// コマンド
        /// </summary>
        [ObservableProperty]
        private string _command = "";

        /// <summary>
        /// アプリケーション実行かどうか
        /// </summary>
        [ObservableProperty]
        private bool _isExecApplication = true;

        /// <summary>
        /// 次のメニューを開くかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isOpenNextMenu = false;

        /// <summary>
        /// 前のメニューに戻るかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isBackPrevMenu = false;

        /// <summary>
        /// ランチャーを継続するかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isContinueHiMenu = true;

        /// <summary>
        /// ランチャーを終了するかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isEndHiMenu = false;

        /// <summary>
        /// ランチャーを最小化するかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isMinimizedHiMenu = false;

        /// <summary>
        /// 非表示かどうか
        /// </summary>
        [ObservableProperty]
        private bool _isHidden = false;

        /// <summary>
        /// ESCキーとして設定するかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isEscapeButton = false;

        #endregion
    }
}
