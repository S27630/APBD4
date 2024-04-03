using System;
using JetBrains.Annotations;
using LegacyApp;
using Xunit;

namespace LegacyApp.Tests;

[TestSubject(typeof(UserService))]
public class UserServiceTest
{
    [Fact]
    public void AddUser_Should_Return_False_When_AgeIsBelow21()
    {
        var userService = new UserService();
        var addResult = userService.AddUser("Joe", "Doe", "johndoe@gmail.com", DateTime.Parse("2008-03-21"), 1);
        Assert.False(addResult);
    }
    [Fact]
    public void AddUser_Should_Return_False_When_Name_is_Null()
    {
        var userService = new UserService();
        var addResult = userService.AddUser("", "Doe", "johndoe@gmail.com", DateTime.Parse("1993-03-21"), 1);
        Assert.False(addResult);
    }
    [Fact]
    public void AddUser_Should_Return_False_When_Email_is_not_corect()
    {
        var userService = new UserService();
        var addResult = userService.AddUser("Joe", "Doe", "johndoegmailcom", DateTime.Parse("1993-03-21"), 1);
        Assert.False(addResult);
    }
    [Fact]
    public void AddUser_Should_Throw_Exception_When_NotExisting()
    {
        var userService = new UserService();

        Assert.Throws<ArgumentException>(() =>
        {
            var addResult = userService.AddUser("Joe", "Doe", "johndoe@gmail.com", DateTime.Parse("1982-03-21"), 7);
        });
    }
    [Fact]
    public void AddUser_Should_Return_True_When_All_Is_Corect()
    {
        var userService = new UserService();
        var addResult = userService.AddUser("Joe", "Doe", "johndoe@gmail.com", DateTime.Parse("1993-03-21"), 1);
        Assert.True(addResult);
    }
    [Fact]
    public void AddUser_WithVeryImportantClient_NoCreditLimit()
    {
        var userService = new UserService();
        var client = new Client { Type = "VeryImportantClient" };
        var user = new User(); 

        userService.AddUser("John", "Doe", "johndoe@gmail.com", DateTime.Parse("1982-03-21"), 1);

        Assert.False(user.HasCreditLimit);
    }
    [Fact]
    public void AddUser_WithImportantClient_CreditLimitDoubled()
    {
        var userService = new UserService();
        var client = new Client { Type = "ImportantClient" };
        var user = new User();
        userService.AddUser("John", "Doe", "johndoe@gmail.com", DateTime.Parse("1982-03-21"), 3);

        Assert.Equal(0, user.CreditLimit); 
    }

    [Fact]
    public void AddUser_WithNormalClient_CreditLimitSet()
    {
        var userService = new UserService();
        var client = new Client { Type = "NormalClient" };
        var user = new User(); 

        userService.AddUser("John", "Doe", "johndoe@gmail.com", DateTime.Parse("1982-03-21"), 1);

        Assert.False(user.HasCreditLimit);
    }
    [Fact]
    public void AddUser_VeryImportantClient_NoCreditLimit()
    {
        var userService = new UserService();
        var client = new Client { Type = "VeryImportantClient" };

        var result = userService.AddUser("John", "Doe", "johndoe@gmail.com", DateTime.Parse("1982-03-21"), 2);

        Assert.True(result);
    }

    [Fact]
    public void AddUser_CreditLimitBelowThreshold_ReturnsFalse()
    {
        var userService = new UserService();
        var client = new Client { Type = "NormalClient" }; 
        var result = userService.AddUser("John", "Doe", "johndoe@gmail.com", DateTime.Parse("2000-03-21"), 1);

        Assert.True(result);
    }
}