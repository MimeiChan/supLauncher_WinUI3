using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using supLauncher.Core.Models;
using supLauncher.Core.Services;

namespace supLauncher.WinUI.ViewModels
{
    /// <summary>
    /// メインウィンドウのビューモデル
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly MenuService _menuService;
        private readonly ApplicationLaunchService _launchService;
        private readonly MenuNavigationHistory _navigationHistory;

        /// <summary>
        /// 現在のメニューページ
        /// </summary>
        private MenuPage _currentMenu;

        /// <summary>
        /// 現在のメニューページを取得
        /// </summary>
        public MenuPage CurrentMenu => _currentMenu;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="menuService">メニューサービス</param>
        /// <param name="launchService">アプリケーション起動サービス</param>
        /// <param name="navigationHistory">ナビゲーション履歴</param>
        public MainWindowViewModel(
            MenuService menuService,
            ApplicationLaunchService launchService,
            MenuNavigationHistory navigationHistory)
        {
            _menuService = menuService;
            _launchService = launchService;
            _navigationHistory = navigationHistory;

            // デフォルトのメニューを読み込む
            LoadDefaultMenu();

            // コマンド初期化
            InitializeCommands();
        }

        #region プロパティ

        /// <summary>
        /// メニューボタンのコレクション
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MenuButtonViewModel> _menuButtons = new ObservableCollection<MenuButtonViewModel>();

        /// <summary>
        /// 選択されているボタンのインデックス
        /// </summary>
        [ObservableProperty]
        private int _selectedButtonIndex = 0;

        /// <summary>
        /// 背景ブラシ
        /// </summary>
        [ObservableProperty]
        private SolidColorBrush _backgroundBrush = new SolidColorBrush(Microsoft.UI.Colors.LightGray);

        /// <summary>
        /// 背景画像ソース
        /// </summary>
        [ObservableProperty]
        private ImageSource _backgroundImageSource;

        /// <summary>
        /// 背景画像の表示方法
        /// </summary>
        [ObservableProperty]
        private Stretch _backgroundImageStretch = Stretch.None;

        /// <summary>
        /// 背景画像の表示状態
        /// </summary>
        [ObservableProperty]
        private Visibility _backgroundImageVisibility = Visibility.Collapsed;

        /// <summary>
        /// メニューバーの表示状態
        /// </summary>
        [ObservableProperty]
        private Visibility _menuBarVisibility = Visibility.Visible;

        /// <summary>
        /// 編集メニューの表示状態
        /// </summary>
        [ObservableProperty]
        private Visibility _editMenuVisibility = Visibility.Visible;

        /// <summary>
        /// ステータスバーの表示状態
        /// </summary>
        [ObservableProperty]
        private Visibility _statusBarVisibility = Visibility.Visible;

        /// <summary>
        /// ステータステキスト
        /// </summary>
        [ObservableProperty]
        private string _statusText = "";

        /// <summary>
        /// モードテキスト
        /// </summary>
        [ObservableProperty]
        private string _modeText = "編集モード";

        /// <summary>
        /// 編集モードかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isEditMode = true;

        /// <summary>
        /// 実行モードかどうか
        /// </summary>
        public bool IsExecuteMode
        {
            get => !IsEditMode;
            set => IsEditMode = !value;
        }

        /// <summary>
        /// 現在のメニュー項目が非表示かどうか
        /// </summary>
        [ObservableProperty]
        private bool _isCurrentItemHidden = false;

        /// <summary>
        /// 現在のメニュー項目がESCキーかどうか
        /// </summary>
        [ObservableProperty]
        private bool _isCurrentItemEscapeButton = false;

        /// <summary>
        /// 貼り付けが可能かどうか
        /// </summary>
        [ObservableProperty]
        private bool _canPaste = false;

        /// <summary>
        /// メニューの列数
        /// </summary>
        [ObservableProperty]
        private int _menuColumns = 2;

        #endregion

        #region コマンド

        /// <summary>
        /// 新規作成コマンド
        /// </summary>
        public ICommand NewCommand { get; private set; }

        /// <summary>
        /// 開くコマンド
        /// </summary>
        public ICommand OpenCommand { get; private set; }

        /// <summary>
        /// 環境設定コマンド
        /// </summary>
        public ICommand SettingsCommand { get; private set; }

        /// <summary>
        /// 終了コマンド
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// 切り取りコマンド
        /// </summary>
        public ICommand CutCommand { get; private set; }

        /// <summary>
        /// コピーコマンド
        /// </summary>
        public ICommand CopyCommand { get; private set; }

