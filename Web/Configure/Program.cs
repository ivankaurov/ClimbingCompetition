namespace ClimbingCompetition.Web.Configure
{
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
                await Task.CompletedTask;
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
