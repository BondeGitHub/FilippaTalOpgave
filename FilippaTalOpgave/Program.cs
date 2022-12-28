using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FilippaTalOpgave
{
     class Program
    {
        static readonly CancellationTokenSource cts = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            //Skriv tallene fra 1 til 9, så du får et ni-cifret tal, men hvor 1 skal gå op i det første ciffer (læst fra venstre), hvor 2 skal gå op i tallet, 
            //der består af de to første cifre (læst fra venstre), hvor tallet 3 går op i tallet, der består af de tre første cifre osv.
            //
            // A B C D E F G H I
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            // UK : Below is an english translation of the above in danish:
            //
            // UK : MFRs daughters Math exercise
            // UK : Write all the number from 1 to 9, in such a way that you get a 9 digit number, where 1 can be divided into the first digit(from left side), where 2 can be divided into the first 2 digits(from left side),
            // UK : where 3 can be divided into the first 3 digits, where 4 can be divided into the first 4 digits ...

            int minimum = 123456789;
            int maximum = 987654321;

            Stopwatch s = new Stopwatch();
            
            int workerThreads=0;
            int completionPortThreads =0;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            Console.WriteLine();
            Console.WriteLine("workerThreads : " + workerThreads + "          completionPortThreads : " + completionPortThreads);
            Console.WriteLine();
            Console.Write("Type 1 for non multi threaded or type 4 for multithreaded  THIS MULTITHREADING NEEDS TO BE FIXED :  ");
            ///TODO : 13-09-2020 : Fix that the threads keeps running although one thread has found the result. https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
            ///TODO : 12-12-2022 : Wrt cancel task : https://johnthiriet.com/cancel-asynchronous-operation-in-csharp
            if (Int32.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine($"Mimimum number : {minimum} ");
                Console.WriteLine($"Maximum number : {maximum} ");
                if (choice == 1) //Non multi threaded
                {
                    s.Start();
                    FindTheResultForTheExercise(minimum, maximum);
                }
                if (choice == 4) //Multithreaded
                {
                    s.Start();
                    await FindTheResultForTheExerciseUsingThreading(minimum, maximum);
                }
                
                if (choice != 1 && choice != 4)
                {
                    Console.WriteLine("PROBLEMS TO BE HANDLED, not a valid option entered");
                }
            };
            
            s.Stop();
            Console.WriteLine($"Time used (HH:MM:SS:...) = {s.Elapsed}");
            Console.ReadLine();
        }
        /// <summary>
        /// UK : Divides the space into 4 and runs a task for each interval looking for the result for the exercise. 
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        //static async Task FindTheResultForTheExerciseUsingThreading(int minimum, int maximum)
        public static async Task FindTheResultForTheExerciseUsingThreading(int minimum, int maximum)
        {
            var token = cts.Token;
            Task t1 = Task.Run(() => FindTheResultForTheExerciseCancellationToken(minimum, 300000000, token));
            Task t2 = Task.Run(() => FindTheResultForTheExerciseCancellationToken(300000001, 500000000, token));
            Task t3 = Task.Run(() => FindTheResultForTheExerciseCancellationToken(500000001, 700000000, token));
            Task t4 = Task.Run(() => FindTheResultForTheExerciseCancellationToken(700000001, maximum, token));

            try
            {
                await Task.WhenAny(t1, t2, t3, t4);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("CATCH");
            }
            finally
            {
                cts.Dispose();
            }
        }

        /// <summary>
        /// UK : This is the method that finds the result for the exercise.
        /// </summary>
        /// <param name="inputMinimum"></param>
        /// <param name="inputMaximum"></param>
        //private static void FindTheResultForTheExercise(int inputMinimum, int inputMaximum)
        //static async Task FindTheResultForTheExercise(int inputMinimum, int inputMaximum)
        private static void FindTheResultForTheExercise(int inputMinimum, int inputMaximum)
        {
            int temp = 0;
            for (int i = inputMinimum; i <= inputMaximum; i++)
            {
                if (i % 10000000 == 0)
                {
                    //JBD 27-12-2022 COMMENTED OUT : Console.WriteLine($"Reached this far : {i}");
                    Console.WriteLine($"Reached this far : {i}   using threadId {Thread.CurrentThread.ManagedThreadId}");
                }

                if (i % 2 > 0) // tallet skal være ulige (da hvert andet ciffer er lige må de andre være ulige og altså overordnet set et ulige tal).
                               // UK: the result is an odd number (as every second digit is even then the other digits are odd).    
                    if (IsEveryDigitUniqAndEveryAlternateDigitEvenAndOdd(i))
                        if (DoesXDivideIntoTheNumber(i, 9))
                            if (DoesXDivideIntoTheNumber(i, 8))
                                if (DoesXDivideIntoTheNumber(i, 7))
                                    if (DoesXDivideIntoTheNumber(i, 6))
                                        if (DoesXDivideIntoTheNumber(i, 5))
                                            if (DoesXDivideIntoTheNumber(i, 4))
                                                if (DoesXDivideIntoTheNumber(i, 3))
                                                    if (DoesXDivideIntoTheNumber(i, 2))
                                                        if (DoesXDivideIntoTheNumber(i, 1))
                                                        {
                                                            temp = i;
                                                            break;
                                                        }
            }

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"from {inputMinimum} to {inputMaximum}       Resultet er  -  Correct answer is : {temp} ");
            if (temp == 0)
                Console.WriteLine($"from {inputMinimum} to {inputMaximum}       Result is not on the Thread that handled this interval - Resultatet er ikke på tråden som håndterede dette interval");
        }

        /// <summary>
        /// UK : This is the method that finds the result for the exercise (using CancellationToken).
        /// </summary>
        /// <param name="inputMinimum"></param>
        /// <param name="inputMaximum"></param>
        //private static void FindTheResultForTheExercise(int inputMinimum, int inputMaximum)
        //static async Task FindTheResultForTheExercise(int inputMinimum, int inputMaximum,)
        public static void FindTheResultForTheExerciseCancellationToken(int inputMinimum, int inputMaximum, CancellationToken token)
        {
            int temp = 0;
            for (int i = inputMinimum; i <= inputMaximum; i++)
            {
                if (token.IsCancellationRequested)
                {
                    //Console.WriteLine($"from {inputMinimum} to {inputMaximum}       Resultet er  -  Correct answer is : {temp} ");
                    //Console.WriteLine("");
                    
                    break;
                    //token.ThrowIfCancellationRequested();
                    //Console.ReadLine();
                }  
                                
                if (i % 10000000 == 0)
                {
                    Console.WriteLine($"Reached this far : {i}   using threadId {Thread.CurrentThread.ManagedThreadId}");
                }

                if (i % 2 > 0) // tallet skal være ulige (da hvert andet ciffer er lige må de andre være ulige og altså overordnet set et ulige tal).
                               // UK: the result is an odd number (as every second digit is even then the other digits are odd).    
                    if (IsEveryDigitUniqAndEveryAlternateDigitEvenAndOdd(i))
                        if (DoesXDivideIntoTheNumber(i, 9))
                            if (DoesXDivideIntoTheNumber(i, 8))
                                if (DoesXDivideIntoTheNumber(i, 7))
                                    if (DoesXDivideIntoTheNumber(i, 6))
                                        if (DoesXDivideIntoTheNumber(i, 5))
                                            if (DoesXDivideIntoTheNumber(i, 4))
                                                if (DoesXDivideIntoTheNumber(i, 3))
                                                    if (DoesXDivideIntoTheNumber(i, 2))
                                                        if (DoesXDivideIntoTheNumber(i, 1))
                                                        {
                                                            temp = i;
                                                            cts.Cancel();
                                                            //HERHERHERHER break;
                                                        }
            }

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            if (temp > 0)
            Console.WriteLine($" from {inputMinimum} to {inputMaximum}       Resultet er  -  Correct answer is : {temp}");
        }



        /// <summary>
        /// Finder ud af om X går på i tallet.
        /// UK : Finds out if X can be divided into the number.
        /// </summary>
        /// <param name="InputNumber3"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool DoesXDivideIntoTheNumber(int InputNumber3, int x)
        //JBD 06-09-2020 : private static bool GårXOpITallet(int InputTal3, int x)
        {
            int temp2 = FirstXNumbers(InputNumber3, x) % x; //Er der nogen rest ved division. UK : Any remainings from the division.
            if (temp2 == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Tager de første x cifre af det ni cifrede tal. 
        /// UK : Deals with the first x digits of then nine digit number.
        /// </summary>
        /// <param name="InputNumber2"></param>
        /// <param name="FirstX"></param>
        /// <returns></returns>
        private static int FirstXNumbers(int InputNumber2, int FirstX)
        //JBD 06-09-2020 : private static int FørsteXTal(int InputNumber2, int FirstX)
        {
            string tempString2 = InputNumber2.ToString();
            int tempNumber2 = int.Parse(tempString2.Substring(0, FirstX));
            return tempNumber2;
        }

        /// <summary>
        /// Finder ud af om hvert ciffer i det ni cifrede tal er unikt. 
        /// Finds out if each digit in the nine digit number is unique.
        /// </summary>
        /// <param name="InputNumber"></param>
        /// <returns></returns>
        private static bool IsEveryDigitUniq(int InputNumber)
        {
            // A B C D E F G H I
            string tempString = InputNumber.ToString();
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

            for (int i = 0; i < array.Length; i++) //tallet "0" er ikke OK. UK: Digit "0" is not OK as it is not one of the nine digits to be used.
            {
                if (array[i] == 0)
                    return false;
            }

            for (int i = 0; i < array.Length; i++) //er alle de 9 cifre unikke. UK: Are all nine digits unique.
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] == array[j])
                        return false;
                }
            }
            return true;
        }
         /// <summary>
         /// Finder ud af om hvert ciffer er unikt samt undersøger ligeledes om hvert andet ciffer er skiftevis lige og ulige.
         /// UK: Finds out if each digit is unique and if every second digit is even and odd (shifting).
         /// </summary>
         /// <param name="InputTal"></param>
         /// <returns></returns>
        private static bool IsEveryDigitUniqAndEveryAlternateDigitEvenAndOdd(int InputNumber)
        //JBD 06-09-2020 : private static bool ErHvertCifferUniktOgHvertAndetTalSkiftevisLigeOgUlige(int InputNumber)
        {
            // A B C D E F G H I
            string tempString = InputNumber.ToString();
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

            for (int i = 0; i < array.Length; i++) //tallet "0" er ikke OK. UK : Digit "0" is not OK.
            {
                if (array[i] == 0)
                    return false;
                if (i % 2 == 0) //array starter på index 0. UK : Array index starts at index 0.
                {
                    if (!(array[i] % 2 > 0)) //i er ulige og værdien af ciffer i er ulige. UK : i is odd and the digit is odd.
                        return false;
                }
                if (i % 2 > 0)
                {
                    if (!(array[i] % 2 == 0)) //i er lige og værdien af ciffer i er lige. UK : i is even and the digit is even.
                        return false;
                }
            }

            for (int i = 0; i < array.Length; i++) //er alle de 9 cifre unikke. UK : Are all 9 digits unique.
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] == array[j])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finder ud af om hvert ciffer er unikt samt undersøger ligeledes om hvert andet ciffer er skiftevis lige og ulige. Benytter en anden måde til at lave array.
        /// UK: Finds out if every digit is unique and and if every second digit is even and odd (shifting). Using an other way doing array.
        /// </summary>
        /// <param name="InputTal"></param>
        /// <returns></returns>
        private static bool IsEveryDigitUniqAndEveryAlternateDigitEvenAndOddBetterArrayCreation(int InputTal)
        //JBD 06-09-2020 : private static bool ErHvertCifferUniktOgHvertAndetTalSkiftevisLigeOgUligeBetterArrayCreation(int InputTal)
        {
            // A B C D E F G H I
            string tempString = InputTal.ToString();

            char[] array2 = tempString.ToCharArray();

            int[] array = array2.Select(i => Int32.Parse(i.ToString())).ToArray();
                        
            for (int i = 0; i < array.Length; i++) //tallet "0" er ikke OK. UK: Digit "0" is not OK
            {
                if (array[i] == 0)
                    return false;
                if (i % 2 == 0) //array starter på index 0. UK: Array index starts at index 0.
                {
                    if (!(array[i] % 2 > 0)) //i er ulige og værdien af ciffer i er ulige. UK: i is odd and the digit is odd.
                        return false;
                }
                if (i % 2 > 0)
                {
                    if (!(array[i] % 2 == 0)) //i er lige og værdien af ciffer i er lige. UK: i is even and the value of i is even.
                        return false;
                }
            }

            for (int i = 0; i < array.Length; i++) //er alle de 9 cifre unikke. UK: Are all 9 digits unique.
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] == array[j])
                        return false;
                }
            }
            return true;
        }

    }
}

