// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Auth;
using Discounts.Application.Features.Auth.Command.Login;
using Discounts.Application.Features.Auth.Command.RefreshToken;
using Discounts.Application.Features.Auth.Command.Register;
using Discounts.Application.Interfaces.Auth;
using FluentValidation.TestHelper;
using Moq;

namespace Discounts.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_FromIdentityService()
    {
        var command = new LoginCommand("test@email.com", "password");
        var expected = new AuthResponseDto { Token = "access-token", RefreshToken = "refresh-token" };
        _identityServiceMock.Setup(x => x.LoginAsync(command.Email, command.Password)).ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(expected, result);
        _identityServiceMock.Verify(x => x.LoginAsync(command.Email, command.Password), Times.Once);
    }
}

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new LoginCommand("user@example.com", "password123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldHaveError()
    {
        var command = new LoginCommand("", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        var command = new LoginCommand("not-an-email", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHaveError()
    {
        var command = new LoginCommand("user@example.com", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRegisterAsync_AndReturnAuthResponse()
    {
        var command = new RegisterCommand("user@example.com", "pass123", "John", "Doe", "Customer");
        var expected = new AuthResponseDto { Token = "token" };
        _identityServiceMock.Setup(x => x.RegisterAsync(command.Email, command.Password, command.FirstName, command.LastName, command.Role))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(expected, result);
    }
}

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoErrors()
    {
        var command = new RegisterCommand("user@example.com", "pass123", "John", "Doe", "Customer");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "pass123", "John", "Doe", "Customer")]
    [InlineData("bad-email", "pass123", "John", "Doe", "Customer")]
    [InlineData("user@example.com", "", "John", "Doe", "Customer")]
    [InlineData("user@example.com", "123", "John", "Doe", "Customer")] // too short
    [InlineData("user@example.com", "pass123", "", "Doe", "Customer")]
    [InlineData("user@example.com", "pass123", "John", "", "Customer")]
    [InlineData("user@example.com", "pass123", "John", "Doe", "")]
    [InlineData("user@example.com", "pass123", "John", "Doe", "Admin")] // invalid role
    public void Validate_WithInvalidData_ShouldHaveErrors(
        string email, string password, string firstName, string lastName, string role)
    {
        var command = new RegisterCommand(email, password, firstName, lastName, role);
        var result = _validator.TestValidate(command);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("Merchant")]
    [InlineData("merchant")]
    [InlineData("Customer")]
    [InlineData("customer")]
    public void Validate_WithValidRole_ShouldNotHaveRoleError(string role)
    {
        var command = new RegisterCommand("user@example.com", "pass123", "John", "Doe", role);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }
}

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallRefreshTokenAsync_AndReturnNewTokens()
    {
        var command = new RefreshTokenCommand("old-access", "old-refresh");
        var expected = new AuthResponseDto { Token = "new-access", RefreshToken = "new-refresh" };
        _identityServiceMock.Setup(x => x.RefreshTokenAsync(command.AccessToken, command.RefreshToken))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(true);

        Assert.Equal(expected, result);
        _identityServiceMock.Verify(x => x.RefreshTokenAsync(command.AccessToken, command.RefreshToken), Times.Once);
    }
}
