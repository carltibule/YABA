using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using YABA.Data.Context;
using YABA.Models;

namespace YABA.Service.Tests
{
    public partial class UserServiceTests
    {
        [Theory]
        [ClassData(typeof(IsUserRegistedTestData))]
        public void IsUserRegistedTests(IsUserRegisteredScenario scenario)
        {
            var usersDbSet = new Mock<DbSet<User>>();
            usersDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(scenario.Users.Provider);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(scenario.Users.Expression);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(scenario.Users.ElementType);
            usersDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => scenario.Users.GetEnumerator());

            var mockContext = new Mock<YABAReadOnlyContext>();
            mockContext.Setup(x => x.Users).Returns(usersDbSet.Object);

            var userService = new UserService(mockContext.Object, null, null, null);
            var actualIsUserRegistered = userService.IsUserRegistered(scenario.AuthProviderId);
            Assert.Equal(scenario.ExpectedResult, actualIsUserRegistered);
        }
    }

    public class IsUserRegistedTestData : TheoryData<IsUserRegisteredScenario> 
    { 
        public IsUserRegistedTestData() 
        {
            Add(new IsUserRegisteredScenario
            {
                Users = new List<User>
                {
                    new User { Id = 1, Auth0Id = "auth0|TestId1" },
                }.AsQueryable(),
                AuthProviderId = "auth0|TestId1",
                ExpectedResult = true
            });

            // Not Found
            Add(new IsUserRegisteredScenario
            {
                Users = new List<User>
                {
                    new User { Id = 1, Auth0Id = "auth0|TestId1" },
                }.AsQueryable(),
                AuthProviderId = "auth0|TestId2",
                ExpectedResult = false
            });
        }
    }

    public class IsUserRegisteredScenario
    {
        public IQueryable<User> Users { get; set; }
        public string AuthProviderId { get; set; }
        public bool ExpectedResult { get; set; }
        public bool ActualResult { get; set; }
    }
}
