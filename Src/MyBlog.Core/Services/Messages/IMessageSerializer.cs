namespace MyBlog.Core.Services.Messages;

/// <summary>
/// Interface for message serialization and deserialization.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes an object to bytes.
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize.</typeparam>
    /// <param name="data">Object to serialize.</param>
    /// <returns>Serialized bytes.</returns>
    byte[] Serialize<T>(T data);

    /// <summary>
    /// Deserializes bytes to an object.
    /// </summary>
    /// <typeparam name="T">Type of the object to deserialize to.</typeparam>
    /// <param name="data">Bytes to deserialize.</param>
    /// <returns>Deserialized object.</returns>
    T? Deserialize<T>(byte[] data);
}
