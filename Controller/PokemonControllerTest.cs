using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReview.Controllers;
using PokemonReview.Dto;
using PokemonReview.Interfaces;
using PokemonReview.Models;
using PokemonReview.Repository;

namespace PokemonReview.Tests.Controller
{
    public class PokemonControllerTest :IDisposable
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IMapper _mapper;
        private readonly PokemonController _pokemonController;

        public PokemonControllerTest()
        {
            _pokemonRepository = A.Fake<IPokemonRepository>();
            _categoryRepository = A.Fake<ICategoryRepository>();
            _ownerRepository = A.Fake<IOwnerRepository>();
            _typeRepository = A.Fake<ITypeRepository>();
            _mapper = A.Fake<IMapper>();
            _pokemonController = new PokemonController(_pokemonRepository, _categoryRepository, _ownerRepository, _typeRepository, _mapper);
        }

        public void Dispose()
        {
            _pokemonController.Dispose();
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnOk()
        {
            //Arrange
            var pokemons = A.Fake<ICollection<PokemonDto>>();
            var pokemonList = A.Fake<List<PokemonDto>>();
            A.CallTo(() => _mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemonList);
            

            //Act
            var result = _pokemonController.GetPokemons();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnBadRequest()
        {
            //Arrange
            _pokemonController.ModelState.AddModelError("test", "test");

            //Act
            var result = _pokemonController.GetPokemons();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnNotFound()
        {
            //Arrange
            int id = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(false);

            //Act
            var result = _pokemonController.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));

        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnBadRequest()
        {
            //Arrange
            var id = 1;

            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(true);
            _pokemonController.ModelState.AddModelError("test", "test");
            
            //Act
            var result = _pokemonController.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemon_ReturnOk()
        {
            //Arrange
            var id = 1;
            var pokemon = A.Fake<PokemonDto>();
            
            A.CallTo(() => _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(id))).Returns(pokemon); 
            A.CallTo(() => _pokemonRepository.PokemonExists(id)).Returns(true);

            //Act
            var result = _pokemonController.GetPokemon(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnNotFound()
        {
            //Arrange
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(false);

            //Act
            var result = _pokemonController.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnBadRequest()
        {
            //Arrange
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(true);
            _pokemonController.ModelState.AddModelError("test", "test");

            //Act
            var result = _pokemonController.GetPokemon(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }

        [Fact]
        public void PokemonController_GetPokemonByName_ReturnOk()
        {
            //Arrange
            var name = "Test";

            A.CallTo(() => _pokemonRepository.PokemonExists(name)).Returns(true);

            //Act
            var result = _pokemonController.GetPokemon(name);

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

            //Act
            var result = _pokemonController.PostPokemon(pokemonDto) as CreatedAtActionResult; ;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(CreatedAtActionResult));
            result.ActionName.Should().Be("GetPokemon");
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnBadRequest_Null()
        {
            //Arrange
            
            //Act
            var result = _pokemonController.PostPokemon(null);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnBadRequest_ModelStateInvalid()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();
            _pokemonController.ModelState.AddModelError("test", "test");

            //Act
            var result = _pokemonController.PostPokemon(pokemon);
            

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnUnprocessableEntity_PokemonExists()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemon.Name)).Returns(true);

            //Act
            var result = _pokemonController.PostPokemon(pokemon) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            result.Value.Should().Be("This pokemon already exists.");
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnUnprocessableEntity_CategoryDontExists()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemon.Name)).Returns(false);
            A.CallTo(() => _categoryRepository.CategoryExists(pokemon.CategoryId)).Returns(false);
            
            //Act
            var result = _pokemonController.PostPokemon(pokemon) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            result.Value.Should().Be("This category doesn't exist, please check the id and try again.");
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnUnprocessableEntity_OwnerDontExists()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemon.Name)).Returns(false);
            A.CallTo(() => _categoryRepository.CategoryExists(pokemon.CategoryId)).Returns(true);
            A.CallTo(() => _ownerRepository.OwnerExists(pokemon.OwnerId)).Returns(false);

            //Act
            var result = _pokemonController.PostPokemon(pokemon) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            result.Value.Should().Be("This owner doesn't exist, please check the id and try again.");
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnUnprocessableEntity_TypeDontExists()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemon.Name)).Returns(false);
            A.CallTo(() => _categoryRepository.CategoryExists(pokemon.CategoryId)).Returns(true);
            A.CallTo(() => _ownerRepository.OwnerExists(pokemon.OwnerId)).Returns(true);
            A.CallTo(() => _typeRepository.TypeExists(pokemon.TypeId)).Returns(false);

            //Act
            var result = _pokemonController.PostPokemon(pokemon) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
            result.Value.Should().Be("This type doesn't exist, please check the id and try again.");
        }

        [Fact]
        public void PokemonController_PostPokemon_ReturnInternalServerError()
        {
            //Arrange
            var pokemon = A.Fake<PokemonPostDto>();
            var pokemonMapped = A.Fake<Pokemon>();

            A.CallTo(() => _pokemonRepository.PokemonExists(pokemon.Name)).Returns(false);
            A.CallTo(() => _categoryRepository.CategoryExists(pokemon.CategoryId)).Returns(true);
            A.CallTo(() => _ownerRepository.OwnerExists(pokemon.OwnerId)).Returns(true);
            A.CallTo(() => _typeRepository.TypeExists(pokemon.TypeId)).Returns(true);
            A.CallTo(() => _pokemonRepository.CreatePokemon(pokemonMapped, pokemon.CategoryId, pokemon.OwnerId, pokemon.TypeId)).Returns(false);

            //Act
            var result = _pokemonController.PostPokemon(pokemon) as ObjectResult;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Value.Should().Be("Something went wrong saving this pokemon.");
        }
    }
}
