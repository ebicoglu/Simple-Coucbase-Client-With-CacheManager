using System;

namespace CoucbaseClient
{
    class Program
    {
        static void Main()
        {

            var coucbaseClient = new SimpleCoucbaseClient("http://localhost:8091/pools", "beer-sample", "Administrator", null);
            coucbaseClient.N1SqlQueryWithNativeCoucbaseObject("SELECT * FROM `beer-sample` LIMIT 5");
            coucbaseClient.SimpleJobs();

            Console.ReadKey();
        }
    }
}
