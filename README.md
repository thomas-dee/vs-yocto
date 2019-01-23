# vs-yocto

This package allows for registering a *Yocto Windows SDK* (using the meta layer **meta-mingw**) as new *PlatformToolset* for any **Visual Studio 2017** Linux console application.

## How to
1. Ensure that you have installed the *Visual Studio 2017 Linux Development* and *C#* Workload
1. Ensure that you have installed the *Visual Studio C++ Compiler and Libraries for ARM/ARM64*
1. Build your Yocto SDK and extract it anywhere on your Windows machine
1. Checkout or download this repo somewhere.
1. Run the `registerYoctoSDKToVisualStudio.bat` with admin privileges. (Note: This is required to copy the required Toolset files into the Visual Studio installation.) and follow the instructions.

## Note
`registerYoctoSDKToVisualStudio.bat` will build the required `vs-yocto.Build.CPPTasks.GCC.dll`.

This requires C# development workload.

