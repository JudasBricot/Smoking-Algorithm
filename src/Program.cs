using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    const int WIDTH = 100;
    const int HEIGHT = 100;

    const int START_X = WIDTH / 2;
    const int START_Y = HEIGHT / 2;

    const int PICTURE_NB = 100;

    enum ColorDiffReturnMode
    {
        AVERAGE,
        MIN,
        MAX
    };
    const ColorDiffReturnMode COLOR_DIFF_RETURN_MODE = ColorDiffReturnMode.MIN;

    enum NeighborMode
    {
        SQUARE,
        CROSS
    };
    const NeighborMode NEIGHBOR_MODE = NeighborMode.CROSS;

    const bool CREATE_GIF = true;
    const bool CREATE_VIDEO = true;

    struct Point
    {
        public int x,y;
        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    static List<Point> GetNeighborCoords(Point p)
    {
        List<Point> neighbors;
        switch(NEIGHBOR_MODE)
        {
            case NeighborMode.SQUARE:
                neighbors = new List<Point>(8);

                for (int dx = -1; dx <= 1; dx++)
                {
                    if(p.x + dx == -1 || p.x + dx == WIDTH)
                    {
                        continue;
                    }

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if(p.y + dy == -1 || p.y + dy == HEIGHT || dx*dx + dy*dy == 0)
                        {
                            continue;
                        }

                        neighbors.Add(new Point(p.x + dx, p.y + dy));
                    }
                }

                return neighbors;

            case NeighborMode.CROSS:

                neighbors = new List<Point>(4);

                if(p.x != 0)
                {
                    neighbors.Add(new Point(p.x - 1, p.y));
                }
                if(p.x != WIDTH-1)
                {
                    neighbors.Add(new Point(p.x + 1, p.y));
                }
                if(p.y != 0)
                {
                    neighbors.Add(new Point(p.x, p.y - 1));
                }
                if(p.y != HEIGHT-1)
                {
                    neighbors.Add(new Point(p.x, p.y + 1));
                }
                return neighbors;
        }
    }

    static int ColDiff(Color c1, Color c2)
    {
        int r = c1.R - c2.R;
        int g = c1.G - c2.G;
        int b = c1.B - c2.B;

        return r*r + g*g + b*b;
    }

    static int CalcDiff(Color[,] pixels, Point p, Color c)
    {
        List<int> diffs = new List<int>(8);
        foreach (var neighbor in GetNeighborCoords(p))
        {
            if(!pixels[neighbor.x, neighbor.y].IsEmpty)
            {
                diffs.Add(ColDiff(c, pixels[neighbor.x, neighbor.y]));
            }
        }

        switch(COLOR_DIFF_RETURN_MODE)
        {
            case ColorDiffReturnMode.AVERAGE:
                return (int)diffs.Average();
            case ColorDiffReturnMode.MIN:
                return diffs.Min();
            case ColorDiffReturnMode.MAX:
                return diffs.Max();
        }
    }

    static Random rng = new Random();  

    public static void Shuffle(List<Color> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            Color value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    public static void CreateVideo()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "ffmpeg";

        startInfo.Arguments = "-r 24 -f image2 -s " + WIDTH + "x" + HEIGHT + " -i ./media/Pics/pic%03d.png -vcodec libx264 -crf 25  -pix_fmt yuv420p media/Video/test.mp4";

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        process.WaitForExit();
    }

    public static void Main(String[] args)
    {
        Stopwatch sw = new Stopwatch();

        Bitmap bitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format24bppRgb);
        Color[,] pixels = new Color[WIDTH, HEIGHT];

        HashSet<Point> available = new HashSet<Point>();

        GifWriter gifWriter = new GifWriter("media/Pics/result.gif", 100);

        // Initalisation de la liste des couleurs disponibles
        List<Color> availableCol = new List<Color>();
        for (int r = 0; r < 256; r++)
            for (int g = 0; g < 256; g++)
                for (int b = 0; b < 256; b++)
                    availableCol.Add(Color.FromArgb(r,g,b));
        
        // Tri de la liste des couleurs
        availableCol.Sort((c1,c2) => c1.GetHue().CompareTo(c2.GetHue()));

        // Permet de randomiser la première valeur de i pour avoir des couleurs random.
        int rndStart = rng.Next(256*256*256 - WIDTH*HEIGHT);

        // Compteur d'image
        int imgNb = 0;
        int imgInterval = WIDTH * HEIGHT / PICTURE_NB;

        // Initialisation timer
        sw.Start();
        float lastMilliseconds = 0;

        for (int i = rndStart; i < WIDTH*HEIGHT + rndStart; i++)
        {
            Color selectedColor = availableCol[i];
            Point bestPointForColor;
            if(i == rndStart)
            {
                bestPointForColor = new Point(START_X, START_Y);
            }
            else
            {
                bestPointForColor = available.AsParallel().OrderBy(p => CalcDiff(pixels, p, selectedColor)).First();
            }

            // On ajoute la couleur au tableau des couleurs et au bitmap
            pixels[bestPointForColor.x, bestPointForColor.y] = selectedColor;
            bitmap.SetPixel(bestPointForColor.x, bestPointForColor.y, selectedColor);

            // On enlève ce point des points à traiter
            available.Remove(bestPointForColor);

            // On ajoute les voisins à la queue
            foreach (var neighbor in GetNeighborCoords(bestPointForColor))
            {
                if(pixels[neighbor.x,neighbor.y].IsEmpty)
                {
                    available.Add(neighbor);
                }
            }

            if(i % WIDTH == 0)
            {
                Console.WriteLine("Advancement : " + (i-rndStart) * 1.0 / (HEIGHT*WIDTH));
                float deltaTime = lastMilliseconds - sw.ElapsedMilliseconds;
                lastMilliseconds = sw.ElapsedMilliseconds;
                Console.WriteLine("Time Remaining : " + deltaTime * (HEIGHT - i / WIDTH) / 1000 + "s");
                Console.WriteLine("To process points : " + available.Count);
            }

            if(i % imgInterval == 0)
            {
                bitmap.Save("media/Pics/pic" + imgNb.ToString().PadLeft (3, '0') + ".png");
                imgNb++;

                if(CREATE_GIF)
                {
                    gifWriter.WriteFrame(bitmap);
                }
            }
        }

        bitmap.Save("media/Pics/Picture" + PICTURE_NB.ToString().PadLeft (3, '0') + ".png");
        
        if(CREATE_GIF)
        {
            gifWriter.WriteFrame(bitmap);
            gifWriter.Dispose();
        }

        if(CREATE_VIDEO)
        {
            CreateVideo();
        }

        sw.Stop();
    }
}
