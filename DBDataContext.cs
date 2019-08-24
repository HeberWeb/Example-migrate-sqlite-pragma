using ConfectionControl.Model.Local.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.Local
{
    class DBDataContext : DbContext
    {
        public DbSet<UsersLocal> Users { get; set; }
        public DbSet<ClientsLocal> Clients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            var sqliteConnectionInitializer = new CreateOrMigrateDatabaseInitializer<DBDataContext>();
            Database.SetInitializer(sqliteConnectionInitializer);
        }
        private class CreateOrMigrateDatabaseInitializer<TContext> : CreateDatabaseIfNotExists<TContext>, IDatabaseInitializer<TContext>
      where TContext : DbContext
        {
            void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
            {
                base.InitializeDatabase(context);

                if (context.Database.Exists())
                {
                    Migrate(context);
                }
            }

            private void Migrate(DbContext context)
            {
                int version = context.Database.SqlQuery<int>("PRAGMA user_version").First();

                int numTables = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM sqlite_master AS TABLES WHERE TYPE = 'table'").First();

                if (numTables == 0)
                {
                    context.Database.ExecuteSqlCommand(File.ReadAllText("migrations/initial.sql"));
                }

                foreach (var migrationFile in Directory.GetFiles("migrations/", "*.sql"))
                {
                    if (!int.TryParse(Path.GetFileName(migrationFile).Split('.').First(), out int sqlVersion))
                    {
                        continue;
                    }

                    if (sqlVersion <= version)
                    {
                        continue;
                    }

                    var migrationScript = File.ReadAllText(migrationFile);
                    context.Database.ExecuteSqlCommand(migrationScript);
                }
            }
        }
    }
}
