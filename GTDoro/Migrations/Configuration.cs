namespace GTDoro.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class GTDoroConfiguration : DbMigrationsConfiguration<GTDoro.DAL.GTDoroContext>
    {
        private readonly bool _pendingMigrations;

        public GTDoroConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "GTDoro.DAL.GTDoroContext";
            var migrator = new DbMigrator(this);
            _pendingMigrations = migrator.GetPendingMigrations().Any();
        }

        protected override void Seed(GTDoro.DAL.GTDoroContext context)
        {
            //  This method will be called after migrating to the latest version.

            //Microsoft comment says "This method will be called after migrating to the latest version."
            //However my testing shows that it is called every time the software starts

            //Exit if there aren't any pending migrations
            if (!_pendingMigrations) return;

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
