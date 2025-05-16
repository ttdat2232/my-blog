namespace MyBlog.Application.Commands.Auth.Register;

public record RegisterResponse(string AccessToken, int ExpireIn);
