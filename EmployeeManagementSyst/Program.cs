using Microsoft.Data.SqlClient;

namespace EmployeeManagementSyst
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


            Application.Run(new LoadingForm());

            Task.Run(() =>
            {
                SchedulePaySlip schedulePaySlip = new();
                schedulePaySlip.LastRunTime();
            });
            Task.Run(() =>
            {
                AutoWeeklyScheduleSave confirm = new();
                confirm.SetSaveDate();
            });

            Application.Run(new LandingPage());

        }
         
    }
}