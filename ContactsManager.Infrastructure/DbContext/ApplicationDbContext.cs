using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public virtual required DbSet<Country> Countries { get; set; }
        public virtual required DbSet<Person> Persons { get; set; }

        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            string countriesJson = File.ReadAllText("countries.json");
            List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            if (countries != null)
            {
                foreach (Country country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
            }

            string personsJson = File.ReadAllText("persons.json");
            List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
            if (persons != null)
            {
                foreach (Person person in persons)
                {
                    modelBuilder.Entity<Person>().HasData(person);
                }
            }

            modelBuilder.Entity<Person>().Property(person => person.TaxNumber)
                .HasColumnName("TaxNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            // Set attribute [TaxNumber] to be unique
            // modelBuilder.Entity<Person>().HasIndex(person => person.TaxNumber).IsUnique();

            // Add constraint to force [TaxNumber] has a length of 8
            modelBuilder.Entity<Person>().ToTable(table => table.HasCheckConstraint("CHECK_TaxNumber", "LEN([TaxNumber]) = 8"));

            // Table relations can be set up using below
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne(person => person.Country)
            //    .WithMany(country => country.Persons)
            //    .HasForeignKey(person => person.CountryId);
            //});
        }

        public List<Person> GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int InsertPerson(Person person)
        {
            SqlParameter[] sqlParameters =
            {
                new("@PersonId", person.PersonId),
                new("@PersonName", person.PersonName),
                new("@Email", person.Email),
                new("@DateOfBirth", person.DateOfBirth),
                new("@Gender", person.Gender),
                new("@CountryId", person.CountryId),
                new("@Address", person.Address),
                new("@ReceiveNewsletters", person.ReceiveNewsletters),
            };
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonId, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsletters", sqlParameters);
        }
    }
}
