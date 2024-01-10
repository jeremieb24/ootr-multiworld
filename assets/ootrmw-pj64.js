const TCP_PORT = 24818;
const MW_FRONTEND_PROTO_VERSION = 4;
const DEFAULT_PLAYER_NAME = [0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf, 0xdf];
const SRAM_START = 0xA8000000;

var fileHash = null;
var playerID = null;
var playerName = null;
var playerNames = [
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0xdf, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x05],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x01, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x02, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x01],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x03, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x07],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x04, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x05, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x03],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x06, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x07, 0x09],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x08, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x05],
    [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xc9, 0xd6, 0x09, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x00, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x01],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x01, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x07],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x02, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x03, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x03],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x04, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x05, 0x09],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x06, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x05],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x07, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x08, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x01],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x01, 0x09, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x07],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x00, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x01, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x03],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x02, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x03, 0x09],
    [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x05], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x06], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x07], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x08], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x04, 0x09], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x00], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x01], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x02], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x03], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x04], [0xba, 0xd0, 0xc5, 0xdd, 0xd6, 0x02, 0x05, 0x05],
];
var progressiveItems = [
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
];
var itemQueue = [];
var normalGameplay = false;
var progressiveItemsEnable = false;
var potsanity3 = false;

function handle_data(sock, state, buf) {
    var newBuf = new Buffer(state.readBuf.length + buf.length);
    state.readBuf.copy(newBuf);
    buf.copy(newBuf, state.readBuf.length);
    state.readBuf = newBuf;
    if (!state.versionChecked && state.readBuf.length >= 1) {
        // check to make sure the server's protocol version matches ours
        if (state.readBuf[0] != MW_FRONTEND_PROTO_VERSION) {
            sock.close();
            throw 'version mismatch';
        }
        console.log('Connected to multiworld app');
        state.versionChecked = true;
        state.readBuf = state.readBuf.slice(1);
    }
    var eof = false;
    while (state.versionChecked && !eof) {
        if (state.remainingItems > 0) {
            if (state.readBuf.length >= 2) {
                itemQueue.push(state.readBuf.readUInt16BE(0));
                state.readBuf = state.readBuf.slice(2);
                state.remainingItems -= 1;
            } else {
                eof = true;
            }
        } else {
            if (state.readBuf.length >= 1) {
                switch (state.readBuf.readUInt8(0)) {
                    case 0: // ServerMessage::ItemQueue
                        if (state.readBuf.length >= 9) {
                            if (state.readBuf.readUInt32BE(1) != 0) {
                                sock.close();
                                throw 'more than u32::MAX_VALUE items';
                            }
                            itemQueue = [];
                            state.remainingItems = state.readBuf.readUInt32BE(5);
                            state.readBuf = state.readBuf.slice(9);
                        } else {
                            eof = true;
                        }
                        break;
                    case 1: // ServerMessage::GetItem
                        state.remainingItems = 1;
                        state.readBuf = state.readBuf.slice(1);
                        break;
                    case 2: // ServerMessage::PlayerName
                        if (state.readBuf.length >= 10) {
                            playerNames[state.readBuf.readUInt8(1)] = state.readBuf.slice(2, 10);
                            state.readBuf = state.readBuf.slice(10);
                        } else {
                            eof = true;
                        }
                        break;
                    case 3: // ServerMessage::ProgressiveItems
                        if (state.readBuf.length >= 6) {
                            progressiveItems[state.readBuf.readUInt8(1)] = state.readBuf.readUInt32BE(2);
                            state.readBuf = state.readBuf.slice(6);
                        } else {
                            eof = true;
                        }
                        break;
                    default:
                        sock.close();
                        throw 'unknown server command';
                }
            } else {
                eof = true;
            }
        }
    }
}

