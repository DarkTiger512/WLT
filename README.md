# WarlordAwajiTwitch

Lightweight MelonLoader mod scaffold for a future Twitch-driven `!adopt` flow in **Warlord: Awaji**.

## Current flow

`Twitch message -> CommandRouter -> AdoptCommand -> AdoptionService -> IGameIntegration`

`!adopt` currently stores a simple adoption record and forwards the request to a game integration stub. No real in-game soldier creation is implemented yet.

## Structure

- `ModMain.cs` — MelonLoader entry point
- `Twitch/` — Twitch client abstractions and message models
- `Commands/` — command routing and the `AdoptCommand`
- `Game/` — `IGameIntegration` plus a stub `GameIntegration`
- `Adoption/` — adoption records, service, and JSON persistence
- `Config/` — Twitch/channel configuration model
- `tests/` — basic xUnit coverage for command routing and adoption persistence

## Development

```bash
dotnet build WarlordAwajiTwitch.csproj
dotnet test tests/WarlordAwajiTwitch.Tests.csproj
```
