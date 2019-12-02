using System;

namespace RestaurantOrderingApplication
{
    class Program
    {


        static void Main(string[] args)
        {
            RestaurantHelpers HelperClass = new RestaurantHelpers();
            HelperClass.Start();
            Console.ReadKey();
        }
    }
}
