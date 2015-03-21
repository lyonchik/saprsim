﻿
using sapr_sim.WPFCustomElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace sapr_sim.Utils
{
    public class FileService
    {

        public static readonly string PROJECT_EXTENSION = ".ssp";
        public static readonly string PROJECT_ITEM_EXTENSION = ".ssm";

        public ScrollableCanvas open(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                return (ScrollableCanvas) new BinaryFormatter().Deserialize(fs);
            }
        }

        public void save(Canvas currentCanvas, string filepath)
        {
            using (FileStream filestream = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                new BinaryFormatter().Serialize(filestream, currentCanvas);
            }
        }

        public void saveProject()
        {
            Project prj = Project.Instance;
            string pathToProject = prj.FullPath;
            if (!Directory.Exists(pathToProject))
                Directory.CreateDirectory(pathToProject);

            string projectFile = pathToProject + "\\" + prj.ProjectName + PROJECT_EXTENSION;
            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            using (var writer = new StreamWriter(projectFile))
            {
                serializer.Serialize(writer, Project.Instance);
            }
        }

        public void openProject(string filepath)
        {
            if (String.IsNullOrWhiteSpace(filepath))
                throw new ArgumentException();
            if (!File.Exists(filepath) || !filepath.Contains(PROJECT_EXTENSION))
                throw new ProjectException("Указана неверная директория");

            XmlSerializer serializer = new XmlSerializer(typeof(Project));
            using (XmlReader reader = new XmlTextReader(filepath))
            {
                Project openedProject = (Project) serializer.Deserialize(reader);                
                reader.Close();

                // we must set all properties from opened project to our project singletone
                Project.Instance.ProjectName = openedProject.ProjectName;
                Project.Instance.ProjectPath = openedProject.ProjectPath;
                foreach (ProjectItem item in openedProject.Items)
                {
                    item.Canvas = open(item.FullPath);
                    Project.Instance.addProjectItem(item);
                }
            }
        }
    }
}