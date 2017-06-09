﻿using System;
using System.Net;

using Server.Accounting;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    [Path("/login")]
    public class LoginController : BaseController
    {
        public override object HandleRequest(Parameters parameters, HttpListenerContext context)
        {
            var login = GetRequestData<LoginRequest>(context).Login;

            var account = Accounts.GetAccount(login.Username);

            if (account == null)
                return new {Result = (int)LoginResultCode.UnexistantAccount};
            else if (!account.CheckPassword(login.Password))
                return new {Result = (int)LoginResultCode.BadPassword};
            else
            {
                return new
                {
                    LoginResult = new
                    {
                        Code = (int)LoginResultCode.Valid,
                        AccessLevel = (int)account.AccessLevel
                    }
                };
            }
        }

        private class LoginRequest
        {
            public Login Login { get; set; }
        }

        private class Login
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private enum LoginResultCode
        {
            Valid = 1,
            BadPassword = 2,
            UnexistantAccount = 3,
        }
    }
}