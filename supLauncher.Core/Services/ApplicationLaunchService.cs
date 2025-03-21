using System;
using System.Diagnostics;
using System.IO;
using supLauncher.Core.Helpers;
using supLauncher.Core.Models;

namespace supLauncher.Core.Services
{
    /// <summary>
    /// アプリケーション起動サービス
    /// </summary>
    public class ApplicationLaunchService
    {
        /// <summary>
        /// アプリケーション実行結果を表す列挙型
        /// </summary>
        public enum ExecuteResult
        {
            /// <summary>成功</summary>
            Success,
            
            /// <summary>パス変更エラー</summary>
            PathChangeError,
            
            /// <summary>起動エラー</summary>
            LaunchError
        }

        /// <summary>
        /// 起動結果情報
        /// </summary>
        public class LaunchResultInfo
        {
            /// <summary>実行結果</summary>
            public ExecuteResult Result { get; set; } = ExecuteResult.Success;
            
            /// <summary>エラーメッセージ</summary>
            public string ErrorMessage { get; set; } = "";
            
            /// <summary>実行後の処理</summary>
            public MenuItem.ItemAfter AfterAction { get; set; } = MenuItem.ItemAfter.ContinueHiMenu;
        }

        /// <summary>
        /// メニュー項目に基づいてアプリケーションを起動する
        /// </summary>
        /// <param name="menuItem">実行するメニュー項目</param>
        /// <returns>実行結果</returns>
        public LaunchResultInfo LaunchApplication(MenuItem menuItem)
        {
            LaunchResultInfo result = new LaunchResultInfo
            {
                AfterAction = menuItem.After
            };

            if (string.IsNullOrEmpty(menuItem.Command))
            {
                return result;
            }

            string currentPath = Directory.GetCurrentDirectory();
            string commandLine = menuItem.Command;

            try
            {
                // ドライブ名＋フォルダ名＋ファイル名を取り出し
                string fileNamePath = PathUtility.GetDriveFolderFile(commandLine, PathUtility.GetDriveFolderFileMode.PathName);

                // パス名・ファイル名を取り出し
                string pathName = PathUtility.GetPathAndFile(fileNamePath, PathUtility.GetPathAndFileMode.PathName);
                string fileName = PathUtility.GetPathAndFile(fileNamePath, PathUtility.GetPathAndFileMode.FileName);

                // オプション取り出し
                string options = PathUtility.GetDriveFolderFile(commandLine, PathUtility.GetDriveFolderFileMode.CommandLineOption);

                if (!string.IsNullOrEmpty(pathName) && string.Compare(pathName, currentPath, true) != 0)
                {
                    try
                    {
                        // 実行フォルダを変更
                        Directory.SetCurrentDirectory(pathName);
                    }
                    catch
                    {
                        // カレントフォルダの変更に失敗
                        result.Result = ExecuteResult.PathChangeError;
                        result.ErrorMessage = pathName;
                        return result;
                    }
                }

                try
                {
                    // プロセスで起動
                    using (var process = new Process())
                    {
                        process.StartInfo.Arguments = options.TrimEnd(); // 引数
                        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName); // 作業ディレクトリ
                        process.StartInfo.FileName = fileName; // 実行するファイル
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                }
                catch
                {
                    // アプリケーションの起動に失敗
                    if (!string.IsNullOrEmpty(currentPath))
                    {
                        Directory.SetCurrentDirectory(currentPath); // カレントパスを元にもどす
                    }

                    result.Result = ExecuteResult.LaunchError;
                    result.ErrorMessage = fileName;
                    return result;
                }

                // 成功
                if (!string.IsNullOrEmpty(currentPath))
                {
                    Directory.SetCurrentDirectory(currentPath); // カレントパスを元にもどす
                }
            }
            catch (Exception ex)
            {
                // 予期しないエラー
                result.Result = ExecuteResult.LaunchError;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
