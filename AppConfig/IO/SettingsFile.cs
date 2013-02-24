using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using AppConfig.IO;
using System.Xml;

namespace AppConfig.IO
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class SettingsFile
    {

        public SettingsFile()
        {
            SetDefaults();
        }

        #region Static Load Methods
        protected static SettingsFile Load(string FilePath)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("The file specified '" + FilePath + "' does not exist.", FilePath);

            FileStream dataStream = File.OpenRead(FilePath);
            try
            {
                SettingsFile rtn = Load(dataStream);
                rtn.FilePath = FilePath;
                return rtn;
            }
            finally
            {
                dataStream.Dispose();
            }
        }

        protected static SettingsFile Load(Stream dataStream)
        {
            string xmlData;
            Type type;

            PreLoad(dataStream, out type, out xmlData);

            var serializer = new DataContractSerializer(type);

            return Load(xmlData, serializer);
        }

        protected static void PreLoad(Stream dataStream, out Type type, out string xmlData)
        {
            string assemblyFileName;
            string assemblyName;
            string typeName;

            StreamReader sr = new StreamReader(dataStream);
            assemblyFileName = sr.ReadLine();
            assemblyName = sr.ReadLine();
            typeName = sr.ReadLine();
            xmlData = sr.ReadToEnd();

            Assembly assembly = null;
            if (!string.IsNullOrEmpty(assemblyFileName))
                assembly = Assembly.LoadFile(assemblyFileName);
            else
                assembly = Assembly.Load(assemblyName);

            type = assembly.GetType(typeName);
        }
        protected static SettingsFile Load(string xmlData, DataContractSerializer serializer)
        {
            return serializer.ReadObject(new System.Xml.XmlTextReader(new StringReader(xmlData))) as SettingsFile;
        }

        protected static T LoadOrCreate<T>(string FilePath) where T : SettingsFile
        {
            if (!File.Exists(FilePath))
            {
                T rtn = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
                typeof(T).GetMethod("SetDefaults", new Type[] { }).Invoke(rtn, new object[] { });
                typeof(T).GetProperty("FilePath").SetValue(rtn, FilePath, null);
                return rtn;
            }

            return Load(FilePath) as T;
        }
        #endregion

        public string FilePath { get; protected set; }

        public abstract void SetDefaults();

        #region Save Methods
        public virtual void Save()
        {
            SaveAs(FilePath);
        }
        public virtual void SaveAs(string FilePath)
        {
            this.FilePath = FilePath;

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            FileStream fs = File.Open(FilePath, FileMode.Create, FileAccess.Write);
            try
            {
                Save(fs);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }
        public virtual void Save(Stream DestinationStream)
        {
            Type type = this.GetType();

            StreamWriter sw = new StreamWriter(DestinationStream);
            sw.WriteLine(type.Assembly.Location);
            sw.WriteLine(type.Assembly.FullName);
            sw.WriteLine(type.FullName);
            sw.Flush();

            DataContractSerializer serializer = new DataContractSerializer(this.GetType());
            
            serializer.WriteObject(DestinationStream, this);
        }
        #endregion
    }
}
