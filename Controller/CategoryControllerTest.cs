using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PokemonReview.Controllers;
using PokemonReview.Dto;
using PokemonReview.Interfaces;
using PokemonReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonReview.Tests.Controller
{
    public class CategoryControllerTest : IDisposable
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryController _categoryController;

        public CategoryControllerTest()
        {
            _categoryRepository = A.Fake<ICategoryRepository>();
            _mapper = A.Fake<IMapper>();
            _categoryController = new CategoryController(_categoryRepository, _mapper);
        }

        public void Dispose()
        {
            _categoryController.Dispose();
        }

        [Fact]
        public void CategoryController_GetCategories_ReturnBadRequest()
        {
            //Arrange
            _categoryController.ModelState.AddModelError("test", "test");

            //Act
            var result = _categoryController.GetCategories();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void CategoryController_GetCategories_ReturnOk()
        {
            var categoriesList = A.Fake<List<CategoryDto>>();
            var categories = A.Fake<ICollection<Category>>();

            //Arrange
            A.CallTo(() => _categoryRepository.GetCategories()).Returns(categories);

            //Act
            var result = _categoryController.GetCategories();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ById_ReturnNotFound()
        {
            //Arrange
            var id = 1;
            A.CallTo(() => _categoryRepository.CategoryExists(id)).Returns(false);

            //Act
            var result = _categoryController.GetCategory(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ById_ReturnBadRequest()
        {
            //Arrange
            var id = 1;
            A.CallTo(() => _categoryRepository.CategoryExists(id)).Returns(true);
            _categoryController.ModelState.AddModelError("test", "test");

            //Act
            var result = _categoryController.GetCategory(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ById_ReturnOk()
        {
            //Arrange
            var id = 1;
            A.CallTo(() => _categoryRepository.CategoryExists(id)).Returns(true);
            
            //Act
            var result = _categoryController.GetCategory(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ByName_ReturnNotFound()
        {
            //Arrange
            var name = "test";
            A.CallTo(() => _categoryRepository.CategoryExists(name)).Returns(false);

            //Act
            var result = _categoryController.GetCategory(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ByName_ReturnBadRequest()
        {
            //Arrange
            var name = "test";
            A.CallTo(() => _categoryRepository.CategoryExists(name)).Returns(true);
            _categoryController.ModelState.AddModelError("test", "test");

            //Act
            var result = _categoryController.GetCategory(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void CategoryController_GetCategory_ByName_ReturnOk()
        {
            //Arrange
            var name = "test";
            A.CallTo(() => _categoryRepository.CategoryExists(name)).Returns(true);

            //Act
            var result = _categoryController.GetCategory(name);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}
