﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CleanSync
{
    public static class DataInputOutput<T> where T : class // specify T be a class
    {

        public static T LoadFromBinary(string path)
        {
            FileStream fileStream = null;
            T dataObject = null;
            try
            {
                fileStream = new FileStream(path, System.IO.FileMode.Open);
            }

            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Does not exist a file with path: " + path);
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("The Directory of the file " + path + " does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + path + " denied!");
            }
            catch (Exception)
            {
                throw new Exception("Failed to open file for binary file load.");
            }
            
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            try
            {
                dataObject = binaryFormatter.Deserialize(fileStream) as T;
            }
            catch (Exception)
            {
                throw new Exception("Failed to load binary.");
            }
            finally
            {
                fileStream.Close();
            }
            return dataObject;
        }

        public static void SaveToBinary(string path, T dataObject)
        {
            FileStream fileStream = null;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentNullException("The path " + path + " cannot be null.");
                }
                catch (UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException("Access to: " + path + " denied!");
                }
                catch (Exception)
                {
                    throw new Exception("Cannot create directory at path: " + path);
                }
            try
            {
                fileStream = new FileStream(path, System.IO.FileMode.Create);
            }

            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Does not exist a file with path: " + path);
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("The Directory of the file " + path + " does not exist.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + path + " denied!");
            }          
            catch (Exception)
            {
                throw new Exception("Failed to open file for binary save.");
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(fileStream, dataObject);
            }
            catch (Exception)
            {

                throw new Exception("Failed to save binary.");
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
