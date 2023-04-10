using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PokemonReview.Controllers;
using PokemonReview.Dto;
using PokemonReview.Interfaces;
using PokemonReview.Models;
using PokemonReview.Repository;

namespace PokemonReview.Tests.Controller
{
    public class PokemonControllerTest
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IMapper _mapper;

        public PokemonControllerTest()
        {
            _pokemonRepository = A.Fake<IPokemonRepository>();
            _categoryRepository = A.Fake<ICategoryRepository>();
            _ownerRepository = A.Fake<IOwnerRepository>();
            _typeRepository = A.Fake<ITypeRepository>();
            _mapper = A.Fake<IMapper>();
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnOk()
        {
            //Arrange
            var pokemons = A.Fake<ICollection<PokemonDto>>();
            var pokemonList = A.Fake<List<PokemonDto>>();
            A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);

            //Act
            var result = controller.GetPokemons();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnBadRequest()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = controller.GetPokemons();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnNotFound()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            int id = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(false);

            //Act
            var result = controller.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));

        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnBadRequest()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            var id = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(true);
            controller.ModelState.AddModelError("test", "test");
            
            //Act
            var result = controller.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnOk()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            var id = 1;
            var pokemon = A.Fake<PokemonDto>();
            
            A.CallTo(() => _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id))).Returns(pokemon); 
            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(true);

            //Act
            var result = controller.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnNotFound()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(false);

            //Act
            var result = controller.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnBadRequest()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(true);
            controller.ModelState.AddModelError("test", "test");

            //Act
            var result = controller.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnOk()
        {
            //Arrange
            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(true);

            //Act
            var result = controller.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnCreatedAtAction()
        {
            //Arrange
            
            var pokemon = A.Fake<Pokemon>();
            var pokemons = A.Fake<ICollection<PokemonPostDto>>();
            var pokemonDto = A.Fake<PokemonPostDto>();
            var pokemonList = A.Fake<IList<PokemonPostDto>>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemonDto.Name)).Returns(false);
            A.CallTo(() => _categoryRepository.CategoryExists(pokemonDto.CategoryId)).Returns(true);
            A.CallTo(() => _ownerRepository.OwnerExists(pokemonDto.OwnerId)).Returns(true);
            A.CallTo(() => _typeRepository.TypeExists(pokemonDto.TypeId)).Returns(true);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonDto)).Returns(pokemon);
            A.CallTo(() => _mapper.Map<Pokemon>(pokemonDto)).Returns(pokemon);
            A.CallTo(() => _pokemonRepository.CreatePokemon(pokemon, pokemonDto.CategoryId, pokemonDto.OwnerId, pokemonDto.TypeId)).Returns(true);

            var controller = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);

            //Act
            var result = controller.PostPokemon(pokemonDto) as CreatedAtActionResult; ;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedAtActionResult));
            result.ActionName.Should().Be("GetPokemon");
        }

    }
}
