# TODO

* Convert `appsettings.json` to YAML format and update the config loaders.
* Clean Architecture in C# generally has projects for infrastructure, domain, application, and presentation, in addition to the test projects.
    1. `ECMABasic55` is the presentation layer.
    2. Let's rename `ECMABasic.Core` to `ECMABasic.Application`.
    3. `ECMABasic.Test` should be moved from `./src` to `./test` and renamed to `ECMABasic.Application.Test`.
    4. The `ECMABasic.Application` and `ECMABasic.Application.Test` projects should be reviewed for anything that should be moved to the Domain or Infrastructure layers.
