using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace BoilerPlateCompiledQuery
{
    public class CompiledQueriesApp
    {
        public static async Task Main()
        {
            // Sample data
            List<Person> persons = new List<Person>
            {
                new Person { Name = "John", Age = 25 },
                new Person { Name = "Jane", Age = 30 },
                new Person { Name = "Mark", Age = 40 },
                // Add more persons as needed
            };

            // Insert data (async)
            await using (var context = new YourDbContext())
            {
                await context.Persons.AddRangeAsync(persons);
                await context.SaveChangesAsync();
            }

            // Get persons older than 30 using compiled query (async)
            await using (var context = new YourDbContext())
            {
                var compiledQuery = EF.CompileAsyncQuery<IEnumerable<Person>>(
                    dbContext => dbContext.Persons.Where(p => p.Age > 30));
                
                var personsOlderThan30 = await compiledQuery(context).ToListAsync();
                PrintPersons(personsOlderThan30);
            }

            // Get persons with a specific name using compiled query (async)
            await using (var context = new YourDbContext())
            {
                var nameToSearch = "John";
                var compiledQuery = EF.CompileAsyncQuery<IEnumerable<Person>>(
                    dbContext => dbContext.Persons.Where(p => p.Name == nameToSearch));
                
                var personsWithNameJohn = await compiledQuery(context).ToListAsync();
                PrintPersons(personsWithNameJohn);
            }

            // Update persons using compiled query (async)
            await using (var context = new YourDbContext())
            {
                var compiledQuery = EF.CompileAsyncQuery<IEnumerable<Person>>(
                    dbContext => dbContext.Persons.Where(p => p.Age > 30));

                var personsToUpdate = await compiledQuery(context).ToListAsync();
                personsToUpdate.ForEach(p => p.Age += 1);
                await context.SaveChangesAsync();
            }

            // Delete persons using compiled query (async)
            await using (var context = new YourDbContext())
            {
                var compiledQuery = EF.CompileAsyncQuery<IEnumerable<Person>>(
                    dbContext => dbContext.Persons.Where(p => p.Age < 25));

                var personsToDelete = await compiledQuery(context).ToListAsync();
                context.Persons.RemoveRange(personsToDelete);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("Compiled queries completed successfully.");
        }

        private static void PrintPersons(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                Console.WriteLine($"Id: {person.Id}, Name: {person.Name}, Age: {person.Age}");
            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class YourDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("YourDatabaseName");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Persons");
        }
    }
}
