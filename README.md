# Ketameri Translator

Ketameri Translator is a .NET-based tool that allows users to translate standard text into the Ketameri script—a fictional, "ancient alien" looking language—and vice versa. The project includes both a core translation library and a cross-platform user interface built with .NET MAUI.

## 🚀 Features

- **Text-to-Image Translation**: Convert English text into stylized Ketameri glyphs rendered as images.
- **Image-to-Text Translation**: Reverse translate images of Ketameri script back into readable text.
- **Custom Styling**: Support for custom font colors, highlight colors, and background images.
- **Right-to-Left Reading**: The Ketameri script is designed to be read from right to left, mimicking scripts like Hebrew.
- **Cross-Platform UI**: A .NET MAUI application that brings the translator to Android, iOS, Windows, and MacCatalyst.

## 🛠️ Technical Overview

The project is architected as a series of specialized modules:

- **`Phonetics`**: Handles the mapping between characters and their phonetic representations in Ketameri.
- **`Tokenizer`**: Manages the grouping of characters into tokens for the translation process.
- **`Graphic` & `Glyph`**: Handles the rendering of visual symbols using **SkiaSharp**.
- **`LineJoinerSplitter` & `PageJoinerSpliter`**: Manages the layout of translated text, handling how glyphs are joined into lines and how lines are arranged on a page.
- **`SymbConvert`**: The high-level API that orchestrates the translation and reverse translation workflows.
- **`Symbs`**: The .NET MAUI frontend providing a user-friendly interface for the translation engine.

## 💻 Technologies Used

- **Language**: C#
- **Framework**: .NET / .NET MAUI
- **Graphics Engine**: [SkiaSharp](https://github.com/mono/SkiaSharp)
- **License**: MIT

## 📂 Project Structure

```text
├── Glyph/              # Glyph definitions and rendering
├── Graphic/            # Image caching and graphic utilities
├── LineJoinerSplitter/  # Logic for assembling glyphs into lines
├── PageJoinerSpliter/   # Logic for assembling lines into pages
├── Phonetics/          # Character to Ketameri mapping
├── Recognizer/         # Logic for recognizing glyphs from images
├── Resources/          # Assets, glyph data, and sample texts
├── Symbol/             # Symbol type definitions
├── SymbConvert/        # Main translation API
├── SymbProcess/        # Translation processing utilities
├── Tokenizer/          # Text tokenization logic
└── Symbs/              # .NET MAUI Application UI
```

## ⚖️ License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## ✍️ Author

**Charles Timothy Montgomery** ([@chazz_the_intrepid](https://github.com/chazzofalf))

---

*Note: This project is intended for entertainment purposes. Have fun, cause a bit of mischief, and confuse your friends with ancient alien messages!*