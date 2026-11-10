namespace ShopTheThao.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ShopTheThao.Data.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ShopTheThao.Data.AppDbContext context)
        {
            ShopTheThao.Data.DbSeeder.Run(context);
        }
    }
}
