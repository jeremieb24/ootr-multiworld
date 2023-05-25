using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Common;

namespace MidosHouse.OotrMultiworld;

internal class Native {
    [DllImport("multiworld")] internal static extern void log(OwnedStringHandle msg);
    [DllImport("multiworld")] internal static extern void error_free(IntPtr error);
    [DllImport("multiworld")] internal static extern Error error_from_string(OwnedStringHandle text);
    [DllImport("multiworld")] internal static extern StringHandle error_debug(Error error);
    [DllImport("multiworld")] internal static extern StringHandle error_display(Error error);
    [DllImport("multiworld")] internal static extern ClientResult open_gui(OwnedStringHandle version);
    [DllImport("multiworld")] internal static extern void client_result_free(IntPtr client_res);
    [DllImport("multiworld")] internal static extern bool client_result_is_ok(ClientResult client_res);
    [DllImport("multiworld")] internal static extern Client client_result_unwrap(IntPtr client_res);
    [DllImport("multiworld")] internal static extern Error client_result_unwrap_err(IntPtr client_res);
    [DllImport("multiworld")] internal static extern void client_free(IntPtr client);
    [DllImport("multiworld")] internal static extern void string_free(IntPtr s);
    [DllImport("multiworld")] internal static extern UnitResult client_set_player_id(Client client, byte id);
    [DllImport("multiworld")] internal static extern void unit_result_free(IntPtr unit_res);
    [DllImport("multiworld")] internal static extern bool unit_result_is_ok(UnitResult unit_res);
    [DllImport("multiworld")] internal static extern Error unit_result_unwrap_err(IntPtr unit_res);
    [DllImport("multiworld")] internal static extern UnitResult client_reset_player_id(Client client);
    [DllImport("multiworld")] internal static extern UnitResult client_set_player_name(Client client, IntPtr name);
    [DllImport("multiworld")] internal static extern UnitResult client_set_file_hash(Client client, IntPtr hash);
    [DllImport("multiworld")] internal static extern UnitResult client_set_save_data(Client client, IntPtr save);
    [DllImport("multiworld")] internal static extern OptMessageResult client_try_recv_message(Client client);
    [DllImport("multiworld")] internal static extern void opt_message_result_free(IntPtr opt_msg_res);
    [DllImport("multiworld")] internal static extern sbyte opt_message_result_kind(OptMessageResult opt_msg_res);
    [DllImport("multiworld")] internal static extern ushort opt_message_result_item_queue_len(OptMessageResult opt_msg_res);
    [DllImport("multiworld")] internal static extern ushort opt_message_result_item_kind_at_index(OptMessageResult opt_msg_res, ushort index);
    [DllImport("multiworld")] internal static extern byte opt_message_result_world_id(OptMessageResult opt_msg_res);
    [DllImport("multiworld")] internal static extern uint opt_message_result_progressive_items(OptMessageResult opt_msg_res);
    [DllImport("multiworld")] internal static extern IntPtr opt_message_result_filename(OptMessageResult opt_msg_res);
    [DllImport("multiworld")] internal static extern Error opt_message_result_unwrap_err(IntPtr opt_msg_res);
    [DllImport("multiworld")] internal static extern UnitResult client_send_item(Client client, uint key, ushort kind, byte target_world);
}

internal class StringHandle : SafeHandle {
    internal StringHandle() : base(IntPtr.Zero, true) {}

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    public string AsString() {
        int len = 0;
        while (Marshal.ReadByte(this.handle, len) != 0) { len += 1; }
        byte[] buffer = new byte[len];
        Marshal.Copy(this.handle, buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.string_free(this.handle);
        }
        return true;
    }
}

internal class ClientResult : SafeHandle {
    internal ClientResult() : base(IntPtr.Zero, true) {}

    static internal ClientResult open() {
        using (var versionHandle = new OwnedStringHandle(VersionInfo.MainVersion)) {
            return Native.open_gui(versionHandle);
        }
    }

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.client_result_free(this.handle);
        }
        return true;
    }

    internal bool IsOk() => Native.client_result_is_ok(this);

    internal Client Unwrap() {
        var client = Native.client_result_unwrap(this.handle);
        this.handle = IntPtr.Zero; // client_result_unwrap takes ownership
        return client;
    }

    internal Error UnwrapErr() {
        var err = Native.client_result_unwrap_err(this.handle);
        this.handle = IntPtr.Zero; // client_result_unwrap_err takes ownership
        return err;
    }
}

