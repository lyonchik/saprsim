﻿using Entities;
using EntityTransformator;
using Kernel;
using sapr_sim.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Statistics;
using System.Collections.ObjectModel;
using Statistics.Beans;
using Statistics.extractors;
using Statistics.Extractors.impl;
using DB;
using Entities.impl;

namespace sapr_sim
{

    public partial class RunSimulation : Window
    {

        private Canvas canvas;
        private Controller controller;

        public RunSimulation(ProjectItem pi, Controller controller)
        {
            InitializeComponent();
            this.canvas = pi.Canvas;
            this.controller = controller;
            processName.Content = pi.Name;            
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Model.Instance.timeRestriction = TimeConverter.fromHumanToModel(Project.Instance.TimeRestiction);
            TimeTrackerEngine.clear();

            try {
                using (var ctx = new SaprSimDbContext())
                {
                    Model model = Model.Instance;

                    model.addProject(new Entities.Project());
                    ctx.models.Add(model);


                    //Model model = new Model();
                    //model.id = 1;


                    //Procedure proc = new Procedure();
                    //proc.name = "TEST ENROLLMENT";
                    //proc.id = 2;

                    //Entities.Project prj = new Entities.Project();
                    //prj.id = 102;
                    //prj.name = "adf";

                    //model.entities.Add(proc);
                    //model.projects.Add(prj);

                    //ctx.models.Add(model); 


                    ctx.SaveChanges();

                }
            }
            catch (Exception ex){
                Console.WriteLine(ex);
            }


            try
            {
                using (var ctx = new SaprSimDbContext())
                {
                    var model = ctx.models.Where(i => i.id == 0 || i.id == 1);
                  //  var proc = ctx.procedures.Where(i => i.id == 0 || i.id == 1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            controller.simulate();
                
            TimeWithMeasure simulationTime = TimeConverter.fromModelToHuman(controller.SimulationTime);
            time.Content = Math.Round(simulationTime.doubleValue, 3) + " " + simulationTime.measure.Name;
            runStatus.Content = ProcessingStateMethods.GetDescription(controller.SimulationState);                
            totalRunCount.Content = totalRunCount.Content == null ? 1 : int.Parse(totalRunCount.Content.ToString()) + 1;
            if (controller.SimulationState == ProcessingState.FINE)
                image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/success.png", UriKind.Absolute)); 
            else
                image.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/failure.png", UriKind.Absolute));



            ProjectsStatisticsDE prjExt = new ProjectsStatisticsDE();
            ICollection<ProjectBean> projectStat = prjExt.extract();

            ProcedureStatisticsDE procExtr = new ProcedureStatisticsDE();
            ICollection<ProcedureBean> procedureStat = procExtr.extract();

            ResourceStatisticDE reExt = new ResourceStatisticDE();
            ICollection<ResourceBean> resourseStat = reExt.extract();


            taskTab.ItemsSource = projectStat;
            procedureTab.ItemsSource = procedureStat;
            resourceTab.ItemsSource = resourseStat;

        }
    }
}