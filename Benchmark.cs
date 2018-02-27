// -*- c-basic-offset: 2; indent-tabs-mode: nil-*-

// Benchmarks of .Net threading primitives, i.e. Lazy<>, Task<>, lock
// and Interlocked.CompareExchange(), based on microbenchmarking code by
// Peter Sestoft.
//
// fbie@itu.dk * 2018-01-23
// sestoft@itu.dk * 2013-06-02, 2015-05-08

using System;
using System.Threading;
using System.Threading.Tasks;

class Benchmark {
  public static void Main(String[] args) {
    SystemInfo();
    Mark8("lazy-create", d => {
        Lazy<int> l = new Lazy<int>(() => 42);
        return 0;
      });
    Mark8("lazy-compute", d => {
        Lazy<int> l = new Lazy<int>(() => 23);
        return l.Value;
      });
    Mark8("lazy-compute-pub", d => {
        Lazy<int> l = new Lazy<int>(() => 23, LazyThreadSafetyMode.PublicationOnly);
        return l.Value;
      });
    Mark8("lazy-compute-ex&pub", d => {
        Lazy<int> l = new Lazy<int>(() => 23, LazyThreadSafetyMode.ExecutionAndPublication);
        return l.Value;
      });

    try {
      int workers = 0, completion = 0;
      ThreadPool.GetMaxThreads(out workers, out completion);
      ThreadPool.SetMinThreads(workers, completion);
    } catch (Exception e) {
      Console.WriteLine("# ERROR: Could not change TPL minimum thread count.");
    }

    Mark8("task-create", d => {
        Task<int> t = new Task<int>(() => 23);
        return 0;
      });
    Mark8("task-create-run", d => {
        Task<int> t = new Task<int>(() => 23);
        t.RunSynchronously();
        return t.Result;
      });
    Mark8("task-run", d => Task.Run(() => 23).Id);

    object o = new object();
    int i = 0;
    Mark8("lock", d => {
        lock (o) {
          ++i;
        }
        return i;
      });
    object m = new object();
    Mark8("cas-success", d => {
        Interlocked.CompareExchange(ref m, m, o);
        return 666;
      });
    Mark8("cas-fail", d => {
        Interlocked.CompareExchange(ref m, o, o);
        return 666;
      });

  }

  // ========== Infrastructure code ==========

  private static void SystemInfo() {
    Console.WriteLine("# OS          {0}",
      Environment.OSVersion.VersionString);
    Console.WriteLine("# .NET vers.  {0}",
      Environment.Version);
    Console.WriteLine("# 64-bit OS   {0}",
      Environment.Is64BitOperatingSystem);
    Console.WriteLine("# 64-bit proc {0}",
      Environment.Is64BitProcess);
    Console.WriteLine("# CPU         {0}; {1} \"cores\"",
      Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"),
      Environment.ProcessorCount);
    Console.WriteLine("# Date        {0:s}",
      DateTime.Now);
  }

  public static double Mark8(String msg, String info, Func<int,double> f,
                             int n = 10, double minTime = 0.25) {
    int count = 1, totalCount = 0;
    double dummy = 0.0, runningTime = 0.0, st = 0.0, sst = 0.0;
    do {
      count *= 2;
      st = sst = 0.0;
      for (int j=0; j<n; j++) {
        Timer t = new Timer();
        for (int i=0; i<count; i++)
          dummy += f(i);
        runningTime = t.Check();
        double time = runningTime * 1e9 / count;
        st += time;
        sst += time * time;
	totalCount += count;
      }
    } while (runningTime < minTime && count < Int32.MaxValue/2);
    double mean = st/n, sdev = Math.Sqrt((sst - mean*mean*n)/(n-1));
    Console.WriteLine("{0,-25} {1}{2,15:F1} ns {3,10:F2} {4,10:D}", msg, info, mean, sdev, count);
    return dummy / totalCount;
  }

  public static double Mark8(String msg, Func<int,double> f,
                             int n = 10, double minTime = 0.25) {
    return Mark8(msg, "", f, n, minTime);
  }

  public static double Mark8Setup(String msg, String info, Func<int,double> f,
				  Action setup = null, int n = 10, double minTime = 0.25) {
    int count = 1, totalCount = 0;
    double dummy = 0.0, runningTime = 0.0, st = 0.0, sst = 0.0;
    do {
      count *= 2;
      st = sst = 0.0;
      for (int j=0; j<n; j++) {
        Timer t = new Timer();
        for (int i=0; i<count; i++) {
          t.Pause();
          if (setup != null)
	    setup();
          t.Play();
          dummy += f(i);
        }
        runningTime = t.Check();
        double time = runningTime * 1e9 / count;
        st += time;
        sst += time * time;
	totalCount += count;
      }
    } while (runningTime < minTime && count < Int32.MaxValue/2);
    double mean = st/n, sdev = Math.Sqrt((sst - mean*mean*n)/(n-1));
    Console.WriteLine("{0,-25} {1}{2,15:F1} ns {3,10:F2} {4,10:D}", msg, info, mean, sdev, count);
    return dummy / totalCount;
  }

  public static double Mark8Setup(String msg, Func<int,double> f,
				  Action setup = null, int n = 10, double minTime = 0.25) {
    return Mark8Setup(msg, "", f, setup, n, minTime);
  }
}

// Crude timing utility ----------------------------------------

public class Timer {
  private readonly System.Diagnostics.Stopwatch stopwatch
    = new System.Diagnostics.Stopwatch();
  public Timer() { Play(); }
  public double Check() { return stopwatch.ElapsedMilliseconds / 1000.0; }
  public void Pause() { stopwatch.Stop(); }
  public void Play() { stopwatch.Start(); }
}