        /// <summary>
        /// 貼り付けコマンド
        /// </summary>
        public ICommand PasteCommand { get; private set; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// 非表示トグルコマンド
        /// </summary>
        public ICommand ToggleHiddenCommand { get; private set; }

        /// <summary>
        /// ESCキートグルコマンド
        /// </summary>
        public ICommand ToggleEscapeCommand { get; private set; }

        /// <summary>
        /// ボタンクリックコマンド
        /// </summary>
        public ICommand ButtonClickCommand { get; private set; }

        #endregion

        #region メソッド

        /// <summary>
        /// コマンド初期化
        /// </summary>
        private void InitializeCommands()
        {
            NewCommand = new RelayCommand<string>(ExecuteNewCommand);
            OpenCommand = new RelayCommand<string>(ExecuteOpenCommand);
            SettingsCommand = new RelayCommand(ExecuteSettingsCommand);
            ExitCommand = new RelayCommand(ExecuteExitCommand);

            CutCommand = new RelayCommand(ExecuteCutCommand);
            CopyCommand = new RelayCommand(ExecuteCopyCommand);
            PasteCommand = new RelayCommand(ExecutePasteCommand, () => CanPaste);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);

            ToggleHiddenCommand = new RelayCommand(ExecuteToggleHiddenCommand);
            ToggleEscapeCommand = new RelayCommand(ExecuteToggleEscapeCommand);

            ButtonClickCommand = new RelayCommand<int>(ExecuteButtonClickCommand);
        }

        /// <summary>
        /// デフォルトメニューを読み込む
        /// </summary>
        private void LoadDefaultMenu()
        {
            // デフォルトのXMLファイル名を決定
            string defaultFile = "";

            // コマンドライン引数があれば取得
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                defaultFile = args[1];
            }

            // メニュー読み込み
            _currentMenu = _menuService.LoadMenu(defaultFile);