function handle_frame(write, error) {
    // read player ID
    var zeldaz_rdram = mem.getblock(ADDR_ANY_RDRAM.start + 0x11a5d0 + 0x1c, 6);
    var coopContextAddr = null;
    if (zeldaz_rdram[0] == 0x5a && zeldaz_rdram[1] == 0x45 && zeldaz_rdram[2] == 0x4c && zeldaz_rdram[3] == 0x44 && zeldaz_rdram[4] == 0x41 && zeldaz_rdram[5] == 0x5a) {
        var randoContextAddr = mem.u32[ADDR_ANY_RDRAM.start + 0x1c6e90 + 0x15d4];
        if (randoContextAddr >= 0x80000000 && randoContextAddr != 0xffffffff) {
            var newCoopContextAddr = mem.u32[randoContextAddr];
            if (newCoopContextAddr >= 0x80000000 && newCoopContextAddr != 0xffffffff) {
                var coopContextVersion = mem.u32[newCoopContextAddr];
                if (coopContextVersion < 2) {
                    return error('randomizer version too old (version 5.1.4 or higher required)');
                }
                if (coopContextVersion > 7) {
                    return error("randomizer version too new (version " + mem.u32[newCoopContextAddr] + "; please tell Fenhl that Mido's House Multiworld needs to be updated)");
                }
                if (coopContextVersion == 7) {
                    var branchID = mem.u8[0xb000001c];
                    if (branchID == 0x45 || branchID == 0xfe) {
                        // on Dev-Rob and dev-fenhl, version 7 is https://github.com/OoTRandomizer/OoT-Randomizer/pull/2069
                        potsanity3 = true;
                    } else {
                        return error("randomizer version too new (version " + mem.u32[newCoopContextAddr] + "; please tell Fenhl that Mido's House Multiworld needs to be updated)");
                    }
                } else {
                    potsanity3 = false;
                }
                if (coopContextVersion >= 3) {
                    mem.u8[newCoopContextAddr + 0x000a] = 1; // enable MW_SEND_OWN_ITEMS for server-side tracking
                }
                if (coopContextVersion >= 4) {
                    var newFileHash = mem.getblock(newCoopContextAddr + 0x0814, 5);
                    if (fileHash === null || newFileHash != fileHash) {
                        const fileHashPacket = new ArrayBuffer(6);
                        var fileHashPacketView = new DataView(fileHashPacket);
                        fileHashPacketView.setUint8(0, 4); // message: file hash changed
                        for (var c = 0; c < 5; c++) {
                            fileHashPacketView.setUint8(c + 1, newFileHash[c]);
                        }
                        write(new Buffer(new Uint8Array(fileHashPacket)));
                        fileHash = newFileHash;
                    }
                }
                if (coopContextVersion >= 5) {
                    progressiveItemsEnable = true;
                    mem.u8[newCoopContextAddr + 0x000b] = 1; // MW_PROGRESSIVE_ITEMS_ENABLE
                } else {
                    progressiveItemsEnable = false;
                }
                if (mem.u32[ADDR_ANY_RDRAM.start + 0x11a5d0 + 0x135c] == 0) { // game mode == gameplay
                    if (!normalGameplay) {
                        const saveDataPacket = new ArrayBuffer(0x1451);
                        var saveDataPacketView = new DataView(saveDataPacket);
                        saveDataPacketView.setUint8(0, 3); // message: save data loaded
                        var saveDataByteArray = new Uint8Array(saveDataPacket);
                        saveDataByteArray.set(new Uint8Array(mem.getblock(ADDR_ANY_RDRAM.start + 0x11a5d0, 0x1450)), 1);
                        write(new Buffer(saveDataByteArray));
                        normalGameplay = true;
                    }
                } else {
                    normalGameplay = false;
                }
                coopContextAddr = newCoopContextAddr;
                var newPlayerID = mem.u8[newCoopContextAddr + 0x4];
                if (newPlayerID !== playerID) {
                    const playerIdPacket = new ArrayBuffer(2);
                    var playerIdPacketView = new DataView(playerIdPacket);
                    playerIdPacketView.setUint8(0, 0); // message: player ID changed
                    playerIdPacketView.setUint8(1, newPlayerID);
                    write(new Buffer(new Uint8Array(playerIdPacket)));
                    playerID = newPlayerID;
                    if (playerName !== null) {
                        playerNames[playerID] = playerName;
                    }
                }
            } else {
                normalGameplay = false;
            }
        } else {
            normalGameplay = false;
        }
    } else {
        normalGameplay = false;
    }
    // sync player names
    var newPlayerName;
    var zeldaz_sram = mem.getblock(SRAM_START + 0x0020 + 0x1c, 6);
    if (playerID === null) {
        // player ID null, setting default player name
        newPlayerName = DEFAULT_PLAYER_NAME;
    } else if (zeldaz_sram[0] == 0x5a && zeldaz_sram[1] == 0x45 && zeldaz_sram[2] == 0x4c && zeldaz_sram[3] == 0x44 && zeldaz_sram[4] == 0x41 && zeldaz_sram[5] == 0x5a) {
        // get own player name from save file
        newPlayerName = mem.getblock(SRAM_START + 0x0020 + 0x0024, 8);
        // always fill player names in co-op context (some player names may go missing seemingly at random while others stay intact, so this has to run every frame)
        if (coopContextAddr !== null) {
            for (var world = 0; world < 256; world++) {
                for (var c = 0; c < 8; c++) {
                    mem.u8[coopContextAddr + 0x0014 + world * 0x8 + c] = playerNames[world][c];
                }
                // fill progressive items of other players
                if (progressiveItemsEnable) {
                    mem.u32[coopContextAddr + 0x081c + world * 0x4] = progressiveItems[world];
                }
            }
        }
    } else {
        // file 1 does not exist, reset player name
        newPlayerName = DEFAULT_PLAYER_NAME;
    }
    var playerNameChanged = false;
    if (playerName === null) {
        playerNameChanged = true;
    } else {
        for (var c = 0; c < 8; c++) {
            if (newPlayerName[c] != playerName[c]) {
                playerNameChanged = true;
                break;
            }
        }
    }
    if (playerNameChanged) {
        const playerNamePacket = new ArrayBuffer(9);
        var playerNamePacketView = new DataView(playerNamePacket);
        playerNamePacketView.setUint8(0, 1); // message: player name changed
        for (var c = 0; c < 8; c++) {
            playerNamePacketView.setUint8(c + 1, newPlayerName[c]);
        }
        write(new Buffer(new Uint8Array(playerNamePacket)));
        playerName = newPlayerName;
    }
    if (playerID !== null && coopContextAddr !== null) {
        // send item
        var outgoingKeyHi = 0;
        var outgoingKeyLo = 0;
        if (potsanity3) {
            outgoingKeyHi = mem.u32[coopContextAddr + 0x0c1c];
            outgoingKeyLo = mem.u32[coopContextAddr + 0x0c20];
        } else {
            outgoingKeyLo = mem.u32[coopContextAddr + 0xc];
        }
        if (outgoingKeyHi != 0 || outgoingKeyLo != 0) {
            var kind = mem.u16[coopContextAddr + 0x10];
            var player = mem.u8[coopContextAddr + 0x13];
            if (outgoingKeyHi = 0 && outgoingKeyLo == 0xff05ff) {
                //Debug($"P{this.playerID}: Found an item {kind} for player {player} sent via network, ignoring");
            } else {
                //Debug($"P{this.playerID}: Found {outgoingKey}, an item {kind} for player {player}");
                const sendItemPacket = new ArrayBuffer(12);
                var sendItemPacketView = new DataView(sendItemPacket);
                sendItemPacketView.setUint8(0, 2); // message: send item
                sendItemPacketView.setUint32(1, outgoingKeyHi);
                sendItemPacketView.setUint32(5, outgoingKeyLo);
                sendItemPacketView.setUint16(9, kind);
                sendItemPacketView.setUint8(11, player);
                write(new Buffer(new Uint8Array(sendItemPacket)));
            }
            mem.u16[coopContextAddr + 0x10] = 0;
            mem.u16[coopContextAddr + 0x12] = 0;
            if (potsanity3) {
                mem.u32[coopContextAddr + 0x0c1c] = 0;
                mem.u32[coopContextAddr + 0x0c20] = 0;
            } else {
                mem.u32[coopContextAddr + 0xc] = 0;
            }
        }
        // receive item
        var stateLogo = mem.u32[ADDR_ANY_RDRAM.start + 0x11f200];
        var stateMain = mem.s8[ADDR_ANY_RDRAM.start + 0x11b92f];
        var stateMenu = mem.s8[ADDR_ANY_RDRAM.start + 0x1d8dd5];
        var currentScene = mem.u8[ADDR_ANY_RDRAM.start + 0x1c8545];
        if (
            stateLogo != 0x802c5880 && stateLogo != 0 && stateMain != 1 && stateMain != 2 && stateMenu == 0 && (
                (currentScene < 0x2c || currentScene > 0x33) && currentScene != 0x42 && currentScene != 0x4b // don't receive items in shops to avoid a softlock when buying an item at the same time as receiving one
            )
        ) {
            if (mem.u16[coopContextAddr + 0x8] == 0) {
                var internalCount = mem.u16[ADDR_ANY_RDRAM.start + 0x11a5d0 + 0x90];
                var externalCount = itemQueue.length;
                if (internalCount < externalCount) {
                    var item = itemQueue[internalCount];
                    //Debug($"P{this.playerID}: Received an item {item} from another player");
                    mem.u16[coopContextAddr + 0x8] = item;
                    mem.u16[coopContextAddr + 0x6] = item == 0xca ? (playerID == 1 ? 2 : 1) : playerID;
                } else if (internalCount > externalCount) {
                    console.log('warning: gap in received items: internal count is ' + internalCount + ' but external queue is ' + itemQueue);
                }
            }
        }
    }
}

