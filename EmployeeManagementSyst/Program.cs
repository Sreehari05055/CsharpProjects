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
                ViewRota viewRota = new ViewRota();
                viewRota.LastRunTime();
            });
            Task.Run(() =>
            {
                PaySlip paySlip = new PaySlip();
                paySlip.LastRunTime();
            });
            MainPage obj = new MainPage();
            obj.InitiateServer();
            obj.EmployeeDetails();
            obj.EmployeePayment();
            obj.AdminTable();
            obj.EmployeeCardDetails();
            obj.HoursTable();
            obj.RotaTable();
            Application.Run(new MainPage());
            
        }
    }
}