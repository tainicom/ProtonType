#region License
//   Copyright 2019-2021 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using tainicom.ProtonType.App.Common;


namespace tainicom.ProtonType.App.FileDocuments
{
    public class RecentFilesMgr
    {
        public IPersist Persister { get; set; }

        public void UseRegistryPersister() { Persister = new RegistryPersister(); }
        public void UseRegistryPersister(string key) { Persister = new RegistryPersister(key); }

        public void UseXmlPersister() { Persister = new XmlPersister(); }
        public void UseXmlPersister(string filepath) { Persister = new XmlPersister(filepath); }
        public void UseXmlPersister(Stream stream) { Persister = new XmlPersister(stream); }

        public int MaxNumberOfFiles { get; set; }


        //-----------------------------------------------------------------------------------------

        public RecentFilesMgr()
        {
            Persister = new RegistryPersister();

            MaxNumberOfFiles = 10;
        }

        //-----------------------------------------------------------------------------------------



        public List<RecentFileInfo> RecentFiles
        {
            get 
            { 
                var filenameList = Persister.RecentFiles(MaxNumberOfFiles);
                var list = new List<RecentFileInfo>();
                foreach (var fullFilename in filenameList)
                    list.Add(new RecentFileInfo(fullFilename));

                return list;
            }
        }
        public void RemoveFile(string filepath) { Persister.RemoveFile(filepath, MaxNumberOfFiles); }
        public void InsertFile(string filepath) { Persister.InsertFile(filepath, MaxNumberOfFiles); }

        //-----------------------------------------------------------------------------------------

        public interface IPersist
        {
            List<string> RecentFiles(int max);
            void InsertFile(string filepath, int max);
            void RemoveFile(string filepath, int max);
        }


        //-----------------------------------------------------------------------------------------

        static class ApplicationAttributes
        {
            static readonly Assembly _Assembly = null;

            static readonly AssemblyTitleAttribute _Title = null;
            static readonly AssemblyCompanyAttribute _Company = null;
            static readonly AssemblyCopyrightAttribute _Copyright = null;
            static readonly AssemblyProductAttribute _Product = null;

            public static string Title { get; private set; }
            public static string CompanyName { get; private set; }
            public static string Copyright { get; private set; }
            public static string ProductName { get; private set; }

            static Version _Version = null;
            public static string Version { get; private set; }

            static ApplicationAttributes()
            {
                try
                {
                    Title = String.Empty;
                    CompanyName = String.Empty;
                    Copyright = String.Empty;
                    ProductName = String.Empty;
                    Version = String.Empty;

                    _Assembly = Assembly.GetEntryAssembly();

                    if (_Assembly != null)
                    {
                        object[] attributes = _Assembly.GetCustomAttributes(false);

                        foreach (object attribute in attributes)
                        {
                            Type type = attribute.GetType();

                            if (type == typeof(AssemblyTitleAttribute)) _Title = (AssemblyTitleAttribute)attribute;
                            if (type == typeof(AssemblyCompanyAttribute)) _Company = (AssemblyCompanyAttribute)attribute;
                            if (type == typeof(AssemblyCopyrightAttribute)) _Copyright = (AssemblyCopyrightAttribute)attribute;
                            if (type == typeof(AssemblyProductAttribute)) _Product = (AssemblyProductAttribute)attribute;
                        }

                        _Version = _Assembly.GetName().Version;
                    }

                    if (_Title != null) Title = _Title.Title;
                    if (_Company != null) CompanyName = _Company.Company;
                    if (_Copyright != null) Copyright = _Copyright.Copyright;
                    if (_Product != null) ProductName = _Product.Product;
                    if (_Version != null) Version = _Version.ToString();
                }
                catch { }
            }
        }

        //-----------------------------------------------------------------------------------------

        private class RegistryPersister : IPersist
        {
            public string RegistryKey { get; set; }

            public RegistryPersister()
            {
                RegistryKey =
                    "Software\\" +
                    ApplicationAttributes.CompanyName + "\\" +
                    ApplicationAttributes.ProductName + "\\" +
                    "RecentFileList";
            }

