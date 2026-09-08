# Ketameri Text Detection: Assumptions, Process, and Design

## Assumptions
- **Glyph Geometry**: All Ketameri glyphs are strictly square.
- **Reading Direction**: Text is read and processed from right to left (RTL).
- **Data Encoding**: Pixel data is encoded in the least significant bit (LSB) of the Red channel. A pixel is considered "active" (bit 1) if `(Red & 1) == 0`.
- **Normalization**: Any glyph image can be normalized to a 9x9 grid for recognition purposes without loss of identity.
- **Spacing**: In a line of text, glyphs are separated by a padding width equal to 1/9th of the glyph height.

## Process

### 1. Line Splitting
- **Segmentation**: The `LineJoinerSplitter` divides a line bitmap into individual square glyphs.
- **Sizing**: Using the line height as the base `fontsize`, it extracts regions of `fontsize` x `fontsize` at intervals of `fontsize + (fontsize / 9)`.
- **Ordering**: The resulting sequence of glyphs is reversed to convert the visual RTL order into a logical processing order.

### 2. Glyph Recognition
The `Recognizer` takes a square bitmap and performs the following:
- **Downsampling**: The image is scaled to a 9x9 `SKBitmap` (`borderless_mini`).
- **Classification**: The system checks the pixel at coordinate `(2, 1)`. If the bit is active, the glyph is processed as a **Number**; otherwise, it is a **Letter**.
- **Number Extraction**:
    - The system scans specific clusters of pixels in the 9x9 grid.
    - It calculates a numerical value using a base-3 accumulation logic based on the state of these clusters.
- **Letter Extraction**:
    - The system samples a predefined set of 12 coordinates:
      `(6,0), (8,2), (8,6), (6,8), (2,8), (0,6), (0,2), (2,0), (4,2), (6,4), (4,6), (2,4)`
    - These bits are aggregated into a binary value (`p * 2 + c`).
    - This unique identifier is used to retrieve the corresponding `CachedGraphic`.

## Design

### Grid-Based Encoding
The core design relies on a spatial bit-mapping system. Rather than using traditional OCR or pattern matching, Ketameri glyphs act as "QR-like" barcodes where specific coordinates in a 9x9 grid map directly to identity values.

### Resolution Independence
By forcing a scale-down to 9x9 pixels before analysis, the recognizer is decoupled from the actual resolution of the input image, provided the input is square.

### RTL Architecture
The right-to-left nature of the language is handled at the earliest possible stage (during line splitting), ensuring that subsequent tokenization and translation steps can operate on a standard linear sequence.
