namespace MyBlog.Application.Commands.Users.Login;

public record LoginResponse(string AccessToken, int ExpireIn);
