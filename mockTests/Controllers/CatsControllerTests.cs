using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mock.depart.Controllers;
using mock.depart.Models;
using mock.depart.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.depart.Controllers.Tests
{
    [TestClass()]
    public class CatsControllerTests
    {
        [TestMethod()]
        public void Delete_CatNotFound()
        {
            // ÉTAPE 1 : Création des mocks 
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            // Étape 2: Configuration des mocks 
            // Configurez pour que le mock du service retourne null quand on demande any chat
            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(value: null);
            // Configurez pour que le mock du controller retourne "1" comme UserId
            controller.Setup(c => c.UserId).Returns("1");


            // Étape 3: Appel de la méthode à tester
            var actionResult = controller.Object.DeleteCat(0);

            // Étape 4: Vérifier le résultat
            var result = actionResult.Result as NotFoundResult;
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Delete_WrongOwner()
        {
            // ÉTAPE 1: Création des mocks
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };
           
            // ÉTAPE 2: Création des objets simulés
            CatOwner owner = new CatOwner()
            {
                Id = "11111"
            };

            Cat cat = new Cat()
            {
                Id = 1,
                Name = "Stimpy",
                CatOwner = owner,
                CuteLevel = Cuteness.Amazing
            };

            // ÉTAPE 3: Configuration des mocks
            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(cat);
            controller.Setup(c => c.UserId).Returns("1");

            // ÉTAPE 4: Appel de la méthode à tester
            var actionResult = controller.Object.DeleteCat(0);

            // ÉTAPE 5: Vérification du résultat
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is not yours", result.Value);
        }

        [TestMethod()]
        public void Delete_TooCute()
        {
            // ÉTAPE 1 : Création des mocks 
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            // ÉTAPE 2: Création des objets simulés
            CatOwner owner = new CatOwner()
            {
                Id = "11111"
            };

            Cat cat = new Cat()
            {
                Id = 1,
                Name = "Stimpy",
                CatOwner = owner,
                CuteLevel = Cuteness.Amazing
            };

            // ÉTAPE 3: Configuration des mocks
            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(cat);
            controller.Setup(c => c.UserId).Returns("11111");

            // ÉTAPE 4: Appel de la méthode à tester
            var actionResult = controller.Object.DeleteCat(0);

            // ÉTAPE 5: Vérification du résultat
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is too cute", result.Value);
        }

        [TestMethod()]
        public void Delete_Ok()
        {
            // ÉTAPE 1 : Création des mocks 
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            // ÉTAPE 2: Création des objets simulés
            CatOwner owner = new CatOwner()
            {
                Id = "11111"
            };

            Cat cat = new Cat()
            {
                Id = 1,
                Name = "Stimpy",
                CatOwner = owner,
                CuteLevel = Cuteness.BarelyOk
            };

            // ÉTAPE 3: Configuration des mocks
            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(cat);
            serviceMock.Setup(s => s.Delete(It.IsAny<int>())).Returns(cat);
            controller.Setup(c => c.UserId).Returns("11111");

            // ÉTAPE 4: Appel de la méthode à tester
            var actionResult = controller.Object.DeleteCat(0);

            // ÉTAPE 5: Vérification du résultat
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);

            Cat? catresult = (Cat?)result!.Value;
            Assert.AreEqual(cat.Id, catresult!.Id);
        }
    }
}