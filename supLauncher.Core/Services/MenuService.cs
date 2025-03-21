using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using supLauncher.Core.Models;

namespace supLauncher.Core.Services
{
    /// <summary>
    /// メニュー関連のサービスを提供するクラス
    /// 元のCMenuPageクラスのファイル操作部分を分離
    /// </summary>
    public class MenuService
    {
        private readonly MenuNavigationHistory _navigationHistory;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="navigationHistory">ナビゲーション履歴</param>
        public MenuService(MenuNavigationHistory navigationHistory)
        {
            _navigationHistory = navigationHistory;
        }

        /// <summary>
        /// メニューファイルを読み込む
        /// </summary>
        /// <param name="fileName">メニューファイルパス</param>
        /// <returns>読み込んだメニューページ</returns>
        public MenuPage LoadMenu(string fileName)
        {
            // 新しいメニューページを作成
            var menuPage = CreateDefaultMenuPage();
            menuPage.FileName = fileName;

            // ファイルが存在しない場合はデフォルト設定を返す
            if (!File.Exists(menuPage.FileName))
            {
                return menuPage;
            }

            try
            {
                // XMLファイルを読み込む
                XDocument doc = XDocument.Load(menuPage.FileName);
                XElement root = doc.Root;

                // EnvironmentTitleセクションの読み込み
                XElement envTitle = root.Element("EnvironmentTitle");
                if (envTitle != null)
                {
                    // メニュータイトル
                    XElement title = envTitle.Element("Title");
                    if (title != null)
                    {
                        menuPage.Title = title.Value;
                    }

                    // メニュー項目
                    XElement items = envTitle.Element("Items");
                    if (items != null)
                    {
                        foreach (XElement item in items.Elements("Item"))
                        {
                            int index;
                            if (item.Attribute("index") != null && int.TryParse(item.Attribute("index").Value, out index) && index > 0)
                            {
                                index--; // XMLでは1から始まるインデックスを使用

                                // 必要に応じてメニュー項目数を拡張
                                EnsureMenuItemCount(menuPage, index + 1);

                                MenuItem menuItem = new MenuItem();

                                XElement itemTitle = item.Element("Title");
                                if (itemTitle != null)
                                {
                                    menuItem.Title = itemTitle.Value;
                                }

                                XElement itemComment = item.Element("Comment");
                                if (itemComment != null)
                                {
                                    menuItem.Comment = itemComment.Value;
                                }

                                XElement itemCommand = item.Element("Command");
                                if (itemCommand != null)
                                {
                                    menuItem.Command = itemCommand.Value;
                                }

                                XElement itemFlag = item.Element("Flag");
                                if (itemFlag != null)
                                {
                                    menuItem.Flag = itemFlag.Value;
                                }

                                menuPage.MenuItems[index] = menuItem;
                            }
                        }
                    }
                }

                // ExecuteEnvironmentセクションの読み込み
                XElement execEnv = root.Element("ExecuteEnvironment");
                if (execEnv != null)
                {
                    XElement rows = execEnv.Element("Rows");
                    if (rows != null) menuPage.Rows = Convert.ToInt32(rows.Value);

                    XElement cols = execEnv.Element("Cols");
                    if (cols != null) menuPage.Columns = Convert.ToInt32(cols.Value);

                    XElement width = execEnv.Element("Width");
                    if (width != null) menuPage.Width = Convert.ToInt32(width.Value);

                    XElement height = execEnv.Element("Height");
                    if (height != null) menuPage.Height = Convert.ToInt32(height.Value);

                    XElement fontName = execEnv.Element("FontName");
                    if (fontName != null) menuPage.FontName = fontName.Value;

                    XElement fontSize = execEnv.Element("FontSize");
                    if (fontSize != null) menuPage.FontSize = float.Parse(fontSize.Value);

                    XElement fontBold = execEnv.Element("FontBold");
                    if (fontBold != null) menuPage.FontBold = Convert.ToBoolean(fontBold.Value);

                    XElement fontItalic = execEnv.Element("FontItalic");
                    if (fontItalic != null) menuPage.FontItalic = Convert.ToBoolean(fontItalic.Value);

                    XElement fontUnderline = execEnv.Element("FontUnderline");
                    if (fontUnderline != null) menuPage.FontUnderline = Convert.ToBoolean(fontUnderline.Value);

                    XElement dispPosition = execEnv.Element("DispPosition");
                    if (dispPosition != null) menuPage.DisplayPosition = (MenuPage.MenuDispPosition)(Convert.ToInt32(dispPosition.Value) - 1);

                    XElement bgFile = execEnv.Element("BGFile");
                    if (bgFile != null) menuPage.BackgroundImageFile = bgFile.Value;

                    XElement bgTile = execEnv.Element("BGTile");
                    if (bgTile != null) menuPage.BackgroundImageTile = Convert.ToBoolean(bgTile.Value);

                    XElement menuVisible = execEnv.Element("MenuVisible");
                    if (menuVisible != null) menuPage.MenuVisible = Convert.ToBoolean(menuVisible.Value);

                    XElement statusBarVisible = execEnv.Element("StatusBarVisible");
                    if (statusBarVisible != null) menuPage.StatusBarVisible = Convert.ToBoolean(statusBarVisible.Value);

                    XElement backColor = execEnv.Element("BackColor");
                    if (backColor != null) menuPage.BackColor = Color.FromArgb(Convert.ToInt32(backColor.Value));

                    XElement buttonColor = execEnv.Element("ButtonColor");
                    if (buttonColor != null) menuPage.ButtonColor = Color.FromArgb(Convert.ToInt32(buttonColor.Value));

                    XElement textColor = execEnv.Element("TextColor");
                    if (textColor != null) menuPage.TextColor = Color.FromArgb(Convert.ToInt32(textColor.Value));

                    XElement highLightTextColor = execEnv.Element("HighLightTextColor");
                    if (highLightTextColor != null) menuPage.HighlightTextColor = Color.FromArgb(Convert.ToInt32(highLightTextColor.Value));

                    XElement cancelButton = execEnv.Element("CancelButton");
                    if (cancelButton != null) menuPage.CancelButtonIndex = Convert.ToInt32(cancelButton.Value);

                    XElement lockPassword = execEnv.Element("LockPassword");
                    if (lockPassword != null) menuPage.LockPassword = lockPassword.Value;
                }

                // Currentセクションの読み込み
                XElement current = root.Element("Current");
                if (current != null)
                {
                    XElement currentX = current.Element("CurrentX");
                    if (currentX != null) menuPage.CurrentX = Convert.ToInt32(currentX.Value);

                    XElement currentY = current.Element("CurrentY");
                    if (currentY != null) menuPage.CurrentY = Convert.ToInt32(currentY.Value);
                }

                // メニュー項目数の調整
                EnsureMenuItemCount(menuPage, menuPage.Rows * menuPage.Columns);

                menuPage.LockOn = !string.IsNullOrEmpty(menuPage.LockPassword);
                menuPage.IsChanged = false;

                return menuPage;
            }
            catch (Exception)
            {
                // 読み込みに失敗した場合はデフォルト設定を返す
                return CreateDefaultMenuPage();
            }
        }