if (typeof PJ64_JSAPI_VERSION === 'undefined') {
    // first edition (PJ64 2.4 to 3.x) (https://htmlpreview.github.io/?https://github.com/project64/project64/blob/5d0d9927b1fd91e9647eb799f68e132804de924e/apidoc.htm)
    var drawCallback = null;
    var sock = new Socket();
    var state = {
        versionChecked: false,
        remainingItems: 0,
        readBuf: new Buffer(0),
    };
    sock.on('close', function() {
        if (drawCallback !== null) {
            events.remove(drawCallback);
            drawCallback = null;
        }
        console.log('Connection to multiworld app lost');
        throw 'connection to multiworld app lost';
    });
    // This version of PJ64's API doesn't have any way to detect a failed connection
    // (no error event handler, no setTimeout), so we have to preemptively give troubleshooting info.
    console.log('Attempting to connect to multiworld app…');
    console.log("This should take less than 5 seconds. If you don't see “connected” below, make sure the app is running.");
    console.log('If you need help, you can ask in #setup-support on the OoT Randomizer Discord or in #general on the OoTR MW Tournament Discord. Feel free to ping @fenhl.');
    sock.connect({host: "127.0.0.1", port: TCP_PORT}, function() {
        console.log('Connected.');
        const handshake = new ArrayBuffer(1);
        new DataView(handshake).setUint8(0, MW_FRONTEND_PROTO_VERSION);
        sock.write(new Buffer(new Uint8Array(handshake)), function() {
            sock.on('data', function(buf) {
                handle_data(sock, state, new Buffer(buf));
            });
            drawCallback = events.ondraw(function() {
                handle_frame(sock.write, function(error_msg) {
                    sock.close();
                    throw error_msg;
                });
            });
        });
    });
} else if (PJ64_JSAPI_VERSION === 'jsapi-2') {
    // second edition (PJ64 4.0+) (https://htmlpreview.github.io/?https://github.com/project64/project64/blob/develop/JS-API-Documentation.html)
    function file_exists(path) {
        return exec('IF EXIST "' + path.replace('"', '""') + '" echo exists') == 'exists\r\n';
    }

    var appdata = exec('echo %LOCALAPPDATA%').slice(0, -2);
    if (file_exists(appdata + '\\Fenhl\\OoTR Multiworld\\cache\\gui.exe')) {
        var server = new Server();
        var sockets = [];
        server.on('connection', function(sock) {
            var sock_idx = sockets.push(sock) - 1;
            var state = {
                versionChecked: false,
                remainingItems: 0,
                readBuf: new Buffer(0),
            };
            sock.on('close', function() {
                sockets[sock_idx] = null;
                console.log('Connection to multiworld app lost');
            });
            const handshake = new ArrayBuffer(1);
            new DataView(handshake).setUint8(0, MW_FRONTEND_PROTO_VERSION);
            sock.write(new Buffer(new Uint8Array(handshake)), function() {
                sock.on('data', function(buf) {
                    handle_data(sock, state, buf);
                });
            });
        });
        server.listen(TCP_PORT, '127.0.0.1'); //TODO test if port 0 is supported here, pass `server.port` to gui.exe if it is
        exec('PowerShell -Command "Start-Process \'' + appdata.replace('"', '""').replace("'", "''") + '\\Fenhl\\OoTR Multiworld\\cache\\gui.exe\' pj64v4"');
        setInterval(function() {
            if (sockets.length > 0) {
                handle_frame(function(buf) {
                    for (var i = 0; i < sockets.length; i++) {
                        if (sockets[i] !== null) {
                            sockets[i].write(buf);
                        }
                    }
                }, function(error_msg) {
                    sock.close();
                    throw error_msg;
                });
            }
        }, 50);
    } else {
        throw 'The companion app seems to be missing. This can happen if you upgraded Project64 from version 3 to version 4. Try running the installer again.';
    }
} else {
    throw "Project64 version too new (API version" + PJ64_JSAPI_VERSION + "; please tell Fenhl that Mido's House Multiworld needs to be updated)";
}
