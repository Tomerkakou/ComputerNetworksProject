using Newtonsoft.Json;

namespace ComputerNetworksProject.Services
{
    public static class SessionExtensions
    {
        //public readonly IHttpContextAccessor _htpctx;
        //public Session(IHttpContextAccessor htpctx) {
        //    _htpctx=htpctx;
        //}

        public static void SetObject(this ISession session, string key,object value)
        {
        //_htpctx.HttpContext?.Session.SetString(key, JsonConvert.SerializeObject(obj));
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            //var value = _htpctx.HttpContext?.Session.GetString(key);
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void ClearObject(this ISession session ,string key)
        {
            session.Remove(key);
        }
    }
}
