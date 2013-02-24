using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;
using System.Runtime.Serialization;

namespace AppConfig.IO
{
    [DataContract]
    [Serializable]
    public abstract class ResourceAndSettingsFile : SettingsFile
    {

        #region Construtors
        public ResourceAndSettingsFile()
        {
            zipFile = new ZipFile();
            FileID = Guid.NewGuid();
            IsFileNameSet = false;
        }
        public ResourceAndSettingsFile(string FilePath)
        {
            zipFile = new ZipFile(FilePath);
            FileID = Guid.NewGuid();
            IsFileNameSet = true;
        }
        #endregion

        #region Properties
        [DataMember]
        private Guid FileID { get; set; }

        public bool IsFileNameSet { get; private set; }
        public bool HasUnsavedEntries { get; private set; }
        protected ZipFile zipFile { get; private set; }
        public ICollection<string> EntryNames
        {
            get { return zipFile.EntryFileNames; }
        }
        #endregion

        #region Methods
        public void AddFile(string DirectoryPathInArchive, string FileName)
        {
            zipFile.AddFile(FileName, DirectoryPathInArchive);
            HasUnsavedEntries = true;
        }
        public void AddStream(string DirectoryPathInArchive, Stream stream)
        {
            zipFile.AddEntry(DirectoryPathInArchive, stream);
            HasUnsavedEntries = true;
        }
        public void ExtractToFile(string EntryName, string DirectoryName)
        {
            if (HasUnsavedEntries)
                Save(true);

            ZipEntry zipEntry = zipFile.Entries.SingleOrDefault(a => a.FileName == EntryName);
            if (zipEntry == null)
                throw new FileNotFoundException();
            zipEntry.Extract(DirectoryName, ExtractExistingFileAction.OverwriteSilently);
        }
        public Stream ExtractToStream(string EntryName)
        {
            if (HasUnsavedEntries)
                Save(true);

            ZipEntry zipEntry = zipFile.Entries.SingleOrDefault(a => a.FileName == EntryName);
            if (zipEntry == null)
                throw new FileNotFoundException();
            MemoryStream rtn = new MemoryStream();
            zipEntry.Extract(rtn);
            rtn.Position = 0;
            return rtn;
        }
        #endregion

        #region Load Methods
        protected static void PreLoad(string FilePath, out Type type, out string xmlData)
        {
            ZipFile zipFile = new ZipFile(FilePath);

            //Read the settings file and create it with the load method in the on settings class
            ZipEntry settingEntry = zipFile.Entries.SingleOrDefault(a => a.FileName == "Settings.xml");
            if (settingEntry == null)
                throw new Exception();

            MemoryStream ms = new MemoryStream();
            settingEntry.Extract(ms);
            ms.Position = 0;
            SettingsFile.PreLoad(ms, out type, out xmlData);
        }
        public static new ResourceAndSettingsFile Load(string FilePath)
        {
            ZipFile zipFile = new ZipFile(FilePath);

            //Read the settings file and create it with the load method in the on settings class
            ZipEntry settingEntry = zipFile.Entries.SingleOrDefault(a => a.FileName == "Settings.xml");
            if (settingEntry == null)
                throw new Exception();

            MemoryStream ms = new MemoryStream();
            settingEntry.Extract(ms);
            ms.Position = 0;
            ResourceAndSettingsFile rtn = SettingsFile.Load(ms) as ResourceAndSettingsFile;

            rtn.zipFile = zipFile;
            return rtn;
        }
        protected static ResourceAndSettingsFile Load(string FilePath, DataContractSerializer serializer, string xmlData)
        {
            ZipFile zipFile = new ZipFile(FilePath);

            var rtn = SettingsFile.Load(xmlData, serializer) as ResourceAndSettingsFile;

            rtn.zipFile = zipFile;
            return rtn;
        }
        #endregion

        #region Save Methods
        public override void Save()
        {
            if (!IsFileNameSet)
                throw new Exception("The 'Save' method is not allowed when a file name has not been set.  Check the 'IsFileNameSet' property and use the 'SaveAs' method when it is false.");

            Save(false);

            HasUnsavedEntries = false;
        }
        private void Save(bool TempSave)
        {
            string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "App.Config\\Templating\\" + FileID.ToString() + ".zip");

            string filePath = (TempSave) ? tempFilePath : base.FilePath;

            using (MemoryStream ms = new MemoryStream())
            {
                SaveConfiguration(ms);
                ms.Position = 0;

                if (!zipFile.EntryFileNames.Contains("Settings.xml"))
                    zipFile.AddEntry("Settings.xml", ms);
                else
                    zipFile.UpdateEntry("Settings.xml", ms);

                zipFile.Save(filePath);
            }

            //Delete the temp file if we have saved it to a real file
            if (!TempSave && File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
        public override void SaveAs(string FilePath)
        {
            base.FilePath = FilePath;
            IsFileNameSet = true;
            this.Save();
        }
        public override void Save(Stream DestinationStream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SaveConfiguration(ms);
                ms.Position = 0;

                if (!zipFile.EntryFileNames.Contains("Settings.xml"))
                    zipFile.AddEntry("Settings.xml", ms);
                else
                    zipFile.UpdateEntry("Settings.xml", ms);

                zipFile.Save(DestinationStream);
            }
        }
        protected virtual void SaveConfiguration(Stream DestinationStream)
        {
            base.Save(DestinationStream);
        }
        #endregion
    }
}
