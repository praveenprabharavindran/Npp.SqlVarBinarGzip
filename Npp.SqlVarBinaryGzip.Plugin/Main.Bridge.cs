/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Npp.DotNet.Plugin;

namespace Npp.SqlVarBinaryGzip.Plugin;

internal partial class Main
{
    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.GetFuncsArray" />
    [UnmanagedCallersOnly(EntryPoint = "getFuncsArray", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static IntPtr GetFuncsArray(IntPtr nbF)
    {
        return OnGetFuncsArray(nbF);
    }

    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.GetName" />
    [UnmanagedCallersOnly(EntryPoint = "getName", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static IntPtr GetName()
    {
        return OnGetName();
    }

    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.IsUnicode" />
    [UnmanagedCallersOnly(EntryPoint = "isUnicode", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static NativeBool IsUnicode()
    {
        return OnIsUnicode();
    }

    #region "4. Expose interface methods to unmanaged callers (like notepad++.exe)"

    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnSetInfo" />
    [UnmanagedCallersOnly(EntryPoint = "setInfo", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static unsafe void SetInfo(NppData* notepadPlusData)
    {
        PluginData.NppData = *notepadPlusData;
        Instance.OnSetInfo();
    }

    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnBeNotified" />
    [UnmanagedCallersOnly(EntryPoint = "beNotified", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static unsafe void BeNotified(ScNotification* notification)
    {
        Instance.OnBeNotified(*notification);
    }

    /// <inheritdoc cref="Npp.DotNet.Plugin.IDotNetPlugin.OnMessageProc" />
    [UnmanagedCallersOnly(EntryPoint = "messageProc", CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static NativeBool MessageProc(uint msg, UIntPtr wParam, IntPtr lParam)
    {
        return Instance.OnMessageProc(msg, wParam, lParam);
    }

    #endregion
}