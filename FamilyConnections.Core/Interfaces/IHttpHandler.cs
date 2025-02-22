using FamilyConnections.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyConnections.Core.Interfaces
{
    public interface IHttpHandler
    {
        public HttpContext Context { get; set; }

        bool CookiesHasKey(string key);
        bool CurrentChanged(eKeys key, string currentId);
        string GetCookieValue(string key);
        T GetSessionValue<T>(eKeys key);
        void RemoveSessionValue(string key);
        void ResetSessionValue(string key, object value);
        bool SessionHasKey(eKeys key);
        void SetToCookie(string key, string currentId);
        void SetToSession<T>(eKeys key, T value);
    }
}
