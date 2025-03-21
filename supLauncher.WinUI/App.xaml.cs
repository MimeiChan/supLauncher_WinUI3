using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using supLauncher.Core.Models;
using supLauncher.Core.Services;
using supLauncher.WinUI.ViewModels;
using System;

namespace supLauncher.WinUI
{
    /// <summary>
    /// アプリケーションのエントリポイント
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        public ServiceProvider ServiceProvider { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // サービスコレクションを設定
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// アプリケーション起動時に呼ばれるメソッド
        /// </summary>
        /// <param name="args">起動引数</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        /// <summary>
        /// DIサービスの設定
        /// </summary>
        /// <param name="services">サービスコレクション</param>
        private void ConfigureServices(ServiceCollection services)
        {
            // モデル登録
            services.AddSingleton<MenuNavigationHistory>();

            // サービス登録
            services.AddSingleton<MenuService>();
            services.AddSingleton<ApplicationLaunchService>();

            // ViewModel登録
            services.AddTransient<MainWindowViewModel>();
        }

        /// <summary>
        /// メインウィンドウの参照
        /// </summary>
        private Window m_window;

        /// <summary>
        /// 現在のアプリケーションインスタンスを取得する
        /// </summary>
        public static new App Current => (App)Application.Current;
    }
}
