namespace ClimbingCompetition.Web.Configure
{
    using ClimbingEntities;
    using System;
    using System.Linq;
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
                using (var ctx = await ClimbingContext2.InitExistingDatabase(
                    "",
                    "",
                    "",
                     DbAccessCore.BaseContext.WhatToDo.CreateOrUpdate))
                {
                    var t = ctx.Teams.FirstOrDefault(tx => tx.Code == 78);
                    if (t == null)
                    {
                        t = new ClimbingEntities.Teams.Team(ctx)
                        {
                            Code = 78,
                            Name = "Санкт-Петербург",
                        };
                        t.CalculateFullCode();
                        ctx.Teams.Add(t);

                        var n = await ctx.SaveChangesAsync();
                    }
                }

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
