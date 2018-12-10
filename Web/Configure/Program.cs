namespace ClimbingCompetition.Web.Configure
{
    using ClimbingEntities;
    using System;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            try
            {
                Console.WriteLine("Configuring DB");
                await ClimbingContext2.InitExistingDatabase(
                    "",
                    "",
                    "",
                     DbAccessCore.BaseContext.WhatToDo.CreateOrUpdate);

                Console.WriteLine("Configure completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Configure failed: {ex}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
