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

            Task.Run(() =>
            {
               SchedulePaySlip schedulePaySlip = new SchedulePaySlip(); 
               schedulePaySlip.LastRunTime();
            });
            Task.Run(() =>
            {
               WeeklySaveConfirm confirm = new WeeklySaveConfirm();
                confirm.SetSaveDate();
            });

            MainPage obj = new MainPage();
            obj.InitiateServer();
            obj.EmployeeDetails();
            obj.EmployeePayment();
            obj.AdminTable();
            obj.EmployeeCardDetails();
            obj.HoursTable();
            obj.RotaTable();
            obj.LastExecTable();
            Application.Run(new MainPage());
            
        }
    }
}