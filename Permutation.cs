// Java program to print all 
// permutations using Johnson
// and Trotter algorithm.
using System;
   
public class GfG{
       
    private  static bool LEFT_TO_RIGHT = true;
    private  static bool RIGHT_TO_LEFT = false;
   
    // Utility functions for
    // finding the position 
    // of largest mobile 
    // integer in a[].
    public static int searchArr(int []a, int n,
                                    int mobile)
    {
        for (int i = 0; i < n; i++)
               
            if (a[i] == mobile)
                return i + 1;
           
        return 0;
    }
   
    // To carry out step 1
    // of the algorithm i.e.
    // to find the largest
    // mobile integer.
    public static int getMobile(int []a,
                   bool []dir, int n)
    {
        int mobile_prev = 0, mobile = 0;
           
        for (int i = 0; i < n; i++)
        {
            // direction 0 represents
            // RIGHT TO LEFT.
            if (dir[a[i] - 1] == RIGHT_TO_LEFT &&
                                          i != 0)
            {
                if (a[i] > a[i - 1] && 
                            a[i] > mobile_prev)
                {
                    mobile = a[i];
                    mobile_prev = mobile;
                }
            }
   
            // direction 1 represents
            // LEFT TO RIGHT.
            if (dir[a[i] - 1] == LEFT_TO_RIGHT && i!=n - 1)
            {
                if (a[i] > a[i + 1] && 
                            a[i] > mobile_prev)
                {
                    mobile = a[i];
                    mobile_prev = mobile;
                }
            }
        }
       
        if (mobile == 0 && mobile_prev == 0)
            return 0;
        else
            return mobile;
    }
   
    // Prints a single 
    // permutation
    public static int printOnePerm(int []a, bool []dir, int n, int j)
    {
        int mobile = getMobile(a, dir, n);
        int pos = searchArr(a, n, mobile);
   
        // swapping the elements
        // according to the
        // direction i.e. dir[].
        if (dir[a[pos - 1] - 1] == RIGHT_TO_LEFT)
        {
            int temp = a[pos - 1];
            a[pos - 1] = a[pos - 2];
            a[pos - 2] = temp;
        }
        else if (dir[a[pos - 1] - 1] == LEFT_TO_RIGHT)
        {
            int temp = a[pos];
            a[pos] = a[pos - 1];
            a[pos - 1] = temp;
        }
   
        // changing the directions
        // for elements greater 
        // than largest mobile integer.
        for (int i = 0; i < n; i++)
        {
            if (a[i] > mobile)
            {
                if (dir[a[i] - 1] == LEFT_TO_RIGHT)
                    dir[a[i] - 1] = RIGHT_TO_LEFT;
           
                else if (dir[a[i] - 1] == RIGHT_TO_LEFT)
                    dir[a[i] - 1] = LEFT_TO_RIGHT;
            }
        }

        if(j %  18 == 0)
        {
            Console.Write("{");
            for (int i = 0; i < n-1; i++)
                Console.Write(a[i] - 1 + ", ");

            Console.WriteLine(a[n-1] - 1 + "},");    
        }          
        return 0;
    }
   
    // To end the algorithm
    // for efficiency it ends
    // at the factorial of n
    // because number of
    // permutations possible 
    // is just n!.
    public static int fact(int n)
    {
        int res = 1;
       
        for (int i = 1; i <= n; i++)
            res = res * i;
        return res;
    }
   
    // This function mainly
    // calls printOnePerm()
    // one by one to print
    // all permutations.
    public static void printPermutation(int n)
    {
        // To store current
        // permutation
        int []a = new int[n];
   
        // To store current
        // directions
        bool []dir = new bool[n];
   
        // storing the elements
        // from 1 to n and
        // printing first permutation.
        Console.Write("{");
        for (int i = 0; i < n-1; i++)
        {
            a[i] = i + 1;
            Console.Write(a[i] - 1 + ", ");
        }
        a[n-1] =  n;
        Console.Write(a[n-1] - 1 + "},");
           
        Console.Write("\n");
   
        // initially all directions
        // are set to RIGHT TO 
        // LEFT i.e. 0.
        for (int i = 0; i < n; i++)
            dir[i] = RIGHT_TO_LEFT;
   
        // for generating permutations
        // in the order.
        for (int i = 1; i < fact(n); i++)
            printOnePerm(a, dir, n, i);
    }
       
    // Driver function 
    public static void ain()
    {
        int n = 8;
        printPermutation(n);
    }
       
}
   
// This code is contributed by nitin mittal.