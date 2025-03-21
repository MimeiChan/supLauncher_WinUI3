using System;
using System.IO;

namespace supLauncher.Core.Helpers
{
    /// <summary>
    /// パス操作に関するユーティリティクラス
    /// 元のCFunctionsクラスの機能を継承
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// コマンドラインからパスとコマンドラインオプションを抽出する
        /// </summary>
        public enum GetDriveFolderFileMode
        {
            /// <summary>パス名を取得</summary>
            PathName,
            
            /// <summary>コマンドラインオプションを取得</summary>
            CommandLineOption
        }

        /// <summary>
        /// パスとファイル名を抽出する
        /// </summary>
        public enum GetPathAndFileMode
        {
            /// <summary>パス名を取得</summary>
            PathName,
            
            /// <summary>ファイル名を取得</summary>
            FileName
        }

        /// <summary>
        /// コマンドラインからパス名またはコマンドラインオプションを取得する
        /// </summary>
        /// <param name="commandLine">コマンドライン</param>
        /// <param name="mode">取得モード</param>
        /// <returns>パス名またはコマンドラインオプション</returns>
        public static string GetDriveFolderFile(string commandLine, GetDriveFolderFileMode mode)
        {
            string fileNameSave;
            int position;

            if (commandLine.StartsWith("\"") && commandLine.IndexOf("\"", 1) != -1)
            {
                fileNameSave = commandLine.Substring(1, commandLine.IndexOf("\"", 1) - 1);
                position = commandLine.IndexOf("\"", 1) + 1;
            }
            else if (commandLine.StartsWith("'") && commandLine.IndexOf("'", 1) != -1)
            {
                fileNameSave = commandLine.Substring(1, commandLine.IndexOf("'", 1) - 1);
                position = commandLine.IndexOf("'", 1) + 1;
            }
            else
            {
                if (commandLine.IndexOf(" ") != -1)
                {
                    fileNameSave = commandLine.Substring(0, commandLine.IndexOf(" "));
                    position = commandLine.IndexOf(" ") + 1;
                }
                else
                {
                    fileNameSave = commandLine;
                    position = 0;
                }
            }

            if (fileNameSave.Trim().IndexOf("/") != -1)
            {
                fileNameSave = fileNameSave.Trim().Substring(0, fileNameSave.Trim().IndexOf("/"));
                position = fileNameSave.Trim().IndexOf("/");
            }

            if (mode == GetDriveFolderFileMode.PathName)
            {
                return fileNameSave;
            }
            else
            {
                if (position == 0)
                {
                    return "";
                }
                else
                {
                    return commandLine.Trim().Substring(position);
                }
            }
        }

        /// <summary>
        /// パス名・ファイル名を取り出し
        /// </summary>
        /// <param name="allFileName">完全なファイルパス</param>
        /// <param name="mode">取得モード</param>
        /// <returns>パス名またはファイル名</returns>
        public static string GetPathAndFile(string allFileName, GetPathAndFileMode mode)
        {
            int loopCnt;
            string pathName;
            string fileName;

            pathName = "";
            fileName = allFileName.Trim();
            for (loopCnt = fileName.Length - 1; loopCnt >= 1; loopCnt--)
            {
                if (fileName[loopCnt] == '\\')
                {
                    pathName = fileName.Substring(0, loopCnt);
                    fileName = fileName.Substring(loopCnt + 1);
                    break;
                }
                if (fileName[loopCnt] == ':')
                {
                    pathName = fileName.Substring(0, loopCnt + 1);
                    fileName = fileName.Substring(loopCnt + 1);
                    break;
                }
            }

            if (mode == GetPathAndFileMode.PathName)
            {
                return pathName;
            }
            else
            {
                return fileName;
            }
        }

        /// <summary>
        /// パス名に空白が含まれる場合にダブルコーテーションで囲む
        /// </summary>
        /// <param name="targetPath">対象パス</param>
        /// <returns>必要に応じてクォートされたパス</returns>
        public static string QuoteFullPath(string targetPath)
        {
            if (targetPath.IndexOf(" ") != -1)
            {
                return "\"" + targetPath + "\"";
            }
            else
            {
                return targetPath;
            }
        }
    }
}
