using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace Utils.Saving.Json
{
    [CreateAssetMenu(menuName = "SavingStrategies/JsonFile", fileName = "JsonFileStrategy")]
    public class JsonFileSavingStrategy : SavingStrategy
    {
        [SerializeField] protected string _fileName;

        private readonly string _fileExtension = ".json";

        public override void Save(JObject state)
        {
            string path = GetPathFromSaveFile(_fileName);
            Debug.Log("Saving to " + path);

            using (var textWriter = File.CreateText(path))
            {
                using (var writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    state.WriteTo(writer);
                }
            }
        }

        public override JObject Load()
        {
            string path = GetPathFromSaveFile(_fileName);

            if (!File.Exists(path))
            {
                return new JObject();
            }

            using (var textReader = File.OpenText(path))
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    jsonReader.FloatParseHandling = FloatParseHandling.Double;

                    return JObject.Load(jsonReader);
                }
            }
        }

        public override void Delete()
        {
            File.Delete(GetPathFromSaveFile(_fileName));
        }

        public virtual string GetExtension()
        {
            return _fileExtension;
        }

        protected string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + GetExtension());
        }
    }
}
