using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;

namespace MidosHouse.OotrMultiworld {
    internal class Native {
        [DllImport("multiworld")] internal static extern void log(OwnedStringHandle msg);
        [DllImport("multiworld")] internal static extern StringHandle version_string();
        [DllImport("multiworld")] internal static extern BoolResult update_available();
        [DllImport("multiworld")] internal static extern void bool_result_free(IntPtr bool_res);
        [DllImport("multiworld")] internal static extern bool bool_result_is_ok(BoolResult bool_res);
        [DllImport("multiworld")] internal static extern bool bool_result_unwrap(IntPtr bool_res);
        [DllImport("multiworld")] internal static extern Error bool_result_unwrap_err(IntPtr bool_res);
        [DllImport("multiworld")] internal static extern void error_free(IntPtr error);
        [DllImport("multiworld")] internal static extern Error error_from_string(OwnedStringHandle text);
        [DllImport("multiworld")] internal static extern StringHandle error_debug(Error error);
        [DllImport("multiworld")] internal static extern StringHandle error_display(Error error);
        [DllImport("multiworld")] internal static extern UnitResult run_updater();
        [DllImport("multiworld")] internal static extern ushort default_port();
        [DllImport("multiworld")] internal static extern ClientResult connect_ipv4(ushort port);
        [DllImport("multiworld")] internal static extern ClientResult connect_ipv6(ushort port);
        [DllImport("multiworld")] internal static extern void client_result_free(IntPtr client_res);
        [DllImport("multiworld")] internal static extern bool client_result_is_ok(ClientResult client_res);
        [DllImport("multiworld")] internal static extern Client client_result_unwrap(IntPtr client_res);
        [DllImport("multiworld")] internal static extern void client_set_error(Client client, IntPtr error);
        [DllImport("multiworld")] internal static extern byte client_session_state(Client client);
        [DllImport("multiworld")] internal static extern StringHandle client_debug_err(Client client);
        [DllImport("multiworld")] internal static extern StringHandle client_display_err(Client client);
        [DllImport("multiworld")] internal static extern bool client_has_wrong_password(Client client);
        [DllImport("multiworld")] internal static extern void client_reset_wrong_password(Client client);
        [DllImport("multiworld")] internal static extern bool client_has_wrong_file_hash(Client client);
        [DllImport("multiworld")] internal static extern void client_free(IntPtr client);
        [DllImport("multiworld")] internal static extern Error client_result_unwrap_err(IntPtr client_res);
        [DllImport("multiworld")] internal static extern void string_free(IntPtr s);
        [DllImport("multiworld")] internal static extern ulong client_num_rooms(Client client);
        [DllImport("multiworld")] internal static extern StringHandle client_room_name(Client client, ulong i);
        [DllImport("multiworld")] internal static extern void string_result_free(IntPtr str_res);
        [DllImport("multiworld")] internal static extern bool string_result_is_ok(StringResult str_res);
        [DllImport("multiworld")] internal static extern StringHandle string_result_unwrap(IntPtr str_res);
        [DllImport("multiworld")] internal static extern Error string_result_unwrap_err(IntPtr str_res);
        [DllImport("multiworld")] internal static extern UnitResult client_room_connect(Client client, OwnedStringHandle room_name, OwnedStringHandle room_password);
        [DllImport("multiworld")] internal static extern UnitResult client_set_player_id(Client client, byte id);
        [DllImport("multiworld")] internal static extern void unit_result_free(IntPtr unit_res);
        [DllImport("multiworld")] internal static extern bool unit_result_is_ok(UnitResult unit_res);
        [DllImport("multiworld")] internal static extern Error unit_result_unwrap_err(IntPtr unit_res);
        [DllImport("multiworld")] internal static extern UnitResult client_reset_player_id(Client client);
        [DllImport("multiworld")] internal static extern UnitResult client_set_player_name(Client client, IntPtr name);
        [DllImport("multiworld")] internal static extern UnitResult client_set_file_hash(Client client, IntPtr hash);
        [DllImport("multiworld")] internal static extern UnitResult client_set_save_data(Client client, IntPtr save);
        [DllImport("multiworld")] internal static extern byte client_num_players(Client client);
        [DllImport("multiworld")] internal static extern byte client_player_world(Client client, byte player_idx);
        [DllImport("multiworld")] internal static extern StringHandle client_player_state(Client client, byte player_idx);
        [DllImport("multiworld")] internal static extern StringHandle client_other_room_state(Client client);
        [DllImport("multiworld")] internal static extern UnitResult client_kick_player(Client client, byte player_idx);
        [DllImport("multiworld")] internal static extern UnitResult client_delete_room(Client client);
        [DllImport("multiworld")] internal static extern OptMessageResult client_try_recv_message(Client client, ushort port);
        [DllImport("multiworld")] internal static extern void opt_message_result_free(IntPtr opt_msg_res);
        [DllImport("multiworld")] internal static extern bool opt_message_result_is_ok_some(OptMessageResult opt_msg_res);
        [DllImport("multiworld")] internal static extern ServerMessage opt_message_result_unwrap_unwrap(IntPtr opt_msg_res);
        [DllImport("multiworld")] internal static extern void message_free(IntPtr msg);
        [DllImport("multiworld")] internal static extern bool opt_message_result_is_err(OptMessageResult opt_msg_res);
        [DllImport("multiworld")] internal static extern Error opt_message_result_unwrap_err(IntPtr opt_msg_res);
        [DllImport("multiworld")] internal static extern UnitResult client_send_item(Client client, uint key, ushort kind, byte target_world);
        [DllImport("multiworld")] internal static extern ushort client_item_queue_len(Client client);
        [DllImport("multiworld")] internal static extern ushort client_item_kind_at_index(Client client, ushort index);
        [DllImport("multiworld")] internal static extern IntPtr client_get_player_name(Client client, byte world);
        [DllImport("multiworld")] internal static extern ulong client_get_autodelete_seconds(Client client);
        [DllImport("multiworld")] internal static extern UnitResult client_set_autodelete_seconds(Client client, ulong seconds);
    }

