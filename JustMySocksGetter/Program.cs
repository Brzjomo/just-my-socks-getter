using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JustMySocksGetter
{
    public class Program
    {
        public class V2RayServer
        {
            public string ps { get; set; } = null!;
            public string port { get; set; } = null!;
            public string id { get; set; } = null!;
            public int aid { get; set; }
            public string net { get; set; } = null!;
            public string type { get; set; } = null!;
            public string tls { get; set; } = null!;
            public string add { get; set; } = null!;
        }

        public class YamlServer
        {
            [YamlMember(Alias = "name", ApplyNamingConventions = false)]
            public string Name { get; set; } = "Name";

            [YamlMember(Alias = "server", ApplyNamingConventions = false)]
            public string Server { get; set; } = "Server";

            [YamlMember(Alias = "port", ApplyNamingConventions = false)]
            public int Port { get; set; }

            [YamlMember(Alias = "type", ApplyNamingConventions = false)]
            public string Type { get; set; } = "Type";

            [YamlMember(Alias = "uuid", ApplyNamingConventions = false)]
            public string Uuid { get; set; } = "UUID";

            [YamlMember(Alias = "alterId", ApplyNamingConventions = false)]
            public int AlterId { get; set; }

            [YamlMember(Alias = "cipher", ApplyNamingConventions = false)]
            public string Cipher { get; set; } = "auto";

            [YamlMember(Alias = "tls", ApplyNamingConventions = false)]
            public bool Tls { get; set; }

            [YamlMember(Alias = "skip-cert-verify", ApplyNamingConventions = false)]
            public bool SkipCertVerify { get; set; }

            [YamlMember(Alias = "udp", ApplyNamingConventions = false)]
            public bool Udp { get; set; }
        }

        private class YamlConfig
        {
            [YamlMember(Alias = "port", ApplyNamingConventions = false)]
            public int Port { get; set; }

            [YamlMember(Alias = "socks-port", ApplyNamingConventions = false)]
            public int SocksPort { get; set; }

            [YamlMember(Alias = "allow-lan", ApplyNamingConventions = false)]
            public bool AllowLan { get; set; }

            [YamlMember(Alias = "mode", ApplyNamingConventions = false)]
            public string Mode { get; set; } = "Rule";

            [YamlMember(Alias = "log-level", ApplyNamingConventions = false)]
            public string LogLevel { get; set; } = "info";

            [YamlMember(Alias = "external-controller", ApplyNamingConventions = false)]
            public string ExternalController { get; set; } = ":9090";

            [YamlMember(Alias = "proxies", ApplyNamingConventions = false)]
            public List<YamlServer> Proxies { get; set; }
        }

        public static async Task Main()
        {
            Console.WriteLine("开始运行...");

            // http请求
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            var serverContent = await client.GetStringAsync("https://jmssub.net/members/getsub.php?service=448481&id=54d7e392-0c83-4989-8586-35471ecb7bf8&noss=1");

            // base64解码函数
            static string Base64Decode(string dataIn)
            {
                dataIn = dataIn.Trim()
                    .Replace(Environment.NewLine, "")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace(" ", "");

                if (dataIn.Length % 4 > 0)
                {
                    dataIn = dataIn.PadRight(dataIn.Length + 4 - dataIn.Length % 4, '=');
                }

                byte[] data = Convert.FromBase64String(dataIn);
                return Encoding.UTF8.GetString(data);
            }

            // 解码并拼接为字符串
            serverContent = Base64Decode(serverContent);
            var serverContentText = string.Concat(serverContent);

            // 分割服务器列表文本
            string[] serverList = serverContentText.Split("vmess://");

            // 删除空首行
            if (serverList[0] == "")
            {
                List<string> list = serverList.ToList();
                list.RemoveAt(0);
                serverList = list.ToArray();
            }

            // 解码服务器信息
            for (int i = 0; i < serverList.Length; i++)
            {
                serverList[i] = Base64Decode(serverList[i]);
            }

            // 反序列化
            V2RayServer? Server1 = JsonSerializer.Deserialize<V2RayServer>(serverList[0]);
            V2RayServer? Server2 = JsonSerializer.Deserialize<V2RayServer>(serverList[1]);
            V2RayServer? Server3 = JsonSerializer.Deserialize<V2RayServer>(serverList[2]);
            V2RayServer? Server4 = JsonSerializer.Deserialize<V2RayServer>(serverList[3]);

            static void PrintServerInfo(V2RayServer serverIn)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine($"名称：{serverIn.ps}");
                Console.WriteLine($"端口：{serverIn.port}");
                Console.WriteLine($"用户ID：{serverIn.id}");
                Console.WriteLine($"额外ID：{serverIn.aid}");
                Console.WriteLine($"传输协议：{serverIn.net}");
                Console.WriteLine($"伪装类型：{serverIn.type}");
                Console.WriteLine($"TLS：{serverIn.tls}");
                Console.WriteLine($"IP：{serverIn.add}");
            }

            // 显示服务器信息
            PrintServerInfo(Server1);
            PrintServerInfo(Server2);
            PrintServerInfo(Server3);
            PrintServerInfo(Server4);

            // Test
            // 反序列化
            var input = new StreamReader("./Sample.yml");
            var deserializer = new Deserializer();
            var yamlConfig = deserializer.Deserialize<YamlConfig>(input);

            yamlConfig.Port = 1313;

            Console.WriteLine(yamlConfig.Port);
            Console.WriteLine(yamlConfig.SocksPort);
            Console.WriteLine(yamlConfig.AllowLan);
            Console.WriteLine(yamlConfig.Mode);
            Console.WriteLine(yamlConfig.LogLevel);
            Console.WriteLine(yamlConfig.ExternalController);
            Console.WriteLine(yamlConfig.Proxies[0].Name);

            Console.ReadKey();
        }

        // Sample.yml
        //     port: 7890
        //     socks-port: 7891
        //     allow-lan: true
        //     mode: Rule
        //         log-level: info
        //         external-controller: :9090
        //     proxies:
        //     - {name: Name1, server: Server1, port: 80, type: vmess, uuid: UUID, alterId: 0, cipher: auto, tls: false, skip-cert-verify: true, udp: true}
        // - {name: Name2, server: Server2, port: 80, type: vmess, uuid: UUID, alterId: 0, cipher: auto, tls: false, skip-cert-verify: true, udp: true}
        // - {name: Name3, server: Server3, port: 80, type: vmess, uuid: UUID, alterId: 0, cipher: auto, tls: false, skip-cert-verify: true, udp: true}
        // - {name: Name4, server: Server4, port: 80, type: vmess, uuid: UUID, alterId: 0, cipher: auto, tls: false, skip-cert-verify: true, udp: true}
    }
}