        /// <summary>
        /// メニューをファイルに保存する
        /// </summary>
        /// <param name="menuPage">保存するメニューページ</param>
        /// <returns>保存に成功した場合はtrue</returns>
        public bool SaveMenu(MenuPage menuPage)
        {
            try
            {
                // バージョン情報を含めたHiMenu要素の作成
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("HiMenu",
                        new XAttribute("version", "3.0.0.0")
                    )
                );

                // EnvironmentTitleセクションの作成
                XElement envTitle = new XElement("EnvironmentTitle",
                    new XElement("Title", menuPage.Title),
                    new XElement("Items")
                );
                doc.Root.Add(envTitle);

                // メニュー項目の追加
                XElement items = envTitle.Element("Items");
                for (int intIndex = 0; intIndex < menuPage.MenuItems.Count; intIndex++)
                {
                    MenuItem item = menuPage.MenuItems[intIndex];
                    if (!string.IsNullOrEmpty(item.Title) || !string.IsNullOrEmpty(item.Comment) || 
                        !string.IsNullOrEmpty(item.Command) || item.Flag != "---")
                    {
                        XElement menuItem = new XElement("Item",
                            new XAttribute("index", (intIndex + 1).ToString()),
                            new XElement("Title", item.Title)
                        );

                        if (!string.IsNullOrEmpty(item.Comment))
                        {
                            menuItem.Add(new XElement("Comment", item.Comment));
                        }

                        if (!string.IsNullOrEmpty(item.Command))
                        {
                            menuItem.Add(new XElement("Command", item.Command));
                        }

                        if (item.Flag != "---")
                        {
                            menuItem.Add(new XElement("Flag", item.Flag));
                        }

                        items.Add(menuItem);
                    }
                }

                // ExecuteEnvironmentセクションの作成
                XElement execEnv = new XElement("ExecuteEnvironment",
                    new XElement("Rows", menuPage.Rows),
                    new XElement("Cols", menuPage.Columns),
                    new XElement("Width", menuPage.Width),
                    new XElement("Height", menuPage.Height),
                    new XElement("FontName", menuPage.FontName),
                    new XElement("FontSize", menuPage.FontSize),
                    new XElement("FontBold", menuPage.FontBold),
                    new XElement("FontItalic", menuPage.FontItalic),
                    new XElement("FontUnderline", menuPage.FontUnderline),
                    new XElement("DispPosition", ((int)menuPage.DisplayPosition) + 1),
                    new XElement("BGFile", menuPage.BackgroundImageFile),
                    new XElement("BGTile", menuPage.BackgroundImageTile),
                    new XElement("MenuVisible", menuPage.MenuVisible),
                    new XElement("StatusBarVisible", menuPage.StatusBarVisible),
                    new XElement("BackColor", ColorTranslator.ToOle(menuPage.BackColor)),
                    new XElement("ButtonColor", ColorTranslator.ToOle(menuPage.ButtonColor)),
                    new XElement("TextColor", ColorTranslator.ToOle(menuPage.TextColor)),
                    new XElement("HighLightTextColor", ColorTranslator.ToOle(menuPage.HighlightTextColor))
                );

                if (menuPage.CancelButtonIndex >= 0 && menuPage.CancelButtonIndex < menuPage.Rows * menuPage.Columns)
                {
                    execEnv.Add(new XElement("CancelButton", menuPage.CancelButtonIndex));
                }

                if (menuPage.LockOn)
                {
                    execEnv.Add(new XElement("LockPassword", menuPage.LockPassword));
                }

                doc.Root.Add(execEnv);

                // Currentセクションの作成
                XElement current = new XElement("Current",
                    new XElement("CurrentX", menuPage.CurrentX),
                    new XElement("CurrentY", menuPage.CurrentY)
                );
                doc.Root.Add(current);

                // XMLファイルへの保存
                doc.Save(menuPage.FileName);
                menuPage.IsChanged = false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// メニューの位置情報のみを更新する
        /// </summary>
        /// <param name="menuPage">メニューページ</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>更新に成功した場合はtrue</returns>
        public bool UpdateMenuPosition(MenuPage menuPage, int x, int y)
        {
            menuPage.CurrentX = x;
            menuPage.CurrentY = y;

            try
            {
                if (!string.IsNullOrEmpty(menuPage.FileName) && File.Exists(menuPage.FileName))
                {
                    XDocument doc = XDocument.Load(menuPage.FileName);

                    // Currentセクションの取得または作成
                    XElement current = doc.Root.Element("Current");
                    if (current == null)
                    {
                        current = new XElement("Current");
                        doc.Root.Add(current);
                    }

                    // CurrentX要素の取得または作成
                    XElement currentX = current.Element("CurrentX");
                    if (currentX == null)
                    {
                        currentX = new XElement("CurrentX");
                        current.Add(currentX);
                    }
                    currentX.Value = x.ToString();

                    // CurrentY要素の取得または作成
                    XElement currentY = current.Element("CurrentY");
                    if (currentY == null)
                    {
                        currentY = new XElement("CurrentY");
                        current.Add(currentY);
                    }
                    currentY.Value = y.ToString();

                    // ファイルに保存
                    doc.Save(menuPage.FileName);
                    return true;
                }
            }
            catch (Exception)
            {
                // 失敗してもエラーとして扱わない
            }

            return false;
        }

        /// <summary>
        /// デフォルトのメニューページを作成する
        /// </summary>
        /// <returns>デフォルトのメニューページ</returns>
        private MenuPage CreateDefaultMenuPage()
        {
            MenuPage menuPage = new MenuPage
            {
                Rows = 10,
                Columns = 2,
                Width = 559,
                Height = 406,
                Title = "",
                CurrentX = 0,
                CurrentY = 0,
                CurrentButtonIndex = 0,
                FontName = "ＭＳ Ｐゴシック",
                FontSize = 12,
                FontBold = true,
                FontItalic = false,
                FontUnderline = false,
                DisplayPosition = MenuPage.MenuDispPosition.RootMenu,
                BackgroundImageFile = "",
                BackgroundImageTile = false,
                MenuVisible = true,
                StatusBarVisible = true,
                LockOn = false,
                LockPassword = "",
                BackColor = SystemColors.ControlDark,
                ButtonColor = SystemColors.Control,
                TextColor = SystemColors.ControlText,
                HighlightTextColor = SystemColors.Highlight,
                CancelButtonIndex = -1,
                MenuItems = new List<MenuItem>()
            };

            // 初期メニュー項目の作成
            for (int i = 0; i < menuPage.Rows * menuPage.Columns; i++)
            {
                menuPage.MenuItems.Add(new MenuItem());
            }

            return menuPage;
        }

        /// <summary>
        /// メニュー項目のコレクションが指定サイズ以上になるよう調整する
        /// </summary>
        /// <param name="menuPage">メニューページ</param>
        /// <param name="targetCount">目標とする項目数</param>
        private void EnsureMenuItemCount(MenuPage menuPage, int targetCount)
        {
            int currentCount = menuPage.MenuItems.Count;

            if (targetCount > currentCount)
            {
                // 項目が足りない場合は追加
                for (int i = currentCount; i < targetCount; i++)
                {
                    menuPage.MenuItems.Add(new MenuItem());
                }
            }
        }
    }
}
