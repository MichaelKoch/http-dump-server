using System.Timers;
using System.IO;
using http.dump.service.models;
using System.Text.Json;

namespace http.dump.service.repositories
{
    public class DumpRepository
    {
        private static string _BASE_DIRECTORY = "./dumps";
        public static string BASE_DIRECTORY
        {
            get
            {
                var envBase = Environment.GetEnvironmentVariable("BASE_DIRECTORY");
                if (!string.IsNullOrWhiteSpace(envBase))
                {
                    return envBase;
                }
                return _BASE_DIRECTORY;

            }
            set
            {
                _BASE_DIRECTORY = value;
            }
        }

        public const int TTL_IN_MINUTES = 30;

        public DumpRepository()
        {
            if (!Directory.Exists(BASE_DIRECTORY))
            {
                Directory.CreateDirectory(BASE_DIRECTORY);
            }
            InitCleanUpTimer();
        }


        private System.Timers.Timer InitCleanUpTimer()
        {
            var timer = new System.Timers.Timer { Interval = 30000 };
            timer.Elapsed += (sender, args) => { this.CleanUp(); };
            timer.Start();
            return timer;
        }


        private void CleanUp()
        {
            var now = DateTime.Now;
            var dumpFileNames = Directory.GetFiles(BASE_DIRECTORY);
            foreach (var fileName in dumpFileNames)
            {
                var lastwrite = File.GetLastWriteTime(fileName);
                var lastwriteSpan = now.Subtract(lastwrite);
                if (lastwriteSpan.TotalMinutes > TTL_IN_MINUTES)
                {
                    this.Delete(Path.GetFileNameWithoutExtension(fileName));
                }
            }
        }
        public string Create(DumpModel instance)
        {
            var id = Guid.NewGuid().ToString();
            var fileName = Path.Combine(BASE_DIRECTORY, id + ".json");
            if (File.Exists(fileName)) { File.Delete(fileName); };
            File.WriteAllText(fileName, JsonSerializer.Serialize(instance));
            return id;
        }
        public IList<DumpModel> getAll()
        {
            var retVal = new List<DumpModel>();
            var dumpFileNames = Directory.GetFiles(BASE_DIRECTORY);
            foreach (var fileName in dumpFileNames)
            {
                var instance = JsonSerializer.Deserialize<DumpModel>(File.ReadAllText(fileName));
                if (instance != null)
                {
                    retVal.Add(instance);
                }
            }
            return retVal;

        }

        public void Delete(string id)
        {
            var fileName = Path.Combine(BASE_DIRECTORY, id + ".json");
            if (File.Exists(fileName)) { File.Delete(fileName); };
        }
        public DumpModel? Read(string id)
        {
            var fileName = Path.Combine(BASE_DIRECTORY, id + ".json");
            return JsonSerializer.Deserialize<DumpModel>(File.ReadAllText(fileName));
        }
    }
}
