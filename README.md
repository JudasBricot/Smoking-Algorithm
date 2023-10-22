# Generative Art with the Smoking Algorithm

This project implements the Smoking algorithm, a fascinating generative art method designed to create images that incorporate all 16,777,216 (256^3) colors in a single composition.

## The Smoking Algorithm

The Smoking Algorithm is an innovative approach to generative art. It begins by selecting a comprehensive palette of colors that will be used in the final image. These colors are then ordered according to a specified sequence. To keep track of available neighboring pixels, a neighbor list is created, containing all the adjacent pixels of colored ones.

The process starts with an empty canvas, and in each step, a vacant pixel within the neighbor list is assigned a color, which is subsequently removed from the list. The algorithm begins by choosing a starting pixel, with the first color from the ordered list placed at this pixel and removed from the color list.

In each subsequent step:

1. The algorithm selects the next color from the color list and removes it.
2. It evaluates all available neighbors in the neighbor list to find the pixel where the chosen color fits best. This decision is made mathematically by minimizing the average distance of the color to all direct neighbors of the candidate pixel.

## Results

Here are some stunning results achieved using this algorithm, with colors sorted by hue. The starting point for each image is the center of the canvas:

### "Lemon"
![Lemon](media/results/Lemon.jpg)

### "Plants"
![Plants](media/results/Plants.png)
![Plants GIF](media/results/Plants.gif)

### "Red"
![Red](media/results/Red.jpg)

## Installation

To use this project, you'll need .NET Core. Build and run the project with the following commands:

```bash
dotnet build
dotnet run
```

If you want to create videos from the generated images, make sure you have FFmpeg installed and added to your system's environment variables.