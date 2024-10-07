using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace PPTT.Data
{
    public static class SessionExtensions
    {
        // Método para almacenar un objeto en la sesión como JSON
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Método para recuperar un objeto de la sesión
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

}
