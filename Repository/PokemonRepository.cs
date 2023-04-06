using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PokemonReview.Data;
using PokemonReview.Models;
using PokemonReview.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonReview.Test.Repository
{
    public class PokemonRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);

            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Pokemons.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Pokemons.Add(
                        new Pokemon()
                        {
                            Name = "Pikachu",
                            BirthDate = new DateTime(1903, 1, 1),
                            PokemonCategories = new List<PokemonCategory>()
                            {
                                new PokemonCategory { Category = new Category() { Name = "Mouse"}}
                            },
                            Reviews = new List<Review>()
                            {
                                new Review { Title="Pikachu",Text = "Pickahu is the best pokemon, because it is electric", Rating = 5,
                                Reviewer = new Reviewer(){ FirstName = "Teddy", LastName = "Smith" } },
                                new Review { Title="Pikachu", Text = "Pickachu is the best a killing rocks", Rating = 5,
                                Reviewer = new Reviewer(){ FirstName = "Taylor", LastName = "Jones" } },
                                new Review { Title="Pikachu",Text = "Pickchu, pickachu, pikachu", Rating = 1,
                                Reviewer = new Reviewer(){ FirstName = "Jessica", LastName = "McGregor" } },
                            },
                            PokemonTypes = new List<PokemonType>()
                            {
                                new PokemonType { Type = new PokemonReview.Models.Type() { Name = "Electric"}}
                            }
                        });
                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }

        [Fact]
        public async void PokemonRepository_GetPokemon_ReturnsPokemon()
        {
            //Arrange
            var name = "Pikachu";
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            //Act
            var result = pokemonRepository.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Should().BeOfType<Pokemon>();
        }
    }
}