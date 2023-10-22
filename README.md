# Introduction

This is my attempt as implementing the Smoking algorithm, a generative art method whose target was to find the best way to fit all 256^3 colors in a single image.

# The Smoking Algorithm

The principle of the algorithm is the following. It starts by choosing all colors that will be used in the image, the colors are sorted in a list according to a defined order.
To keep track of available neighbors, a neighbor list is created, it contains all pixels that are adjacent to colored pixel.
We start with an empty image, and at each step, we set the color of an empty pixel that is in the neighbor list, and we remove it from the list. 
The algorithm first choose a pixel to start, the first color of the list is placed at this pixel, adn removed from the color list.

At each step, we pick the first color of the color list, we remove it from the list. Then, we will check for all available neighbors in the neighbor list what is the pixel p where the choosed color fit the most. Mathematically, we minimize the average distance of the color to all direct neighbors of the candidate pixel p.

# Results

Here are some results where the color are sorted by Hue, and the start point is the center of the image :
## "Lemon"
![Screenshot](media/results/Lemon.jpg)
## "Plants"
![Screenshot](media/results/Plants.png)
![til](media/results/Plants.gif)
## "Red"
![Screenshot](media/results/Red.jpg)