            // UIの更新
            UpdateUI();
        }

        /// <summary>
        /// UIを更新する
        /// </summary>
        private void UpdateUI()
        {
            // 表示モードの更新
            MenuBarVisibility = _currentMenu.MenuVisible ? Visibility.Visible : Visibility.Collapsed;
            StatusBarVisibility = _currentMenu.StatusBarVisible ? Visibility.Visible : Visibility.Collapsed;
            EditMenuVisibility = _currentMenu.LockOn ? Visibility.Collapsed : Visibility.Visible;
            IsEditMode = !_currentMenu.LockOn;
            ModeText = _currentMenu.LockOn ? "" : "編集モード";

            // 背景色の更新
            BackgroundBrush = new SolidColorBrush(
                Microsoft.UI.ColorHelper.FromArgb(
                    255,
                    _currentMenu.BackColor.R,
                    _currentMenu.BackColor.G,
                    _currentMenu.BackColor.B));

            // 背景画像の更新
            if (!string.IsNullOrEmpty(_currentMenu.BackgroundImageFile) && File.Exists(_currentMenu.BackgroundImageFile))
            {
                BackgroundImageSource = new BitmapImage(new Uri(_currentMenu.BackgroundImageFile));
                BackgroundImageStretch = _currentMenu.BackgroundImageTile ? Stretch.UniformToFill : Stretch.None;
                BackgroundImageVisibility = Visibility.Visible;
            }
            else
            {
                BackgroundImageVisibility = Visibility.Collapsed;
            }

            // メニュー列数の更新
            MenuColumns = _currentMenu.Columns;

            // メニューボタンの更新
            UpdateMenuButtons();
        }

        /// <summary>
        /// メニューボタンを更新する
        /// </summary>
        private void UpdateMenuButtons()
        {
            MenuButtons.Clear();

            double buttonWidth = 150;
            double buttonHeight = 50;

            for (int i = 0; i < _currentMenu.MenuItems.Count; i++)
            {
                MenuItem item = _currentMenu.MenuItems[i];

                // ボタンの表示状態
                Visibility visibility = Visibility.Visible;
                if (_currentMenu.LockOn && item.NoUse)
                {
                    visibility = Visibility.Collapsed;
                }

                // タイトルテキスト
                string title = item.Title;
                if (!_currentMenu.LockOn)
                {
                    if (_currentMenu.CancelButtonIndex == i)
                    {
                        title = "＜ＥＳＣ＞" + title;
                    }

                    if (item.NoUse)
                    {
                        title = "＜非表示＞" + title;
                    }
                }

                // ボタンの追加
                MenuButtons.Add(new MenuButtonViewModel
                {
                    Index = i,
                    Title = title,
                    Comment = item.Comment,
                    Width = buttonWidth,
                    Height = buttonHeight,
                    Background = new SolidColorBrush(
                        Microsoft.UI.ColorHelper.FromArgb(
                            255,
                            _currentMenu.ButtonColor.R,
                            _currentMenu.ButtonColor.G,
                            _currentMenu.ButtonColor.B)),
                    Foreground = new SolidColorBrush(
                        Microsoft.UI.ColorHelper.FromArgb(
                            255,
                            _currentMenu.TextColor.R,
                            _currentMenu.TextColor.G,
                            _currentMenu.TextColor.B)),
                    FontFamily = new FontFamily(_currentMenu.FontName),
                    FontSize = _currentMenu.FontSize,
                    FontWeight = _currentMenu.FontBold ? Microsoft.UI.Text.FontWeights.Bold : Microsoft.UI.Text.FontWeights.Normal,
                    FontStyle = _currentMenu.FontItalic ? Microsoft.UI.Text.FontStyle.Italic : Microsoft.UI.Text.FontStyle.Normal,
                    Visibility = visibility
                });
            }
        }

        /// <summary>
        /// ボタンの選択状態が変更された
        /// </summary>
        public void OnSelectionChanged()
        {
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // 現在選択されているアイテムの状態を更新
                StatusText = _currentMenu.MenuItems[SelectedButtonIndex].Comment;
                IsCurrentItemHidden = _currentMenu.MenuItems[SelectedButtonIndex].NoUse;
                IsCurrentItemEscapeButton = (_currentMenu.CancelButtonIndex == SelectedButtonIndex);
            }
        }

        /// <summary>
        /// メニュー項目を取得する
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>メニュー項目</returns>
        public MenuItem GetMenuItem(int index)
        {
            if (index >= 0 && index < _currentMenu.MenuItems.Count)
            {
                return _currentMenu.MenuItems[index];
            }
            return new MenuItem();
        }

        /// <summary>
        /// メニュー項目を更新する
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="menuItem">更新後のメニュー項目</param>
        /// <param name="isEscapeButton">ESCキーとして設定するかどうか</param>
        public void UpdateMenuItem(int index, MenuItem menuItem, bool isEscapeButton)
        {
            if (index >= 0 && index < _currentMenu.MenuItems.Count)
            {
                // メニュー項目を更新
                _currentMenu.MenuItems[index] = menuItem;
                
                // ESCキー設定の更新
                if (isEscapeButton)
                {
                    _currentMenu.CancelButtonIndex = index;
                }
                else if (_currentMenu.CancelButtonIndex == index)
                {
                    _currentMenu.CancelButtonIndex = -1;
                }
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// メニュー設定を更新する
        /// </summary>
        /// <param name="menuPage">更新されたメニューページ</param>
        public void UpdateMenuSettings(MenuPage menuPage)
        {
            // 現在のメニューページを更新
            _currentMenu = menuPage;
            
            // UIを更新
            UpdateUI();
        }

        /// <summary>
        /// 指定されたボタンがESCキーとして設定されているかどうかを確認する
        /// </summary>
        /// <param name="index">ボタンのインデックス</param>
        /// <returns>ESCキーとして設定されている場合はtrue</returns>
        public bool IsEscapeButton(int index)
        {
            return _currentMenu.CancelButtonIndex == index;
        }

        /// <summary>
        /// メニューボタンをクリックした
        /// </summary>
        /// <param name="index">クリックされたボタンのインデックス</param>
        public void OnButtonClick(int index)
        {
            if (index < 0 || index >= _currentMenu.MenuItems.Count)
                return;

            MenuItem menuItem = _currentMenu.MenuItems[index];

            if (_currentMenu.LockOn)
            {
                // 実行モード
                ExecuteMenuItem(menuItem, index);
            }
            else
            {
                // 編集モード
                // 編集ダイアログはMainWindowで表示
                SelectedButtonIndex = index;
                StatusText = menuItem.Comment;
            }
        }

        /// <summary>
        /// メニュー項目を実行する
        /// </summary>
        /// <param name="menuItem">実行するメニュー項目</param>
        /// <param name="index">メニュー項目のインデックス</param>
        private void ExecuteMenuItem(MenuItem menuItem, int index)
        {
            switch (menuItem.Attribute)
            {
                case MenuItem.ItemAttribute.ExecApplication:
                    // アプリケーション実行
                    var result = _launchService.LaunchApplication(menuItem);
                    if (result.Result != ApplicationLaunchService.ExecuteResult.Success)
                    {
                        // エラー処理
                        // ToDo: エラーメッセージを表示
                    }
                    break;

                case MenuItem.ItemAttribute.OpenNextMenu:
                    // 次のメニューを開く
                    if (_currentMenu.IsChanged)
                    {
                        // 保存処理
                        _menuService.SaveMenu(_currentMenu);
                    }

                    // 現在のメニューを履歴に追加
                    _navigationHistory.Push(_currentMenu.FileName, index);

                    // 新しいメニューを読み込む
                    _currentMenu = _menuService.LoadMenu(menuItem.Command);
                    UpdateUI();
                    break;

                case MenuItem.ItemAttribute.BackPrevMenu:
                    // 前のメニューに戻る
                    if (_currentMenu.IsChanged)
                    {
                        // 保存処理
                        _menuService.SaveMenu(_currentMenu);
                    }

                    // 履歴から前のメニューを取得
                    var history = _navigationHistory.Pop();
                    if (history != null)
                    {
                        // 前のメニューを読み込む
                        _currentMenu = _menuService.LoadMenu(history.FileName);
                        UpdateUI();
                        SelectedButtonIndex = history.ButtonIndex;
                    }
                    break;
            }

            // メニュー項目実行後の処理
            switch (menuItem.After)
            {
                case MenuItem.ItemAfter.EndHiMenu:
                    // 終了
                    ExecuteExitCommand();
                    break;

                case MenuItem.ItemAfter.MinimizedHiMenu:
                    // 最小化
                    if (App.Current is App app && app.MainWindow != null)
                    {
                        // ToDo: ウィンドウを最小化
                    }
                    break;
            }
        }

        #endregion

        #region コマンド実行

        /// <summary>
        /// 新規作成コマンドを実行
        /// </summary>
        private void ExecuteNewCommand(string filePath)
        {
            // 現在の変更を保存するか確認
            if (_currentMenu.IsChanged)
            {
                // ToDo: 保存確認ダイアログを表示
                _menuService.SaveMenu(_currentMenu);
            }

            // 新しいメニューページを作成
            _currentMenu = new MenuPage
            {
                FileName = filePath
            };

            // デフォルト設定を適用
            _currentMenu.Title = "新しいメニュー";
            _currentMenu.Rows = 10;
            _currentMenu.Columns = 2;
            _currentMenu.Width = 559;
            _currentMenu.Height = 406;
            _currentMenu.MenuVisible = true;
            _currentMenu.StatusBarVisible = true;
            _currentMenu.DisplayPosition = MenuPage.MenuDispPosition.ScreenCenter;
            _currentMenu.FontName = "ＭＳ Ｐゴシック";
            _currentMenu.FontSize = 12;
            _currentMenu.FontBold = true;
            _currentMenu.FontItalic = false;
            _currentMenu.FontUnderline = false;
            _currentMenu.BackColor = System.Drawing.SystemColors.ControlDark;
            _currentMenu.ButtonColor = System.Drawing.SystemColors.Control;
            _currentMenu.TextColor = System.Drawing.SystemColors.ControlText;
            _currentMenu.HighlightTextColor = System.Drawing.SystemColors.Highlight;
            _currentMenu.BackgroundImageFile = "";
            _currentMenu.BackgroundImageTile = false;
            _currentMenu.LockOn = false;
            _currentMenu.LockPassword = "";
            _currentMenu.CancelButtonIndex = -1;

            // メニュー項目を初期化
            _currentMenu.MenuItems.Clear();
            for (int i = 0; i < _currentMenu.Rows * _currentMenu.Columns; i++)
            {
                _currentMenu.MenuItems.Add(new MenuItem());
            }

            // メニューファイルを保存
            _menuService.SaveMenu(_currentMenu);

            // UIを更新
            UpdateUI();
        }

        /// <summary>
        /// 開くコマンドを実行
        /// </summary>
        private void ExecuteOpenCommand(string filePath)
        {
            // 現在の変更を保存するか確認
            if (_currentMenu.IsChanged)
            {
                // ToDo: 保存確認ダイアログを表示
                _menuService.SaveMenu(_currentMenu);
            }

            // メニューファイルを読み込む
            _currentMenu = _menuService.LoadMenu(filePath);

            // UIを更新
            UpdateUI();
        }

        /// <summary>
        /// 環境設定コマンドを実行
        /// </summary>
        private void ExecuteSettingsCommand()
        {
            // 環境設定ダイアログはMainWindowで表示
        }

        /// <summary>
        /// 終了コマンドを実行
        /// </summary>
        private void ExecuteExitCommand()
        {
            // 現在の変更を保存するか確認
            if (_currentMenu.IsChanged)
            {
                // ToDo: 保存確認ダイアログを表示
                _menuService.SaveMenu(_currentMenu);
            }

            // アプリケーションを終了
            Application.Current.Exit();
        }

        /// <summary>
        /// 切り取りコマンドを実行
        /// </summary>
        private void ExecuteCutCommand()
        {
            // 切り取り処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // クリップボードに項目をコピー
                // ToDo: クリップボード処理
                
                // 選択項目をクリア
                _currentMenu.MenuItems[SelectedButtonIndex] = new MenuItem();
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// コピーコマンドを実行
        /// </summary>
        private void ExecuteCopyCommand()
        {
            // コピー処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // クリップボードに項目をコピー
                // ToDo: クリップボード処理
                
                CanPaste = true;
            }
        }

        /// <summary>
        /// 貼り付けコマンドを実行
        /// </summary>
        private void ExecutePasteCommand()
        {
            // 貼り付け処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count && CanPaste)
            {
                // クリップボードから項目を貼り付け
                // ToDo: クリップボード処理
                
                // 仮実装: テスト用のデータを貼り付け
                _currentMenu.MenuItems[SelectedButtonIndex] = new MenuItem
                {
                    Title = "貼り付けたアイテム",
                    Comment = "クリップボードから貼り付けられたアイテム",
                    Command = "notepad.exe",
                    Attribute = MenuItem.ItemAttribute.ExecApplication,
                    After = MenuItem.ItemAfter.ContinueHiMenu,
                    NoUse = false
                };
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// 削除コマンドを実行
        /// </summary>
        private void ExecuteDeleteCommand()
        {
            // 削除処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // 選択項目をクリア
                _currentMenu.MenuItems[SelectedButtonIndex] = new MenuItem();
                
                // ESCキーの設定をクリア
                if (_currentMenu.CancelButtonIndex == SelectedButtonIndex)
                {
                    _currentMenu.CancelButtonIndex = -1;
                }
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// 非表示トグルコマンドを実行
        /// </summary>
        private void ExecuteToggleHiddenCommand()
        {
            // 非表示トグル処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // 非表示状態を切り替え
                _currentMenu.MenuItems[SelectedButtonIndex].NoUse = !_currentMenu.MenuItems[SelectedButtonIndex].NoUse;
                IsCurrentItemHidden = _currentMenu.MenuItems[SelectedButtonIndex].NoUse;
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// ESCキートグルコマンドを実行
        /// </summary>
        private void ExecuteToggleEscapeCommand()
        {
            // ESCキートグル処理
            if (SelectedButtonIndex >= 0 && SelectedButtonIndex < _currentMenu.MenuItems.Count)
            {
                // ESCキー設定を切り替え
                if (_currentMenu.CancelButtonIndex == SelectedButtonIndex)
                {
                    _currentMenu.CancelButtonIndex = -1;
                    IsCurrentItemEscapeButton = false;
                }
                else
                {
                    _currentMenu.CancelButtonIndex = SelectedButtonIndex;
                    IsCurrentItemEscapeButton = true;
                }
                
                // UIを更新
                UpdateUI();
            }
        }

        /// <summary>
        /// ボタンクリックコマンドを実行
        /// </summary>
        /// <param name="index">クリックされたボタンのインデックス</param>
        private void ExecuteButtonClickCommand(int index)
        {
            OnButtonClick(index);
        }

        #endregion
    }

    /// <summary>
    /// メニューボタンのビューモデル
    /// </summary>
    public class MenuButtonViewModel : ObservableObject
    {
        /// <summary>インデックス</summary>
        [ObservableProperty]
        private int _index;

        /// <summary>タイトル</summary>
        [ObservableProperty]
        private string _title = "";

        /// <summary>コメント</summary>
        [ObservableProperty]
        private string _comment = "";

        /// <summary>幅</summary>
        [ObservableProperty]
        private double _width;

        /// <summary>高さ</summary>
        [ObservableProperty]
        private double _height;

        /// <summary>背景</summary>
        [ObservableProperty]
        private Brush _background;

        /// <summary>前景色</summary>
        [ObservableProperty]
        private Brush _foreground;

        /// <summary>フォントファミリー</summary>
        [ObservableProperty]
        private FontFamily _fontFamily;

        /// <summary>フォントサイズ</summary>
        [ObservableProperty]
        private double _fontSize;

        /// <summary>フォントウェイト</summary>
        [ObservableProperty]
        private Microsoft.UI.Text.FontWeight _fontWeight;

        /// <summary>フォントスタイル</summary>
        [ObservableProperty]
        private Microsoft.UI.Text.FontStyle _fontStyle;

        /// <summary>表示状態</summary>
        [ObservableProperty]
        private Visibility _visibility;
    }
}
