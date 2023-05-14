using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;


namespace feast_mansion_project.Helper
{
	public static class SessionHelper
	{
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetUserId(this ISession session, int userId)
        {
            session.SetInt32("UserId", userId);
        }

        public static int? GetUserId(this ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static void SetIsAuthenticated(this ISession session, bool isAuthenticated)
        {
            session.SetBoolean("IsAuthenticated", isAuthenticated);
        }

        public static bool GetIsAuthenticated(this ISession session)
        {
            return session.GetBoolean("IsAuthenticated");
        }

        private static void SetBoolean(this ISession session, string key, bool value)
        {
            session.SetString(key, value.ToString());
        }

        private static bool GetBoolean(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null && bool.TryParse(value, out bool result) && result;
        }
    }
}

