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

        public HttpHandler() { }

        public bool SessionHasKey(eKeys key)
        {
            return Context.Session.Keys.Contains(key.ToString());
        }

        public void SetToSession<T>(eKeys key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            var bytes = Encoding.ASCII.GetBytes(json);
            Context.Session.Set(key.ToString(), bytes);
        }

        public T GetSessionValue<T>(eKeys key) where T : class
        {
            var bytes = Context.Session.Get(key.ToString());
            var json = Encoding.ASCII.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
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

        public void SetContext(HttpContext context)
        {
            //if (Context == null) 
            Context = context;
        }
    }
}
