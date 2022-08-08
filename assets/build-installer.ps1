function ThrowOnNativeFailure {
    if (-not $?) {
        throw 'Native Failure'
    }
}

#TODO make sure BizHawk is up to date

cargo build --release --package=multiworld-updater
ThrowOnNativeFailure

cargo build --release --package=multiworld-csharp
ThrowOnNativeFailure

cargo build --release --package=multiworld-bizhawk
ThrowOnNativeFailure

cargo build --release --package=multiworld-pj64-gui
ThrowOnNativeFailure

cargo build --release --package=multiworld-installer
ThrowOnNativeFailure
