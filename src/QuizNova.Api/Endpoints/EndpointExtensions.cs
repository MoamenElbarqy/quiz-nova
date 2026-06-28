namespace QuizNova.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAdminEndpoints();
        app.MapAuthEndpoints();
        app.MapCollegeEndpoints();
        app.MapCourseEndpoints();
        app.MapEnrollmentEndpoints();
        app.MapGradingEndpoints();
        app.MapInstructorEndpoints();
        app.MapQuizAttemptEndpoints();
        app.MapQuizEndpoints();
        app.MapStudentEndpoints();

        return app;
    }
}
