# Ketameri Text Recognition Upgrade Plan: Natural Environments

## Goal
Transition the recognition system from a "digital barcode" reader (relying on LSB encoding and perfect geometry) to a robust OCR-like system capable of recognizing hand-drawn or natural-image Ketameri text.

## Design Philosophy
The current system is "Deterministic": it checks specific pixels for specific bits. To handle natural images, we must move to "Probabilistic" recognition: analyzing shapes, contours, and intensity patterns.

---

## Proposed Design & Architecture

### 1. Image Pre-processing Pipeline
Instead of reading raw pixels, the image must be normalized to isolate the "ink" from the "background".
- **Grayscale Conversion**: Remove color noise.
- **Adaptive Thresholding**: Use algorithms (like Otsu's Method) to create a binary (black/white) image, handling non-uniform lighting.
- **Noise Reduction**: Apply Gaussian blur to smooth edges and morphological operations (Dilation/Erosion) to close gaps in hand-drawn lines.
- **Deskewing**: Detect the dominant angle of text lines and rotate the image to align it horizontally.

### 2. Dynamic Segmentation (Replacing `LineJoinerSplitter`)
Replacing fixed-width slicing with object-based detection.
- **Contour Detection**: Use connected-component labeling to find "blobs" of foreground pixels.
- **Bounding Boxes**: Calculate the minimal rectangle enclosing each blob.
- **Filtering**: Discard blobs that are too small (noise) or too large (background artifacts) based on expected glyph aspect ratios.
- **Line Clustering**: Group bounding boxes into lines based on their Y-coordinates and horizontal proximity.
- **RTL Sorting**: Sort detected glyphs in each line from right to left.

### 3. Robust Glyph Recognition (Replacing `Recognizer`)
Moving from LSB bit-checking to shape analysis.
- **Normalization**: 
    - Crop the glyph to its bounding box.
    - Pad and resize to a standard resolution (e.g., 32x32).
    - Center the glyph using the center of mass.
- **Classification Strategy (Three Tiers of Complexity)**:
    - **Tier 1 (Template Matching)**: Compare the normalized glyph against a library of "Golden Glyphs" using Mean Squared Error (MSE) or Structural Similarity Index (SSIM).
    - **Tier 2 (Feature Extraction)**: Extract key points (e.g., intersection counts, line orientations, or HOG - Histogram of Oriented Gradients) and use a k-Nearest Neighbors (k-NN) classifier.
    - **Tier 3 (Neural Network)**: Train a small Convolutional Neural Network (CNN) on synthetic data (the existing glyphs augmented with rotation, thickness variations, and noise).

---

## Implementation Steps

### Phase 1: Foundations (The "Clean" Path)
1. **Integrate an Image Processing Library**: Ensure a library like OpenCV or a similar C# wrapper (e.g., EmguCV or SkiaSharp's advanced filters) is available.
2. **Build the Binarization Tool**: Implement adaptive thresholding to convert a natural image into a high-contrast binary map.
3. **Implement Contour Detection**: replace `LineJoinerSplitter`'s fixed loops with a function that finds and returns `SKRect`s for all detected glyphs.

### Phase 2: Recognition Overhaul
4. **Develop the Normalizer**: Create a utility that takes a `SKRect` from a larger image and produces a centered, normalized 32x32 grayscale bitmap.
5. **Create a Synthetic Dataset**: Generate thousands of variations of the current glyph set (varying line thickness, adding "jitter" to lines) to act as a training/reference set.
6. **Implement Template Matching as a Baseline**: Create a recognizer that finds the "closest match" from the golden set.

### Phase 3: Intelligence & Refinement
7. **Shift to Probabilistic Scoring**: Instead of returning a single `CachedGraphic`, return a list of candidates with confidence scores.
8. **Contextual Correction**: (Optional) implement a basic "dictionary" or pattern check to suggest corrections for ambiguous glyphs.
9. **UI integration**: Update the app to allow users to "crop" the area of interest before recognition to reduce background noise.

## Comparison Summary

| Feature | Current System | Upgraded System |
| :--- | :--- | :--- |
| **Background** | Must be uniform / LSB hidden | Any (using Adaptive Thresholding) |
| **Foreground** | LSB of Red Channel | Contrast-based (Ink vs Paper) |
| **Geometry** | Perfectly square, fixed padding | Bounding-box based, variable spacing |
| **Accuracy** | 100% (Digital) / 0% (Natural) | Probabilistic (High for Natural) |
| **Method** | Coordinate Bit-Checking | Shape/Feature Analysis |