internal class Client : SafeHandle {
    internal Client() : base(IntPtr.Zero, true) {}

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.client_free(this.handle);
        }
        return true;
    }

    internal void SetPlayerID(byte? id) {
        if (id == null) {
            Native.client_reset_player_id(this);
        } else {
            Native.client_set_player_id(this, id.Value);
        }
    }

    internal void SetPlayerName(IReadOnlyList<byte> name) {
        var namePtr = Marshal.AllocHGlobal(8);
        Marshal.Copy(name.ToArray(), 0, namePtr, 8);
        Native.client_set_player_name(this, namePtr);
        Marshal.FreeHGlobal(namePtr);
    }

    internal void SendSaveData(IReadOnlyList<byte> saveData) {
        var savePtr = Marshal.AllocHGlobal(0x1450);
        Marshal.Copy(saveData.ToArray(), 0, savePtr, 0x1450);
        Native.client_set_save_data(this, savePtr);
        Marshal.FreeHGlobal(savePtr);
    }

    internal void SendFileHash(IReadOnlyList<byte> fileHash) {
        var hashPtr = Marshal.AllocHGlobal(5);
        Marshal.Copy(fileHash.ToArray(), 0, hashPtr, 5);
        Native.client_set_file_hash(this, hashPtr);
        Marshal.FreeHGlobal(hashPtr);
    }

    internal OptMessageResult TryRecv() => Native.client_try_recv_message(this);
    internal void SendItem(uint key, ushort kind, byte targetWorld) => Native.client_send_item(this, key, kind, targetWorld);
}

internal class Error : SafeHandle {
    internal Error() : base(IntPtr.Zero, true) {}

    static internal Error from_string(string text) {
        using (var textHandle = new OwnedStringHandle(text)) {
            return Native.error_from_string(textHandle);
        }
    }

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.error_free(this.handle);
        }
        return true;
    }

    internal StringHandle Debug() => Native.error_debug(this);
    internal StringHandle Display() => Native.error_display(this);
}

internal class OptMessageResult : SafeHandle {
    internal OptMessageResult() : base(IntPtr.Zero, true) {}

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.opt_message_result_free(this.handle);
        }
        return true;
    }

    internal sbyte Kind() => Native.opt_message_result_kind(this);
    internal ushort ItemQueueLen() => Native.opt_message_result_item_queue_len(this);
    internal ushort ItemAtIndex(ushort index) => Native.opt_message_result_item_kind_at_index(this, index);
    internal byte WorldId() => Native.opt_message_result_world_id(this);
    internal uint ProgressiveItems() => Native.opt_message_result_progressive_items(this);

    internal List<byte> Filename() {
        var name = new byte[8];
        Marshal.Copy(Native.opt_message_result_filename(this), name, 0, 8);
        return name.ToList();
    }

    internal Error UnwrapErr() {
        var err = Native.opt_message_result_unwrap_err(this.handle);
        this.handle = IntPtr.Zero; // opt_msg_result_unwrap_err takes ownership
        return err;
    }
}

internal class UnitResult : SafeHandle {
    internal UnitResult() : base(IntPtr.Zero, true) {}

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Native.unit_result_free(this.handle);
        }
        return true;
    }

    internal bool IsOk() => Native.unit_result_is_ok(this);

    internal Error UnwrapErr() {
        var err = Native.unit_result_unwrap_err(this.handle);
        this.handle = IntPtr.Zero; // unit_result_unwrap_err takes ownership
        return err;
    }
}

internal class OwnedStringHandle : SafeHandle {
    internal OwnedStringHandle(string value) : base(IntPtr.Zero, true) {
        var bytes = Encoding.UTF8.GetBytes(value);
        this.handle = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, this.handle, bytes.Length);
        Marshal.WriteByte(handle, bytes.Length, 0);
    }

    public override bool IsInvalid {
        get { return this.handle == IntPtr.Zero; }
    }

    protected override bool ReleaseHandle() {
        if (!this.IsInvalid) {
            Marshal.FreeHGlobal(this.handle);
        }
        return true;
    }
}

