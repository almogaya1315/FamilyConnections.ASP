using FamilyConnections.Core.Enums;
using FamilyConnections.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.BL.Handlers
{
    public class HttpHandler : IHttpHandler
    {
        public HttpContext Context { get; set; }

        public HttpHandler(HttpContext context)
        {
            Context = context;
        }

        public bool SessionHasKey(eKeys key)
        {
            return Context.Session.Keys.Contains(key.ToString());
        }

        public void SetToSession<T>(eKeys key, T value)
        {
            Context.Session.Set(key.ToString(), Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(value)));
        }

        public T GetSessionValue<T>(eKeys key)  
        {
            var sessionByte = Context.Session.Get(key.ToString());
            var t = JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(sessionByte));
            return t;
        }

        public bool CookiesHasKey(string key)
        {
            return Context.Request.Cookies.ContainsKey(key);
        }

        public string GetCookieValue(string key)
        {
            return Context.Request.Cookies[key];
        }

        public void SetToCookie(string key, string currentId)
        {
            Context.Response.Cookies.Append(key, currentId);
        }

        public void ResetSessionValue(string key, object value)
        {
            if (Context.Session.Keys.Contains(key)) RemoveSessionValue(key);
            var str = value is string ? (string)value : JsonConvert.SerializeObject(value);
            Context.Session.Set(key, Encoding.ASCII.GetBytes(str));
        }

        public void RemoveSessionValue(string key)
        {
            Context.Session.Remove(key);
        }

        public bool CurrentChanged(eKeys key, string currentId)
        {
            var currentChanged = false;
            var currentPersonKey = key.ToString();
            if (Context.Session.TryGetValue(currentPersonKey, out byte[] sessionCurrentIdByte))
            {
                var seesionCurrentId = System.Text.Encoding.ASCII.GetString(sessionCurrentIdByte);
                if (seesionCurrentId != currentId)
                {
                    Context.Session.Remove(currentPersonKey);
                    Context.Session.Set(currentPersonKey, System.Text.Encoding.ASCII.GetBytes(currentId));
                    currentChanged = true;
                }
            }
            else
            {
                Context.Session.Set(currentPersonKey, System.Text.Encoding.ASCII.GetBytes(currentId));
                currentChanged = true;
            }
            return currentChanged;
        }
    }
}
