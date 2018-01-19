# Documentação `(RASCUNHO)`

Segue umas pequenas notas do que se pretende com esse projeto.

#### CommandLineApp mínima

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

#### CommandLineApp com versão

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Version("1.0.0-alpha-67890");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

#### CommandLineApp com direitos autorais

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

#### CommandLineApp com sinopse

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Synopsis(new string[]
{
    "This is a my command line app to exemplify the use of this",
    "exceptional library. In the next few lines you'll see how",
    "easy it is to write a command line program."
});

cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

#### CommandLineApp mais completo

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App")

    .Version("1.0.0-alpha-67890");
    .Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");
    .Synopsis(new string[] {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program." });

cmdApp.PrintUsage();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

```csharp
cmdApp.PrintHelp();
```
_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

_@output:_
```
cmd - My Command Line App [version 1.0.0-alpha-67890]
Copyright (c) 1991-2018 E5R Development Team. All rights reserved.

This is a my command line app to exemplify the use of this
exceptional library. In the next few lines you'll see how
easy it is to write a command line program.

USAGE:
    $ cmd [<options>|<flags>] [<args>]
```

#### Definindo e usando opções

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Option("Action", "The app action")
      .Template("-a|--action");

var args = new string[] { "-a","My Action Value" };
                  // or { "-a=""My Action Value""" };
                  // or { "--action","My Action Value" };
                  // or { "--action=""My Action Value""" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.True(options.HasValue("Action"));
Assert.Equal(options.ValueOf("Action"), "My Action Value");

Assert.False(options.HasValue("NotFoundAction"));
```

Como visto acima, podemos informar uma opção de várias formas, veja:

```powershell
$ program -a "My Action Value"
# { "-a", "My Action Value" }

$ program -a="My Action Value"
# { "-a="My Action Value" }

$ program --action "My Action Value"
# { "--action", "My Action Value" }

$ program --action="My Action Value"
# { "--action=""My Action Value""" }
```

Mas para fins de didática, usaremos sempre a mesma forma abaixo para simplificar
o entendimento (porém você está livre para usar a forma que preferir):

```powershell
$ program -a "My Action Value"
# { "-a", "My Action Value" }
```