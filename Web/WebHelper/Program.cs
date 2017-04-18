using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHelper
{
    using System.Threading;

    using ClimbingEntities;

    using DbAccessCore;

    using Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var pn = new ProgressNotifier();

                pn.InitProgress += (s, e) => Console.WriteLine($"{e.Data1} {e.Data2}");

                ClimbingContext2.InitExistingDatabase(
                    "Data source=SSQL-12R2WEB01.client.parking.ru;Initial catalog=ivankaur_qsgu3;User ID=ivankaur_qsgu3;Password=6EPz3GDmxA",
                    "admin",
                    "Climbing1",
                    BaseContext.WhatToDo.CreateOrUpdate,
                    null,
                    CancellationToken.None).Wait();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }
    }
}
