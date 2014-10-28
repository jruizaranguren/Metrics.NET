﻿
using System;
using System.Diagnostics;
using System.Linq;
using Metrics.Core;
using Metrics.Logging;
using Metrics.MetricData;

namespace Metrics.PerfCounters
{
    public static class PerformanceCounters
    {
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        private const string TotalInstance = "_Total";
        private const string GlobalInstance = "_Global_";

        private const string Exceptions = ".NET CLR Exceptions";
        private const string Memory = ".NET CLR Memory";
        private const string LocksAndThreads = ".NET CLR LocksAndThreads";
        private const string Networking = ".NET CLR Networking 4.0.0.0";

        public static void RegisterSystemCounters(MetricsContext context)
        {
            context.Register("Available RAM", Unit.MegaBytes, "Memory", "Available MBytes", tags: "memory");
            context.Register("Free System Page Table Entries", Unit.Custom("entries"), "Memory", "Free System Page Table Entries", tags: "memory");
            context.Register("Pages Input/sec", Unit.Custom("pages/s"), "Memory", "Pages Input/sec", tags: "memory");
            context.Register("Pages/sec", Unit.Custom("pages/s"), "Memory", "Pages/sec", tags: "memory");
            context.Register("Pool Nonpaged MBytes", Unit.MegaBytes, "Memory", "Pool Nonpaged Bytes", derivate: v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Pool Paged MBytes", Unit.MegaBytes, "Memory", "Pool Paged Bytes", derivate: v => v / (1024 * 1024.0), tags: "memory");

            context.Register("CPU Usage", Unit.Custom("%"), "Processor", "% Processor Time", TotalInstance, tags: "cpu");
            context.Register("Interrupts / sec", Unit.Custom("interrupts/s"), "Processor", "Interrupts/sec", TotalInstance, tags: "cpu");
            context.Register("% Interrupt time", Unit.Custom("%"), "Processor", "% Interrupt Time", TotalInstance, tags: "cpu");
            context.Register("% User Timer", Unit.Custom("%"), "Processor", "% User Time", TotalInstance, tags: "cpu");
            context.Register("% Privileged Timer", Unit.Custom("%"), "Processor", "% Privileged Time", TotalInstance, tags: "cpu");
            context.Register("% DPC Time", Unit.Custom("%"), "Processor", "% DPC Time", TotalInstance, tags: "cpu");

            context.Register("Logical Disk Avg. sec/Read", Unit.Custom("ms"), "LogicalDisk", "Avg. Disk sec/Read", TotalInstance, v => v * 1024.0, tags: "disk");
            context.Register("Logical Disk Avg. sec/Write", Unit.Custom("ms"), "LogicalDisk", "Avg. Disk sec/Write", TotalInstance, v => v * 1024.0, tags: "disk");
            context.Register("Logical Disk Transfers/sec", Unit.Custom("Transfers"), "LogicalDisk", "Disk Transfers/sec", TotalInstance, tags: "disk");
            context.Register("Logical Disk Writes/sec", Unit.Custom("kb/s"), "LogicalDisk", "Disk Reads/sec", TotalInstance, f => f / 1024.0, tags: "disk");
            context.Register("Logical Disk Reads/sec", Unit.Custom("kb/s"), "LogicalDisk", "Disk Writes/sec", TotalInstance, f => f / 1024.0, tags: "disk");

            context.Register("Physical Disk Avg. sec/Read", Unit.Custom("ms"), "PhysicalDisk", "Avg. Disk sec/Read", TotalInstance, v => v * 1024.0, tags: "disk");
            context.Register("Physical Disk Avg. sec/Write", Unit.Custom("ms"), "PhysicalDisk", "Avg. Disk sec/Write", TotalInstance, v => v * 1024.0, tags: "disk");
            context.Register("Physical Disk Transfers/sec", Unit.Custom("Transfers"), "PhysicalDisk", "Disk Transfers/sec", TotalInstance, tags: "disk");
            context.Register("Physical Disk Writes/sec", Unit.Custom("kb/s"), "PhysicalDisk", "Disk Reads/sec", TotalInstance, f => f / 1024.0, tags: "disk");
            context.Register("Physical Disk Reads/sec", Unit.Custom("kb/s"), "PhysicalDisk", "Disk Writes/sec", TotalInstance, f => f / 1024.0, tags: "disk");
        }

        public static void RegisterAppCounters(MetricsContext context)
        {
            var app = Process.GetCurrentProcess().ProcessName;

            context.Register("Private MBytes ", Unit.MegaBytes, "Process", "Private Bytes", app, derivate: v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Working Set ", Unit.MegaBytes, "Process", "Working Set", app, derivate: v => v / (1024 * 1024.0), tags: "memory");

            context.Register("Mb in all Heaps", Unit.MegaBytes, Memory, "# Bytes in all Heaps", app, v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Gen 0 heap size", Unit.MegaBytes, Memory, "Gen 0 heap size", app, v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Gen 1 heap size", Unit.MegaBytes, Memory, "Gen 1 heap size", app, v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Gen 2 heap size", Unit.MegaBytes, Memory, "Gen 2 heap size", app, v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Large Object Heap size", Unit.MegaBytes, Memory, "Large Object Heap size", app, v => v / (1024 * 1024.0), tags: "memory");
            context.Register("Allocated Bytes/second", Unit.KiloBytes, Memory, "Allocated Bytes/sec", app, v => v / 1024.0, tags: "memory");

            context.Register("Time in GC", Unit.Custom("%"), Memory, "% Time in GC", app, tags: "memory");
            context.Register("Pinned Objects", Unit.Custom("Objects"), Memory, "# of Pinned Objects", app, tags: "memory");

            context.Register("Total Exceptions", Unit.Custom("Exceptions"), Exceptions, "# of Exceps Thrown", app, tags: "exceptions");
            context.Register("Exceptions Thrown / Sec", Unit.Custom("Exceptions"), Exceptions, "# of Exceps Thrown / Sec", app, tags: "exceptions");
            context.Register("Except Filters / Sec", Unit.Custom("Filters"), Exceptions, "# of Filters / Sec", app, tags: "exceptions");
            context.Register("Finallys / Sec", Unit.Custom("Finallys"), Exceptions, "# of Finallys / Sec", app, tags: "exceptions");
            context.Register("Throw to Catch Depth / Sec", Unit.Custom("Stack Frames"), Exceptions, "Throw to Catch Depth / Sec", app, tags: "exceptions");


            context.Register("Logical Threads", Unit.Threads, LocksAndThreads, "# of current logical Threads", app, tags: "threads");
            context.Register("Physical Threads", Unit.Threads, LocksAndThreads, "# of current physical Threads", app, tags: "threads");
            context.Register("Contention Rate / Sec", Unit.Custom("Attempts"), LocksAndThreads, "Contention Rate / Sec", app, tags: "threads");
            context.Register("Total Contentions", Unit.Custom("Attempts"), LocksAndThreads, "Total # of Contentions", app, tags: "threads");
            context.Register("Queue Length / sec", Unit.Threads, LocksAndThreads, "Queue Length / sec", app, tags: "threads");

            context.Register("IO Data Operations/sec", Unit.Custom("IOPS"), "Process", "IO Data Operations/sec", app, tags: "disk");
            context.Register("IO Other Operations/sec", Unit.Custom("IOPS"), "Process", "IO Other Operations/sec", app, tags: "disk");

            ThreadPoolMetrics.RegisterThreadPoolGauges(context);
        }

        private static MetricsContext Register(this MetricsContext context, string name, Unit unit,
            string category, string counter, string instance = null,
            Func<double, double> derivate = null,
            MetricTags tags = default(MetricTags))
        {
            log.Debug(() => string.Format("Registering performance counter Name {0} Category {1} Instance {2}", counter, category, instance));

            if (PerformanceCounterCategory.Exists(category))
            {
                if (instance == null || PerformanceCounterCategory.InstanceExists(instance, category))
                {
                    if (PerformanceCounterCategory.CounterExists(counter, category))
                    {
                        var counterTags = new MetricTags(tags.Tags.Concat(new[] { "PerfCounter" }));
                        if (derivate == null)
                        {
                            context.Advanced.Gauge(name, () => new PerformanceCounterGauge(category, counter, instance), unit, counterTags);
                        }
                        else
                        {
                            context.Advanced.Gauge(name, () => new DerivedGauge(new PerformanceCounterGauge(category, counter, instance), derivate), unit, counterTags);
                        }
                        return context;
                    }
                }
            }

            log.ErrorFormat("Performance counter does not exist Name {0} Category {1} Instance {2}", counter, category, instance);
            return context;
        }
    }
}
