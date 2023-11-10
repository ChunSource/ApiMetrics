using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace PublicQuery.Command
{
    public static class Command
    {
        public static SqlSugarClient GetDataBase()
        {
            SqlSugarClient db = null;
            //string connstring = "Server=;Port=;Database=;Uid=;Pwd=;";
            string connstring = null;
            try
            {
                if (string.IsNullOrEmpty(connstring))
                {
                    throw new Exception("无法获取数据库信息");
                }
                //ICacheService mycache = new SugarCache();
                db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = connstring,//连接符字串
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.SystemTable,//设置从数据库表

                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        //缓存
                        //DataInfoCacheService = mycache
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true
                    }
                });
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //App.PrintToMiniProfiler("SqlSugar", "Info", FormatHelper.FormatParam(sql, pars));
                    Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                };
                db.Aop.OnError = (exp) =>//SQL报错
                {
                    if(exp.Sql.Length<1000)
                    {
                        Console.WriteLine(exp.Sql + "\r\n");
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n"+connstring);
                throw ex;
            }
            return db;
        }

        //SQLSUGAR匿名select前面不会自动加上c,这个函数的作用就是自动加上c
        static public JArray AddCPrefix(JArray array)
        {
            JArray newArray = new JArray();
            foreach (JObject obj in array)
            {
                JObject newObj = new JObject();
                var itors = obj.Properties();
                foreach (var itor in itors)
                {
                    var oldName = itor.Name;
                    var newName = "c" + oldName;
                    var value = itor.Value;
                    newObj[newName] = value;
                }
                newArray.Add(newObj);
            }
            return newArray;
        }

        static public JObject AddCPrefix(JObject obj)
        {
            JObject newObj = new JObject();
            var itors = obj.Properties();
            foreach (var itor in itors)
            {
                var oldName = itor.Name;
                var newName = "c" + oldName;
                var value = itor.Value;
                newObj[newName] = value;
            }
            return newObj;
        }

        //获取准确的时间
        public static DateTime AccuracyDatetime()
        {
            string currentTime = new DateTimeOffset(DateTime.UtcNow).ToString();
            return DateTime.Parse(currentTime);
        }

        //计算MD5
        public static string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string enc = BitConverter.ToString(md5.ComputeHash(bytes));
            enc = enc.Replace("-", "").ToLower();
            return enc;
        }

        //生成随机数
        public static string CreateRandomCode(int codeLen = 4)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9";
            string[] allCharAry = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeLen; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(8);
                if (temp == t)
                {
                    return CreateRandomCode(codeLen);
                }
                temp = t;
                randomCode += allCharAry[t];
            }
            return randomCode;
        }

        // 将字符串转换成base64格式,使用UTF8字符集
        public static string Base64Encode(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return Convert.ToBase64String(bytes);
        }

        // 将base64格式，转换utf8
        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string getJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }

            #region AES加密解密 
            /// <summary>
            /// 128位处理key 
            /// </summary>
            /// <param name="keyArray">原字节</param>
            /// <param name="key">处理key</param>
            /// <returns></returns>
            private static byte[] GetAesKey(byte[] keyArray, string key)
        {
            byte[] newArray = new byte[16];
            if (keyArray.Length < 16)
            {
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i >= keyArray.Length)
                    {
                        newArray[i] = 0;
                    }
                    else
                    {
                        newArray[i] = keyArray[i];
                    }
                }
            }
            return newArray;
        }
        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key, bool autoHandle = true)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            if (autoHandle)
            {
                keyArray = GetAesKey(keyArray, key);
            }
            byte[] toEncryptArray = Convert.FromBase64String(content);

            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion
    }
}
