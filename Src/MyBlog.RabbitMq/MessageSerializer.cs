using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyBlog.Core.Services.Messages;

namespace MyBlog.RabbitMq;

public class MessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _serializerOpts;

    public MessageSerializer()
    {
        _serializerOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public T? Deserialize<T>(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return default;
        }

        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json, _serializerOpts);
    }

    public byte[] Serialize<T>(T data)
    {
        if (Equals(data, default(T)))
        {
            throw new ArgumentNullException(nameof(data));
        }

        var json = JsonSerializer.Serialize(data, _serializerOpts);
        return Encoding.UTF8.GetBytes(json);
    }
}
