# Dev notes

## Package a .Net application

### Linux

There appear to be very few guidelines on how to package and distribute a .Net
Core application for Linux.
For Arch Linux, the instructions are outdated and refer to Mono: https://wiki.archlinux.org/index.php/CLR_package_guidelines
The very few packages using the .Net Core runtime on the Arch Linux repos include
all dependencies except the runtime itself - see for instance embyserver https://www.archlinux.org/packages/community/any/emby-server/

## Windows

*TODO*

## Build with CoreRT

Steps to build with [CoreRT] on Arch Linux updated to 8/2019:

 1. Install `clang` v3.9+ and other dependencies as listed here: https://github.com/dotnet/corert/blob/master/samples/prerequisites.md
 2. `export CppCompilerAndLinker=clang` (see https://github.com/dotnet/corert/issues/5654 )
 3. `sudo ln -s /usr/lib/libtinfo.so.6 /usr/lib/libtinfo.so.5` (see https://github.com/dotnet/corert/issues/7096 )
 4. Follow "Add CoreRT to your project" and "Restore and publish your app" steps from https://github.com/dotnet/corert/blob/master/samples/HelloWorld/README.md

It builds but then fails at runtime, probably due to some reflection mechanism in F# not being fullly supported in CoreRT (see: https://github.com/dotnet/corert/issues/2057 and https://github.com/dotnet/corert/issues/7605#issuecomment-510539851 )


```
➜  publish git:(master) ✗ ./SayIt
Unhandled Exception: EETypeRva:0x0277BEE8(System.Reflection.MissingRuntimeArtifactException): MakeGenericMethod() cannot create this generic method instantiation because the instantiation was not metadata-enabled: 'Microsoft.FSharp.Core.PrintfImpl.Specializations<Microsoft.FSharp.Core.Unit,System.String,System.String>.Final1<Microsoft.FSharp.Reflection.UnionCaseInfo>(System.String,Microsoft.FSharp.Core.FSharpFunc<Microsoft.FSharp.Reflection.UnionCaseInfo,System.String>,System.String)' For more information, please visit http://go.microsoft.com/fwlink/?LinkID=616868
   at Internal.Reflection.Core.Execution.ExecutionEnvironment.GetMethodInvoker(RuntimeTypeInfo, QMethodDefinition, RuntimeTypeInfo[], MemberInfo) + 0x285
   at System.Reflection.Runtime.MethodInfos.NativeFormat.NativeFormatMethodCommon.GetUncachedMethodInvoker(RuntimeTypeInfo[], MemberInfo) + 0xa5
   at System.Reflection.Runtime.MethodInfos.RuntimeNamedMethodInfo`1.GetUncachedMethodInvoker(RuntimeTypeInfo[], MemberInfo) + 0x37
   at System.Reflection.Runtime.MethodInfos.RuntimeConstructedGenericMethodInfo.get_UncachedMethodInvoker() + 0x3b
   at System.Reflection.Runtime.MethodInfos.RuntimeMethodInfo.get_MethodInvoker() + 0x19f
   at System.Reflection.Runtime.MethodInfos.RuntimeNamedMethodInfo`1.MakeGenericMethod(Type[]) + 0x3f6
   at Microsoft.FSharp.Core.PrintfImpl.PrintfBuilder`3.buildPlainFinal(Object[], Type[]) + 0x423
   at Microsoft.FSharp.Core.PrintfImpl.PrintfBuilder`3.Build[T](String) + 0x74
   at Microsoft.FSharp.Core.PrintfImpl.Cache`4.generate(String) + 0x5e
   at Microsoft.FSharp.Core.PrintfImpl.-cctor@1531-174.Invoke(String) + 0x39
   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey, Func`2) + 0x117
   at Microsoft.FSharp.Core.PrintfImpl.Cache`4.get(String) + 0x59
   at Microsoft.FSharp.Core.PrintfImpl.Cache`4.Get(PrintfFormat`4) + 0xe4
   at Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThen[TResult, T](FSharpFunc`2, PrintfFormat`4) + 0x49
   at Argu.PreCompute.description@485.Invoke(Unit) + 0xd5
   at System.Lazy`1.ViaFactory(LazyThreadSafetyMode) + 0x102
   at System.Lazy`1.ExecutionAndPublication(LazyHelper, Boolean) + 0x68
   at System.Lazy`1.CreateValue() + 0xd1
   at System.Lazy`1.get_Value() + 0x28
   at Argu.PreCompute.postProcess@627(UnionArgInfo.UnionArgInfo) + 0x583
   at Argu.PreCompute.checkUnionArgInfo(UnionArgInfo.UnionArgInfo) + 0x1c
   at <StartupCode$Argu>.$ArgumentParser.-cctor@87-1.Invoke(Unit) + 0x60
   at System.Lazy`1.ViaFactory(LazyThreadSafetyMode) + 0x102
   at System.Lazy`1.ExecutionAndPublication(LazyHelper, Boolean) + 0x68
   at System.Lazy`1.CreateValue() + 0xd1
   at System.Lazy`1.get_Value() + 0x28
   at Argu.ArgumentParser`1..ctor(FSharpOption`1, FSharpOption`1, FSharpOption`1, FSharpOption`1, FSharpOption`1) + 0x19f
   at Argu.ArgumentParser.Create[Template](FSharpOption`1, FSharpOption`1, FSharpOption`1, FSharpOption`1, FSharpOption`1) + 0x5e
   at Sayit.Config.getConfiguration(String[]) + 0xc9
   at Sayit.Program.main(String[]) + 0x35
   at SayIt!<BaseAddress>+0x1503347
   at SayIt!<BaseAddress>+0x15033dd
```


 [corert]: https://github.com/dotnet/corert
