/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.ComponentModel;
using Npp.DotNet.Plugin;

namespace Npp.SqlVarBinaryGzip.Plugin
{
    internal class PluginOptions : DefaultSettings
    {
        [Description("Maximum data size"), Category("Integer")]
        public int MaxDataSize { get; set; } = 512 * 512;

        public string FilePath
        {
            get
            {
                var folderName = SqlVarBinaryGzip.Plugin.Main.PluginName.Trim(new char[] { '\0', ' ' });
                var configDir = new DirectoryInfo(Path.Combine(NppUtils.ConfigDirectory, folderName));
                if (!configDir.Exists)
                    configDir = Directory.CreateDirectory(Path.Combine(NppUtils.ConfigDirectory, folderName));
                return Path.Combine(configDir.FullName, "settings.SqlVarBinaryGzip.ini");
            }
        }

        public void Load() => base.Load(FilePath);
        public void Save() => base.Save(FilePath);
        public override void OpenFile() => NppUtils.Notepad.OpenFile(FilePath);
    }
}
