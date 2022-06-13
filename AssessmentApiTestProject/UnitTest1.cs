using Xunit;
using Moq;
using System.Threading.Tasks;
using AssessmentApiProject.Models;
using AssessmentApiProject.Services;
using Microsoft.Extensions.Caching.Memory;

public class UserServiceTests
{

    private IMemoryCache GetMemoryCacheWithData(List<User> users)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set(UserService.CacheKey, users);
        return cache;
    }

    [Fact]
    public async Task AddUser_ValidUser_ReturnsSuccessMessage()
    {
        // Arrange
        var cacheData = new Dictionary<string, object>();
        var cacheMock = new Mock<IMemoryCache>();
        cacheMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
            .Returns((string key, out object value) => cacheData.TryGetValue(key, out value));

        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns((object key) =>
            {
                cacheEntryMock.SetupGet(entry => entry.Key).Returns(key);
                cacheEntryMock.Setup(entry => entry.Dispose())
                    .Callback(() => cacheData.Remove(key.ToString()));
                return cacheEntryMock.Object;
            });

        var userService = new UserService(cacheMock.Object);
        var user = new User { Name = "John Doe", Address = "123 Main St" };

        // Act
        var result = await userService.AddUser(user);

        // Assert
        Assert.Equal("User added successfully.", result);
    }

    [Fact]
        public async Task AddUser_DuplicateName_ReturnsErrorMessage()
    {
        // Arrange
        var cacheData = new Dictionary<string, object>
        {
            { UserService.CacheKey, new List<User> { new User { Name = "John Doe", Address = "123 Main St" } } }
        };

        var cacheMock = new Mock<IMemoryCache>();
        cacheMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
            .Returns((string key, out object value) => cacheData.TryGetValue(key, out value));

        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns((object key) =>
            {
                cacheEntryMock.SetupGet(entry => entry.Key).Returns(key);
                cacheEntryMock.Setup(entry => entry.Dispose())
                    .Callback(() => cacheData.Remove(key.ToString()));
                return cacheEntryMock.Object;
            });

        var userService = new UserService(cacheMock.Object);
        var user = new User { Name = "John Doe", Address = "123 Main St" };
        // Act
        var result = await userService.AddUser(user);

        // Assert
        Assert.Equal("Name already exists. Please use a different name.", result);
    }


    [Fact]
    public async Task UpdateUser_ValidUser_ReturnsSuccessMessage()
    {
        // Arrange
        string existingUserName = "JohnDoe";
        User existingUser = new User { Name = existingUserName, Address = "Old Address" };
        User updatedUser = new User { Name = existingUserName, Address = "New Address" };

        var cache = GetMemoryCacheWithData(new List<User> { existingUser });
        var userService = new UserService(cache);

        // Act
        string result = await userService.UpdateUser(existingUserName, updatedUser);

        // Assert
        Assert.Equal("User updated successfully.", result);
        Assert.Equal(updatedUser.Address, existingUser.Address);
    }

    [Fact]
    public async Task UpdateUser_UserNotFound_ReturnsNotFoundMessage()
    {
        // Arrange
        string existingUserName = "JohnDoe";
        User updatedUser = new User { Name = existingUserName, Address = "New Address" };

        var cache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(cache);

        // Act
        string result = await userService.UpdateUser(existingUserName, updatedUser);

        // Assert
        Assert.Equal("No user found.", result);
    }

    [Fact]
    public async Task DeleteUser_ValidUser_ReturnsSuccessMessage()
    {
        // Arrange
        string existingUserName = "JohnDoe";
        User existingUser = new User { Name = existingUserName, Address = "Old Address" };

        var cache = GetMemoryCacheWithData(new List<User> { existingUser });
        var userService = new UserService(cache);

        // Act
        string result = await userService.DeleteUser(existingUserName);

        // Assert
        Assert.Equal("User deleted successfully.", result);
        Assert.DoesNotContain(existingUser, cache.Get<List<User>>(UserService.CacheKey));
    }

    [Fact]
    public async Task DeleteUser_UserNotFound_ReturnsNotFoundMessage()
    {
        // Arrange
        string existingUserName = "JohnDoe";

        var cache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(cache);

        // Act
        string result = await userService.DeleteUser(existingUserName);

        // Assert
        Assert.Equal("No user found.", result);
    }

    [Fact]
    public async Task GetAllUsers_WithCachedUsers_ReturnsListOfUsers()
    {
        // Arrange
        var cacheData = new Dictionary<string, object>
        {
            { UserService.CacheKey, new List<User> { new User { Name = "John Doe", Address = "123 Main St" } } }
        };

        var cacheMock = new Mock<IMemoryCache>();
        cacheMock.Setup(x => x.TryGetValue(UserService.CacheKey, out It.Ref<object>.IsAny)).Returns((string key, out object value) => cacheData.TryGetValue(key, out value));

        var userService = new UserService(cacheMock.Object);

        // Act
        var result = await userService.GetAllUsers();

        // Assert
        Assert.Single(result); // Use Assert.Single to check for collection size
        Assert.Equal("John Doe", result[0].Name);
        Assert.Equal("123 Main St", result[0].Address);
    }

    [Fact]
    public async Task GetAllUsers_WithNoCachedUsers_ReturnsEmptyList()
    {
        // Arrange
        var cacheData = new Dictionary<string, object>();
        var cacheMock = new Mock<IMemoryCache>();
        var userService = new UserService(cacheMock.Object);
        var cachedUsers = new List<User>();
        cacheMock.Setup(x => x.TryGetValue(UserService.CacheKey, out It.Ref<object>.IsAny)).Returns(false);

        // Act
        var result = await userService.GetAllUsers();

        // Assert
        Assert.Empty(result);
    }
}
