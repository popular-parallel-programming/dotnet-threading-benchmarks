# Benchmarking .Net Threading Primitives #

Last updated by fbie@itu.dk, 2018-02-27.

A small set of benchmarks to determine the cost of

1. Initializing and computing `Lazy<T>` objects with different thread-safety settings;
2. Initializing and running `Task<T>` objects;
3. Primitive locking; and
4. Succeeding and Failing calls to `Interlocked.CompareExchange()`.

Both, the computations in `Lazy<T>` and `Task<T>` from 1. and 2. only return constant values.


# How To Use #

On Unix-like systems running Mono:

```bash
$ csc Benchmark.cs
$ mono Benchmark.exe
```

On Windows:
```batch
> csc Benchmark.csc
> Benchmark.exe
```


# Results #

On Ubuntu 16.04 + Mono JIT compiler version 5.4.1.7, Intel i7:

```
# OS          Unix 4.4.0.116
# .NET vers.  4.0.30319.42000
# 64-bit OS   True
# 64-bit proc True
# CPU         ; 4 "cores"
# Date        2018-02-27T09:50:35
lazy-create                          24.6 ns       0.80   16777216
lazy-compute                         88.1 ns       7.26    4194304
lazy-compute-pub                     66.1 ns       5.94    4194304
lazy-compute-ex&pub                 102.9 ns      18.75    4194304
task-create                          53.5 ns       2.56    8388608
task-run-2                          790.4 ns     200.69     524288
task-run-4                          889.4 ns     265.11     524288
lock                                 60.6 ns       6.49    4194304
cas-success                          12.0 ns       0.03   33554432
cas-fail                             10.1 ns       0.03   33554432
```


On Windows 10, Virtual Box, Intel i7:

```
# OS          Microsoft Windows NT 6.2.9200.0
# .NET vers.  4.0.30319.42000
# 64-bit OS   True
# 64-bit proc True
# CPU         Intel64 Family 6 Model 61 Stepping 4, GenuineIntel; 2 "cores"
# Date        2018-02-27T00:48:02
lazy-create                          30,4 ns       0,72   16777216
lazy-compute                        112,6 ns       3,01    4194304
lazy-compute-pub                     93,2 ns       3,91    4194304
lazy-compute-ex&pub                 116,8 ns       3,26    4194304
task-create                          66,2 ns       1,88    4194304
task-run-2                          364,7 ns      34,37    1048576
lock                                 32,4 ns       0,97    8388608
cas-success                          11,8 ns       0,14   33554432
cas-fail                             12,0 ns       0,29   33554432
```


On Windows 10, Intel Xeon:

```
# OS          Microsoft Windows NT 6.2.9200.0
# .NET vers.  4.0.30319.42000
# 64-bit OS   True
# 64-bit proc True
# CPU         Intel64 Family 6 Model 63 Stepping 2, GenuineIntel; 48 "cores"
# Date        2018-02-27T09:53:43
lazy-create                          25,5 ns       0,10   16777216
lazy-compute                        103,0 ns       0,08    4194304
lazy-compute-pub                     85,8 ns       0,08    4194304
lazy-compute-ex&pub                 105,8 ns       7,98    4194304
task-create                          61,2 ns       0,20    4194304
task-run-2                          404,2 ns      13,69    1048576
task-run-4                          314,6 ns       1,31    1048576
task-run-8                          500,5 ns       1,33     524288
task-run-16                        1400,0 ns       1,80     262144
task-run-32                        2063,0 ns     129,36     262144
lock                                 30,6 ns       1,94    8388608
cas-success                          11,3 ns       0,02   33554432
cas-fail                             11,3 ns       0,01   33554432
```
