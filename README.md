<h1 align="center">Better Farming</h1>

<p align="center">
  Faster farming interactions for <a href="https://store.steampowered.com/app/3024130/">PAO PAO</a>, without speeding up animations or changing crop rules.
</p>

<p align="center">
  <a href="https://store.steampowered.com/app/3024130/"><img alt="PAO PAO" src="https://img.shields.io/badge/PAO%20PAO-Steam%203024130-1b2838?logo=steam&logoColor=white"></a>
  <img alt="Version 1.0.0" src="https://img.shields.io/badge/version-1.0.0-2563eb">
  <img alt="BepInEx 6 IL2CPP" src="https://img.shields.io/badge/BepInEx-6%20IL2CPP-5c2d91">
  <a href="LICENSE"><img alt="MIT License" src="https://img.shields.io/badge/license-MIT-16a34a"></a>
</p>

## Features

| Action | Default behavior |
|---|---:|
| Till soil | 2 full digging cycles |
| Harvest a mature crop | 2 full harvesting cycles |
| Clear a dead plant | 2 full clearing cycles |
| Remove a planted crop | 3 full digging cycles |
| Water a garden bed | 5× original progress |

## Install

1. Install [BepInEx 6 Unity IL2CPP Windows x64](https://builds.bepinex.dev/projects/bepinex_be) in the PAO PAO game directory.
2. Start and close PAO PAO once so BepInEx can generate its IL2CPP interop assemblies.
3. Download `BetterFarming.dll` from [Releases](https://github.com/Satoheii/PaoPao-Better-Farming/releases) and copy it to `BepInEx\plugins`.
4. Start PAO PAO. The configuration file will be created at `BepInEx\config\com.satohei.paopao.betterfarming.cfg`.

## Configuration

| Setting | Default | Range | Description |
|---|---:|---:|---|
| `Enabled` | `true` | `true` / `false` | Enables all farming adjustments. |
| `TillingAnimationCycles` | `2` | `1`–`20` | Digging cycles required to till soil. |
| `HarvestAnimationCycles` | `2` | `1`–`20` | Harvesting cycles required to collect a mature crop. |
| `ClearingAnimationCycles` | `2` | `1`–`20` | Clearing cycles required to remove a dead plant. |
| `CropClearingAnimationCycles` | `3` | `1`–`20` | Digging cycles required to remove a planted crop. |
| `WaterSpeedMultiplier` | `5.0` | `0.1`–`20.0` | Watering progress relative to the original game. |

Edit the configuration while the game is closed, then restart the game.

## Compatibility and Help

Tested with PAO PAO Steam build `26220223`, Unity `6000.3.12f1`, IL2CPP metadata `39`, and plugin version `1.0.0`.

PAO PAO is an Early Access game, so updates may require a new plugin build. If the plugin does not load or has no effect:

- Confirm that the DLL is directly inside `BepInEx\plugins` and `Enabled = true`.
- Start the game once after an update so BepInEx can refresh its interop assemblies.
- Check `BepInEx\LogOutput.log` for `Better Farming 1.0.0 loaded.` or an error.

Report problems through [GitHub Issues](https://github.com/Satoheii/PaoPao-Better-Farming/issues) with the game build, BepInEx and plugin versions, configuration, reproduction steps, and relevant log lines. Remove personal information from logs before posting.

## Uninstall

Close the game, delete `BepInEx\plugins\BetterFarming.dll`, and optionally delete its configuration file.

## Building from source

Install the .NET 6 SDK or newer, install BepInEx 6 Unity IL2CPP in a local copy of PAO PAO, and start the game once. The project uses local references from `BepInEx\core` and `BepInEx\interop`; they are not NuGet packages.

Pass the game directory containing `PaoPao.exe` when building:

```bat
dotnet build BetterFarming.csproj --configuration Release -p:PaoPaoGameDir="C:\Program Files (x86)\Steam\steamapps\common\PaoPao"
```

Alternatively, set `PAOPAO_GAME_DIR`:

```powershell
$env:PAOPAO_GAME_DIR = 'C:\Program Files (x86)\Steam\steamapps\common\PaoPao'
dotnet build BetterFarming.csproj --configuration Release
```

`PaoPaoGameDir` takes priority over `PAOPAO_GAME_DIR`. The plugin is written to `bin\Release\BetterFarming.dll`. A `Missing build reference` error means the game directory, BepInEx installation, or generated interop assemblies are incomplete.

The repository does not include, download, or copy BepInEx or generated game assemblies. Do not redistribute those dependencies.

## License

[MIT](LICENSE). This is an unofficial fan-made mod and is not affiliated with or endorsed by ArtDock or Dreland Enterprises.
