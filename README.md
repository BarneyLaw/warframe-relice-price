# Warframe Relic Price Overlay

A real-time Windows overlay that automatically displays [Warframe Market](https://warframe.market/) prices for relic rewards while you play. When a relic is cracked open at the end of a mission, the overlay reads the reward screen using OCR and shows the current lowest market price for each item — so you can make an informed pick without alt-tabbing.

> **Status:** Work-in-progress 🚧

## Features

- **Automatic reward detection** — continuously monitors for the reward selection screen and triggers price lookups when it appears.
- **OCR-powered text extraction** — uses [Tesseract](https://github.com/charlesw/tesseract) to read item names directly from the game screen.
- **Fuzzy matching** — leverages [FuzzySharp](https://github.com/JakeBayer/FuzzySharp) (Levenshtein distance) to correct OCR inaccuracies and reliably match items to the known reward pool.
- **Live market prices** — fetches the current lowest listing from the [Warframe Market API](https://warframe.market/).
- **Transparent in-game overlay** — a borderless WPF window that sits on top of Warframe, displaying prices right next to each reward.
- **Focus-aware** — the overlay only appears while Warframe is in the foreground; it hides automatically when you switch away.

## Tech Stack

| Component | Technology |
|-----------|------------|
| Language | C# (.NET 8.0) |
| UI | WPF (Windows Presentation Foundation) |
| OCR | Tesseract 5.2.0 |
| Fuzzy matching | FuzzySharp 2.0.2 |
| Market data | Warframe Market REST API (v2) |

## Prerequisites

- **Windows 10/11**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/BarneyLaw/warframe-relice-price.git
   cd warframe-relice-price
   ```

2. **Build**

   ```bash
   dotnet build
   ```

3. **Run**

   ```bash
   dotnet run --project warframe-relice-price
   ```

4. **Launch Warframe** and open a relic at the end of a mission — the overlay will appear with live prices for each reward.

## How It Works

1. **Process tracking** — the app watches for the Warframe process and tracks its window position and focus state.
2. **Screen capture** — every 750 ms it captures a small region of the screen where the reward UI appears.
3. **Reward detection** — Tesseract OCR checks whether the captured region contains the reward selection screen. Four consecutive positive detections are required before proceeding (to avoid false positives).
4. **Text extraction** — once confirmed, all reward text boxes are extracted in parallel.
5. **Fuzzy matching** — each extracted string is matched against a known pool of relic reward names using Levenshtein distance.
6. **Price lookup** — the Warframe Market API is queried concurrently for each matched item, returning the current lowest sell price.
7. **Overlay rendering** — prices are drawn on a transparent WPF canvas positioned over the game window.
8. **Exit detection** — when the reward screen is no longer detected the overlay returns to its idle state.

## Project Structure

```
warframe-relice-price/
├── Core/                  # Application controller & state machine
├── OCRVision/             # Tesseract wrapper, screen capture, reward detection
├── OverlayUI/             # WPF overlay window & rendering
├── Rewards/
│   ├── Data/              # Canonical reward item pool
│   ├── Models/            # Fuzzy matching & item models
│   ├── Processing/        # OCR → match → price pipeline
│   └── Services/          # Warframe Market API client
├── WarframeTracker/       # Warframe process & window tracking
└── Utils/                 # Win32 interop, logging, global state
```

## Contact

If you have questions or run into issues, feel free to reach out:

- leifsenlaw993@gmail.com
- williamjliow@gmail.com

