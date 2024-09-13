/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.InteropServices;
using System.Windows.Forms;
using Npp.DotNet.Plugin;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.SqlVarBinaryGzip.Plugin;

/// <summary>
///     Extends <see cref="DotNetPlugin" />.
/// </summary>
internal partial class Main : DotNetPlugin
{
    private static readonly Main Instance;
    private static readonly PluginOptions Config;
    public static readonly string PluginName = "SqlBinaryGZip Plugin\0";

    static Main()
    {
        Instance = new Main();
        PluginData.PluginNamePtr = Marshal.StringToHGlobalUni(PluginName);
        Config = new PluginOptions();
    }

    /// <summary><see cref="Npp.SqlVarBinaryGzip.Plugin.Main" /> should be a singleton class</summary>
    private Main()
    {
    }

    /// <inheritdoc cref="IDotNetPlugin.OnSetInfo" />
    public override void OnSetInfo()
    {
        var decompressKey = new ShortcutKey(TRUE, TRUE, TRUE, 122); // Ctrl + Alt + Shift + F11
        var compressKey = new ShortcutKey(TRUE, TRUE, TRUE, 123); // Ctrl + Alt + Shift  + F12
        Utils.SetCommand("Decompress", Decompress, decompressKey);
        Utils.SetCommand("Compress", Compress, compressKey);
        Utils.SetCommand("Plugin &settings", OpenConfigFile);
        Utils.MakeSeparator();
        Utils.SetCommand("&About", DisplayInfo);
    }

    /// <inheritdoc cref="IDotNetPlugin.OnBeNotified" />
    public override void OnBeNotified(ScNotification notification)
    {
        if (notification.Header.HwndFrom == PluginData.NppData.NppHandle)
        {
            var code = notification.Header.Code;
            switch ((NppMsg)code)
            {
                case NppMsg.NPPN_READY:
                    Config?.Load();
                    break;
                case NppMsg.NPPN_FILESAVED:
                    if (string.Compare(Config.FilePath, NppUtils.GetCurrentPath(),
                            StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        Config?.Load();
                    }
                    break;
                case NppMsg.NPPN_SHUTDOWN:
                    Config?.Save();
                    // clean up resources
                    PluginData.PluginNamePtr = IntPtr.Zero;
                    break;
            }
        }
    }

    /// <summary>
    ///     Decompresses the selected text.
    /// </summary>
    private static void Decompress()
    {
        try
        {
            var processor = new SqlBinaryGZipDataProcessor(Config.MaxDataSize);
            var selectedText = NppUtils.Editor.GetSelText();
            var decompressed = processor.Decompress(selectedText);
            NppUtils.Editor.ReplaceSel(decompressed);
        }
        catch (Exception ex)
        {
            MessageBox.Show(@"Operation failed: " + ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    ///     Compresses the selected text.
    /// </summary>
    private static void Compress()
    {
        try
        {
            var processor = new SqlBinaryGZipDataProcessor(Config.MaxDataSize);
            var selectedText = NppUtils.Editor.GetSelText();
            var compressed = processor.Compress(selectedText);
            NppUtils.Editor.ReplaceSel(compressed);
        }
        catch (Exception ex)
        {
            MessageBox.Show(@"Operation failed: " + ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    ///     Shows the plugin's version number in a system dialog.
    /// </summary>
    private static void DisplayInfo()
    {
        _ =
            MsgBoxDialog(
                PluginData.NppData.NppHandle,
                $"Current version: {NppUtils.AssemblyVersionString}\0",
                $"About {PluginName}",
                (uint)(MsgBox.ICONINFORMATION | MsgBox.OK)
            );
    }

    /// <summary>
    ///     Open the plugin's INI file in Notepad++.
    /// </summary>
    private static void OpenConfigFile()
    {
        Config?.OpenFile();
    }
}