using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicQuery.Command
{
    public class HttpMessage : Object
    {
        public HttpMessage()
        {
            buffer["code"] = 200;
            buffer["message"] = "成功";
            
        }

        private JObject buffer = new JObject();


        public int code
        {
            set { buffer["code"] = value; }
            get { return (int)buffer["code"]; }
        }

        public string message
        {
            get { return (string)buffer["message"]; }
            set { buffer["message"] = value; }
        }

        public object data
        {
            get { return (string)buffer["data"]; }
            set
            {
                if (value is JObject)
                {
                    buffer["data"] = (JObject)value;
                }
                else if (value is JArray)
                {
                    buffer["data"] = (JArray)value;
                }
                else if (value is string)
                {
                    buffer["data"] = (string)value;
                }
                else
                {
                    buffer["data"] = (JToken)value;
                }
            }
        }

        public object this[string key]
        {
            get { return buffer[key]; }
            set 
            { 
                if(value is string)
                {
                    buffer[key] = (string)value;
                }
                if (value is int)
                {
                    buffer[key] = (int)value;
                }
                if (value is JArray)
                {
                    buffer[key] = (JArray)value;
                }
                if (value is JObject)
                {
                    buffer[key] = (JObject)value;
                }
                if (value is bool)
                {
                    buffer[key] = (bool)value;
                }
            }
        }

        public override string ToString()
        {
            return buffer.ToString();
        }

        public JObject Result()
        {
            return buffer;
        }
    }
}