            public RegistryPersister(string key)
            {
                RegistryKey = key;
            }

            string Key(int i) { return i.ToString("00"); }

            public List<string> RecentFiles(int max)
            {
                RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (k == null) k = Registry.CurrentUser.CreateSubKey(RegistryKey);

                List<string> list = new List<string>(max);

                for (int i = 0; i < max; i++)
                {
                    string filename = (string)k.GetValue(Key(i));

                    if (String.IsNullOrEmpty(filename)) break;

                    list.Add(filename);
                }

                return list;
            }

            public void InsertFile(string filepath, int max)
            {
                RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (k == null) Registry.CurrentUser.CreateSubKey(RegistryKey);
                k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);

                RemoveFile(filepath, max);

                for (int i = max - 2; i >= 0; i--)
                {
                    string sThis = Key(i);
                    string sNext = Key(i + 1);

                    object oThis = k.GetValue(sThis);
                    if (oThis == null) continue;

                    k.SetValue(sNext, oThis);
                }

                k.SetValue(Key(0), filepath);
            }

            public void RemoveFile(string filepath, int max)
            {
                RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (k == null) return;

                for (int i = 0; i < max; i++)
                {
                again:
                    string s = (string)k.GetValue(Key(i));
                    if (s != null && s.Equals(filepath, StringComparison.CurrentCultureIgnoreCase))
                    {
                        RemoveFile(i, max);
                        goto again;
                    }
                }
            }

            void RemoveFile(int index, int max)
            {
                RegistryKey k = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (k == null) return;

                k.DeleteValue(Key(index), false);

                for (int i = index; i < max - 1; i++)
                {
                    string sThis = Key(i);
                    string sNext = Key(i + 1);

                    object oNext = k.GetValue(sNext);
                    if (oNext == null) break;

                    k.SetValue(sThis, oNext);
                    k.DeleteValue(sNext);
                }
            }
        }

        //-----------------------------------------------------------------------------------------

        private class XmlPersister : IPersist
        {
            public string Filepath { get; set; }
            public Stream Stream { get; set; }

            public XmlPersister()
            {
                Filepath =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        ApplicationAttributes.CompanyName + "\\" +
                        ApplicationAttributes.ProductName + "\\" +
                        "RecentFileList.xml");
            }

            public XmlPersister(string filepath)
            {
                Filepath = filepath;
            }

            public XmlPersister(Stream stream)
            {
                Stream = stream;
            }

            public List<string> RecentFiles(int max)
            {
                return Load(max);
            }

            public void InsertFile(string filepath, int max)
            {
                Update(filepath, true, max);
            }

            public void RemoveFile(string filepath, int max)
            {
                Update(filepath, false, max);
            }

            void Update(string filepath, bool insert, int max)
            {
                List<string> old = Load(max);

                List<string> list = new List<string>(old.Count + 1);

                if (insert) list.Add(filepath);

                CopyExcluding(old, filepath, list, max);

                Save(list, max);
            }

            void CopyExcluding(List<string> source, string exclude, List<string> target, int max)
            {
                foreach (string s in source)
                    if (!String.IsNullOrEmpty(s))
                        if (!s.Equals(exclude, StringComparison.OrdinalIgnoreCase))
                            if (target.Count < max)
                                target.Add(s);
            }

            class SmartStream : IDisposable
            {
                bool _IsStreamOwned = true;
                Stream _Stream = null;

                public Stream Stream { get { return _Stream; } }

                public static implicit operator Stream(SmartStream me) { return me.Stream; }

                public SmartStream(string filepath, FileMode mode)
                {
                    _IsStreamOwned = true;

                    Directory.CreateDirectory(Path.GetDirectoryName(filepath));

                    _Stream = File.Open(filepath, mode);
                }

                public SmartStream(Stream stream)
                {
                    _IsStreamOwned = false;
                    _Stream = stream;
                }

