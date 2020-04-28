using System;

namespace FilippaTalOpgave
{
    class Program
    {
        static void Main(string[] args)
        {
            //Skriv tallene fra 1 til 9, så du får et ni-cifret tal, men hvor 1 skal gå op i det første ciffer, hvor 2 skal gå op i tallet, 
            //der består af de to første cifre, hvor tallet 3 går op i tallet, der består af de tre første cifre osv.
            //
            // A B C D E F G H I

            int temp = 0;
            int minimum = 123456789;
            int maximum = 987654321;

            Console.WriteLine($"Mimimum tal : {minimum} ");
            Console.WriteLine($"Maximum tal : {maximum} ");

            for (int i = minimum; i <= maximum; i++)
            {
                if (i%10000000 == 0)
                {
                    Console.WriteLine($"Så langt : {i}");
                }
                if (ErHvertCifferUnikt(i))
                    if (GårXOpITallet(i, 9))
                        if (GårXOpITallet(i, 8))
                            if (GårXOpITallet(i, 7))
                                if (GårXOpITallet(i, 6))
                                    if (GårXOpITallet(i, 5))
                                        if (GårXOpITallet(i, 4))
                                            if (GårXOpITallet(i, 3))
                                                if (GårXOpITallet(i, 2))
                                                    if (GårXOpITallet(i, 1))
                                                    {
                                                        temp = i;
                                                        break;
                                                    }
            }
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"Løsningnen er : {temp} ");

            Console.ReadLine();
        }
        /// <summary>
        /// Finder ud af om X går på i tallet.
        /// </summary>
        /// <param name="InputTal3"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool GårXOpITallet(int InputTal3, int x)
        {
            int temp2 = FørsteXTal(InputTal3, x) % x; //Er der nogen rest ved division.
            if (temp2 == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tager de første x cifre af det nicifrede tal.
        /// </summary>
        /// <param name="InputTal2"></param>
        /// <param name="FørsteX"></param>
        /// <returns></returns>
        private static int FørsteXTal(int InputTal2, int FørsteX)
        {
            string tempString2 = InputTal2.ToString();
            int tempTal2 = int.Parse(tempString2.Substring(0, FørsteX));
            return tempTal2;
        }

        /// <summary>
        /// Finder ud af om hvert ciffer i det nicifrede tal er unikt.
        /// </summary>
        /// <param name="InputTal"></param>
        /// <returns></returns>
        private static bool ErHvertCifferUnikt(int InputTal)
        {
            string tempString = InputTal.ToString();
            string A = tempString.Substring(0, 1);
            string B = tempString.Substring(1, 1);
            string C = tempString.Substring(2, 1);
            string D = tempString.Substring(3, 1);
            string E = tempString.Substring(4, 1);
            string F = tempString.Substring(5, 1); 
            string G = tempString.Substring(6, 1);
            string H = tempString.Substring(7, 1);
            string I = tempString.Substring(8, 1);
                        
            int[] array = { int.Parse(A), int.Parse(B), int.Parse(C), int.Parse(D), int.Parse(E), int.Parse(F), int.Parse(G), int.Parse(H), int.Parse(I) };

            for (int i = 0; i < array.Length; i++) //tallet "0" er ikke OK.
            {
                if (array[i] == 0)
                {
                    return false;
                }
            }
            
            for (int i = 0; i < array.Length; i++) //er alle de 9 cifre unikke
            {
                for (int j = i+1; j < array.Length; j++)
                {
                    if (array[i] == array[j]) 
                        return false;
                }
            }
            return true;
        }
    }
}