using System.Diagnostics;

class Log
{
    public static List<double> cpuUsage = [];
    public static List<double> ramUsage = [];
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

        ultimoTiempoTotal = tiempoTotal;
        UltimoGuardado = tiempoActual;

        //RAM
        var ram = process.WorkingSet64 / (1024 * 1024);

        ramUsage.Add(ram);
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
        stop = true;
        var cpu = cpuUsage.Average();
        var ram = ramUsage.Average();
        TimeSpan tiempoEjecucion = DateTime.UtcNow - MomentoEjecucion;

        string logInfo = $"\n{estado} | CPU medio: {cpu:F2}%, pico de CPU: {cpuUsage.Max():F2}%, RAM usada de media: {ram:F2} MB, pico de RAM: {ramUsage.Max():F2} MB, Tiempo empleado: {tiempoEjecucion:hh\\:mm\\:ss}";
        cpuUsage.Clear();
        ramUsage.Clear();
        
        File.AppendAllText("log/Rendimiento.log", logInfo);
    }
    public static void ClearLog() => File.WriteAllText("log/Rendimiento.log", string.Empty);

}