    internal class BoolResult : SafeHandle {
        internal BoolResult() : base(IntPtr.Zero, true) {}

        public override bool IsInvalid {
            get { return this.handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle() {
            if (!this.IsInvalid) {
                Native.bool_result_free(this.handle);
            }
            return true;
        }

        internal bool IsOk() => Native.bool_result_is_ok(this);

        internal bool Unwrap() {
            var inner = Native.bool_result_unwrap(this.handle);
            this.handle = IntPtr.Zero; // bool_result_unwrap takes ownership
            return inner;
        }

        internal Error UnwrapErr() {
            var err = Native.bool_result_unwrap_err(this.handle);
            this.handle = IntPtr.Zero; // bool_result_unwrap_err takes ownership
            return err;
        }
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

        internal byte SessionState() => Native.client_session_state(this);
        internal StringHandle DebugErr() => Native.client_debug_err(this);
        internal StringHandle DisplayErr() => Native.client_display_err(this);

        internal bool HasWrongPassword() => Native.client_has_wrong_password(this);
        internal void ResetWrongPassword() => Native.client_reset_wrong_password(this);
        internal ulong NumRooms() => Native.client_num_rooms(this);
        internal StringHandle RoomName(ulong i) => Native.client_room_name(this, i);

        internal UnitResult CreateJoinRoom(string roomName, string password) {
            using (var nameHandle = new OwnedStringHandle(roomName)) {
                using (var passwordHandle = new OwnedStringHandle(password)) {
                    return Native.client_room_connect(this, nameHandle, passwordHandle);
                }
            }
        }

        internal UnitResult SetPlayerID(byte? id) {
            if (id == null) {
                return Native.client_reset_player_id(this);
            } else {
                return Native.client_set_player_id(this, id.Value);
            }
        }

        internal UnitResult SetPlayerName(List<byte> name) {
            var namePtr = Marshal.AllocHGlobal(8);
            Marshal.Copy(name.ToArray(), 0, namePtr, 8);
            var res = Native.client_set_player_name(this, namePtr);
            Marshal.FreeHGlobal(namePtr);
            return res;
        }

        internal UnitResult SendSaveData(List<byte> saveData) {
            var savePtr = Marshal.AllocHGlobal(0x1450);
            Marshal.Copy(saveData.ToArray(), 0, savePtr, 0x1450);
            var res = Native.client_set_save_data(this, savePtr);
            Marshal.FreeHGlobal(savePtr);
            return res;
        }

        internal UnitResult SendFileHash(List<byte> fileHash) {
            var hashPtr = Marshal.AllocHGlobal(5);
            Marshal.Copy(fileHash.ToArray(), 0, hashPtr, 5);
            var res = Native.client_set_file_hash(this, hashPtr);
            Marshal.FreeHGlobal(hashPtr);
            return res;
        }

        internal List<byte> GetPlayerName(byte world) {
            var name = new byte[8];
            Marshal.Copy(Native.client_get_player_name(this, world), name, 0, 8);
            return name.ToList();
        }

        internal bool HasWrongFileHash() => Native.client_has_wrong_file_hash(this);
        internal byte NumPlayers() => Native.client_num_players(this);
        internal byte PlayerWorld(byte player_idx) => Native.client_player_world(this, player_idx);
        internal StringHandle PlayerState(byte player_idx) => Native.client_player_state(this, player_idx);
        internal StringHandle OtherState() => Native.client_other_room_state(this);
        internal OptMessageResult TryRecv(ushort port) => Native.client_try_recv_message(this, port);
        internal UnitResult SendItem(uint key, ushort kind, byte targetWorld) => Native.client_send_item(this, key, kind, targetWorld);
        internal ushort ItemQueueLen() => Native.client_item_queue_len(this);
        internal ushort Item(ushort index) => Native.client_item_kind_at_index(this, index);
        internal UnitResult KickPlayer(byte player_idx) => Native.client_kick_player(this, player_idx);
        internal UnitResult DeleteRoom() => Native.client_delete_room(this);
        internal ulong AutodeleteSeconds() => Native.client_get_autodelete_seconds(this);
        internal UnitResult SetAutodeleteSeconds(ulong seconds) => Native.client_set_autodelete_seconds(this, seconds);
    }

    internal class ClientResult : SafeHandle {
        internal ClientResult() : base(IntPtr.Zero, true) {}

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

        internal void SetAsClientState(Client client) {
            Native.client_set_error(client, this.handle);
            this.handle = IntPtr.Zero; // client_set_error takes ownership of the Error
        }
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

        internal bool IsOkSome() => Native.opt_message_result_is_ok_some(this);
        internal bool IsErr() => Native.opt_message_result_is_err(this);

        internal ServerMessage UnwrapUnwrap() {
            var msg = Native.opt_message_result_unwrap_unwrap(this.handle);
            this.handle = IntPtr.Zero; // opt_message_result_unwrap_unwrap takes ownership
            return msg;
        }

        internal Error UnwrapErr() {
            var err = Native.opt_message_result_unwrap_err(this.handle);
            this.handle = IntPtr.Zero; // opt_msg_result_unwrap_err takes ownership
            return err;
        }
    }

    internal class ServerMessage : SafeHandle {
        internal ServerMessage() : base(IntPtr.Zero, true) {}

        public override bool IsInvalid {
            get { return this.handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle() {
            if (!this.IsInvalid) {
                Native.message_free(this.handle);
            }
            return true;
        }
    }

    internal class StringResult : SafeHandle {
        internal StringResult() : base(IntPtr.Zero, true) {}

        public override bool IsInvalid {
            get { return this.handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle() {
            if (!this.IsInvalid) {
                Native.string_result_free(this.handle);
            }
            return true;
        }

        internal bool IsOk() => Native.string_result_is_ok(this);

        internal StringHandle Unwrap() {
            var s = Native.string_result_unwrap(this.handle);
            this.handle = IntPtr.Zero; // string_result_unwrap takes ownership
            return s;
        }

        internal Error UnwrapErr() {
            var err = Native.string_result_unwrap_err(this.handle);
            this.handle = IntPtr.Zero; // string_result_unwrap_err takes ownership
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

    internal class DurationFormatter {
        public string Text { get; set; }
        public ulong Seconds { get; set; }

        internal DurationFormatter(ulong seconds) {
            this.Seconds = seconds;

            ulong mins = seconds / 60;
            ulong secs = seconds % 60;

            ulong hours = mins / 60;
            mins = mins % 60;

            ulong days = hours / 24;
            hours = hours % 24;

            List<string> parts = new List<string>();
            if (days > 0) {
                string plural = days == 1 ? "" : "s";
                parts.Add($"{days} day{plural}");
            }
            if (hours > 0) {
                string plural = hours == 1 ? "" : "s";
                parts.Add($"{hours} hour{plural}");
            }
            if (mins > 0) {
                string plural = mins == 1 ? "" : "s";
                parts.Add($"{mins} minute{plural}");
            }
            if (secs > 0) {
                string plural = secs == 1 ? "" : "s";
                parts.Add($"{secs} second{plural}");
            }
            switch (parts.Count) {
                case 0: {
                    this.Text = "0 seconds";
                    break;
                }
                case 1: {
                    this.Text = parts[0];
                    break;
                }
                case 2: {
                    this.Text = $"{parts[0]} and {parts[1]}";
                    break;
                }
                default: {
                    string last = parts.Last();
                    parts.RemoveAt(parts.Count - 1);
                    this.Text = $"{String.Join(", ", parts)}, and {last}";
                    break;
                }
            }
        }
    }

    [ExternalTool("Mido's House Multiworld", Description = "Play interconnected Ocarina of Time Randomizer seeds")]
    [ExternalToolEmbeddedIcon("MidosHouse.OotrMultiworld.Resources.icon.ico")]
    public sealed class MainForm : ToolFormBase, IExternalToolForm {
        private Label state = new Label();
        private Label debugInfo = new Label();
        private ComboBox rooms = new ComboBox();
        private TextBox password = new TextBox();
        private Button createJoinButton = new Button();
        private Label version = new Label();
        private Button deleteRoomButton = new Button();
        private Button roomOptionsButton = new Button();
        private Form roomOptionsForm = new Form();
        private List<Label> playerStates = new List<Label>();
        private List<Button> kickButtons = new List<Button>();
        private Label otherState = new Label();

        private ushort port = Native.default_port();
        private Client? client;
        private uint? coopContextAddr;
        private byte? playerID;
        private List<byte> playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
        private List<byte> fileHash = new List<byte> { 0xff, 0xff, 0xff, 0xff, 0xff };
        private bool normalGameplay = false;

        public ApiContainer? _apiContainer { get; set; }
        private ApiContainer APIs => _apiContainer ?? throw new NullReferenceException();

        public override bool BlocksInputWhenFocused { get; } = false;
        protected override string WindowTitleStatic => "Mido's House Multiworld for BizHawk";

        public override bool AskSaveChanges() => true; //TODO warn before leaving an active game?

        public MainForm() {
            SuspendLayout();
            this.ClientSize = new Size(509, 256);
            this.Icon = new Icon(typeof(MainForm).Assembly.GetManifestResourceStream("MidosHouse.OotrMultiworld.Resources.icon.ico"));

            this.state.TabIndex = 0;
            this.state.AutoSize = true;
            this.state.Location = new Point(12, 9);
            this.Controls.Add(this.state);

            this.debugInfo.TabIndex = 1;
            this.debugInfo.AutoSize = true;
            this.debugInfo.Location = new Point(12, 42);
            this.debugInfo.Visible = false;
            this.Controls.Add(this.debugInfo);

            this.rooms.TabIndex = 2;
            this.rooms.Location = new Point(12, 42);
            this.rooms.Size = new Size(485, 25);
            this.rooms.Enabled = false;
            this.rooms.Items.Add("Loading room list…");
            this.rooms.SelectedIndex = 0;
            this.rooms.AutoCompleteMode = AutoCompleteMode.Append;
            this.rooms.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.rooms.SelectedIndexChanged += (s, e) => {
                if (this.client != null) {
                    this.UpdateLobbyState(this.client, false);
                }
            };
            this.rooms.TextChanged += (s, e) => {
                if (this.client != null) {
                    this.UpdateLobbyState(this.client, false);
                }
            };
            this.Controls.Add(this.rooms);

            this.password.TabIndex = 3;
            this.password.Location = new Point(12, 82);
            this.password.Size = new Size(485, 25);
            this.password.UseSystemPasswordChar = true;
            //TODO (.net 5) add PlaceholderText (“Password”)
            this.password.TextChanged += (s, e) => {
                this.createJoinButton.Enabled = this.rooms.Enabled && this.rooms.Text.Length > 0 && this.password.Text.Length > 0;
            };
            this.password.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    e.SuppressKeyPress = true;
                    CreateJoinRoom();
                }
            };
            this.Controls.Add(this.password);

            this.createJoinButton.TabIndex = 4;
            this.createJoinButton.Location = new Point(11, 119);
            this.createJoinButton.AutoSize = true;
            this.createJoinButton.Text = "Create/Join";
            this.createJoinButton.Enabled = false;
            this.createJoinButton.Click += (s, e) => {
                CreateJoinRoom();
            };
            this.Controls.Add(this.createJoinButton);

            this.version.TabIndex = 5;
            this.version.Location = new Point(162, 119);
            this.version.AutoSize = false;
            this.version.Size = new Size(335, 25);
            this.version.TextAlign = ContentAlignment.MiddleRight;
            using (var versionString = Native.version_string()) {
                this.version.Text = versionString.AsString();
            }
            this.Controls.Add(this.version);

            this.deleteRoomButton.TabIndex = 6;
            this.deleteRoomButton.Location = new Point(11, 42);
            this.deleteRoomButton.AutoSize = true;
            this.deleteRoomButton.Text = "Delete Room";
            this.deleteRoomButton.Enabled = true;
            this.deleteRoomButton.Click += (s, e) => {
                if (this.client != null) {
                    if (this.DialogController.ShowMessageBox2(this, "Are you sure you want to delete this room? Items that have already been sent will be lost forever!", null, EMsgBoxIcon.Question)) {
                        using (var res = this.client.DeleteRoom()) {
                            if (!res.IsOk()) {
                                using (var err = res.UnwrapErr()) {
                                    SetError(err);
                                }
                            }
                        }
                    }
                }
            };
            this.Controls.Add(this.deleteRoomButton);

            this.roomOptionsButton.TabIndex = 7;
            this.roomOptionsButton.Location = new Point(211, 42);
            this.roomOptionsButton.AutoSize = true;
            this.roomOptionsButton.Text = "Options";
            this.roomOptionsButton.Enabled = true;
            this.roomOptionsButton.Click += (s, e) => {
                if (this.client != null) {
                    if (this.roomOptionsForm.Visible) {
                        this.roomOptionsForm.BringToFront();
                    } else {
                        this.roomOptionsForm = new Form();
                        this.roomOptionsForm.ClientSize = new Size(509, 256);
                        this.roomOptionsForm.Icon = new Icon(typeof(MainForm).Assembly.GetManifestResourceStream("MidosHouse.OotrMultiworld.Resources.icon.ico"));

                        Label autodeleteDeltaLabel = new Label();
                        autodeleteDeltaLabel.Text = "Automatically delete this room if no items are sent for:";
                        autodeleteDeltaLabel.TabIndex = 0;
                        autodeleteDeltaLabel.AutoSize = true;
                        autodeleteDeltaLabel.Location = new Point(12, 9);
                        this.roomOptionsForm.Controls.Add(autodeleteDeltaLabel);

                        ComboBox autodeleteDelta = new ComboBox();
                        autodeleteDelta.TabIndex = 1;
                        autodeleteDelta.Location = new Point(12, 42);
                        autodeleteDelta.Size = new Size(485, 25);
                        autodeleteDelta.DropDownStyle = ComboBoxStyle.DropDownList;
                        autodeleteDelta.DisplayMember = "Text";
                        autodeleteDelta.ValueMember = "Seconds";
                        autodeleteDelta.Enabled = true;
                        autodeleteDelta.Items.Add(new DurationFormatter(60 * 60 * 24));
                        autodeleteDelta.Items.Add(new DurationFormatter(60 * 60 * 24 * 7));
                        autodeleteDelta.Items.Add(new DurationFormatter(60 * 60 * 24 * 90));
                        ulong currentSeconds = this.client.AutodeleteSeconds();
                        switch (currentSeconds) {
                            case 60 * 60 * 24: {
                                autodeleteDelta.SelectedIndex = 0;
                                break;
                            }
                            case 60 * 60 * 24 * 7: {
                                autodeleteDelta.SelectedIndex = 1;
                                break;
                            }
                            case 60 * 60 * 24 * 90: {
                                autodeleteDelta.SelectedIndex = 2;
                                break;
                            }
                            default: {
                                autodeleteDelta.Items.Add(new DurationFormatter(currentSeconds));
                                autodeleteDelta.SelectedIndex = 3;
                                break;
                            }
                        }
                        autodeleteDelta.SelectedValueChanged += (s, e) => {
                            if (this.client != null) {
                                DurationFormatter selected_duration = (DurationFormatter) autodeleteDelta.SelectedItem;
                                using (var res = this.client.SetAutodeleteSeconds(selected_duration.Seconds)) {
                                    if (!res.IsOk()) {
                                        using (var err = res.UnwrapErr()) {
                                            SetError(err);
                                        }
                                    }
                                }
                            }
                        };
                        this.roomOptionsForm.Controls.Add(autodeleteDelta);

                        this.roomOptionsForm.Show();
                    }
                }
            };
            this.Controls.Add(this.roomOptionsButton);

            this.otherState.TabIndex = 6;
            this.otherState.Location = new Point(12, 42);
            this.otherState.AutoSize = true;
            this.otherState.Visible = false;
            this.Controls.Add(this.otherState);

            ResumeLayout(true);
        }

        private void CreateJoinRoom() {
            if (this.client != null) {
                using (var res = this.client.CreateJoinRoom(this.rooms.Text, this.password.Text)) {
                    if (!res.IsOk()) {
                        using (var err = res.UnwrapErr()) {
                            SetError(err);
                        }
                    }
                }
            }
        }

        public override void Restart() {
            APIs.Memory.SetBigEndian(true);
            if ((APIs.GameInfo.GetGameInfo()?.Name ?? "Null") == "Null") {
                this.state.Text = "Please open the ROM…";
                HideLobbyUI();
                HideRoomUI();
                return;
            }
            if (this.client == null) {
                this.state.Text = "Checking for updates…";
                using (var update_available_res = Native.update_available()) {
                    if (update_available_res.IsOk()) {
                        if (update_available_res.Unwrap()) {
                            this.state.Text = "An update is available";
                            using (var run_updater_res = Native.run_updater()) {
                                if (!run_updater_res.IsOk()) {
                                    using (var error = run_updater_res.UnwrapErr()) {
                                        SetError(error);
                                    }
                                    return;
                                }
                            }
                        }
                    } else {
                        using (var error = update_available_res.UnwrapErr()) {
                            SetError(error);
                        }
                        return;
                    }
                }
                this.state.Text = "Connecting…";
                using (var res6 = Native.connect_ipv6(this.port)) {
                    if (res6.IsOk()) {
                        this.client = res6.Unwrap();
                    } else {
                        using (var res4 = Native.connect_ipv4(this.port)) {
                            if (res4.IsOk()) {
                                this.client = res4.Unwrap();
                            } else {
                                //TODO TCP connections unavailable, try WebSocket instead. If that fails too, offer self-hosting/direct connections
                                using (var err = res4.UnwrapErr()) {
                                    SetError(err);
                                }
                                this.rooms.Items[0] = "Failed to load room list";
                            }
                        }
                    }
                }
            }
            this.playerID = null;
            if (this.client != null) {
                UpdateUI(this.client);
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
                if (this.client.SessionState() == 4) { // Room
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
                                                using (var res = this.client.SendSaveData(APIs.Memory.ReadByteRange(0x11a5d0, 0x1450, "RDRAM"))) {
                                                    if (!res.IsOk()) {
                                                        using (var err = res.UnwrapErr()) {
                                                            SetError(err);
                                                        }
                                                    }
                                                }
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
            } else {
                this.normalGameplay = false;
            }
        }

        private void ReceiveMessage(Client client) {
            using (var res = client.TryRecv(this.port)) {
                if (res.IsOkSome()) {
                    using (var msg = res.UnwrapUnwrap()) {
                        UpdateUI(client);
                    }
                } else if (res.IsErr()) {
                    using (var err = res.UnwrapErr()) {
                        SetError(err);
                    }
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
                APIs.Memory.WriteU32(coopContextAddr + 0xc, 0, "System Bus");
                APIs.Memory.WriteU16(coopContextAddr + 0x10, 0, "System Bus");
                APIs.Memory.WriteU16(coopContextAddr + 0x12, 0, "System Bus");
            }
        }

        private void ReceiveItem(Client client, uint coopContextAddr, byte playerID) {
            if (client.HasWrongFileHash()) { return; }
            var stateLogo = APIs.Memory.ReadU32(0x11f200, "RDRAM");
            var stateMain = APIs.Memory.ReadS8(0x11b92f, "RDRAM");
            var stateMenu = APIs.Memory.ReadS8(0x1d8dd5, "RDRAM");
            var currentScene = APIs.Memory.ReadU8(0x1c8545, "RDRAM");
            if (
                stateLogo != 0x802c_5880 && stateLogo != 0 && stateMain != 1 && stateMain != 2 && stateMenu == 0 && (
                    (currentScene < 0x2c || currentScene > 0x33) && currentScene != 0x42 && currentScene != 0x4b // don't receive items in shops to avoid a softlock when buying an item at the same time as receiving one
                )
            ) {
                if (APIs.Memory.ReadU16(coopContextAddr + 0x8, "System Bus") == 0) {
                    var internalCount = (ushort) APIs.Memory.ReadU16(0x11a5d0 + 0x90, "RDRAM");
                    var externalCount = client.ItemQueueLen();
                    if (internalCount < externalCount) {
                        var item = client.Item((ushort) internalCount);
                        //Debug($"P{playerID}: Received an item {item} from another player");
                        APIs.Memory.WriteU16(coopContextAddr + 0x8, item, "System Bus");
                        APIs.Memory.WriteU16(coopContextAddr + 0x6, item == 0xca ? (playerID == 1 ? 2u : 1) : playerID, "System Bus");
                    } else if (internalCount > externalCount) {
                        // warning: gap in received items
                    }
                }
            }
        }

        private void UpdateUI(Client client) {
            switch (client.SessionState()) {
                case 0: { // Error
                    using (var display = client.DisplayErr()) {
                        this.state.Text = $"error: {display.AsString()}";
                    }
                    using (var debug = client.DebugErr()) {
                        this.debugInfo.Text = $"debug info: {debug.AsString()}";
                    }
                    this.debugInfo.Visible = true;
                    HideLobbyUI();
                    HideRoomUI();
                    break;
                }
                case 1: { // Init
                    this.state.Text = "Loading room list…";
                    this.debugInfo.Visible = false;
                    HideLobbyUI();
                    HideRoomUI();
                    break;
                }
                case 2: { // InitAutoRejoin
                    this.state.Text = "Reconnecting to room…";
                    this.debugInfo.Visible = false;
                    HideLobbyUI();
                    HideRoomUI();
                    break;
                }
                case 3: { // Lobby
                    this.UpdateLobbyState(client, true);
                    this.debugInfo.Visible = false;
                    break;
                }
                case 4: { // Room
                    this.UpdateRoomState(client);
                    this.debugInfo.Visible = false;
                    break;
                }
                case 5: { // Closed
                    this.state.Text = "You have been disconnected. Reopen the tool to reconnect."; //TODO reconnect button
                    this.debugInfo.Visible = false;
                    HideLobbyUI();
                    HideRoomUI();
                    break;
                }
                default: {
                    SetError(Error.from_string("received unknown session state type"));
                    break;
                }
            }
        }

        private void UpdateLobbyState(Client client, bool updateRoomList) {
            if (client.HasWrongPassword()) {
                this.DialogController.ShowMessageBox(this, "wrong password", null, EMsgBoxIcon.Error);
                client.ResetWrongPassword();
                this.password.Text = "";
            }
            var numRooms = client.NumRooms();
            this.state.Text = "Join or create a room:";
            SuspendLayout();
            HideRoomUI();
            this.rooms.Visible = true;
            this.password.Visible = true;
            this.createJoinButton.Visible = true;
            this.version.Visible = true;
            if (updateRoomList) {
                this.rooms.SelectedItem = null;
                this.rooms.Items.Clear();
                for (ulong i = 0; i < numRooms; i++) {
                    this.rooms.Items.Add(client.RoomName(i).AsString());
                }
            }
            this.rooms.Enabled = true;
            if (this.rooms.Text.Length > 0) {
                this.createJoinButton.Enabled = this.password.Text.Length > 0;
                if (this.rooms.Items.Contains(this.rooms.Text)) {
                    this.createJoinButton.Text = "Join";
                } else {
                    this.createJoinButton.Text = "Create";
                }
            } else {
                this.createJoinButton.Enabled = false;
                this.createJoinButton.Text = "Create/Join";
            }
            ResumeLayout(true);
        }

        private void UpdateRoomState(Client client) {
            if (client.HasWrongFileHash()) {
                if (this.DialogController.ShowMessageBox2(this, "This room is for a different seed. Delete this room? (Items that have already been sent will be lost forever!)", null, EMsgBoxIcon.Warning)) {
                    using (var res = client.DeleteRoom()) {
                        if (!res.IsOk()) {
                            using (var e = res.UnwrapErr()) {
                                SetError(e);
                            }
                        }
                    }
                } else {
                    var n = client.NumPlayers();
                    for (byte player_idx = 0; player_idx < n; player_idx++) {
                        if (client.PlayerWorld(player_idx) == this.playerID) {
                            using (var res = client.KickPlayer(player_idx)) {
                                if (!res.IsOk()) {
                                    using (var e = res.UnwrapErr()) {
                                        SetError(e);
                                    }
                                }
                            }
                        }
                    }
                }
                return;
            }
            SuspendLayout();
            HideLobbyUI();
            this.deleteRoomButton.Visible = true;
            this.roomOptionsButton.Visible = true;
            var num_players = client.NumPlayers();
            for (byte player_idx = 0; player_idx < num_players; player_idx++) {
                if (player_idx >= this.playerStates.Count) {
                    var kickButton = new Button();
                    kickButton.TabIndex = 2 * player_idx + 6;
                    kickButton.Location = new Point(12, 40 * player_idx + 82);
                    kickButton.AutoSize = true;
                    kickButton.Enabled = true;
                    var closurePlayerIdx = player_idx;
                    kickButton.Click += (s, e) => {
                        using (var res = client.KickPlayer(closurePlayerIdx)) {
                            if (!res.IsOk()) {
                                using (var err = res.UnwrapErr()) {
                                    SetError(err);
                                }
                            }
                        }
                    };
                    this.Controls.Add(kickButton);
                    this.kickButtons.Add(kickButton);

                    var playerState = new Label();
                    playerState.TabIndex = 2 * player_idx + 7;
                    playerState.Location = new Point(92, 40 * player_idx + 82);
                    playerState.AutoSize = true;
                    this.Controls.Add(playerState);
                    this.playerStates.Add(playerState);
                }
                using (var stateText = client.PlayerState(player_idx)) {
                    this.playerStates[player_idx].Text = stateText.AsString();
                }
                this.playerStates[player_idx].Visible = true;
                this.kickButtons[player_idx].Text = client.PlayerWorld(player_idx) == this.playerID ? "Leave" : "Kick";
                this.kickButtons[player_idx].Visible = true;
            }
            this.otherState.TabIndex = 2 * num_players + 6;
            this.otherState.Location = new Point(12, 40 * num_players + 82);
            this.otherState.Visible = true;
            this.otherState.Text = client.OtherState().AsString();
            if (num_players < this.playerStates.Count) {
                for (var player_idx = num_players; player_idx < this.playerStates.Count; player_idx++) {
                    this.Controls.Remove(this.playerStates[player_idx]);
                    this.Controls.Remove(this.kickButtons[player_idx]);
                }
                this.playerStates.RemoveRange(num_players, this.playerStates.Count - num_players);
                this.kickButtons.RemoveRange(num_players, this.kickButtons.Count - num_players);
            }
            ResumeLayout();
        }

        private void ReadPlayerID() {
            var oldPlayerID = this.playerID;
            if ((APIs.GameInfo.GetGameInfo()?.Name ?? "Null") == "Null") {
                this.playerID = null;
                this.state.Text = "Please open the ROM…";
                PerformLayout();
            } else {
                var romIdent = APIs.Memory.ReadByteRange(0x20, 0x15, "ROM");
                if (!Enumerable.SequenceEqual(romIdent, new List<byte>(Encoding.UTF8.GetBytes("THE LEGEND OF ZELDA \0")))) {
                    this.playerID = null;
                    this.state.Text = $"Expected OoTR, found {APIs.GameInfo.GetGameInfo()?.Name ?? "Null"}";
                    PerformLayout();
                } else {
                    //TODO also check OoTR version bytes and error on vanilla OoT
                    var newText = "Waiting for game…";
                    if (Enumerable.SequenceEqual(APIs.Memory.ReadByteRange(0x11a5d0 + 0x1c, 6, "RDRAM"), new List<byte>(Encoding.UTF8.GetBytes("ZELDAZ")))) { // don't set or reset player ID while rom is loaded but not properly initialized
                        var randoContextAddr = APIs.Memory.ReadU32(0x1c6e90 + 0x15d4, "RDRAM");
                        if (randoContextAddr >= 0x8000_0000 && randoContextAddr != 0xffff_ffff) {
                            var newCoopContextAddr = APIs.Memory.ReadU32(randoContextAddr, "System Bus");
                            if (newCoopContextAddr >= 0x8000_0000 && newCoopContextAddr != 0xffff_ffff) {
                                switch (APIs.Memory.ReadU32(newCoopContextAddr, "System Bus")) {
                                    case 0 | 1:
                                        using (var error = Error.from_string("randomizer version too old (version 5.1.4 or higher required)")) {
                                            SetError(error);
                                        }
                                        this.coopContextAddr = null;
                                        return;
                                    case 2:
                                        break; // supported, but no MW_SEND_OWN_ITEMS support
                                    case 3:
                                        APIs.Memory.WriteU8(newCoopContextAddr + 0x0a, 1, "System Bus"); // enable MW_SEND_OWN_ITEMS for server-side tracking
                                        break;
                                    case 4:
                                        APIs.Memory.WriteU8(newCoopContextAddr + 0x0a, 1, "System Bus"); // enable MW_SEND_OWN_ITEMS for server-side tracking
                                        var newFileHash = APIs.Memory.ReadByteRange(newCoopContextAddr + 0x0814, 5, "System Bus");
                                        if (this.client != null && !Enumerable.SequenceEqual(this.fileHash, newFileHash)) {
                                            using (var res = this.client.SendFileHash(newFileHash)) {
                                                if (!res.IsOk()) {
                                                    using (var err = res.UnwrapErr()) {
                                                        SetError(err);
                                                    }
                                                }
                                            }
                                            this.fileHash = newFileHash;
                                        }
                                        break;
                                    default:
                                        using (var error = Error.from_string("randomizer version too new (please tell Fenhl that Mido's House Multiworld needs to be updated)")) {
                                            SetError(error);
                                        }
                                        this.coopContextAddr = null;
                                        return;
                                }
                                this.coopContextAddr = newCoopContextAddr;
                                this.playerID = (byte?) APIs.Memory.ReadU8(newCoopContextAddr + 0x4, "System Bus");
                                newText = $"Connected as world {this.playerID}";
                            } else {
                                this.coopContextAddr = null;
                            }
                        }
                    }
                    if (this.state.Text != newText) {
                        this.state.Text = newText;
                        PerformLayout();
                    }
                }
            }
            if (this.client != null && this.playerID != oldPlayerID) {
                using (var res = this.client.SetPlayerID(this.playerID)) {
                    if (res.IsOk()) {
                        UpdateUI(this.client);
                    } else {
                        using (var err = res.UnwrapErr()) {
                            SetError(err);
                        }
                    }
                }
            }
        }

        private void SyncPlayerNames() {
            var oldPlayerName = this.playerName;
            if (this.playerID == null) {
                this.playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
            } else {
                if (Enumerable.SequenceEqual(APIs.Memory.ReadByteRange(0x0020 + 0x1c, 6, "SRAM"), new List<byte>(Encoding.UTF8.GetBytes("ZELDAZ")))) {
                    // get own player name from save file
                    this.playerName = APIs.Memory.ReadByteRange(0x0020 + 0x0024, 8, "SRAM");
                    // always fill player names in co-op context (some player names may go missing seemingly at random while others stay intact, so this has to run every frame)
                    if (this.client != null && this.client.SessionState() == 4 /* Room */ && this.coopContextAddr != null) {
                        for (var world = 1; world < 256; world++) {
                            APIs.Memory.WriteByteRange(this.coopContextAddr.Value + 0x14 + world * 0x8, this.client.GetPlayerName((byte) world), "System Bus");
                        }
                    }
                } else {
                    // file 1 does not exist, reset player name
                    this.playerName = new List<byte> { 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf };
                }
            }
            if (this.client != null && !Enumerable.SequenceEqual(this.playerName, oldPlayerName)) {
                using (var res = this.client.SetPlayerName(this.playerName)) {
                    if (res.IsOk()) {
                        UpdateUI(this.client);
                    } else {
                        using (var err = res.UnwrapErr()) {
                            SetError(err);
                        }
                    }
                }
            }
        }

        private void SetError(Error error) {
            if (this.client == null) {
                using (var display = error.Display()) {
                    this.state.Text = $"error: {display.AsString()}";
                }
                using (var debug = error.Debug()) {
                    this.debugInfo.Text = $"debug info: {debug.AsString()}";
                }
                this.debugInfo.Visible = true;
                //TODO more error UI (like in iced GUIs)
                HideLobbyUI();
                HideRoomUI();
            } else {
                error.SetAsClientState(this.client);
                UpdateUI(this.client); //TODO more error UI (like in iced GUIs)
            }
        }

        private void HideLobbyUI() {
            this.rooms.Visible = false;
            this.password.Visible = false;
            this.createJoinButton.Visible = false;
            this.version.Visible = false;
        }

        private void HideRoomUI() {
            this.deleteRoomButton.Visible = false;
            this.roomOptionsButton.Visible = false;
            for (var player_idx = 0; player_idx < this.playerStates.Count; player_idx++) {
                this.playerStates[player_idx].Visible = false;
                this.kickButtons[player_idx].Visible = false;
            }
            this.otherState.Visible = false;
        }
    }
}