                public void Dispose()
                {
                    if (_IsStreamOwned && _Stream != null) _Stream.Dispose();

                    _Stream = null;
                }
            }

            SmartStream OpenStream(FileMode mode)
            {
                if (!String.IsNullOrEmpty(Filepath))
                {
                    return new SmartStream(Filepath, mode);
                }
                else
                {
                    return new SmartStream(Stream);
                }
            }

            List<string> Load(int max)
            {
                List<string> list = new List<string>(max);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (SmartStream ss = OpenStream(FileMode.OpenOrCreate))
                    {
                        if (ss.Stream.Length == 0) return list;

                        ss.Stream.Position = 0;

                        byte[] buffer = new byte[1 << 20];
                        for (; ; )
                        {
                            int bytes = ss.Stream.Read(buffer, 0, buffer.Length);
                            if (bytes == 0) break;
                            ms.Write(buffer, 0, bytes);
                        }

                        ms.Position = 0;
                    }

                    XmlTextReader x = null;

                    try
                    {
                        x = new XmlTextReader(ms);

                        while (x.Read())
                        {
                            switch (x.NodeType)
                            {
                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.Whitespace:
                                    break;

                                case XmlNodeType.Element:
                                    switch (x.Name)
                                    {
                                        case "RecentFiles": break;

                                        case "RecentFile":
                                            if (list.Count < max) list.Add(x.GetAttribute(0));
                                            break;

                                        default: Debug.Assert(false); break;
                                    }
                                    break;

                                case XmlNodeType.EndElement:
                                    switch (x.Name)
                                    {
                                        case "RecentFiles": return list;
                                        default: Debug.Assert(false); break;
                                    }
                                    break;

                                default:
                                    Debug.Assert(false);
                                    break;
                            }
                        }
                    }
                    finally
                    {
                        if (x != null) x.Close();
                    }
                }
                return list;
            }

            void Save(List<string> list, int max)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlTextWriter x = null;

                    try
                    {
                        x = new XmlTextWriter(ms, Encoding.UTF8);
                        if (x == null) { Debug.Assert(false); return; }

                        x.Formatting = Formatting.Indented;

                        x.WriteStartDocument();

                        x.WriteStartElement("RecentFiles");

                        foreach (string filepath in list)
                        {
                            x.WriteStartElement("RecentFile");
                            x.WriteAttributeString("Filepath", filepath);
                            x.WriteEndElement();
                        }

                        x.WriteEndElement();

                        x.WriteEndDocument();

                        x.Flush();

                        using (SmartStream ss = OpenStream(FileMode.Create))
                        {
                            ss.Stream.SetLength(0);

                            ms.Position = 0;

                            byte[] buffer = new byte[1 << 20];
                            for (; ; )
                            {
                                int bytes = ms.Read(buffer, 0, buffer.Length);
                                if (bytes == 0) break;
                                ss.Stream.Write(buffer, 0, bytes);
                            }
                        }
                    }
                    finally
                    {
                        if (x != null) x.Close();
                    }
                }
            }

        }

        public class RecentFileInfo
        {
            public string FullFilename { get; private set; }
            public string Filename { get; private set; }
            public string FullPath { get; private set; }

            public RecentFileInfo(string fullFilename)
            {
                this.FullFilename = fullFilename;
                Filename = Path.GetFileName(fullFilename);
                FullPath = Path.GetDirectoryName(fullFilename) + "\\";
            }

            public System.Windows.Media.ImageSource FileIcon
            {
                get
                {
                    string absolutePath = FullFilename;
                    try
                    {
                        // TODO: use native Win32 or Registry to get icon.
                        return FileIconInterop.GetIconFromFile(absolutePath, true);
                    }
                    catch (FileNotFoundException)
                    {
                        Uri oUri = new Uri("pack://application:,,,/ProtonType;component/Icons/FileWarning.png");
                        return new System.Windows.Media.Imaging.BitmapImage(oUri);
                    }
                }
            }

        }

    }
}
