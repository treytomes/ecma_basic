# TODO

## Next

* We need to run a gap analysis comparing the capabilities of ECMABasic55 with the requirements in `docs\ECMA-55_1st_edition_january_1978.pdf`.  Any missing features need to be specced out.
* `docs\ECMA-6, 7-Bit Coded Character Set.pdf` should be reviewed for any relevant requirements.  What is the purpose of the document?  Should it's implementation wait for a later version?  I'm considering a Godot-based presentation layer, but not until this project is more complete.
* Review `https://github.com/treytomes/iron_kernel/blob/main/IronKernel/Program.cs`.
    * I use a standard pattern to bootstrap an application that includes a set of Microsoft and System packages you can see in this list: `https://github.com/treytomes/iron_kernel/blob/main/IronKernel/IronKernel.csproj`
    * We need to update `./ECMABasic55/Program.cs` to follow a similar pattern, including dependency injection, command-line parameters, and a rolling timestamp-based logger.

## Pending

* Convert `appsettings.json` to YAML format and update the config loaders.
* Clean Architecture in C# generally has projects for infrastructure, domain, application, and presentation, in addition to the test projects.
    1. `ECMABasic55` is the presentation layer.
    2. Let's rename `ECMABasic.Core` to `ECMABasic.Application`.
    3. `ECMABasic.Test` should be moved from `./src` to `./test` and renamed to `ECMABasic.Application.Test`.
    4. The `ECMABasic.Application` and `ECMABasic.Application.Test` projects should be reviewed for anything that should be moved to the Domain or Infrastructure layers.

## Future

* A Godot presentation layer could include a virtual disk system that can read and write `.DSK` image files.
* Within Godot, implement retro shader effects for scan lines and fading out the corners of the screen to mimic an old TV display.
* Implement an old-school terminal using the Extended ASCII character set.  Maybe 80x25?  Hold the screen to the aspect ratio you might expect from a TV or monitor in the late 80s.
