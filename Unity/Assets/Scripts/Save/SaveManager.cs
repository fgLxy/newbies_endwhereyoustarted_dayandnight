
using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace SunAndMoon
{
    public class SaveManager
    {
        private static SaveManager _instance;
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveManager();
                }
                return _instance;
            }
        }
        private SaveData _data;

        public SaveData GetCurrentSaveData()
        {
            return _data;
        }

        public async UniTask StartAsync()
        {
            _data = await Load();
        }

        public async UniTask Save(SaveData data)
        {
            var bytes = data.Serialize();
            var path = Application.persistentDataPath + "/Save/save.bytes";
            var backPath = path + ".bak";
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
            if (fileInfo.Exists)
            {
                File.Move(path, backPath);
            }
            await File.WriteAllBytesAsync(path, bytes);
            if  (File.Exists(backPath))
            {
                File.Delete(backPath);
            }
        }

        public async UniTask<SaveData> Load()
        {
            var path = Application.persistentDataPath + "/Save/save.bytes";
            var backPath = path + ".bak";
            if (File.Exists(backPath))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.Move(backPath, path);
            }
            if (File.Exists(path))
            {
                var data = await File.ReadAllBytesAsync(path);
                return SaveData.Deserialize(data);
            }
            else
            {
                var save = new SaveData();
                await Save(save);
                return save;
            }
        }
    }
}