using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CleanSyncMini
{
    public static class DataInputOutput<T> where T : class // specify T be a class
    {

        public static T LoadFromBinary(string path)
        {
            FileStream fileStream = null;
            T dataObject = null;
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }
                fileStream = new FileStream(path, System.IO.FileMode.Open);
            }

            catch (Exception)
            {

                throw;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            try
            {
                dataObject = binaryFormatter.Deserialize(fileStream) as T;
            }
            catch (Exception)
            {

                throw;
            }
            fileStream.Close();
            return dataObject;
        }

        public static void SaveToBinary(string path, T dataObject)
        {
            FileStream fileStream = null;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                fileStream = new FileStream(path, System.IO.FileMode.Create);
            }

            catch (Exception)
            {

                throw;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(fileStream, dataObject);
            }
            catch (Exception)
            {

                throw;
            }
            fileStream.Close();
        }
    }
}
