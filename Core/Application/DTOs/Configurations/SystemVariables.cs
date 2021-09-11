using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Configurations {
    public class SystemVariables {
        public string jwtsecret { get; set; }
        public string version { get; set; }
        public string appName { get; set; }
        public string environmentName { get; set; }
        public string siteRoot { get; set; }
        public bool debug { get; set; }
        public bool identityExpires { get; set; } = true;
        public int identityExpiryMins { get; set; } = 1440; //1 Day
        public DBConfig MySQL { get; set; }
        public DBConfig SQLite { get; set; }
        public DBConfig MongoDB { get; set; }
        public EmailParam EmailParam { get; set; }
        public Termii Termii { get; set; }
        public QueueServer QueueServer { get; set; }        
        public List<KeysTemplate> PasswordSalt { get; set; }
        public ElasticSearch ElasticSearch { get; set; }
    }
    public class DBConfig {
        public string server { get; set; }
        public string port { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class EmailParam {
        public string fromAddress { get; set; }
        public string fromName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string smtpServer { get; set; }
        public int smtpPort { get; set; }
    }
    public class Termii {
        public string root { get; set; }
        public string sendSMS { get; set; }
        public string sendOTP { get; set; }
        public string sendToken { get; set; }
        public string verify { get; set; }
        public string api_key { get; set; }
        public string senderID { get; set; }
    }
    public class QueueServer {
        public string mqhost { get; set; }
        public string mquser { get; set; }
        public string mqpw { get; set; }
        public Jobs Jobs { get; set; }
    }

    public class Jobs {
        public string emailTrigger { get; set; }
    }

    public class KeysTemplate {
        public string salt { get; set; }
        public int saltIndex { get; set; }
    }
    public class ElasticSearch {
        public BasicAuthentication BasicAuthentication { get; set; }
        public string[] nodes { get; set; }
        public ApiKeyAuthentication ApiKeyAuthentication { get; set; }
    }
    public class BasicAuthentication {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class ApiKeyAuthentication {
        public string id { get; set; }
        public string apiKey { get; set; }
    }
}