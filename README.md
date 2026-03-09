# GoldSrcMovement for CS2

Welcome to **GoldSrcMovement**! This CounterStrikeSharp plugin brings back the legendary, fast-paced movement mechanics of Half-Life 1 / Counter-Strike 1.6 (the GoldSrc engine) into modern Counter-Strike 2.

Unlike other movement plugins that rely heavily on simulation commands or client-side prediction manipulation, **GoldSrcMovement** achieves Quake-style movement physics using clean, pure C# math and server-side velocity injections, guaranteeing a 100% stable and smooth experience for players without `sv_cheats`.

## 🚀 Features

- **Pure Auto-Bhop**: Hold jump to seamlessly bunnyhop. A direct Z-axis velocity injection (`301.99`) bypasses stamina penalties and engine limits natively.
- **Quake AirAcceleration**: Feel the responsive mid-air strafing of CS 1.6. Your strafe vectors are mathematically injected into the player's entity velocity on every tick.
- **Zero Landing Lag**: `VelocityModifier` is continuously clamped to `1.0`, meaning your movement speed never drops when landing from a jump.
- **Instant Ducking**: Experience the classic zero-cooldown fast crouch. The plugin forcefully overrides the `DuckAmount` and `DuckSpeed` state modifiers directly upon crouching.
- **Pre-Strafing**: Build ground velocity higher than `250` units/sec just by running in an arc.
- **Cheat-Free**: Doesn't require server cheats. Uses exactly ONE server ConVar (`sv_stepsize 18`) to recreate old edge friction physics.

## 🛠️ Installation

1. Install [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) on your CS2 server.
2. Download the latest release of `GoldSrcMovement`.
3. Drag the `GoldSrcMovement` folder into your `game/csgo/addons/counterstrikesharp/plugins/` directory.
4. Restart your server or change the map. 

You can toggle the plugin in-game using the console command `css_gsrc_toggle` (requires `@css/generic` admin permissions).

## ⚙️ Configuration

A configuration file will be auto-generated at `addons/counterstrikesharp/plugins/GoldSrcMovement/GoldSrcMovement.json`.
You can enable/disable individual features such as `AutoBhopEnabled`, `RemoveLandingLag`, `InstantDuck`, and `QuakeAirAcceleration`.

## ❤️ Support & Contact

Plugin created by **Adiru (amazingb01)**.
- **GitHub**: [Adiru3](https://github.com/Adiru3)
- **Support & Donate**: [About Me & Donate](https://adiru3.github.io/Donate/)