[ExternalTool("Mido's House Multiworld", Description = "Play interconnected Ocarina of Time Randomizer seeds")]
[ExternalToolEmbeddedIcon("MidosHouse.OotrMultiworld.Resources.icon.ico")]
public sealed class MainForm : ToolFormBase, IExternalToolForm {
    private Client? client;
    private uint? coopContextAddr;
    private List<byte> fileHash = new List<byte> { 0xff, 0xff, 0xff, 0xff, 0xff };
    private byte? playerID;
    private List<byte> playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
    private List<List<byte>> playerNames = new List<List<byte>> {
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x05 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x01 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x07 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x03 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x09 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x05 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x01 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x07 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x03 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x09 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x05 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x01 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x07 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x03 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x09 },
        new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x05 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x06 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x07 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x08 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x09 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x00 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x01 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x02 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x03 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x04 }, new List<byte> { 0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x05 },
    };
    private bool progressiveItemsEnable = false;
    private List<uint> progressiveItems = new List<uint>(Enumerable.Repeat(0u, 256));
    private List<ushort> itemQueue = new List<ushort>();
    private bool normalGameplay = false;

    public ApiContainer? _apiContainer { get; set; }
    private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

    public override bool BlocksInputWhenFocused { get; } = false;
    protected override string WindowTitleStatic => "Mido's House Multiworld for BizHawk";

    public override bool AskSaveChanges() => true; //TODO warn before leaving an active game?

    public MainForm() {
        this.ShowInTaskbar = false;
        this.WindowState = FormWindowState.Minimized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.Size = new Size(0, 0);
        this.Icon = new Icon(typeof(MainForm).Assembly.GetManifestResourceStream("MidosHouse.OotrMultiworld.Resources.icon.ico"));
    }

    public override void Restart() {
        APIs.Memory.SetBigEndian(true);
        if (this.client == null) {
            using (var res = ClientResult.open()) {
                if (res.IsOk()) {
                    this.client = res.Unwrap();
                } else {
                    using (var err = res.UnwrapErr()) {
                        SetError(err);
                    }
                }
            }
        }
        this.playerID = null;
        if (this.client != null) {
            this.client.SetPlayerID(this.playerID);
        }
    }

    public override void UpdateValues(ToolFormUpdateType type) {
        if (type != ToolFormUpdateType.PreFrame && type != ToolFormUpdateType.FastPreFrame) {
            return;
        }
        if ((APIs.GameInfo.GetGameInfo()?.Name ?? "Null") == "Null") {
            this.normalGameplay = false;
            return;
        }
        if (this.client != null) {
            ReceiveMessage(this.client);
            if (this.playerID == null) {
                ReadPlayerID();
            } else {
                SyncPlayerNames();
                if (this.coopContextAddr != null) {
                    if (Enumerable.SequenceEqual(APIs.Memory.ReadByteRange(0x11a5d0 + 0x1c, 6, "RDRAM"), new List<byte>(Encoding.UTF8.GetBytes("ZELDAZ")))) { // don't read save data while rom is loaded but not properly initialized
                        var randoContextAddr = APIs.Memory.ReadU32(0x1c6e90 + 0x15d4, "RDRAM");
                        if (randoContextAddr >= 0x8000_0000 && randoContextAddr != 0xffff_ffff) {
                            var newCoopContextAddr = APIs.Memory.ReadU32(randoContextAddr, "System Bus");
                            if (newCoopContextAddr >= 0x8000_0000 && newCoopContextAddr != 0xffff_ffff) {
                                if (APIs.Memory.ReadU32(0x11a5d0 + 0x135c, "RDRAM") == 0) { // game mode == gameplay
                                    if (!this.normalGameplay) {
                                        this.client.SendSaveData(APIs.Memory.ReadByteRange(0x11a5d0, 0x1450, "RDRAM"));
                                        this.normalGameplay = true;
                                    }
                                } else {
                                    this.normalGameplay = false;
                                }
                            } else {
                                this.normalGameplay = false;
                            }
                        } else {
                            this.normalGameplay = false;
                        }
                    } else {
                        this.normalGameplay = false;
                    }

                    SendItem(this.client, this.coopContextAddr.Value);
                    ReceiveItem(this.client, this.coopContextAddr.Value, this.playerID.Value);
                } else {
                    this.normalGameplay = false;
                }
            }
        } else {
            this.normalGameplay = false;
        }
    }

    private void ReceiveMessage(Client client) {
        using (var msg = client.TryRecv()) {
            switch (msg.Kind()) {
                case -1: // no message
                    break;
                case -2: // error
                    using (var err = msg.UnwrapErr()) {
                        SetError(err);
                    }
                    break;
                case -3: // GUI closed
                    if (this.client != null) {
                        this.client.Dispose();
                        this.client = null;
                    }
                    this.Close();
                    break;
                case 0: // ServerMessage::ItemQueue
                    this.itemQueue.Clear();
                    var len = msg.ItemQueueLen();
                    for (ushort i = 0; i < len; i++) {
                        this.itemQueue.Add(msg.ItemAtIndex(i));
                    }
                    break;
                case 1: // ServerMessage::GetItem
                    this.itemQueue.Add(msg.ItemAtIndex(0));
                    break;
                case 2: // ServerMessage::PlayerName
                    this.playerNames[msg.WorldId()] = msg.Filename();
                    break;
                case 3: // ServerMessage::ProgressiveItems
                    this.progressiveItems[msg.WorldId()] = msg.ProgressiveItems();
                    break;
                default:
                    using (var error = Error.from_string("BizHawk frontend received unknown command from client")) {
                        SetError(error);
                    }
                    return;
            }
        }
    }

    private void SendItem(Client client, uint coopContextAddr) {
        var outgoingKey = APIs.Memory.ReadU32(coopContextAddr + 0xc, "System Bus");
        if (outgoingKey != 0) {
            var kind = (ushort) APIs.Memory.ReadU16(coopContextAddr + 0x10, "System Bus");
            var player = (byte) APIs.Memory.ReadU16(coopContextAddr + 0x12, "System Bus");
            if (outgoingKey == 0xff05ff) {
                //Debug($"P{this.playerID}: Found an item {kind} for player {player} sent via network, ignoring");
            } else {
                //Debug($"P{this.playerID}: Found {outgoingKey}, an item {kind} for player {player}");
                client.SendItem(outgoingKey, kind, player);
            }
            APIs.Memory.WriteU16(coopContextAddr + 0x10, 0, "System Bus");
            APIs.Memory.WriteU16(coopContextAddr + 0x12, 0, "System Bus");
            APIs.Memory.WriteU32(coopContextAddr + 0xc, 0, "System Bus");
        }
    }

    private void ReceiveItem(Client client, uint coopContextAddr, byte playerID) {
        var stateLogo = APIs.Memory.ReadU32(0x11f200, "RDRAM");
        var stateMain = APIs.Memory.ReadS8(0x11b92f, "RDRAM");
        var stateMenu = APIs.Memory.ReadS8(0x1d8dd5, "RDRAM");
        var currentScene = APIs.Memory.ReadU8(0x1c8545, "RDRAM");
        // The following conditional will be made redundant by https://github.com/TestRunnerSRL/OoT-Randomizer/pull/1867. Keep it for back-compat for now.
        if (
            stateLogo != 0x802c_5880 && stateLogo != 0 && stateMain != 1 && stateMain != 2 && stateMenu == 0 && (
                (currentScene < 0x2c || currentScene > 0x33) && currentScene != 0x42 && currentScene != 0x4b // don't receive items in shops to avoid a softlock when buying an item at the same time as receiving one
            )
        ) {
            if (APIs.Memory.ReadU16(coopContextAddr + 0x8, "System Bus") == 0) {
                var internalCount = (ushort) APIs.Memory.ReadU16(0x11a5d0 + 0x90, "RDRAM");
                var externalCount = this.itemQueue.Count;
                if (internalCount < externalCount) {
                    var item = this.itemQueue[internalCount];
                    //Debug($"P{playerID}: Received an item {item} from another player");
                    APIs.Memory.WriteU16(coopContextAddr + 0x8, item, "System Bus");
                    APIs.Memory.WriteU16(coopContextAddr + 0x6, item == 0x00ca ? (playerID == 1 ? 2u : 1) : playerID, "System Bus");
                } else if (internalCount > externalCount) {
                    // warning: gap in received items
                }
            }
        }
    }

    private void ReadPlayerID() {
        var oldPlayerID = this.playerID;
        if ((APIs.GameInfo.GetGameInfo()?.Name ?? "Null") == "Null") {
            this.playerID = null;
            //TODO send state to GUI? ("Please open the ROM…")
        } else {
            var romIdent = APIs.Memory.ReadByteRange(0x20, 0x15, "ROM");
            if (!Enumerable.SequenceEqual(romIdent, new List<byte>(Encoding.UTF8.GetBytes("THE LEGEND OF ZELDA \0")))) {
                this.playerID = null;
                //TODO send state to GUI? ($"Expected OoTR, found {APIs.GameInfo.GetGameInfo()?.Name ?? "Null"}")
            } else {
                //TODO also check OoTR version bytes and error on vanilla OoT
                var newText = "Waiting for game…";
                if (Enumerable.SequenceEqual(APIs.Memory.ReadByteRange(0x11a5d0 + 0x1c, 6, "RDRAM"), new List<byte>(Encoding.UTF8.GetBytes("ZELDAZ")))) { // don't set or reset player ID while rom is loaded but not properly initialized
                    var randoContextAddr = APIs.Memory.ReadU32(0x1c6e90 + 0x15d4, "RDRAM");
                    if (randoContextAddr >= 0x8000_0000 && randoContextAddr != 0xffff_ffff) {
                        var newCoopContextAddr = APIs.Memory.ReadU32(randoContextAddr, "System Bus");
                        if (newCoopContextAddr >= 0x8000_0000 && newCoopContextAddr != 0xffff_ffff) {
                            var coopContextVersion = APIs.Memory.ReadU32(newCoopContextAddr, "System Bus");
                            if (coopContextVersion < 2) {
                                using (var error = Error.from_string("randomizer version too old (version 5.1.4 or higher required)")) {
                                    SetError(error);
                                }
                                this.coopContextAddr = null;
                                return;
                            }
                            if (coopContextVersion > 5) {
                                using (var error = Error.from_string("randomizer version too new (please tell Fenhl that Mido's House Multiworld needs to be updated)")) {
                                    SetError(error);
                                }
                                this.coopContextAddr = null;
                                return;
                            }
                            if (coopContextVersion == 5) {
                                if (APIs.Memory.ReadU8(0x1c, "ROM") == 0xfe) {
                                    // on dev-fenhl, version 5 is https://github.com/OoTRandomizer/OoT-Randomizer/pull/1871
                                    this.progressiveItemsEnable = true;
                                    APIs.Memory.WriteU8(newCoopContextAddr + 0x000b, 1); // MW_PROGRESSIVE_ITEMS_ENABLE
                                } else {
                                    using (var error = Error.from_string("randomizer version too new (please tell Fenhl that Mido's House Multiworld needs to be updated)")) {
                                        SetError(error);
                                    }
                                    this.coopContextAddr = null;
                                    return;
                                }
                            } else {
                                this.progressiveItemsEnable = false;
                            }
                            if (coopContextVersion >= 3) {
                                APIs.Memory.WriteU8(newCoopContextAddr + 0x000a, 1, "System Bus"); // enable MW_SEND_OWN_ITEMS for server-side tracking
                            }
                            if (coopContextVersion >= 4) {
                                var newFileHash = APIs.Memory.ReadByteRange(newCoopContextAddr + 0x0814, 5, "System Bus");
                                if (this.client != null && !Enumerable.SequenceEqual(this.fileHash, newFileHash)) {
                                    this.client.SendFileHash(newFileHash);
                                    this.fileHash = new List<byte>(newFileHash);
                                }
                            }
                            this.coopContextAddr = newCoopContextAddr;
                            this.playerID = (byte?) APIs.Memory.ReadU8(newCoopContextAddr + 0x4, "System Bus");
                            newText = $"Connected as world {this.playerID}";
                        } else {
                            this.coopContextAddr = null;
                        }
                    }
                }
            }
        }
        if (this.client != null && this.playerID != oldPlayerID) {
            this.client.SetPlayerID(this.playerID);
        }
    }

    private void SyncPlayerNames() {
        var oldPlayerName = this.playerName;
        if (this.playerID == null) {
            this.playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
        } else {
            if (Enumerable.SequenceEqual(APIs.Memory.ReadByteRange(0x0020 + 0x1c, 6, "SRAM"), new List<byte>(Encoding.UTF8.GetBytes("ZELDAZ")))) {
                // get own player name from save file
                this.playerName = new List<byte>(APIs.Memory.ReadByteRange(0x0020 + 0x0024, 8, "SRAM"));
                // always fill player names in co-op context (some player names may go missing seemingly at random while others stay intact, so this has to run every frame)
                if (this.coopContextAddr != null) {
                    for (var world = 0; world < 256; world++) {
                        APIs.Memory.WriteByteRange(this.coopContextAddr.Value + 0x14 + world * 0x8, this.playerNames[world], "System Bus");
                        // fill progressive items of other players
                        if (progressiveItemsEnable) {
                            APIs.Memory.WriteU32(this.coopContextAddr.Value + 0x081c + world * 0x4, this.progressiveItems[world], "System Bus");
                        }
                    }
                }
            } else {
                // file 1 does not exist, reset player name
                this.playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
            }
        }
        if (this.client != null && !Enumerable.SequenceEqual(this.playerName, oldPlayerName)) {
            this.client.SetPlayerName(this.playerName);
        }
    }

    private void SetError(Error error) {
        using (var debug = error.Debug()) {
            using (var display = error.Display()) {
                this.DialogController.ShowMessageBox(this, $"{display.AsString()}\n\ndebug info: {debug.AsString()}", "Error in Mido's House Multiworld for BizHawk", EMsgBoxIcon.Error);
            }
        }
        if (this.client != null) {
            this.client.Dispose();
            this.client = null;
        }
        this.Close();
    }
}
