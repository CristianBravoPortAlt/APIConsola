using System.Diagnostics;

class Log
{
    public static List<double> cpuUsage = [];
    public static List<double> ramUsage = [];
    public static List<double> cpuUsageTotal = [];
    public static List<double> ramUsageTotal = [];
    public static bool stop = false;
    private static DateTime MomentoEjecucion;
    private static TimeSpan ultimoTiempoTotal = Process.GetCurrentProcess().TotalProcessorTime;
    private static DateTime UltimoGuardado = DateTime.UtcNow;

    static void AddLog()
    {
        var process = Process.GetCurrentProcess();

        //CPU
        var tiempoTotal = process.TotalProcessorTime;
        var tiempoActual = DateTime.UtcNow;

        double porcentaje = 0;
        double segundosDeIntervalo = (tiempoActual - UltimoGuardado).TotalSeconds;
        if (segundosDeIntervalo > 0)
        {
            porcentaje = (tiempoTotal - ultimoTiempoTotal).TotalMilliseconds / (segundosDeIntervalo * 1000 * Environment.ProcessorCount) * 100;
        }
        if (porcentaje > 100)
        {
            porcentaje = 100;
        }
        cpuUsage.Add(porcentaje);
        cpuUsageTotal.Add(porcentaje);

        ultimoTiempoTotal = tiempoTotal;
        UltimoGuardado = tiempoActual;

        //RAM
        var ram = process.WorkingSet64 / (1024 * 1024);

        ramUsage.Add(ram);
        ramUsageTotal.Add(ram);
    }
    public static void StartLogging(int intervaloMs = 500)
    {
        stop = false;
        MomentoEjecucion = DateTime.UtcNow;
        Task.Run(async () =>
        {
            while (!stop)
            {
                AddLog();
                await Task.Delay(intervaloMs);
            }
        });
    }
    public static void StopLogging(string estado)
    {
        double cpu = 0, ram = 0, cpuUsageAvg = 0, ramUsageAvg = 0, cpuMax = 0, ramMax = 0, cpuUsageMax = 0, ramUsageMax = 0;
        try
        {
            cpu = cpuUsage.Average();
            ram = ramUsage.Average();
            cpuUsageAvg = cpuUsageTotal.Average();
            ramUsageAvg = ramUsageTotal.Average();
            cpuMax = cpuUsage.Max();
            ramMax = ramUsage.Max();
            cpuUsageMax = cpuUsageTotal.Max();
            ramUsageMax = ramUsageTotal.Max();
        }
        catch (Exception)
        {
        }
        stop = true;
        TimeSpan tiempoEjecucion = DateTime.UtcNow - MomentoEjecucion;
        string logInfo = string.Empty;
        if (estado == "Fin de la ejecución")
        {
            logInfo = $"\nCPU Y RAM TOTAL | CPU medio: {cpuUsageAvg:F2}%, pico de CPU: {cpuUsageTotal.Max():F2}%, RAM usada de media: {ramUsageAvg:F2} MB, pico de RAM: {ramUsageMax:F2} MB";
        }
        else
            logInfo = $"\n{estado} | CPU medio: {cpu:F2}%, pico de CPU: {cpuMax:F2}%, RAM usada de media: {ram:F2} MB, pico de RAM: {ramMax:F2} MB, Tiempo empleado: {tiempoEjecucion:hh\\:mm\\:ss}";

        cpuUsage.Clear();
        ramUsage.Clear();

        File.AppendAllText("log/Rendimiento.log", logInfo);
    }
    public static void ClearLog() => File.WriteAllText("log/Rendimiento.log", string.Empty);

}
