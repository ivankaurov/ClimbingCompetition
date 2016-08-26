using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;

namespace ClimbingEntities
{
    sealed class ClimbingContextMigration2 : DbMigrationsConfiguration<ClimbingContext2>
    {
        public ClimbingContextMigration2()
        {
            AutomaticMigrationDataLossAllowed = false;
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ClimbingContext2 context)
        {
        }
    }
}
