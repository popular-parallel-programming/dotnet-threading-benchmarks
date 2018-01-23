# Benchmarking .Net Threading Primitives #

Last updated by fbie@itu.dk, 2018-01-23.

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
# OS          Unix 4.4.0.109
# .NET vers.  4.0.30319.42000
# 64-bit OS   True
# 64-bit proc True
# CPU         ; 4 "cores"
# Date        2018-01-23T10:55:23
lazy-create                          25.6 ns       1.10   16777216
lazy-compute                         81.3 ns       2.23    4194304
lazy-compute-pub                     61.9 ns       7.84    8388608
lazy-compute-ex&pub                  84.1 ns       2.80    4194304
task-create                          57.9 ns       2.95    4194304
task-create-run                     249.5 ns      12.86    1048576
lock                                 31.9 ns       1.63    8388608
cas-success                          12.5 ns       0.19   33554432
cas-fail                             10.5 ns       0.05   33554432
```


On Windows 10, Virtual Box, Intel i7:

```
# OS          Microsoft Windows NT 6.2.9200.0
# .NET vers.  4.0.30319.42000
# 64-bit OS   True
# 64-bit proc True
# CPU         Intel64 Family 6 Model 61 Stepping 4, GenuineIntel; 2 "cores"
# Date        2018-01-23T01:58:58
lazy-create                          30,5 ns       0,85    8388608
lazy-compute                        124,1 ns       2,75    4194304
lazy-compute-pub                    105,7 ns       2,50    4194304
lazy-compute-ex&pub                 128,3 ns       6,41    2097152
task-create                          71,7 ns       2,82    4194304
task-create-run                     404,2 ns      13,72    1048576
lock                                 35,9 ns       1,43    8388608
cas-success                          13,9 ns       0,33   33554432
cas-fail                             13,8 ns       0,24   33554432
```
