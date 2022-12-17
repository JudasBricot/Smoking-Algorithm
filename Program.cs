using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    const int WIDTH = 256;
    const int HEIGHT = 256;

    const int START_X = WIDTH / 2;
    const int START_Y = HEIGHT / 2;

    const int PICTURE_NB = 10;

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
        var neighbors = new List<Point>(8);
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

        return diffs.Min();
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

    public static void Main(String[] args)
    {
        Bitmap bitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format24bppRgb);
        Color[,] pixels = new Color[WIDTH, HEIGHT];
        HashSet<Point> available = new HashSet<Point>();

        // Initalisation de la liste des couleurs disponibles
        List<Color> availableCol = new List<Color>();
        for (int r = 0; r < 256; r++)
            for (int g = 0; g < 256; g++)
                for (int b = 0; b < 256; b++)
                    availableCol.Add(Color.FromArgb(r,g,b));
        
        // Tri de la liste des couleurs
        availableCol.Sort((c1,c2) => c1.GetHue().CompareTo(c2.GetHue()));

        for (int i = 0; i < WIDTH*HEIGHT; i++)
        {
            Color selectedColor = availableCol[i];
            Point bestPointForColor;

            if(available.Count == 0)
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

            // On enlève ce point de spoints à traiter
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
                Console.WriteLine("Advancement : " + i * 1.0 / (HEIGHT*WIDTH));
            }

            if(i % ((WIDTH * HEIGHT) / PICTURE_NB) == 0)
            {
                bitmap.Save("Picture n°" + i / ((WIDTH * HEIGHT) / PICTURE_NB) + ".jpg");
            }
        }

        bitmap.Save("Result.jpg");
    }
}