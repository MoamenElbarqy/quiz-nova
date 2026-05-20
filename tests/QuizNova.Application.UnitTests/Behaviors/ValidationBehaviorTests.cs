using FluentValidation;
using FluentValidation.Results;

using MediatR;

using NSubstitute;

using QuizNova.Application.Common.Behaviours;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

using Xunit;

namespace QuizNova.Application.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateCourseCommand, Result<CourseDto>> _validationBehavior;
    private readonly IValidator<CreateCourseCommand> _mockValidator;
    private readonly RequestHandlerDelegate<Result<CourseDto>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<Result<CourseDto>>>();
        _mockValidator = Substitute.For<IValidator<CreateCourseCommand>>();

        _validationBehavior = new ValidationBehavior<CreateCourseCommand, Result<CourseDto>>(_mockValidator);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createCourseCommand = new CreateCourseCommand("C# Basics", Guid.NewGuid(), 50, 100);
        var courseResponse = (Result<CourseDto>)new CourseDto(Guid.NewGuid(), "C# Basics", Guid.NewGuid(), "Instructor Name", 5, 2, 100);

        _mockValidator
            .ValidateAsync(createCourseCommand, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _mockNextBehavior.Invoke(Arg.Any<CancellationToken>()).Returns(courseResponse);

        // Act
        var result = await _validationBehavior.Handle(createCourseCommand, _mockNextBehavior,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(courseResponse.Value, result.Value);
}

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var createCourseCommand = new CreateCourseCommand("C# Basics", Guid.NewGuid(), 50, 100);

        List<ValidationFailure> validationFailures =
            [new(propertyName: "property1", errorMessage: "property1 is invalid")];

        _mockValidator
            .ValidateAsync(createCourseCommand, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(createCourseCommand, _mockNextBehavior,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("property1", result.TopError.Code);
        Assert.Equal("property1 is invalid", result.TopError.Description);
}

    [Fact]
    public async Task InvokeValidationBehavior_WhenNoValidator_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createCourseCommand = new CreateCourseCommand("C# Basics", Guid.NewGuid(), 50, 100);
        var validationBehavior = new ValidationBehavior<CreateCourseCommand, Result<CourseDto>>();

        var courseResponse = (Result<CourseDto>)new CourseDto(Guid.NewGuid(), "C# Basics", Guid.NewGuid(), "Instructor Name", 5, 2, 100);

        _mockNextBehavior.Invoke(Arg.Any<CancellationToken>()).Returns(courseResponse);

        // Act
        var result = await validationBehavior.Handle(createCourseCommand, _mockNextBehavior,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(courseResponse.Value, result.Value);
}
}