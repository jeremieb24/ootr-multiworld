#![deny(rust_2018_idioms, unused, unused_crate_dependencies, unused_import_braces, unused_lifetimes, unused_qualifications, warnings)]
#![forbid(unsafe_code)]

use {
    std::convert::Infallible as Never,
    async_proto::Protocol as _,
    itertools::Itertools as _,
    tokio::net::TcpStream,
    multiworld::{
        LobbyClientMessage,
        ServerMessage,
    },
};

#[derive(Debug, thiserror::Error)]
enum ParseApiKeyError {
    #[error(transparent)] Int(#[from] std::num::ParseIntError),
    #[error("API key had an odd number of characters")]
    ExtraNybble,
    #[error("API key had wrong length")]
    VecLen(Vec<u8>),
}

impl From<Vec<u8>> for ParseApiKeyError {
    fn from(v: Vec<u8>) -> Self {
        Self::VecLen(v)
    }
}

fn parse_api_key(s: &str) -> Result<[u8; 32], ParseApiKeyError> {
    let mut tuples = s.chars().tuples();
    let key = (&mut tuples).map(|(hi, lo)| u8::from_str_radix(&format!("{hi}{lo}"), 16)).try_collect::<_, Vec<_>, _>()?.try_into()?;
    if tuples.into_buffer().next().is_some() { return Err(ParseApiKeyError::ExtraNybble) }
    Ok(key)
}

#[derive(clap::Parser)]
#[clap(version)]
struct Args {
    id: u64,
    #[clap(parse(try_from_str = parse_api_key))]
    api_key: [u8; 32],
}

#[derive(Debug, thiserror::Error)]
enum Error {
    #[error(transparent)] Client(#[from] multiworld::ClientError),
    #[error(transparent)] Io(#[from] tokio::io::Error),
    #[error(transparent)] Read(#[from] async_proto::ReadError),
    #[error(transparent)] Write(#[from] async_proto::WriteError),
    #[error("server error: {0}")]
    Server(String),
}

#[wheel::main(debug)]
async fn main(Args { id, api_key }: Args) -> Result<Never, Error> {
    let mut tcp_stream = TcpStream::connect((multiworld::ADDRESS_V4, multiworld::PORT)).await?;
    for room_name in multiworld::handshake(&mut tcp_stream).await? {
        println!("initial room: {room_name:?}");
    }
    LobbyClientMessage::Login { id, api_key }.write(&mut tcp_stream).await?;
    loop {
        match ServerMessage::read(&mut tcp_stream).await? {
            ServerMessage::Error(msg) => return Err(Error::Server(msg)),
            ServerMessage::NewRoom(room_name) => println!("new room: {room_name:?}"),
            ServerMessage::AdminLoginSuccess { active_connections } => {
                println!("admin login success, active connections:");
                for (room_name, count) in active_connections {
                    println!("{room_name:?}: {count}");
                }
                println!("end active connections");
            }
            ServerMessage::EnterRoom { .. } |
            ServerMessage::PlayerId(_) |
            ServerMessage::ResetPlayerId(_) |
            ServerMessage::ClientConnected |
            ServerMessage::PlayerDisconnected(_) |
            ServerMessage::UnregisteredClientDisconnected |
            ServerMessage::PlayerName(_, _) |
            ServerMessage::ItemQueue(_) |
            ServerMessage::GetItem(_) => unreachable!(),
        }
    }
}
