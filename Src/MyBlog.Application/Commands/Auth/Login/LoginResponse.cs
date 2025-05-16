namespace MyBlog.Application.Commands.Auth.Login;

public record LoginResponse(string AccessToken, int ExpireIn);
